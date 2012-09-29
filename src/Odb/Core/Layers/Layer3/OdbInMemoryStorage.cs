using System.Collections.Generic;
using System.Text;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer3.Engine;
using NDatabase.Odb.Core.Layers.Layer3.Oid;
using NDatabase.Odb.Core.Transaction;
using NDatabase.Tool.Wrappers.Map;

namespace NDatabase.Odb.Core.Layers.Layer3
{
    /// <summary>
    ///   A cache of objects.
    /// </summary>
    public sealed class OdbInMemoryStorage : IOdbInMemoryStorage
    {
        /// <summary>
        ///   To resolve cyclic reference, keep track of objects being inserted
        /// </summary>
        private IDictionary<object, ObjectInsertingInfo> _insertingObjects;

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

        public OdbInMemoryStorage()
        {
            _objects = new OdbHashMap<object, OID>();
            _oids = new OdbHashMap<OID, object>();
            _unconnectedZoneOids = new OdbHashMap<OID, OID>();
            _objectInfoPointersCacheFromOid = new OdbHashMap<OID, ObjectInfoHeader>();
            _insertingObjects = new OdbHashMap<object, ObjectInsertingInfo>();
            _readingObjectInfo = new OdbHashMap<OID, object[]>();
            _objectPositionsByIds = new OdbHashMap<OID, IdInfo>();
        }

        #region IInMemoryStorage Members

        public void AddObject(OID oid, object o, ObjectInfoHeader objectInfoHeader)
        {
            if (oid == null)
                throw new OdbRuntimeException(NDatabaseError.CacheNullOid);

            _oids[oid] = o;
            _objects[o] = oid;
            _objectInfoPointersCacheFromOid[oid] = objectInfoHeader;
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

            _objectInfoPointersCacheFromOid[objectInfoHeader.GetOid()] = objectInfoHeader;
            // For monitoring purpose
        }

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
        }

        public void RemoveObject(object o)
        {
            if (o == null)
                throw new OdbRuntimeException(
                    NDatabaseError.CacheNullObject.AddParameter(" while removing object from the cache"));

            OID oid;
            var success = _objects.TryGetValue(o, out oid);
            if (success)
            {
                _oids.Remove(oid);
                _objects.Remove(o);
            }

            // FIXME URL in HashMap What should we do?
            _objectInfoPointersCacheFromOid.Remove(oid);
            _unconnectedZoneOids.Remove(oid);
            // For monitoring purpose
        }

        public bool Contains(object @object)
        {
            return _objects.ContainsKey(@object);
        }

        public object GetObject(OID oid)
        {
            if (oid == null)
                throw new OdbRuntimeException(NDatabaseError.CacheNullOid.AddParameter("oid"));

            object value;
            _oids.TryGetValue(oid, out value);
            
            return value;
        }

        public ObjectInfoHeader GetObjectInfoHeaderFromObject(object o)
        {
            OID oid;
            _objects.TryGetValue(o, out oid);

            ObjectInfoHeader objectInfoHeader;
            _objectInfoPointersCacheFromOid.TryGetValue(oid, out objectInfoHeader);
            
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

            return objectInfoHeader;
        }

        public OID GetOid(object o)
        {
            OID oid;
            _objects.TryGetValue(o, out oid);
            
            return oid ?? StorageEngineConstant.NullObjectId;
        }

        public void SavePositionOfObjectWithOid(OID oid, long objectPosition)
        {
            if (oid == null)
                throw new OdbRuntimeException(NDatabaseError.CacheNullOid);

            var idInfo = new IdInfo(oid, objectPosition, IDStatus.Active);
            _objectPositionsByIds[oid] = idInfo;
            // For monitoring purpose
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

            if (idInfo == null)
                return StorageEngineConstant.ObjectIsNotInCache;

            // object is not in the cache
            return !IDStatus.IsActive(idInfo.Status)
                       ? StorageEngineConstant.DeletedObjectPosition
                       : idInfo.Position;
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

        public OID IdOfInsertingObject(object o)
        {
            if (o == null)
                return StorageEngineConstant.NullObjectId;

            ObjectInsertingInfo objectInsertingInfo;

            _insertingObjects.TryGetValue(o, out objectInsertingInfo);

            return objectInsertingInfo != null
                       ? objectInsertingInfo.OID
                       : StorageEngineConstant.NullObjectId;
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

        public override string ToString()
        {
            var buffer = new StringBuilder();
            buffer.Append("C=");
            buffer.Append(_objects.Count).Append(" objects ");
            buffer.Append(_oids.Count).Append(" oids ");
            buffer.Append(_objectInfoPointersCacheFromOid.Count).Append(" pointers");
            buffer.Append(_objectPositionsByIds.Count).Append(" pos by oid");
            return buffer.ToString();
        }
    }
}
