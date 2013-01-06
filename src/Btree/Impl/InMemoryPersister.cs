using NDatabase.Odb;

namespace NDatabase.Btree.Impl
{
    /// <summary>
    ///   In memory persister
    /// </summary>
    internal sealed class InMemoryPersister : IBTreePersister
    {
        #region IBTreePersister Members

        public IBTreeNode LoadNodeById(object id)
        {
            return null;
        }

        public object SaveNode(IBTreeNode node)
        {
            return null;
        }

        
        public void Close()
        {
        }

        public object DeleteNode(IBTreeNode parent)
        {
            return null;
        }

        public IBTree LoadBTree(object id)
        {
            return null;
        }

        public OID SaveBTree(IBTree tree)
        {
            return null;
        }

        public void SetBTree(IBTree tree)
        {
        }

        public void Flush()
        {
        }

        #endregion
    }
}
