using System;
using System.Collections.Generic;
using System.Text;
using NDatabase.Odb.Core;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Transaction;
using NDatabase.Odb.Impl.Core.Layers.Layer3.Engine;
using NDatabase.Odb.Impl.Core.Layers.Layer3.Oid;
using NDatabase.Tool.Wrappers.Map;

namespace NDatabase.Odb.Impl.Core.Transaction
{
    /// <summary>
    ///   A cache of object.
    /// </summary>
    /// <remarks>
    ///   A cache of object. <pre>Cache objects by object, by position, by oids,...</pre>
    /// </remarks>
    /// <author>olivier s</author>
    public sealed class Cache : ICache
    {
        private static int _nbObjects;
        private static int _nbOids;
        private static int _nbOih;
        private static int _nbCallsToGetObjectInfoHeaderFromOid;
        private static int _nbCallsToGetObjectInfoHeaderFromObject;
        private static int _nbCallsToGetObjectWithOid;

        /// <summary>
        ///   To resolve cyclic reference, keep track of objects being inserted
        /// </summary>
        private IDictionary<object, ObjectInsertingInfo> _insertingObjects;

        private readonly string _name;

        /// <summary>
        ///   Entry to get object info pointers (position,next object pos, previous object pos and class info pos) from the id
        /// </summary>
        private IDictionary<OID, ObjectInfoHeader> _objectInfoPointersCacheFromOid;

        /// <summary>
        ///   <pre>To resolve the update of an id object position:
        ///     When an object is full updated(the current object is being deleted and a new one os being created),
        ///     the id remain the same but its position change.</pre>
        /// </summary>
        /// <remarks>
        ///   <pre>To resolve the update of an id object position:
        ///     When an object is full updated(the current object is being deleted and a new one os being created),
        ///     the id remain the same but its position change.
        ///     But the update is done in transaction, so it is not flushed until the commit happens
        ///     So after the update when i need the position to make the old object a pointer, i have no way to get
        ///     the right position. To resolve this, i keep a cache of ids where i keep the non commited value</pre>
        /// </remarks>
        private IDictionary<OID, IdInfo> _objectPositionsByIds;

        /// <summary>
        ///   object cache - used to know if object exist in the cache TODO use hashcode instead?
        /// </summary>
        private IDictionary<object, OID> _objects;

        /// <summary>
        ///   Entry to get an object from its oid
        /// </summary>
        private IDictionary<OID, object> _oids;

        /// <summary>
        ///   To resolve cyclic reference, keep track of objects being read
        /// </summary>
        private IDictionary<OID, object[]> _readingObjectInfo;

        /// <summary>
        ///   To keep track of the oid that have been created or modified in the current transaction
        /// </summary>
        private IDictionary<OID, OID> _unconnectedZoneOids;

        public Cache(string name)
        {
            _name = name;
            _objects = new OdbHashMap<object, OID>();
            _oids = new OdbHashMap<OID, object>();
            _unconnectedZoneOids = new OdbHashMap<OID, OID>();
            _objectInfoPointersCacheFromOid = new OdbHashMap<OID, ObjectInfoHeader>();
            _insertingObjects = new OdbHashMap<object, ObjectInsertingInfo>();
            _readingObjectInfo = new OdbHashMap<OID, object[]>();
            _objectPositionsByIds = new OdbHashMap<OID, IdInfo>();
        }

        public string Name
        {
            get { return _name; }
        }

        #region ICache Members

        public void AddObject(OID oid, object o, ObjectInfoHeader objectInfoHeader)
        {
            if (oid == null)
                throw new OdbRuntimeException(NDatabaseError.CacheNullOid);

            if (CheckHeaderPosition() && objectInfoHeader.GetPosition() == -1)
                throw new OdbRuntimeException(
                    NDatabaseError.CacheNegativePosition.AddParameter("Adding OIH with position = -1"));

            // TODO : Should remove first inserted object and not clear all cache
            if (_objects.Count > OdbConfiguration.GetMaxNumberOfObjectInCache())
            {
                // clear();
                ManageFullCache();
            }

            _oids[oid] = o;
            try
            {
                _objects[o] = oid;
            }
            catch (ArgumentNullException)
            {
            }
            // FIXME URL in HashMap What should we do?
            // In some case, the object can throw exception when added to the
            // cache
            // because Map.put, end up calling the equals method that can throw
            // exception
            // This is the case of URL that has a transient attribute handler
            // that is used in the URL.equals method
            _objectInfoPointersCacheFromOid[oid] = objectInfoHeader;
            // For monitoring purpose
            _nbObjects = _objects.Count;
            _nbOids = _oids.Count;
            _nbOih = _objectInfoPointersCacheFromOid.Count;
        }

        /// <summary>
        ///   Only adds the Object info - used for non committed objects
        /// </summary>
        public void AddObjectInfo(ObjectInfoHeader objectInfoHeader)
        {
            if (objectInfoHeader.GetOid() == null)
                throw new OdbRuntimeException(NDatabaseError.CacheNullOid);

            if (objectInfoHeader.GetClassInfoId() == null)
            {
                throw new OdbRuntimeException(
                    NDatabaseError.CacheObjectInfoHeaderWithoutClassId.AddParameter(objectInfoHeader.GetOid()));
            }

            // TODO : Should remove first inserted object and not clear all cache
            if (_objectInfoPointersCacheFromOid.Count > OdbConfiguration.GetMaxNumberOfObjectInCache())
                ManageFullCache();

            _objectInfoPointersCacheFromOid[objectInfoHeader.GetOid()] = objectInfoHeader;
            // For monitoring purpose
            _nbObjects = _objects.Count;
            _nbOids = _oids.Count;
            _nbOih = _objectInfoPointersCacheFromOid.Count;
        }

        //throw new ODBRuntimeException(Error.CACHE_IS_FULL.addParameter(objectInfoPointersCacheFromOid.size()).addParameter(OdbConfiguration.getMaxNumberOfObjectInCache()));
        public void StartInsertingObjectWithOid(object o, OID oid, NonNativeObjectInfo nnoi)
        {
            // In this case oid can be -1,because object is beeing inserted and do
            // not have yet a defined oid.
            if (o == null)
                return;

            ObjectInsertingInfo objectInsertingInfo;
            _insertingObjects.TryGetValue(o, out objectInsertingInfo);

            if (objectInsertingInfo == null)
                _insertingObjects[o] = new ObjectInsertingInfo(oid, 1);
            else
                objectInsertingInfo.Level++;
        }

        // No need to update the map, it is a reference.
        public void UpdateIdOfInsertingObject(object o, OID oid)
        {
            if (oid == null)
                throw new OdbRuntimeException(NDatabaseError.CacheNullOid);

            var objectInsertingInfo = _insertingObjects[o];
            if (objectInsertingInfo != null)
                objectInsertingInfo.OID = oid;
        }

        public void EndInsertingObject(object o)
        {
            var objectInsertingInfo = _insertingObjects[o];

            if (objectInsertingInfo.Level == 1)
                _insertingObjects.Remove(o);
            else
                objectInsertingInfo.Level--;
        }

        public void RemoveObjectWithOid(OID oid)
        {
            if (oid == null)
                throw new OdbRuntimeException(NDatabaseError.CacheNullOid);

            object value;
            _oids.TryGetValue(oid, out value);

            _oids.Remove(oid);
            if (value != null)
                _objects.Remove(value);

            // FIXME URL in HashMap What should we do?
            _objectInfoPointersCacheFromOid.Remove(oid);
            _unconnectedZoneOids.Remove(oid);
            // For monitoring purpose
            _nbObjects = _objects.Count;
            _nbOids = _oids.Count;
            _nbOih = _objectInfoPointersCacheFromOid.Count;
        }

        public void RemoveObject(object o)
        {
            if (o == null)
                throw new OdbRuntimeException(
                    NDatabaseError.CacheNullObject.AddParameter(" while removing object from the cache"));

            OID oid;
            _objects.TryGetValue(o, out oid);
            _oids.Remove(oid);

            try
            {
                _objects.Remove(o);
            }
            catch (ArgumentNullException)
            {
            }

            // FIXME URL in HashMap What should we do?
            _objectInfoPointersCacheFromOid.Remove(oid);
            _unconnectedZoneOids.Remove(oid);
            // For monitoring purpose
            _nbObjects = _objects.Count;
            _nbOids = _oids.Count;
            _nbOih = _objectInfoPointersCacheFromOid.Count;
        }

        public bool ExistObject(object @object)
        {
            return _objects.ContainsKey(@object);
        }

        public object GetObjectWithOid(OID oid)
        {
            if (oid == null)
                throw new OdbRuntimeException(NDatabaseError.CacheNullOid.AddParameter("oid"));

            object value;
            _oids.TryGetValue(oid, out value);
            _nbCallsToGetObjectWithOid++;
            return value;
        }

        public ObjectInfoHeader GetObjectInfoHeaderFromObject(object o, bool throwExceptionIfNotFound)
        {
            OID oid;
            _objects.TryGetValue(o, out oid);

            ObjectInfoHeader objectInfoHeader;
            _objectInfoPointersCacheFromOid.TryGetValue(oid, out objectInfoHeader);
            if (objectInfoHeader == null && throwExceptionIfNotFound)
                throw new OdbRuntimeException(NDatabaseError.ObjectDoesNotExistInCache.AddParameter(o.ToString()));

            _nbCallsToGetObjectInfoHeaderFromObject++;
            return objectInfoHeader;
        }

        public ObjectInfoHeader GetObjectInfoHeaderFromOid(OID oid, bool throwExceptionIfNotFound)
        {
            if (oid == null)
                throw new OdbRuntimeException(NDatabaseError.CacheNullOid);

            ObjectInfoHeader objectInfoHeader;
            _objectInfoPointersCacheFromOid.TryGetValue(oid, out objectInfoHeader);
            if (objectInfoHeader == null && throwExceptionIfNotFound)
                throw new OdbRuntimeException(NDatabaseError.ObjectWithOidDoesNotExistInCache.AddParameter(oid));

            _nbCallsToGetObjectInfoHeaderFromOid++;
            return objectInfoHeader;
        }

        public OID GetOid(object o, bool throwExceptionIfNotFound)
        {
            OID oid;
            _objects.TryGetValue(o, out oid);
            if (oid != null)
                return oid;

            if (throwExceptionIfNotFound)
                throw new OdbRuntimeException(NDatabaseError.ObjectDoesNotExistInCache);

            return StorageEngineConstant.NullObjectId;
        }

        public void SavePositionOfObjectWithOid(OID oid, long objectPosition)
        {
            if (oid == null)
                throw new OdbRuntimeException(NDatabaseError.CacheNullOid);

            var idInfo = new IdInfo(oid, objectPosition, IDStatus.Active);
            _objectPositionsByIds[oid] = idInfo;
            // For monitoring purpose
            _nbObjects = _objects.Count;
            _nbOids = _oids.Count;
            _nbOih = _objectInfoPointersCacheFromOid.Count;
        }

        public void MarkIdAsDeleted(OID oid)
        {
            if (oid == null)
                throw new OdbRuntimeException(NDatabaseError.CacheNullOid);

            IdInfo idInfo;

            _objectPositionsByIds.TryGetValue(oid, out idInfo);
            if (idInfo != null)
                idInfo.Status = IDStatus.Deleted;
            else
            {
                idInfo = new IdInfo(oid, -1, IDStatus.Deleted);
                _objectPositionsByIds[oid] = idInfo;
            }
        }

        public bool IsDeleted(OID oid)
        {
            if (oid == null)
                throw new OdbRuntimeException(NDatabaseError.CacheNullOid);

            IdInfo idInfo;
            _objectPositionsByIds.TryGetValue(oid, out idInfo);
            if (idInfo != null)
                return idInfo.Status == IDStatus.Deleted;

            return false;
        }

        /// <summary>
        ///   Returns the position or -1 if it is not is the cache or StorageEngineConstant.NULL_OBJECT_ID_ID if it has been marked as deleted
        /// </summary>
        public long GetObjectPositionByOid(OID oid)
        {
            if (oid == null)
                return StorageEngineConstant.NullObjectIdId;

            IdInfo idInfo;
            _objectPositionsByIds.TryGetValue(oid, out idInfo);

            if (idInfo != null)
            {
                if (!IDStatus.IsActive(idInfo.Status))
                    return StorageEngineConstant.DeletedObjectPosition;
                return idInfo.Position;
            }

            // object is not in the cache
            return StorageEngineConstant.ObjectIsNotInCache;
        }

        public void ClearOnCommit()
        {
            _objectPositionsByIds.Clear();
            _unconnectedZoneOids.Clear();
        }

        public void Clear(bool setToNull)
        {
            if (_objects != null)
            {
                _objects.Clear();
                _oids.Clear();
                _objectInfoPointersCacheFromOid.Clear();
                _insertingObjects.Clear();
                _objectPositionsByIds.Clear();
                _readingObjectInfo.Clear();
                _unconnectedZoneOids.Clear();
            }

            if (setToNull)
            {
                _objects = null;
                _oids = null;
                _objectInfoPointersCacheFromOid = null;
                _insertingObjects = null;
                _objectPositionsByIds = null;
                _readingObjectInfo = null;
                _unconnectedZoneOids = null;
            }
        }

        public void ClearInsertingObjects()
        {
            _insertingObjects.Clear();
        }

        public override string ToString()
        {
            var buffer = new StringBuilder();
            buffer.Append("C=");
            buffer.Append(_objects.Count).Append(" objects ");
            buffer.Append(_oids.Count).Append(" oids ");
            buffer.Append(_objectInfoPointersCacheFromOid.Count).Append(" pointers");
            buffer.Append(_objectPositionsByIds.Count).Append(" pos by oid");
            // buffer.append(insertingObjects.size()).append(" inserting
            // objects\n");
            // buffer.append(readingObjectInfo.size()).append(" reading objects");
            return buffer.ToString();
        }

        public string ToCompleteString()
        {
            var buffer = new StringBuilder();
            buffer.Append(_objects.Count).Append(" Objects=").Append(_objects).Append("\n");
            buffer.Append(_oids.Count).Append(" Objects from pos").Append(_oids).Append("\n");
            buffer.Append(_objectInfoPointersCacheFromOid.Count).Append(" Pointers=").Append(
                _objectInfoPointersCacheFromOid);
            return buffer.ToString();
        }

        public int GetNumberOfObjects()
        {
            return _objects.Count;
        }

        public int GetNumberOfObjectHeader()
        {
            return _objectInfoPointersCacheFromOid.Count;
        }

        public OID IdOfInsertingObject(object o)
        {
            if (o == null)
                return StorageEngineConstant.NullObjectId;

            ObjectInsertingInfo objectInsertingInfo;

            _insertingObjects.TryGetValue(o, out objectInsertingInfo);

            if (objectInsertingInfo != null)
                return objectInsertingInfo.OID;
            return StorageEngineConstant.NullObjectId;
        }

        public int InsertingLevelOf(object o)
        {
            ObjectInsertingInfo objectInsertingInfo;

            _insertingObjects.TryGetValue(o, out objectInsertingInfo);
            if (objectInsertingInfo == null)
                return 0;
            return objectInsertingInfo.Level;
        }

        public bool IsReadingObjectInfoWithOid(OID oid)
        {
            if (oid == null)
                return false;
            // throw new
            // ODBRuntimeException(Error.CACHE_NULL_OID);
            return _readingObjectInfo[oid] != null;
        }

        public NonNativeObjectInfo GetReadingObjectInfoFromOid(OID oid)
        {
            if (oid == null)
                throw new OdbRuntimeException(NDatabaseError.CacheNullOid);

            var values = _readingObjectInfo[oid];
            if (values == null)
                return null;
            return (NonNativeObjectInfo) values[1];
        }

        public void StartReadingObjectInfoWithOid(OID oid, NonNativeObjectInfo objectInfo)
        {
            if (oid == null)
                throw new OdbRuntimeException(NDatabaseError.CacheNullOid);
            var objects = _readingObjectInfo[oid];
            if (objects == null)
            {
                // The key is the oid, the value is an array of 2 objects :
                // 1-the read count, 2-The object info
                // Here we are saying that the object with oid 'oid' is
                // being read for the first time
                var values = new object[] {(short) 1, objectInfo};
                _readingObjectInfo[oid] = values;
            }
            else
            {
                // Here the object is already being read. It is necessary to
                // increase the read count
                var currentReadCount = ((short) objects[0]);
                objects[0] = (short) (currentReadCount + 1);
            }
        }

        // Object is in memory, do not need to re-put in map. The key has
        // not changed
        // readingObjectInfo.put(oid, objects);
        public void EndReadingObjectInfo(OID oid)
        {
            if (oid == null)
                throw new OdbRuntimeException(NDatabaseError.CacheNullOid.AddParameter("oid"));
            var values = _readingObjectInfo[oid];
            if (values == null || values[0] == null)
                throw new OdbRuntimeException(NDatabaseError.ObjectInfoNotInTempCache.AddParameter(oid).AddParameter("?"));
            var readCount = ((short) values[0]);
            if (readCount == 1)
                _readingObjectInfo.Remove(oid);
            else
                values[0] = (short) (readCount - 1);
        }

        // Object is in memory, do not need to re-put in map. The key has
        // not changed
        // readingObjectInfo.put(oid, values);
        public IDictionary<OID, object> GetOids()
        {
            return _oids;
        }

        public IDictionary<OID, ObjectInfoHeader> GetObjectInfoPointersCacheFromOid()
        {
            return _objectInfoPointersCacheFromOid;
        }

        public IDictionary<object, OID> GetObjects()
        {
            return _objects;
        }

        public bool ObjectWithIdIsInCommitedZone(OID oid)
        {
            return !_unconnectedZoneOids.ContainsKey(oid);
        }

        public void AddOIDToUnconnectedZone(OID oid)
        {
            _unconnectedZoneOids.Add(oid, oid);
        }

        #endregion

        private static void ManageFullCache()
        {
            if (OdbConfiguration.AutomaticallyIncreaseCacheSize())
                OdbConfiguration.SetMaxNumberOfObjectInCache(
                    (long) (OdbConfiguration.GetMaxNumberOfObjectInCache() * 1.2));
        }

        public void SetOids(IDictionary<OID, object> oids)
        {
            _oids = oids;
        }

        public void SetObjectInfoPointersCacheFromOid(IDictionary<OID, ObjectInfoHeader> objectInfoPointersCacheFromOid)
        {
            _objectInfoPointersCacheFromOid = objectInfoPointersCacheFromOid;
        }

        public void SetObjects(IDictionary<object, OID> objects)
        {
            _objects = objects;
        }

        public static string Usage()
        {
            var buffer = new StringBuilder();
            buffer.Append("NbObj=").Append(_nbObjects);
            buffer.Append(" - NbOIDs=").Append(_nbOids);
            buffer.Append(" - NbOIHs=").Append(_nbOih);
            buffer.Append(" - Calls2getObjectWitOid=").Append(_nbCallsToGetObjectWithOid);
            buffer.Append(" - Calls2getObjectInfoHeaderFromOid=").Append(_nbCallsToGetObjectInfoHeaderFromOid);
            buffer.Append(" - Calls2getObjectInfoHeaderFromObject=").Append(_nbCallsToGetObjectInfoHeaderFromObject);
            return buffer.ToString();
        }

        private static bool CheckHeaderPosition()
        {
            return false;
        }
    }
}
