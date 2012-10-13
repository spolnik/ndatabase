using System;

namespace NDatabase2.Btree
{
    /// <author>olivier</author>
    public interface IKeyAndValue
    {
        string ToString();

        IComparable GetKey();

        void SetKey(IComparable key);

        object GetValue();

        void SetValue(object value);
    }
}