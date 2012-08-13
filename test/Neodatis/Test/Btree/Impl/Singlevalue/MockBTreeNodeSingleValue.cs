using System;
using NDatabase.Btree;
using NDatabase.Odb.Impl.Core.Btree;

namespace NeoDatis.Test.Btree.Impl.Singlevalue
{
    [Serializable]
    public class MockBTreeNodeSingleValue : OdbBtreeNodeSingle
    {
        private string name;

        public MockBTreeNodeSingleValue(IBTree btree, string name) : base(btree)
        {
            this.name = name;
        }
    }
}
