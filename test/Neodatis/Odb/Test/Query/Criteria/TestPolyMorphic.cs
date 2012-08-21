using NDatabase.Odb;
using NDatabase.Odb.Core.Query;
using NDatabase.Odb.Core.Query.Criteria;
using NDatabase.Odb.Impl.Core.Query.Criteria;
using NDatabase.Odb.Impl.Core.Query.Values;
using NUnit.Framework;
using Test.Odb.Test.VO.Human;

namespace Test.Odb.Test.Query.Criteria
{
	[TestFixture]
    public class TestPolyMorphic : ODBTest
	{
		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void Test1()
		{
			DeleteBase("multi");
			IOdb odb = Open("multi");
			odb.Store(new Animal("dog", "M", "my dog"));
			odb.Store(new Animal("cat", "F", "my cat"));
			odb.Store(new Man("Joe"));
			odb.Store(new Woman("Karine"));
			odb.Close();
			odb = Open("multi");
			IQuery q = new CriteriaQuery
				(typeof(object));
			
			IObjects<object> os = odb.GetObjects<object>(q);
			Println(os);
			odb.Close();
			AssertEquals(4, os.Count);
			DeleteBase("multi");
		}

		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void Test2()
		{
			DeleteBase("multi");
			IOdb odb = Open("multi");
			odb.Store(new Animal("dog", "M", "my dog"));
			odb.Store(new Animal("cat", "F", "my cat"));
			odb.Store(new Man("Joe"));
			odb.Store(new Woman("Karine"));
			odb.Close();
			odb = Open("multi");
			IQuery q = new CriteriaQuery
				(typeof(Human));
			
			IObjects<Human> os = odb.GetObjects<Human>(q);
			Println(os);
			odb.Close();
			AssertEquals(2, os.Count);
			DeleteBase("multi");
		}

		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void Test3()
		{
			DeleteBase("multi");
			IOdb odb = Open("multi");
			odb.Store(new Animal("dog", "M", "my dog"));
			odb.Store(new Animal("cat", "F", "my cat"));
			odb.Store(new Man("Joe"));
			odb.Store(new Woman("Karine"));
			odb.Close();
			odb = Open("multi");
			IValuesQuery q = new ValuesCriteriaQuery
				(typeof(object)).Field("specie");
			
			IValues os = odb.GetValues(q);
			Println(os);
			odb.Close();
			AssertEquals(4, os.Count);
			DeleteBase("multi");
		}

		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void Test4()
		{
			DeleteBase("multi");
			IOdb odb = Open("multi");
			odb.Store(new Animal("dog", "M", "my dog"));
			odb.Store(new Animal("cat", "F", "my cat"));
			odb.Store(new Man("Joe"));
			odb.Store(new Woman("Karine"));
			odb.Close();
			odb = Open("multi");
			IValuesQuery q = new ValuesCriteriaQuery
				(typeof(Human)).Field("specie");
			
			IValues os = odb.GetValues(q);
			Println(os);
			odb.Close();
			AssertEquals(2, os.Count);
			DeleteBase("multi");
		}

		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void Test5()
		{
			DeleteBase("multi");
			IOdb odb = Open("multi");
			odb.Store(new Animal("dog", "M", "my dog"));
			odb.Store(new Animal("cat", "F", "my cat"));
			odb.Store(new Man("Joe"));
			odb.Store(new Woman("Karine"));
			odb.Close();
			odb = Open("multi");
			IValuesQuery q = new ValuesCriteriaQuery
				(typeof(Man)).Field("specie");
			
			IValues os = odb.GetValues(q);
			Println(os);
			odb.Close();
			AssertEquals(1, os.Count);
			DeleteBase("multi");
		}

		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void Test6()
		{
			DeleteBase("multi");
			IOdb odb = Open("multi");
			odb.Store(new Animal("dog", "M", "my dog"));
			odb.Store(new Animal("cat", "F", "my cat"));
			odb.Store(new Man("Joe"));
			odb.Store(new Woman("Karine"));
			odb.Close();
			odb = Open("multi");
			CriteriaQuery q = new CriteriaQuery
				(typeof(object));
			
			System.Decimal nb = odb.Count(q);
			Println(nb);
			odb.Close();
			AssertEquals(new System.Decimal(4), nb);
			DeleteBase("multi");
		}

		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void Test7()
		{
			int size = 3000;
            string baseName = GetBaseName();
			IOdb odb = Open(baseName);
			for (int i = 0; i < size; i++)
			{
				odb.Store(new Animal("dog", "M", "my dog"));
				odb.Store(new Animal("cat", "F", "my cat"));
				odb.Store(new Man("Joe" + i));
				odb.Store(new Woman("Karine" + i));
			}
			odb.Close();
			odb = Open(baseName);
			CriteriaQuery q = new CriteriaQuery
				(typeof(object));
			
			System.Decimal nb = odb.Count(q);
			Println(nb);
			odb.Close();
			AssertEquals(new System.Decimal(4 * size), nb);
			DeleteBase(baseName);
		}

		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void Test8()
		{
			int size = 3000;
            string baseName = GetBaseName();
			IOdb odb = Open(baseName);
			for (int i = 0; i < size; i++)
			{
				odb.Store(new Animal("dog" + i, "M", "my dog" + i));
				odb.Store(new Animal("cat" + i, "F", "my cat" + i));
				odb.Store(new Man("Joe" + i));
				odb.Store(new Woman("Karine" + i));
			}
			odb.Close();
			odb = Open(baseName);
			CriteriaQuery q = new CriteriaQuery
				(typeof(object), Where.Equal("specie", "man"));
			
			System.Decimal nb = odb.Count(q);
			Println(nb);
			odb.Close();
			AssertEquals(new System.Decimal(1 * size), nb);
			DeleteBase(baseName);
		}
	}
}
