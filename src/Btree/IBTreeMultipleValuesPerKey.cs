using System;
using System.Collections;

namespace NDatabase.Btree
{
    public interface IBTreeMultipleValuesPerKey : IBTree
    {
        IList Search(IComparable key);
    }
}