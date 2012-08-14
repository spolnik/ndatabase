using System;
using NDatabase.Btree;
using NDatabase.Btree.Exception;
using NDatabase.Odb;
using NDatabase.Odb.Impl.Core.Btree;
using NUnit.Framework;
using NeoDatis.Test.Btree.Impl.Singlevalue;
using Test.Odb.Test;

namespace Btree.Odb
{
	/// <author>olivier</author>
	public class SingleValueBTree : ODBTest
	{
		[Test]
        public virtual void Test2SameKeySingleBTree()
		{
		    const string singlevaluebtreeTest1DbName = "singlevaluebtree.test1.db";
            DeleteBase(singlevaluebtreeTest1DbName);

		    using (var odb = OdbFactory.Open(singlevaluebtreeTest1DbName))
		    {
                int size = 1000;
                IBTree tree = new OdbBtreeSingle("test1", 50, new LazyOdbBtreePersister(odb));
                for (int i = 0; i < size; i++)
                {
                    if (i % 10000 == 0)
                    {
                        Println(i);
                    }
                    tree.Insert(i + 1, "value " + (i + 1));
                }
                try
                {
                    for (int i = 0; i < 10; i++)
                    {
                        if (i % 10000 == 0)
                        {
                            Println(i);
                        }
                        tree.Insert(100, "value " + (i + 1));
                        Fail("Single Value Btree should not accept duplcited key");
                    }
                }
                catch (DuplicatedKeyException ex)
                {
                    Console.WriteLine(ex.Message);
                    Assert.Pass();
                }
		    }
		}
	}
}
