using System.Collections.Generic;
using NDatabase.Odb.Core;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Transaction;
using NDatabase.Tool.Wrappers.Map;

namespace NDatabase.Odb.Impl.Core.Transaction
{
    /// <summary>
    ///   A temporary cache of objects.
    /// </summary>
    /// <remarks>
    ///   A temporary cache of objects.
    /// </remarks>
    /// <author>olivier s</author>
    public sealed class TmpCache : ITmpCache
    {
        /// <summary>
        ///   To resolve cyclic reference, keep track of objects being read
        /// </summary>
        private readonly IDictionary<OID, object[]> _readingObjectInfo;

        public TmpCache()
        {
            _readingObjectInfo = new OdbHashMap<OID, object[]>();
        }

        #region ITmpCache Members

        public bool IsReadingObjectInfoWithOid(OID oid)
        {
            if (oid == null)
                return false;
            return _readingObjectInfo.ContainsKey(oid);
        }

        public NonNativeObjectInfo GetReadingObjectInfoFromOid(OID oid)
        {
            if (oid == null)
                throw new OdbRuntimeException(NDatabaseError.CacheNullOid);

            object[] values;
            _readingObjectInfo.TryGetValue(oid, out values);

            if (values == null)
                return null;

            return (NonNativeObjectInfo) values[1];
        }

        public void StartReadingObjectInfoWithOid(OID oid, NonNativeObjectInfo objectInfo)
        {
            if (oid == null)
                throw new OdbRuntimeException(NDatabaseError.CacheNullOid);

            object[] objects;
            _readingObjectInfo.TryGetValue(oid, out objects);

            // TODO : use a value object instead of an array!
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
        public void ClearObjectInfos()
        {
            _readingObjectInfo.Clear();
        }

        public int Size()
        {
            return _readingObjectInfo.Count;
        }

        #endregion
    }
}
