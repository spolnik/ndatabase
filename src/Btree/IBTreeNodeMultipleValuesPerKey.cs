using System;
using System.Collections;

namespace NDatabase2.Btree
{
    /// <summary>
    ///   The interface for btree nodes that accept One Value Per Key
    /// </summary>
    public interface IBTreeNodeMultipleValuesPerKey : IBTreeNode
    {
        IList GetValueAt(int index);

        IList Search(IComparable key);
    }
}