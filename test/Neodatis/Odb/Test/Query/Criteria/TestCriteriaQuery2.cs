using NDatabase.Odb;
using NDatabase.Odb.Core.Query.Criteria;
using NDatabase.Odb.Impl.Core.Query.Criteria;
using NDatabase.Tool.Wrappers;
using NUnit.Framework;
using Test.Odb.Test.VO.Attribute;

namespace Test.Odb.Test.Query.Criteria
{
	[TestFixture]
    public class TestCriteriaQuery2 : ODBTest
	{
		
		[Test]
        public virtual void Test1()
		{
            string BaseName = GetBaseName();
            SetUp(BaseName);
			IOdb odb = Open(BaseName);
			CriteriaQuery aq = new CriteriaQuery(Where.Or().Add(Where.Equal("string1", "test class 1")).Add(Where.Equal("string1", "test class 3")));
            aq.OrderByAsc("string1");
			IObjects<TestClass> l = odb.GetObjects<TestClass>(aq, true, -1, -1);
            odb.Close();

			AssertEquals(2, l.Count);
			TestClass testClass = l.GetFirst();
			AssertEquals("test class 1", testClass.GetString1());
		}

		
		[Test]
        public virtual void Test2()
		{
            string BaseName = GetBaseName();
            SetUp(BaseName);
            IOdb odb = Open(BaseName);
			CriteriaQuery aq = new CriteriaQuery(Where.Not(Where.Equal("string1", "test class 2")));
			IObjects<TestClass> l = odb.GetObjects<TestClass>(aq, true, -1, -1);
			AssertEquals(49, l.Count);
			TestClass testClass = l.GetFirst();
			AssertEquals("test class 0", testClass.GetString1());
			odb.Close();
		}

		
		[Test]
        public virtual void Test3()
		{
            string BaseName = GetBaseName();
            SetUp(BaseName);
            IOdb odb = Open(BaseName);
			CriteriaQuery aq = new CriteriaQuery(Where.Not(Where.Or().Add(Where
				.Equal("string1", "test class 0")).Add(Where.Equal
				("bigDecimal1", new System.Decimal(5)))));
			IObjects<TestClass> l = odb.GetObjects<TestClass>(aq, true, -1, -1);
			AssertEquals(48, l.Count);
			TestClass testClass = (TestClass
				)l.GetFirst();
			AssertEquals("test class 1", testClass.GetString1());
			odb.Close();
		}

		
		[Test]
        public virtual void Test4Sort()
		{
            string BaseName = GetBaseName();
            SetUp(BaseName);
            IOdb odb = Open(BaseName);
			CriteriaQuery aq = new CriteriaQuery(Where.Not(Where.Or().Add(Where.Equal("string1", "test class 2")).Add(Where.Equal("string1", "test class 3"))));
			aq.OrderByDesc("double1,int1");
			IObjects<TestClass> l = odb.GetObjects<TestClass> (aq, true, -1, -1);
			// println(l);
			AssertEquals(48, l.Count);
			TestClass testClass = l.GetFirst();
			AssertEquals("test class 9", testClass.GetString1());
			odb.Close();
		}

		
		[Test]
        public virtual void Test5Sort()
		{
            string BaseName = GetBaseName();
            SetUp(BaseName);
            IOdb odb = Open(BaseName);
			CriteriaQuery aq = new CriteriaQuery(Where
				.Not(Where.Or().Add(Where
				.Equal("string1", "test class 2")).Add(Where.Equal
				("string1", "test class 3"))));
			// aq.orderByDesc("double1,boolean1,int1");
			aq.OrderByDesc("double1,int1");
			IObjects<TestClass> l = odb.GetObjects<TestClass> (aq, true, -1, -1);
			AssertEquals(48, l.Count);
			TestClass testClass = l.GetFirst();
			AssertEquals("test class 9", testClass.GetString1());
			odb.Close();
		}

		
		[Test]
        public virtual void Test6Sort()
		{
            string BaseName = GetBaseName();
            SetUp(BaseName);
            IOdb odb = Open(BaseName);
			ICriterion c = Where
				.Or().Add(Where.Equal("string1", "test class 2"
				)).Add(Where.Equal("string1", "test class 3")).
				Add(Where.Equal("string1", "test class 4")).Add
				(Where.Equal("string1", "test class 5"));
			CriteriaQuery aq = new CriteriaQuery(c);
			aq.OrderByDesc("boolean1,int1");
			IObjects<TestClass> l = odb.GetObjects<TestClass>(aq, true, -1, -1);
			AssertEquals(4, l.Count);
			TestClass testClass = l.GetFirst();
			AssertEquals("test class 3", testClass.GetString1());
			odb.Close();
		}

  		public void SetUp(string BaseName)
		{
			base.SetUp();
			DeleteBase(BaseName);
			IOdb odb = Open(BaseName);
			long start = OdbTime.GetCurrentTimeInTicks();
			int size = 50;
			for (int i = 0; i < size; i++)
			{
				TestClass testClass = new TestClass
					();
				testClass.SetBigDecimal1(new System.Decimal(i));
				testClass.SetBoolean1(i % 3 == 0);
				testClass.SetChar1((char)(i % 5));
				testClass.SetDate1(new System.DateTime(start + i));
				testClass.SetDouble1(((double)(i % 10)) / size);
				testClass.SetInt1(size - i);
				testClass.SetString1("test class " + i);
				odb.Store(testClass);
			}
			// println(testClass.getDouble1() + " | " + testClass.getString1() +
			// " | " + testClass.getInt1());
			odb.Close();
		}

	}
}
