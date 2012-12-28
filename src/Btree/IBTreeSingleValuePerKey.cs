using System;

namespace NDatabase.Btree
{
    public interface IBTreeSingleValuePerKey : IBTree
    {
        object Search(IComparable key);
    }
}