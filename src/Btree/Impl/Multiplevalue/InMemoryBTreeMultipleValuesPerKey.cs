using System;
using System.Collections;
using NDatabase.Odb.Core;

namespace NDatabase.Btree.Impl.Multiplevalue
{
    [Serializable]
    public class InMemoryBTreeMultipleValuesPerKey : AbstractBTree, IBTreeMultipleValuesPerKey
    {
        protected static int NextId = 1;

        protected int Id;

        public InMemoryBTreeMultipleValuesPerKey()
        {
        }

        public InMemoryBTreeMultipleValuesPerKey(string name, int degree, IBTreePersister persister) 
            : base(name, degree, persister)
        {
        }

        public InMemoryBTreeMultipleValuesPerKey(string name, int degree) 
            : base(name, degree, new InMemoryPersister())
        {
            Id = NextId++;
        }

        #region IBTreeMultipleValuesPerKey Members

        public virtual IList Search(IComparable key)
        {
            var theRoot = (IBTreeNodeMultipleValuesPerKey) GetRoot();
            return theRoot.Search(key);
        }

        public override IBTreeNode BuildNode()
        {
            return new InMemoryBTreeNodeMultipleValuesPerKey(this);
        }

        public override object GetId()
        {
            return Id;
        }

        public override void SetId(object id)
        {
            Id = (int) id;
        }

        public override void Clear()
        {
        }

        public override IEnumerator Iterator<T>(OrderByConstants orderBy)
        {
            return new BTreeIteratorMultipleValuesPerKey<T>(this, orderBy);
        }

        #endregion
    }
}