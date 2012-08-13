using NDatabase.Btree;
using NDatabase.Odb.Impl.Core.Btree;
using NUnit.Framework;
using Test.Odb.Test;

namespace Btree.Odb
{
	/// <author>olivier</author>
	public class SingleValueBTree : ODBTest
	{
		[Test]
        public virtual void Test2SameKeySingleBTree()
		{
			int size = 1000;
            IBTree tree = new OdbBtreeSingle();
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
			catch (System.Exception)
			{
                Assert.Pass();
			}
		}
	}
}
