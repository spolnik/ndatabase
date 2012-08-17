using System.Collections;
using System.Collections.Generic;
using System.Text;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer2.Meta.Compare;
using NDatabase.Tool;
using NDatabase.Tool.Wrappers.Map;

namespace NDatabase.Odb.Impl.Core.Layers.Layer2.Meta.Compare
{
    /// <summary>
    ///   Manage Object info differences.
    /// </summary>
    /// <remarks>
    ///   Manage Object info differences. compares two object info and tells which objects in the object hierarchy has changed. This is used by the update to process to optimize it and actually update what has changed
    /// </remarks>
    /// <author>olivier s</author>
    public class ObjectInfoComparator : IObjectInfoComparator
    {
        private const int Size = 5;
        private readonly IDictionary<NonNativeObjectInfo, int> _alreadyCheckingObjects;
        private readonly IList<ArrayModifyElement> _arrayChanges;

        private readonly IList<SetAttributeToNullAction> _attributeToSetToNull;

        private readonly IList<NonNativeObjectInfo> _changedObjectMetaRepresentations;

        private readonly IList<ChangedObjectInfo> _changes;

        private readonly IList<NewNonNativeObjectAction> _newObjectMetaRepresentations;
        private readonly IList<object> _newObjects;
        private IList<IChangedAttribute> _changedAttributeActions;
        private int _maxObjectRecursionLevel;

        private int _nbChanges;

        public ObjectInfoComparator()
        {
            _changedObjectMetaRepresentations = new List<NonNativeObjectInfo>(Size);
            _attributeToSetToNull = new List<SetAttributeToNullAction>(Size);
            _alreadyCheckingObjects = new OdbHashMap<NonNativeObjectInfo, int>(Size);
            _newObjects = new List<object>(Size);
            _newObjectMetaRepresentations = new List<NewNonNativeObjectAction>(Size);
            _changes = new List<ChangedObjectInfo>(Size);
            _changedAttributeActions = new List<IChangedAttribute>(Size);
            _arrayChanges = new List<ArrayModifyElement>();
            _maxObjectRecursionLevel = 0;
        }

        #region IObjectInfoComparator Members

        public virtual bool HasChanged(AbstractObjectInfo aoi1, AbstractObjectInfo aoi2)
        {
            return HasChanged(aoi1, aoi2, -1);
        }

        public virtual AbstractObjectInfo GetChangedObjectMetaRepresentation(int i)
        {
            return _changedObjectMetaRepresentations[i];
        }

        public virtual IList<ChangedObjectInfo> GetChanges()
        {
            return _changes;
        }

        public virtual IList<NewNonNativeObjectAction> GetNewObjectMetaRepresentations()
        {
            return _newObjectMetaRepresentations;
        }

        public virtual NewNonNativeObjectAction GetNewObjectMetaRepresentation(int i)
        {
            return _newObjectMetaRepresentations[i];
        }

        public virtual IList<object> GetNewObjects()
        {
            return _newObjects;
        }

        public virtual int GetMaxObjectRecursionLevel()
        {
            return _maxObjectRecursionLevel;
        }

        public virtual IList<IChangedAttribute> GetChangedAttributeActions()
        {
            return _changedAttributeActions;
        }

        public virtual IList<SetAttributeToNullAction> GetAttributeToSetToNull()
        {
            return _attributeToSetToNull;
        }

        public virtual void Clear()
        {
            _changedObjectMetaRepresentations.Clear();
            _attributeToSetToNull.Clear();
            _alreadyCheckingObjects.Clear();
            _newObjects.Clear();
            _newObjectMetaRepresentations.Clear();
            _changes.Clear();
            _changedAttributeActions.Clear();
            _arrayChanges.Clear();
            _maxObjectRecursionLevel = 0;
            _nbChanges = 0;
        }

        public virtual int GetNbChanges()
        {
            return _nbChanges;
        }

        public virtual IList<ArrayModifyElement> GetArrayChanges()
        {
            return _arrayChanges;
        }

        #endregion

        private bool HasChanged(AbstractObjectInfo aoi1, AbstractObjectInfo aoi2, int objectRecursionLevel)
        {
            // If one is null and the other not
            if (aoi1.IsNull() != aoi2.IsNull())
                return true;
            if (aoi1.IsNonNativeObject() && aoi2.IsNonNativeObject())
                return HasChanged((NonNativeObjectInfo) aoi1, (NonNativeObjectInfo) aoi2, objectRecursionLevel + 1);
            if (aoi1.IsNative() && aoi2.IsNative())
                return HasChanged((NativeObjectInfo) aoi1, (NativeObjectInfo) aoi2);
            return false;
        }

        private static bool HasChanged(NativeObjectInfo aoi1, NativeObjectInfo aoi2)
        {
            if (aoi1.GetObject() == null && aoi2.GetObject() == null)
                return false;
            if (aoi1.GetObject() == null || aoi2.GetObject() == null)
                return true;

            return !aoi1.GetObject().Equals(aoi2.GetObject());
        }

        private bool HasChanged(NonNativeObjectInfo nnoi1, NonNativeObjectInfo nnoi2, int objectRecursionLevel)
        {
            var hasChanged = false;
            // If the object is already being checked, return false, this second
            // check will not affect the check
            int n;
            _alreadyCheckingObjects.TryGetValue(nnoi2, out n);
            if (n != 0)
                return false;
            // Put the object in the temporary cache
            _alreadyCheckingObjects[nnoi1] = 1;
            _alreadyCheckingObjects[nnoi2] = 1;
            // Warning ID Start with 1 and not 0
            for (var id = 1; id <= nnoi1.GetMaxNbattributes(); id++)
            {
                var value1 = nnoi1.GetAttributeValueFromId(id);
                // Gets the value by the attribute id to be sure
                // Problem because a new object info may not have the right ids ?
                // Check if
                // the new oiD is ok.
                var value2 = nnoi2.GetAttributeValueFromId(id);
                if (value2 == null)
                {
                    // this means the object to have attribute id
                    StoreChangedObject(nnoi1, nnoi2, id, objectRecursionLevel);
                    hasChanged = true;
                    continue;
                }
                if (value1 == null)
                {
                    //throw new ODBRuntimeException("ObjectInfoComparator.hasChanged:attribute with id "+id+" does not exist on "+nnoi2);
                    // This happens when this object was created with an version of ClassInfo (which has been refactored).
                    // In this case,we simply tell that in place update is not supported so that the object will be rewritten with 
                    // new metamodel
                    continue;
                }
                // If both are null, no effect
                if (value1.IsNull() && value2.IsNull())
                    continue;
                if (value2.IsNull())
                {
                    hasChanged = true;
                    StoreActionSetAttributetoNull(nnoi1, id);
                    continue;
                }
                if (value1.IsNull() && value2.IsNonNativeObject())
                {
                    hasChanged = true;
                    var oi2 = (NonNativeObjectInfo)value2;
                    var positionToUpdateReference = nnoi1.GetAttributeDefinitionPosition(id);
                    StoreNewObjectReference(positionToUpdateReference, oi2, objectRecursionLevel,
                                            nnoi1.GetClassInfo().GetAttributeInfoFromId(id).GetName());
                    continue;
                }
                if (!ClassAreCompatible(value1, value2))
                {
                    var nativeObjectInfo = value2 as NativeObjectInfo;
                    if (nativeObjectInfo != null)
                    {
                        StoreChangedObject(nnoi1, nnoi2, id, objectRecursionLevel);
                        StoreChangedAttributeAction(new ChangedNativeAttributeAction(nnoi1, nnoi2,
                                                                                     nnoi1.GetHeader().
                                                                                         GetAttributeIdentificationFromId
                                                                                         (id), nativeObjectInfo,
                                                                                     objectRecursionLevel,
                                                                                     nnoi1.GetClassInfo().
                                                                                         GetAttributeInfoFromId(id).
                                                                                         GetName()));
                    }
                    var objectReference = value2 as ObjectReference;
                    if (objectReference != null)
                    {
                        var nnoi = (NonNativeObjectInfo) value1;
                        var oref = objectReference;
                        if (!nnoi.GetOid().Equals(oref.GetOid()))
                        {
                            StoreChangedObject(nnoi1, nnoi2, id, objectRecursionLevel);
                            var attributeIdThatHasChanged = id;
                            // this is the exact position where the object reference
                            // definition is stored
                            var attributeDefinitionPosition =
                                nnoi2.GetAttributeDefinitionPosition(attributeIdThatHasChanged);
                            StoreChangedAttributeAction(
                                new ChangedObjectReferenceAttributeAction(attributeDefinitionPosition,
                                                                          objectReference, objectRecursionLevel));
                        }
                        else
                            continue;
                    }
                    hasChanged = true;
                    continue;
                }
                if (value1.IsAtomicNativeObject())
                {
                    if (!value1.Equals(value2))
                    {
                        // storeChangedObject(nnoi1, nnoi2, id,
                        // objectRecursionLevel);
                        StoreChangedAttributeAction(new ChangedNativeAttributeAction(nnoi1, nnoi2,
                                                                                     nnoi1.GetHeader().
                                                                                         GetAttributeIdentificationFromId
                                                                                         (id), (NativeObjectInfo) value2,
                                                                                     objectRecursionLevel,
                                                                                     nnoi1.GetClassInfo().
                                                                                         GetAttributeInfoFromId(id).
                                                                                         GetName()));
                        hasChanged = true;
                        continue;
                    }
                    continue;
                }
                if (value1.IsCollectionObject())
                {
                    var coi1 = (CollectionObjectInfo) value1;
                    var coi2 = (CollectionObjectInfo) value2;
                    var collectionHasChanged = ManageCollectionChanges(nnoi1, nnoi2, id, coi1, coi2,
                                                                        objectRecursionLevel);
                    hasChanged = hasChanged || collectionHasChanged;
                    continue;
                }
                if (value1.IsArrayObject())
                {
                    var aoi1 = (ArrayObjectInfo) value1;
                    var aoi2 = (ArrayObjectInfo) value2;
                    var arrayHasChanged = ManageArrayChanges(nnoi1, nnoi2, id, aoi1, aoi2, objectRecursionLevel);
                    hasChanged = hasChanged || arrayHasChanged;
                    continue;
                }
                if (value1.IsMapObject())
                {
                    var moi1 = (MapObjectInfo) value1;
                    var moi2 = (MapObjectInfo) value2;
                    var mapHasChanged = ManageMapChanges(nnoi1, nnoi2, id, moi1, moi2, objectRecursionLevel);
                    hasChanged = hasChanged || mapHasChanged;
                    continue;
                }
                if (value1.IsEnumObject())
                {
                    var enoi1 = (EnumNativeObjectInfo) value1;
                    var enoi2 = (EnumNativeObjectInfo) value2;
                    var enumHasChanged = !enoi1.GetEnumClassInfo().GetId().Equals(enoi2.GetEnumClassInfo().GetId()) ||
                                          !enoi1.GetEnumName().Equals(enoi2.GetEnumName());
                    hasChanged = hasChanged || enumHasChanged;
                    continue;
                }
                if (value1.IsNonNativeObject())
                {
                    var oi1 = (NonNativeObjectInfo) value1;
                    var oi2 = (NonNativeObjectInfo) value2;
                    // If oids are equal, they are the same objects
                    if (oi1.GetOid() != null && oi1.GetOid().Equals(oi2.GetOid()))
                        hasChanged = HasChanged(value1, value2, objectRecursionLevel + 1) || hasChanged;
                    else
                    {
                        // This means that an object reference has changed.
                        hasChanged = true;
                        // keep track of the position where the reference must be
                        // updated
                        var positionToUpdateReference = nnoi1.GetAttributeDefinitionPosition(id);
                        StoreNewObjectReference(positionToUpdateReference, oi2, objectRecursionLevel,
                                                nnoi1.GetClassInfo().GetAttributeInfoFromId(id).GetName());
                        objectRecursionLevel++;
                    }
                }
            }
            var i1 = _alreadyCheckingObjects[nnoi1];
            var i2 = _alreadyCheckingObjects[nnoi2];
            i1 = i1 - 1;
            i2 = i2 - 1;
            if (i1 == 0)
                _alreadyCheckingObjects.Remove(nnoi1);
            else
                _alreadyCheckingObjects.Add(nnoi1, i1);
            if (i2 == 0)
                _alreadyCheckingObjects.Remove(nnoi2);
            else
                _alreadyCheckingObjects.Add(nnoi2, i2);
            return hasChanged;
        }

        private void StoreNewObjectReference(long positionToUpdateReference, NonNativeObjectInfo oi2,
                                             int objectRecursionLevel, string attributeName)
        {
            var nnnoa = new NewNonNativeObjectAction(positionToUpdateReference, oi2, objectRecursionLevel, attributeName);
            _newObjectMetaRepresentations.Add(nnnoa);
            _nbChanges++;
        }

        private void StoreActionSetAttributetoNull(NonNativeObjectInfo nnoi, int id)
        {
            _nbChanges++;
            var action = new SetAttributeToNullAction(nnoi, id);
            _attributeToSetToNull.Add(action);
        }

        private void StoreArrayChange(NonNativeObjectInfo nnoi, int arrayAttributeId, int arrayIndex,
                                      AbstractObjectInfo value)
        {
            _nbChanges++;
            var ame = new ArrayModifyElement(nnoi, arrayAttributeId, arrayIndex, value);
            _arrayChanges.Add(ame);
        }

        private static bool ClassAreCompatible(AbstractObjectInfo value1, AbstractObjectInfo value2)
        {
            var clazz1 = value1.GetType();
            var clazz2 = value2.GetType();

            return clazz1 == clazz2;
        }

        private void StoreChangedObject(NonNativeObjectInfo aoi1, NonNativeObjectInfo aoi2, int fieldId,
                                        AbstractObjectInfo oldValue, AbstractObjectInfo newValue, string message,
                                        int objectRecursionLevel)
        {
            if (aoi1 != null && aoi2 != null)
            {
                if (aoi1.GetOid() != null && aoi1.GetOid().Equals(aoi2.GetOid()))
                {
                    _changedObjectMetaRepresentations.Add(aoi2);
                    _changes.Add(new ChangedObjectInfo(aoi1.GetClassInfo(), aoi2.GetClassInfo(), fieldId, oldValue,
                                                       newValue, message, objectRecursionLevel));
                    // also the max recursion level
                    if (objectRecursionLevel > _maxObjectRecursionLevel)
                        _maxObjectRecursionLevel = objectRecursionLevel;
                    _nbChanges++;
                }
                else
                {
                    _newObjects.Add(aoi2.GetObject());
                    var fieldName = aoi1.GetClassInfo().GetAttributeInfoFromId(fieldId).GetName();
                    // keep track of the position where the reference must be
                    // updated - use aoi1 to get position, because aoi2 do not have position defined yet
                    var positionToUpdateReference = aoi1.GetAttributeDefinitionPosition(fieldId);
                    StoreNewObjectReference(positionToUpdateReference, aoi2, objectRecursionLevel, fieldName);
                }
            }
            else
            {
                //newObjectMetaRepresentations.add(aoi2);
                DLogger.Info("Non native object with null object");
            }
        }

        private void StoreChangedObject(NonNativeObjectInfo aoi1, NonNativeObjectInfo aoi2, int fieldId,
                                        int objectRecursionLevel)
        {
            _nbChanges++;
            if (aoi1 != null && aoi2 != null)
            {
                _changes.Add(new ChangedObjectInfo(aoi1.GetClassInfo(), aoi2.GetClassInfo(), fieldId,
                                                   aoi1.GetAttributeValueFromId(fieldId),
                                                   aoi2.GetAttributeValueFromId(fieldId), objectRecursionLevel));
                // also the max recursion level
                if (objectRecursionLevel > _maxObjectRecursionLevel)
                    _maxObjectRecursionLevel = objectRecursionLevel;
            }
            else
                DLogger.Info("Non native object with null object");
        }

        /// <summary>
        ///   Checks if something in the Collection has changed, if yes, stores the change
        /// </summary>
        /// <param name="nnoi1"> The first Object meta representation (nnoi = NonNativeObjectInfo) </param>
        /// <param name="nnoi2"> The second object meta representation </param>
        /// <param name="fieldId"> The field index that this collection represents </param>
        /// <param name="coi1"> The Meta representation of the collection 1 (coi = CollectionObjectInfo) </param>
        /// <param name="coi2"> The Meta representation of the collection 2 </param>
        /// <param name="objectRecursionLevel"> </param>
        /// <returns> true if 2 collection representation are different </returns>
        private bool ManageCollectionChanges(NonNativeObjectInfo nnoi1, NonNativeObjectInfo nnoi2, int fieldId,
                                             CollectionObjectInfo coi1, CollectionObjectInfo coi2,
                                             int objectRecursionLevel)
        {
            var collection1 = coi1.GetCollection();
            var collection2 = coi2.GetCollection();
            if (collection1.Count != collection2.Count)
            {
                var buffer = new StringBuilder();
                buffer.Append("Collection size has changed oldsize=").Append(collection1.Count).Append("/newsize=").
                    Append(collection2.Count);
                StoreChangedObject(nnoi1, nnoi2, fieldId, coi1, coi2, buffer.ToString(), objectRecursionLevel);
                return true;
            }
            IEnumerator iterator1 = collection1.GetEnumerator();
            IEnumerator iterator2 = collection2.GetEnumerator();
            var index = 0;
            while (iterator1.MoveNext())
            {
                iterator2.MoveNext();
                var value1 = (AbstractObjectInfo) iterator1.Current;
                var value2 = (AbstractObjectInfo) iterator2.Current;
                var hasChanged = HasChanged(value1, value2, objectRecursionLevel);
                if (hasChanged)
                {
                    // We consider collection has changed only if object are
                    // different, If objects are the same instance, but something in
                    // the object has changed, then the collection has not
                    // changed,only the object
                    if (value1.IsNonNativeObject() && value2.IsNonNativeObject())
                    {
                        var nnoia = (NonNativeObjectInfo) value1;
                        if (nnoia.GetOid() != null && !nnoia.GetOid().Equals(nnoi2.GetOid()))
                        {
                            // Objects are not the same instance -> the collection
                            // has changed
                            StoreChangedObject(nnoi1, nnoi2, fieldId, value1, value2,
                                               string.Format("List element index {0} has changed", index), objectRecursionLevel);
                        }
                    }
                    else
                    {
                        _nbChanges++;
                    }
                    //storeChangedObject(nnoi1, nnoi2, fieldId, value1, value2, "List element index " + index + " has changed", objectRecursionLevel);
                    return true;
                }
                index++;
            }
            return false;
        }

        /// <summary>
        ///   Checks if something in the Arary has changed, if yes, stores the change
        /// </summary>
        /// <param name="nnoi1"> The first Object meta representation (nnoi = NonNativeObjectInfo) </param>
        /// <param name="nnoi2"> The second object meta representation </param>
        /// <param name="fieldId"> The field index that this collection represents </param>
        /// <param name="aoi1"> The Meta representation of the array 1 (aoi = ArraybjectInfo) </param>
        /// <param name="aoi2"> The Meta representation of the array 2 </param>
        /// <param name="objectRecursionLevel"> </param>
        /// <returns> true if the 2 array representations are different </returns>
        private bool ManageArrayChanges(NonNativeObjectInfo nnoi1, NonNativeObjectInfo nnoi2, int fieldId,
                                        ArrayObjectInfo aoi1, ArrayObjectInfo aoi2, int objectRecursionLevel)
        {
            var array1 = aoi1.GetArray();
            var array2 = aoi2.GetArray();
            if (array1.Length != array2.Length)
            {
                var buffer = new StringBuilder();
                buffer.Append("Array size has changed oldsize=").Append(array1.Length).Append("/newsize=").Append(
                    array2.Length);
                StoreChangedObject(nnoi1, nnoi2, fieldId, aoi1, aoi2, buffer.ToString(), objectRecursionLevel);
                return true;
            }
            
            for (var i = 0; i < array1.Length; i++)
            {
                var value1 = (AbstractObjectInfo) array1[i];
                var value2 = (AbstractObjectInfo) array2[i];
                var localHasChanged = HasChanged(value1, value2, objectRecursionLevel);
                if (localHasChanged)
                {
                    StoreArrayChange(nnoi1, fieldId, i, value2);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///   Checks if something in the Map has changed, if yes, stores the change
        /// </summary>
        /// <param name="nnoi1"> The first Object meta representation (nnoi = NonNativeObjectInfo) </param>
        /// <param name="nnoi2"> The second object meta representation </param>
        /// <param name="fieldId"> The field index that this map represents </param>
        /// <param name="moi1"> The Meta representation of the map 1 (moi = MapObjectInfo) </param>
        /// <param name="moi2"> The Meta representation of the map 2 </param>
        /// <param name="objectRecursionLevel"> </param>
        /// <returns> true if the 2 map representations are different </returns>
        private bool ManageMapChanges(NonNativeObjectInfo nnoi1, NonNativeObjectInfo nnoi2, int fieldId,
                                      MapObjectInfo moi1, MapObjectInfo moi2, int objectRecursionLevel)
        {
            if (true)
                return true;
/*
            IDictionary<AbstractObjectInfo, AbstractObjectInfo> map1 = moi1.GetMap();
            IDictionary<AbstractObjectInfo, AbstractObjectInfo> map2 = moi2.GetMap();
            if (map1.Count != map2.Count)
            {
                var buffer = new StringBuilder();
                buffer.Append("Map size has changed oldsize=").Append(map1.Count).Append("/newsize=").Append(map2.Count);
                StoreChangedObject(nnoi1, nnoi2, fieldId, moi1, moi2, buffer.ToString(), objectRecursionLevel);
                return true;
            }
            IEnumerator<AbstractObjectInfo> keys1 = map1.Keys.GetEnumerator();
            IEnumerator<AbstractObjectInfo> keys2 = map2.Keys.GetEnumerator();
            AbstractObjectInfo key1 = null;
            AbstractObjectInfo key2 = null;
            AbstractObjectInfo value1 = null;
            AbstractObjectInfo value2 = null;
            int index = 0;
            while (keys1.MoveNext())
            {
                keys2.MoveNext();
                key1 = keys1.Current;
                key2 = keys2.Current;
                bool keysHaveChanged = HasChanged(key1, key2, objectRecursionLevel);
                if (keysHaveChanged)
                {
                    StoreChangedObject(nnoi1, nnoi2, fieldId, key1, key2, "Map key index " + index + " has changed",
                                       objectRecursionLevel);
                    return true;
                }
                value1 = map1[key1];
                value2 = map2[key2];
                bool valuesHaveChanged = HasChanged(value1, value2, objectRecursionLevel);
                if (valuesHaveChanged)
                {
                    StoreChangedObject(nnoi1, nnoi2, fieldId, value1, value2,
                                       "Map value index " + index + " has changed", objectRecursionLevel);
                    return true;
                }
                index++;
            }
            return false;
*/
        }

        protected virtual void StoreChangedAttributeAction(ChangedNativeAttributeAction caa)
        {
            _nbChanges++;
            _changedAttributeActions.Add(caa);
        }

        protected virtual void StoreChangedAttributeAction(ChangedObjectReferenceAttributeAction caa)
        {
            _nbChanges++;
            _changedAttributeActions.Add(caa);
        }

        public virtual void SetChangedAttributeActions(IList<IChangedAttribute> changedAttributeActions)
        {
            _changedAttributeActions = changedAttributeActions;
        }

        public override string ToString()
        {
            return string.Format("{0} changes", _nbChanges);
        }
    }
}
