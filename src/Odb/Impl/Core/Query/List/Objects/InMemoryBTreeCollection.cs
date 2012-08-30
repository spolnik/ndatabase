using System;
using NDatabase.Btree;
using NDatabase.Btree.Impl.Multiplevalue;
using NDatabase.Odb.Core;

namespace NDatabase.Odb.Impl.Core.Query.List.Objects
{
    /// <summary>
    ///   An implementation of an ordered Collection based on a BTree implementation that holds all objects in memory
    /// </summary>
    /// <author>osmadja</author>
    
    public sealed class InMemoryBTreeCollection<T> : AbstractBTreeCollection<T>
    {
        public InMemoryBTreeCollection() : base(OrderByConstants.OrderByAsc)
        {
        }

        public InMemoryBTreeCollection(OrderByConstants orderByType) : base(orderByType)
        {
        }

        public override IBTree BuildTree(int degree)
        {
            return new InMemoryBTreeMultipleValuesPerKey("default", degree);
        }
    }
}
