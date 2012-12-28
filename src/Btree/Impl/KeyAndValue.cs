using System;
using System.Text;

namespace NDatabase.Btree.Impl
{
    
    public sealed class KeyAndValue : IKeyAndValue
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

        public IComparable GetKey()
        {
            return _key;
        }

        public void SetKey(IComparable key)
        {
            _key = key;
        }

        public object GetValue()
        {
            return _value;
        }

        public void SetValue(object value)
        {
            _value = value;
        }

        #endregion
    }
}
