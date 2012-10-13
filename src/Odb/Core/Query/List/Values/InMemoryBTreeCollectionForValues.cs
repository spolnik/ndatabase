using NDatabase2.Btree;
using NDatabase2.Btree.Impl.Multiplevalue;

namespace NDatabase2.Odb.Core.Query.List.Values
{
    /// <summary>
    ///   An ordered Collection to hold values (not objects) based on a BTree implementation.
    /// </summary>
    /// <remarks>
    ///   An ordered Collection to hold values (not objects) based on a BTree implementation. It holds all values in memory.
    /// </remarks>
    
    public sealed class InMemoryBTreeCollectionForValues : AbstractBTreeCollection<IObjectValues>, IValues
    {
        public InMemoryBTreeCollectionForValues() : base(OrderByConstants.OrderByAsc)
        {
        }

        public InMemoryBTreeCollectionForValues(OrderByConstants orderByType) : base(orderByType)
        {
        }

        #region IValues Members

        public IObjectValues NextValues()
        {
            return Next();
        }

        public new void AddOid(OID oid)
        {
            throw new OdbRuntimeException(NDatabaseError.InternalError.AddParameter("Add Oid not implemented "));
        }

        #endregion

        public override IBTree BuildTree(int degree)
        {
            return new InMemoryBTreeMultipleValuesPerKey("default", degree);
        }
    }
}
