using System.Collections;

namespace NDatabase2.Btree
{
    public abstract class IterarorAdapter : IEnumerator
    {
        #region IEnumerator Members

        public object Current
        {
            get { return GetCurrent(); }
        }

        public abstract bool MoveNext();
        public abstract void Reset();

        #endregion

        public abstract object GetCurrent();
    }
}