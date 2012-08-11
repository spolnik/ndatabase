using NDatabase.Odb;

namespace NDatabase.Btree.Impl
{
    /// <summary>
    ///   TODO check if this class must exist
    /// </summary>
    /// <author>osmadja</author>
    public class InMemoryPersister : IBTreePersister
    {
        #region IBTreePersister Members

        public virtual IBTreeNode LoadNodeById(object id)
        {
            // TODO Auto-generated method stub
            return null;
        }

        public virtual object SaveNode(IBTreeNode node)
        {
            // TODO Auto-generated method stub
            return null;
        }

        /// <exception cref="System.Exception"></exception>
        public virtual void Close()
        {
        }

        // TODO Auto-generated method stub
        public virtual object DeleteNode(IBTreeNode parent)
        {
            // TODO Auto-generated method stub
            return null;
        }

        public virtual IBTree LoadBTree(object id)
        {
            // TODO Auto-generated method stub
            return null;
        }

        public virtual OID SaveBTree(IBTree tree)
        {
            // TODO Auto-generated method stub
            return null;
        }

        public virtual void SetBTree(IBTree tree)
        {
        }

        // TODO Auto-generated method stub
        public virtual void Clear()
        {
        }

        // TODO Auto-generated method stub
        public virtual void Flush()
        {
        }

        #endregion

        // TODO Auto-generated method stub
    }
}