using NDatabase.Odb;

namespace NDatabase.Btree
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

        /// <exception cref="System.Exception"></exception>
        void Close();

        object DeleteNode(IBTreeNode parent);

        void SetBTree(IBTree tree);

        void Clear();

        void Flush();
    }
}