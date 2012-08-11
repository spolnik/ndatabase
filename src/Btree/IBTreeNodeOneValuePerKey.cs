using System;

namespace NDatabase.Btree
{
    /// <summary>
    ///   The interface for btree nodes that accept multiple values for each key
    /// </summary>
    /// <author>olivier</author>
    public interface IBTreeNodeOneValuePerKey : IBTreeNode
    {
        object GetValueAt(int index);

        object Search(IComparable key);
    }
}