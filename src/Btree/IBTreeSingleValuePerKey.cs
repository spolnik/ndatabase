using System;

namespace NDatabase2.Btree
{
    public interface IBTreeSingleValuePerKey : IBTree
    {
        object Search(IComparable key);
    }
}