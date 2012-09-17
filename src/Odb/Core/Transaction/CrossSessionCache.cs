using System;
using System.Collections.Generic;
using System.Linq;
using NDatabase.Tool.Wrappers.Map;

namespace NDatabase.Odb.Core.Transaction
{
    /// <summary>
    ///   A cache that survives the sessions.
    /// </summary>
    /// <remarks>
    ///   A cache that survives the sessions. It is uses to automatically reconnect object to sessions
    /// </remarks>
    public sealed class CrossSessionCache : ICrossSessionCache
    {
        /// <summary>
        ///   To keep track of all caches
        /// </summary>
        private static readonly IDictionary<string, ICrossSessionCache> Instances =
            new OdbHashMap<string, ICrossSessionCache>();

        /// <summary>
        ///   When objects are deleted by oid, the cost is too high to search the object by the oid, so we just keep the deleted oid, and when looking for an object, check if the oid if is the deleted oids, if yes, return null and delete the object
        /// </summary>
        private readonly IDictionary<OID, OID> _deletedOids;

        /// <summary>
        ///   The cache for NDatabase OID.
        /// </summary>
        /// <remarks>
        ///   The cache for NDatabase OID. This cache supports a weak reference and it is sync
        /// </remarks>
        private readonly IDictionary<object, OID> _objects;

        /// <summary>
        ///   Protected constructor for factory-based construction
        /// </summary>
        private CrossSessionCache()
        {
            _objects = new Dictionary<object, OID>();
            _deletedOids = new Dictionary<OID, OID>();
        }

        #region ICrossSessionCache Members

        public void AddObject(object o, OID oid)
        {
            if (o == null)
                return;
            // throw new
            // ODBRuntimeException(NDatabaseError.CACHE_NULL_OBJECT.addParameter(object));
            try
            {
                _objects.Add(o, oid);
            }
            catch (ArgumentNullException)
            {
            }
        }

        // FIXME URL in HashMap What should we do?
        // In some case, the object can throw exception when added to the
        // cache
        // because Map.put, end up calling the equals method that can throw
        // exception
        // This is the case of URL that has a transient attribute handler
        // that is used in the URL.equals method
        public void Clear()
        {
            _objects.Clear();
            _deletedOids.Clear();
        }

        public bool ExistObject(object o)
        {
            if (o == null)
                return false;

            var oid = _objects[o];
            // Then check if oid is in the deleted oid list
            if (_deletedOids.ContainsKey(oid))
            {
                // The object has been marked as deleted
                // removes it from the cache
                _objects.Remove(o);
                _deletedOids.Remove(oid);
                return false;
            }

            return true;
        }

        public OID GetOid(object o)
        {
            if (o == null)
                throw new OdbRuntimeException(NDatabaseError.CacheNullObject.AddParameter("o"));

            var oid = _objects[o];

            if (oid != null)
            {
                if (_deletedOids.ContainsKey(oid))
                {
                    // The object has been marked as deleted
                    // removes it from the cache
                    _objects.Remove(o);
                    _deletedOids.Remove(oid);
                    return null;
                }
                return oid;
            }
            return null;
        }

        public bool IsEmpty()
        {
            return _objects.Count == 0;
        }

        public void RemoveObject(object o)
        {
            if (o == null)
                throw new OdbRuntimeException(
                    NDatabaseError.CacheNullObject.AddParameter(" while removing object from the cache"));

            var oid = _objects[o];
            _objects.Remove(o);
            if (oid != null)
            {
                // Add the oid to deleted oid
                // see junit org.neodatis.odb.test.fromusers.gyowanny_queiroz.TestBigDecimal.test13
                _deletedOids.Add(oid, oid);
            }
        }

        public void RemoveOid(OID oid)
        {
            _deletedOids.Add(oid, oid);
        }

        public int Size()
        {
            return _objects.Count;
        }

        public override string ToString()
        {
            return string.Format("Cross session cache with {0} objects", _objects.Count);
        }

        #endregion

        /// <summary>
        ///   Gets the unique instance for the cache for the identification
        /// </summary>
        public static ICrossSessionCache GetInstance(string baseIdentification)
        {
            ICrossSessionCache cache;
            Instances.TryGetValue(baseIdentification, out cache);

            if (cache == null)
            {
                lock (Instances)
                {
                    cache = new CrossSessionCache();
                    Instances[baseIdentification] = cache;
                }
            }

            return cache;
        }

        public static void ClearAll()
        {
            foreach (var cache in Instances.Keys.Select(key => Instances[key]))
                cache.Clear();

            Instances.Clear();
        }
    }
}
