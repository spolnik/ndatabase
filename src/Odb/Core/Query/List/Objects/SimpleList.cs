using System;
using System.Collections.Generic;
using System.Linq;
using NDatabase2.Tool.Wrappers;

namespace NDatabase2.Odb.Core.Query.List.Objects
{
    /// <summary>
    ///   A simple list to hold query result.
    /// </summary>
    /// <remarks>
    ///   A simple list to hold query result. It is used when no index and no order by is used and inMemory = true
    /// </remarks>
    internal class SimpleList<TItem> : List<TItem>, IInternalObjectSet<TItem>
    {
        private int _currentPosition;

        public SimpleList()
        {
        }

        protected SimpleList(int initialCapacity) : base(initialCapacity)
        {
        }

        #region IObjects<E> Members

        public virtual void AddWithKey(IOdbComparable key, TItem o)
        {
            Add(o);
        }

        public virtual bool AddWithKey(int key, TItem o)
        {
            Add(o);
            return true;
        }

        public virtual TItem GetFirst()
        {
            return Count == 0 ? default(TItem) : this[0];
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

        public IEnumerable<string> GetNames()
        {
            var names = new List<string>();
            var namePrefix = typeof(TItem).Name;

            for (var i = 0; i < Count; i++)
                names.Add(string.Format("{0}_{1}", namePrefix, i));

            return names;
        }

        public IEnumerable<Type> GetTypes()
        {
            var types = new List<Type>();
            for (var i = 0; i < Count; i++)
                types.Add(typeof(TItem));

            return types;
        }

        public IEnumerable<object> GetValues()
        {
            var values = this.Select(item => (object)item).ToList();
            return values;
        }
    }
}
