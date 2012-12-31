using System;

namespace NDatabase.Btree
{
    public interface IKeyAndValue
    {
        IComparable GetKey();

        object GetValue();
    }
}