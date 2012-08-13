using System.Collections;
using NDatabase.Btree;
using NDatabase.Btree.Impl.Multiplevalue;
using NDatabase.Odb.Core;
using NDatabase.Odb.Impl.Core.Btree;
using NUnit.Framework;
using Test.Odb.Test;

namespace Btree.Odb
{
    public class BigBTree : ODBTest
    {
        [Test]
        public virtual void Test1()
        {
            var size = 100000;
            IBTree tree = new OdbBtreeSingle();
            for (var i = 0; i < size; i++)
            {
                if (i % 10000 == 0)
                    Println(i);
                tree.Insert(i + 1, "value " + (i + 1));
            }
            AssertEquals(size, tree.GetSize());
            IEnumerator iterator = new BTreeIteratorSingleValuePerKey<object>(tree, OrderByConstants.OrderByAsc);
            var j = 0;
            while (iterator.MoveNext())
            {
                var o = iterator.Current;
                // println(o);
                j++;
                if (j == size)
                    AssertEquals("value " + size, o);
            }
        }

        [Test]
        public virtual void Test2()
        {
            var size = 100000;
            IBTree tree = new InMemoryBTreeMultipleValuesPerKey("test1", 50);
            for (var i = 0; i < size; i++)
            {
                if (i % 10000 == 0)
                    Println(i);
                tree.Insert(i + 1, "value " + (i + 1));
            }
            AssertEquals(size, tree.GetSize());
            IEnumerator iterator = new BTreeIteratorMultipleValuesPerKey<object>(tree, OrderByConstants.OrderByAsc);
            var j = 0;
            while (iterator.MoveNext())
            {
                var o = iterator.Current;
                // println(o);
                j++;
                if (j == size)
                    AssertEquals("value " + size, o);
            }
        }
    }
}
