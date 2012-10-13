using System;
using System.Collections;
using NDatabase2.Odb.Core;

namespace NDatabase2.Btree.Impl.Singlevalue
{
    
    public abstract class BTreeSingleValuePerKey : AbstractBTree, IBTreeSingleValuePerKey
    {
        protected BTreeSingleValuePerKey()
        {
        }

        protected BTreeSingleValuePerKey(string name, int degree, IBTreePersister persister) 
            : base(name, degree, persister)
        {
        }

        #region IBTreeSingleValuePerKey Members

        public virtual object Search(IComparable key)
        {
            var theRoot = (IBTreeNodeOneValuePerKey) GetRoot();
            return theRoot.Search(key);
        }

        public override IEnumerator Iterator<T>(OrderByConstants orderBy)
        {
            return new BTreeIteratorSingleValuePerKey<T>(this, orderBy);
        }

        public abstract override object GetId();

        public abstract override void SetId(object arg1);

        #endregion
    }
}