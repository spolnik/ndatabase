using System;
using System.Collections;

namespace NDatabase.Btree
{
    /// <summary>
    ///   The interface for btree nodes that accept One Value Per Key
    /// </summary>
    internal interface IBTreeNodeMultipleValuesPerKey : IBTreeNode
    {
        IList GetValueAt(int index);

        IList Search(IComparable key);
    }
}