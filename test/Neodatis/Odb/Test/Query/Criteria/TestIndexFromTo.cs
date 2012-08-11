using NDatabase.Odb;
using NDatabase.Odb.Core.Query;
using NDatabase.Odb.Core.Query.Criteria;
using NDatabase.Odb.Impl.Core.Query.Criteria;
using NDatabase.Tool.Wrappers.List;
using NUnit.Framework;

namespace Test.Odb.Test.Query.Criteria
{
	[TestFixture]
    public class TestIndexFromTo : ODBTest
	{
		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void TestGetLimitedResult1()
		{
			string baseName = GetBaseName();
			int size = 1000;
			IOdb odb = Open(baseName);
			for (int i = 0; i < size; i++)
			{
				odb.Store(new VO.Login.Function("function " + i));
			}
			odb.Close();
			odb = Open(baseName);
            IQuery q = new CriteriaQuery(typeof(VO.Login.Function));
			IObjects<VO.Login.Function> os = odb.GetObjects<VO.Login.Function>(q, true, 0, 1);
			AssertEquals(1, os.Count);
			for (int i = 0; i < os.Count; i++)
			{
				VO.Login.Function f = (VO.Login.Function)os.Next
					();
				AssertEquals("function " + i, f.GetName());
			}
			odb.Close();
			DeleteBase(baseName);
		}

		[Test]
        public virtual void Test()
		{
			string s = "olivier";
			string ss = s.Substring(0, 1);
			AssertEquals(1, ss.Length);
		    ss = s.Substring(0, 2);
		    AssertEquals(2, ss.Length);
			System.Collections.Generic.IList<object> l = new System.Collections.Generic.List<object>();
			l.Add("s1");
			l.Add("s2");
			l.Add("s3");
			l.Add("s4");
			l.Add("s5");
            AssertEquals(1, NDatabaseCollectionUtil.SublistGeneric(l,0, 1).Count);
            AssertEquals(2, NDatabaseCollectionUtil.SublistGeneric(l, 0, 2).Count);
            AssertEquals(3, NDatabaseCollectionUtil.SublistGeneric(l, 0, 3).Count);
		}

		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void TestGetLimitedResult()
		{
			string baseName = GetBaseName();
			int size = 1000;
			IOdb odb = Open(baseName);
			for (int i = 0; i < size; i++)
			{
				odb.Store(new VO.Login.Function("function " + i));
			}
			odb.Close();
			odb = Open(baseName);
            IQuery q = new CriteriaQuery(typeof(VO.Login.Function));
			IObjects<VO.Login.Function> os = odb.GetObjects<VO.Login.Function>(q, true, 0, 10);
			AssertEquals(10, os.Count);
			for (int i = 0; i < 10; i++)
			{
				VO.Login.Function f = (VO.Login.Function)os.Next
					();
				AssertEquals("function " + i, f.GetName());
			}
			odb.Close();
			DeleteBase(baseName);
		}

		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void TestGetLimitedResult2()
		{
			string baseName = GetBaseName();
			int size = 1000;
			IOdb odb = Open(baseName);
			for (int i = 0; i < size; i++)
			{
				odb.Store(new VO.Login.Function("function " + i));
			}
			odb.Close();
			odb = Open(baseName);
            IQuery q = new CriteriaQuery(typeof(VO.Login.Function));
			IObjects<VO.Login.Function> os = odb.GetObjects<VO.Login.Function>(q, true, 10, 20);
			AssertEquals(10, os.Count);
			for (int i = 10; i < 20; i++)
			{
				VO.Login.Function f = (VO.Login.Function)os.Next
					();
				AssertEquals("function " + i, f.GetName());
			}
			odb.Close();
			DeleteBase(baseName);
		}

		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void TestGetLimitedResult3()
		{
			string baseName = GetBaseName();
			int size = 1000;
			IOdb odb = Open(baseName);
			for (int i = 0; i < size; i++)
			{
				if (i < size / 2)
				{
					odb.Store(new VO.Login.Function("function " + i));
				}
				else
				{
					odb.Store(new VO.Login.Function("FUNCTION " + i));
				}
			}
			odb.Close();
			odb = Open(baseName);
			IQuery q = new CriteriaQuery(Where.Like("name", "FUNCTION%"));
			IObjects<VO.Login.Function> os = odb.GetObjects<VO.Login.Function>(q, true, 0, 10);
			AssertEquals(10, os.Count);
			for (int i = size / 2; i < size / 2 + 10; i++)
			{
				VO.Login.Function f = (VO.Login.Function)os.Next
					();
				AssertEquals("FUNCTION " + i, f.GetName());
			}
			odb.Close();
			DeleteBase(baseName);
		}

		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void TestGetLimitedResult4()
		{
			string baseName = GetBaseName();
			int size = 1000;
			IOdb odb = Open(baseName);
			for (int i = 0; i < size; i++)
			{
				if (i < size / 2)
				{
					odb.Store(new VO.Login.Function("function " + i));
				}
				else
				{
					odb.Store(new VO.Login.Function("FUNCTION " + i));
				}
			}
			odb.Close();
			odb = Open(baseName);
			IQuery q = new CriteriaQuery(Where.Like("name", "FUNCTION%"));
			IObjects<VO.Login.Function> os = odb.GetObjects<VO.Login.Function>(q, true, 10, 20);
			AssertEquals(10, os.Count);
			for (int i = size / 2 + 10; i < size / 2 + 20; i++)
			{
				VO.Login.Function f = (VO.Login.Function)os.Next
					();
				AssertEquals("FUNCTION " + i, f.GetName());
			}
			odb.Close();
			DeleteBase(baseName);
		}
	}
}
