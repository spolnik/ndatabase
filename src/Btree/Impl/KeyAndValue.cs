using System;
using System.Text;

namespace NDatabase.Btree.Impl
{
    [Serializable]
    public class KeyAndValue : IKeyAndValue
    {
        private IComparable _key;
        private object _value;

        public KeyAndValue(IComparable key, object value)
        {
            _key = key;
            _value = value;
        }

        #region IKeyAndValue Members

        public override string ToString()
        {
            return new StringBuilder("(").Append(_key).Append("=").Append(_value).Append(") ").ToString();
        }

        public virtual IComparable GetKey()
        {
            return _key;
        }

        public virtual void SetKey(IComparable key)
        {
            _key = key;
        }

        public virtual object GetValue()
        {
            return _value;
        }

        public virtual void SetValue(object value)
        {
            _value = value;
        }

        #endregion
    }
}
