using NDatabase2.Odb.Core;

namespace NDatabase2.Btree
{
    /// <summary>
    ///   An iterator to iterate over NDatabase BTree.
    /// </summary>
    /// <remarks>
    ///   An iterator to iterate over NDatabase BTree.
    /// </remarks>
    public class BTreeIteratorSingleValuePerKey<T> : AbstractBTreeIterator<T>
    {
        public BTreeIteratorSingleValuePerKey(IBTree tree, OrderByConstants orderByType) : base(tree, orderByType)
        {
        }

        public override object GetValueAt(IBTreeNode node, int currentIndex)
        {
            var n = (IBTreeNodeOneValuePerKey) node;
            return n.GetValueAt(currentIndex);
        }
    }
}
