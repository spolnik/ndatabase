using System.Collections.Generic;

namespace NDatabase.Tool.Wrappers.List
{
    public class OdbList<TItem> : List<TItem>, IOdbList<TItem>
    {
        public OdbList()
        {
        }

        public OdbList(int size) : base(size)
        {
        }

        #region IOdbList<E> Members

        public virtual bool AddAll(ICollection<TItem> collection)
        {
            AddRange(collection);
            return true;
        }

        public virtual bool RemoveAll(ICollection<TItem> collection)
        {
            foreach (var item in collection)
                Remove(item);
            return true;
        }

        public virtual TItem Get(int index)
        {
            return base[index];
        }

        public virtual bool IsEmpty()
        {
            return Count == 0;
        }

        public void Set(int index, TItem element)
        {
            Insert(index, element);
        }

        #endregion
    }
}