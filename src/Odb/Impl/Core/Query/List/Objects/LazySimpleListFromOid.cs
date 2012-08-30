using System;
using System.Collections.Generic;
using NDatabase.Odb.Core;
using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Tool.Wrappers;
using NDatabase.Tool.Wrappers.List;

namespace NDatabase.Odb.Impl.Core.Query.List.Objects
{
    /// <summary>
    ///   A simple list to hold query result.
    /// </summary>
    /// <remarks>
    ///   A simple list to hold query result. It is used when no index and no order by is used and inMemory = false This collection does not store the objects, it only holds the OIDs of the objects. When user ask an object the object is lazy loaded by the getObjectFromId method
    /// </remarks>
    
    public sealed class LazySimpleListFromOid<T> : OdbArrayList<T>, IObjects<T>
    {
        /// <summary>
        ///   The odb engine to lazily get objects
        /// </summary>
        private readonly IStorageEngine _engine;

        private readonly OdbArrayList<OID> _oids;

        /// <summary>
        ///   indicate if objects must be returned as instance (true) or as non native objects (false)
        /// </summary>
        private readonly bool _returnInstance;

        /// <summary>
        ///   a cursor when getting objects
        /// </summary>
        private int _currentPosition;

        public LazySimpleListFromOid(IStorageEngine engine, bool returnObjects)
        {
            _engine = engine;
            _returnInstance = returnObjects;
            _oids = new OdbArrayList<OID>();
        }

        #region IObjects<T> Members

        public bool AddWithKey(IOdbComparable key, T @object)
        {
            throw new OdbRuntimeException(NDatabaseError.OperationNotImplemented);
        }

        public bool AddWithKey(int key, T @object)
        {
            throw new OdbRuntimeException(NDatabaseError.OperationNotImplemented);
        }

        public T GetFirst()
        {
            try
            {
                return Get(0);
            }
            catch (Exception e)
            {
                throw new OdbRuntimeException(NDatabaseError.ErrorWhileGettingObjectFromListAtIndex.AddParameter(0), e);
            }
        }

        public bool HasNext()
        {
            return _currentPosition < _oids.Count;
        }

        public IEnumerator<T> Iterator(OrderByConstants orderByType)
        {
            throw new OdbRuntimeException(NDatabaseError.OperationNotImplemented);
        }

        public new int Count
        {
            get { return _oids.Count; }
        }

        public T Next()
        {
            try
            {
                return Get(_currentPosition++);
            }
            catch (Exception e)
            {
                throw new OdbRuntimeException(NDatabaseError.ErrorWhileGettingObjectFromListAtIndex.AddParameter(0), e);
            }
        }

        public void Reset()
        {
            _currentPosition = 0;
        }

        public void AddOid(OID oid)
        {
            _oids.Add(oid);
        }

        #endregion

        public override T Get(int index)
        {
            var oid = _oids[index];
            try
            {
                if (_returnInstance)
                    return (T) _engine.GetObjectFromOid(oid);
                return (T) _engine.GetObjectReader().GetObjectFromOid(oid, false, false);
            }
            catch (Exception)
            {
                throw new OdbRuntimeException(NDatabaseError.ErrorWhileGettingObjectFromListAtIndex.AddParameter(index));
            }
        }
    }
}
