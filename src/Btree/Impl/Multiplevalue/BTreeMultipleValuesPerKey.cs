using System;
using System.Collections;
using NDatabase2.Odb.Core;

namespace NDatabase.Btree.Impl.Multiplevalue
{
    
    public abstract class BTreeMultipleValuesPerKey : AbstractBTree, IBTreeMultipleValuesPerKey
    {
        protected BTreeMultipleValuesPerKey()
        {
        }

        protected BTreeMultipleValuesPerKey(string name, int degree, IBTreePersister persister) 
            : base(name, degree, persister)
        {
        }

        #region IBTreeMultipleValuesPerKey Members

        public virtual IList Search(IComparable key)
        {
            var theRoot = (IBTreeNodeMultipleValuesPerKey) GetRoot();
            return theRoot.Search(key);
        }

        public override IEnumerator Iterator<T>(OrderByConstants orderBy)
        {
            return new BTreeIteratorMultipleValuesPerKey<T>(this, orderBy);
        }

        public abstract override object GetId();

        public abstract override void SetId(object arg1);

        #endregion
    }
}