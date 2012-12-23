using System.Collections.Generic;
using System.Text;

namespace NDatabase2.Tool.Wrappers.List
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

        public virtual bool AddAll(IEnumerable<TItem> collection)
        {
            AddRange(collection);
            return true;
        }

        public virtual bool RemoveAll(IEnumerable<TItem> collection)
        {
            foreach (var item in collection)
                Remove(item);
            return true;
        }

        public virtual TItem Get(int index)
        {
            return base[index];
        }

        public bool IsEmpty()
        {
            return Count == 0;
        }

        #endregion

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("[");

            foreach (var item in this)
                stringBuilder.Append(item.ToString() + ", ");

            if (stringBuilder.Length > 3)
                stringBuilder.Remove(stringBuilder.Length - 3, 2);

            stringBuilder.Append("]");

            return stringBuilder.ToString();
        }
    }
}