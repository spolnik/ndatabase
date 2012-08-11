using System.Threading;
using NDatabase.Odb;
using NDatabase.Odb.Core.Query;
using NDatabase.Odb.Core.Query.Criteria;
using NDatabase.Odb.Impl.Core.Query.Criteria;
using NUnit.Framework;
using System;

namespace Test.Odb.Test.Query.Criteria
{
	[TestFixture]
    public class TestCriteriaQuery : ODBTest
	{
		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void Test1()
		{
            string baseName = GetBaseName();
            SetUp(baseName);
			IOdb odb = Open(baseName);
			CriteriaQuery aq = new CriteriaQuery(Where
				.Or().Add(Where.Equal("name", "function 2")).Add
				(Where.Equal("name", "function 3")));
			IObjects<VO.Login.Function> l = odb.GetObjects<VO.Login.Function>(aq, true, -1, -1);
			AssertEquals(2, l.Count);
			VO.Login.Function f = l.GetFirst
				();
			AssertEquals("function 2", f.GetName());
            Println(l);
			odb.Close();
            Console.ReadLine();
		}

		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void Test2()
		{
            string baseName = GetBaseName();
            SetUp(baseName);
			IOdb odb = Open(baseName);
			CriteriaQuery aq = new CriteriaQuery
				(typeof(VO.Login.Function), Where
				.Not(Where.Equal("name", "function 2")));
            IObjects<VO.Login.Function> l = odb.GetObjects<VO.Login.Function>(aq, true, -1, -1);
			AssertEquals(49, l.Count);
			VO.Login.Function f = (VO.Login.Function)l.GetFirst
				();
			AssertEquals("function 0", f.GetName());
			odb.Close();
		}

		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void Test3()
		{
            string baseName = GetBaseName();
            SetUp(baseName);
			IOdb odb = Open(baseName);
			CriteriaQuery aq = new CriteriaQuery
				(typeof(VO.Login.Function), Where
				.Not(Where.Or().Add(Where
				.Equal("name", "function 2")).Add(Where.Equal("name"
				, "function 3"))));
			IObjects<VO.Login.Function> l = odb.GetObjects<VO.Login.Function>(aq, true, -1, -1);
			AssertEquals(48, l.Count);
			VO.Login.Function f = (VO.Login.Function)l.GetFirst
				();
			AssertEquals("function 0", f.GetName());
			odb.Close();
		}

		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void Test4Sort()
		{
            string baseName = GetBaseName();
            SetUp(baseName);
			int d = OdbConfiguration.GetDefaultIndexBTreeDegree();
			try
			{
				OdbConfiguration.SetDefaultIndexBTreeDegree(40);
				IOdb odb = Open(baseName);
				CriteriaQuery aq = new CriteriaQuery
					(typeof(VO.Login.Function), Where
					.Not(Where.Or().Add(Where
					.Equal("name", "function 2")).Add(Where.Equal("name"
					, "function 3"))));
				aq.OrderByDesc("name");
				// aq.orderByAsc("name");
				IObjects<VO.Login.Function> l = odb.GetObjects<VO.Login.Function>(aq, true, -1, -1);
                odb.Close();

				AssertEquals(48, l.Count);
				VO.Login.Function f = (VO.Login.Function)l.GetFirst
					();
				AssertEquals("function 9", f.GetName());
			}
			finally
			{
				OdbConfiguration.SetDefaultIndexBTreeDegree(d);
			}
		}

		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void TestDate1()
		{
            string baseName = GetBaseName();
            SetUp(baseName);
			IOdb odb = Open(baseName);
			MyDates myDates = new MyDates
				();
			System.DateTime d1 = new System.DateTime();
            Thread.Sleep(100);
			
			System.DateTime d2 = new System.DateTime();
            Thread.Sleep(100);
			System.DateTime d3 = new System.DateTime();
			myDates.SetDate1(d1);
			myDates.SetDate2(d3);
			myDates.SetI(5);
			odb.Store(myDates);
			odb.Close();
			odb = Open(baseName);
			IQuery query = new CriteriaQuery
				(typeof(MyDates), Where
				.And().Add(Where.Le("date1", d2)).Add(Where
				.Ge("date2", d2)).Add(Where.Equal("i", 5)));
			IObjects<MyDates> objects = odb.GetObjects<MyDates>(query);
			AssertEquals(1, objects.Count);
			odb.Close();
		}

		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void TestIequal()
		{

            string baseName = GetBaseName();
            SetUp(baseName);
			IOdb odb = Open(baseName);
			CriteriaQuery aq = new CriteriaQuery
				(typeof(VO.Login.Function), Where
				.Iequal("name", "FuNcTiOn 1"));
			aq.OrderByDesc("name");
            IObjects<VO.Login.Function> l = odb.GetObjects<VO.Login.Function>(aq, true, -1, -1);
			AssertEquals(1, l.Count);
			odb.Close();
		}

		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void TestEqual2()
		{
            string baseName = GetBaseName();
            SetUp(baseName);
			IOdb odb = Open(baseName);
			CriteriaQuery aq = new CriteriaQuery
				(typeof(VO.Login.Function), Where
				.Equal("name", "FuNcTiOn 1"));
			aq.OrderByDesc("name");
            IObjects<VO.Login.Function> l = odb.GetObjects<VO.Login.Function>(aq, true, -1, -1);
			AssertEquals(0, l.Count);
			odb.Close();
		}

		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void TestILike()
		{
            string baseName = GetBaseName();
            SetUp(baseName);
			IOdb odb = Open(baseName);
			CriteriaQuery aq = new CriteriaQuery
				(typeof(VO.Login.Function), Where
				.Ilike("name", "FUNc%"));
			aq.OrderByDesc("name");
            IObjects<VO.Login.Function> l = odb.GetObjects<VO.Login.Function>(aq, true, -1, -1);
			AssertEquals(50, l.Count);
			odb.Close();
		}

		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void TestLike()
		{
            string baseName = GetBaseName();
            SetUp(baseName);
			IOdb odb = Open(baseName);
			CriteriaQuery aq = new CriteriaQuery
				(typeof(VO.Login.Function), Where
				.Like("name", "func%"));
			aq.OrderByDesc("name");
            IObjects<VO.Login.Function> l = odb.GetObjects<VO.Login.Function>(aq, true, -1, -1);
			AssertEquals(50, l.Count);
			odb.Close();
		}

		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void TestLike2()
		{
            string baseName = GetBaseName();
            SetUp(baseName);
			IOdb odb = Open(baseName);
			CriteriaQuery aq = new CriteriaQuery
				(typeof(VO.Login.Function), Where
				.Like("name", "FuNc%"));
			aq.OrderByDesc("name");
            IObjects<VO.Login.Function> l = odb.GetObjects<VO.Login.Function>(aq, true, -1, -1);
			AssertEquals(0, l.Count);
			odb.Close();
		}

		public void SetUp(string baseName)
		{
			base.SetUp();
			DeleteBase(baseName);
			IOdb odb = Open(baseName);
			for (int i = 0; i < 50; i++)
			{
				odb.Store(new VO.Login.Function("function " + i));
			}
			odb.Close();
		}

		
		public void TearDown(string baseName)
		{
			DeleteBase(baseName);
		}
	}
}
