using System;
using System.Collections;

namespace NDatabase2.Btree
{
    public interface IBTreeMultipleValuesPerKey : IBTree
    {
        IList Search(IComparable key);
    }
}