using System.Collections.Generic;
using NDatabase.Tool.Wrappers;

namespace NDatabase.Odb.Core.Query.List.Objects
{
    /// <summary>
    ///   A simple list to hold query result.
    /// </summary>
    /// <remarks>
    ///   A simple list to hold query result. It is used when no index and no order by is used and inMemory = true
    /// </remarks>
    /// <author>osmadja</author>
    
    public class SimpleList<TItem> : List<TItem>, IObjects<TItem>
    {
        private int _currentPosition;

        public SimpleList()
        {
        }

        public SimpleList(int initialCapacity) : base(initialCapacity)
        {
        }

        #region IObjects<E> Members

        public virtual bool AddWithKey(IOdbComparable key, TItem o)
        {
            Add(o);
            return true;
        }

        public virtual bool AddWithKey(int key, TItem o)
        {
            Add(o);
            return true;
        }

        public virtual TItem GetFirst()
        {
            return this[0];
        }

        public virtual bool HasNext()
        {
            return _currentPosition < Count;
        }

        /// <summary>
        ///   The orderByType in not supported by this kind of list
        /// </summary>
        public virtual IEnumerator<TItem> Iterator(OrderByConstants orderByType)
        {
            return GetEnumerator();
        }

        public virtual TItem Next()
        {
            return this[_currentPosition++];
        }

        public virtual void Reset()
        {
            _currentPosition = 0;
        }

        public void AddOid(OID oid)
        {
            throw new OdbRuntimeException(NDatabaseError.InternalError.AddParameter("Add Oid not implemented "));
        }

        #endregion
    }
}
