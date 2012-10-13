using System.Collections;
using System.Collections.Generic;
using System.Text;
using NDatabase2.Btree;
using NDatabase2.Odb.Core.Layers.Layer1.Introspector;
using NDatabase2.Tool.Wrappers;

namespace NDatabase2.Odb.Core.Query.List
{
    /// <summary>
    ///   A collection that uses a BTree as an underlying system to provide ordered by Collections <p></p>
    /// </summary>
    
    public abstract class AbstractBTreeCollection<TItem> : IObjects<TItem>
    {
        private readonly OrderByConstants _orderByType;
        private readonly IBTree _tree;

        [NonPersistent]
        private IEnumerator<TItem> _currentIterator;

        private int _size;

        protected AbstractBTreeCollection(OrderByConstants orderByType)
        {
            // TODO compute degree best value for the size value
            _tree = BuildTree(OdbConfiguration.GetDefaultIndexBTreeDegree());
            _orderByType = orderByType;
        }

        protected AbstractBTreeCollection() : this(OrderByConstants.OrderByNone)
        {
        }

        #region IObjects<TItem> Members

        public virtual TItem GetFirst()
        {
            return Iterator(_orderByType).Current;
        }

        public virtual bool HasNext()
        {
            if (_currentIterator == null)
                _currentIterator = Iterator(_orderByType);
            return _currentIterator.MoveNext();
        }

        public virtual TItem Next()
        {
            if (_currentIterator == null)
                _currentIterator = Iterator(_orderByType);
            return _currentIterator.Current;
        }

        public void Add(TItem o)
        {
            _tree.Insert(_size, o);
            _size++;
        }

        /// <summary>
        ///   Adds the object in the btree with the specific key
        /// </summary>
        /// <param name="key"> </param>
        /// <param name="o"> </param>
        /// <returns> </returns>
        public virtual bool AddWithKey(IOdbComparable key, TItem o)
        {
            _tree.Insert(key, o);
            _size++;
            return true;
        }

        /// <summary>
        ///   Adds the object in the btree with the specific key
        /// </summary>
        /// <param name="key"> </param>
        /// <param name="o"> </param>
        /// <returns> </returns>
        public virtual bool AddWithKey(int key, TItem o)
        {
            _tree.Insert(key, o);
            _size++;
            return true;
        }

        public virtual void Clear()
        {
            _tree.Clear();
        }

        public virtual bool Contains(TItem o)
        {
            throw new OdbRuntimeException(NDatabaseError.OperationNotImplemented.AddParameter("contains"));
        }

        public virtual IEnumerator<TItem> GetEnumerator()
        {
            return Iterator(_orderByType);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _tree.Iterator<TItem>(_orderByType);
        }

        public virtual IEnumerator<TItem> Iterator(OrderByConstants newOrderByType)
        {
            return (IEnumerator<TItem>) _tree.Iterator<TItem>(newOrderByType);
        }

        public virtual bool Remove(TItem o)
        {
            throw new OdbRuntimeException(NDatabaseError.OperationNotImplemented.AddParameter("remove"));
        }

        public virtual int Count
        {
            get { return _size; }
        }

        public virtual bool IsReadOnly
        {
            get { return true; }
        }

        public virtual void Reset()
        {
            _currentIterator = Iterator(_orderByType);
        }

        public void CopyTo(TItem[] ee, int arrayIndex)
        {
            throw new OdbRuntimeException(NDatabaseError.OperationNotImplemented.AddParameter("CopyTo"));
        }

        public void AddOid(OID oid)
        {
            throw new OdbRuntimeException(NDatabaseError.InternalError.AddParameter("Add Oid not implemented "));
        }

        #endregion

        public abstract IBTree BuildTree(int degree);

        public virtual bool AddAll(ICollection<TItem> collection)
        {
            var iterator = collection.GetEnumerator();
            while (iterator.MoveNext())
                Add(iterator.Current);
            return true;
        }

        public virtual bool ContainsAll(ICollection collection)
        {
            throw new OdbRuntimeException(NDatabaseError.OperationNotImplemented.AddParameter("containsAll"));
        }

        public virtual bool IsEmpty()
        {
            return _size == 0;
        }

        public virtual bool RemoveAll(ICollection collection)
        {
            throw new OdbRuntimeException(NDatabaseError.OperationNotImplemented.AddParameter("removeAll"));
        }

        public virtual bool RetainAll(ICollection collection)
        {
            throw new OdbRuntimeException(NDatabaseError.OperationNotImplemented.AddParameter("retainAll"));
        }

        public virtual object[] ToArray()
        {
            return ToArray(new object[_size]);
        }

        public virtual object[] ToArray(object[] objects)
        {
            IEnumerator iterator = GetEnumerator();
            var i = 0;
            while (iterator.MoveNext())
                objects[i++] = iterator.Current;
            return objects;
        }

        protected virtual OrderByConstants GetOrderByType()
        {
            return _orderByType;
        }

        protected virtual IBTree GetTree()
        {
            return _tree;
        }

        public override string ToString()
        {
            var s = new StringBuilder();
            s.Append("size=").Append(_size).Append(" [");
            var iterator = GetEnumerator();
            while (iterator.MoveNext())
            {
                s.Append(iterator.Current);
                if (iterator.MoveNext())
                    s.Append(" , ");
            }
            s.Append("]");
            return s.ToString();
        }
    }
}
