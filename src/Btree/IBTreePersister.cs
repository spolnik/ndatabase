using NDatabase2.Odb;

namespace NDatabase2.Btree
{
    /// <summary>
    ///   Interface used to persist and load btree and btree node from a persistent layer
    /// </summary>
    /// <author>osmadja</author>
    public interface IBTreePersister
    {
        IBTreeNode LoadNodeById(object id);

        object SaveNode(IBTreeNode node);

        OID SaveBTree(IBTree tree);

        IBTree LoadBTree(object id);

        
        void Close();

        object DeleteNode(IBTreeNode parent);

        void SetBTree(IBTree tree);

        void Clear();

        void Flush();
    }
}