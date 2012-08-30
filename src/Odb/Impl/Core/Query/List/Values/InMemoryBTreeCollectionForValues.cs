using System;
using NDatabase.Btree;
using NDatabase.Btree.Impl.Multiplevalue;
using NDatabase.Odb.Core;

namespace NDatabase.Odb.Impl.Core.Query.List.Values
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
            throw new Exception("Add Oid not implemented ");
        }

        #endregion

        public override IBTree BuildTree(int degree)
        {
            return new InMemoryBTreeMultipleValuesPerKey("default", degree);
        }
    }
}
