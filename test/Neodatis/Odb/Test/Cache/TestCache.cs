using System.Threading;
using NDatabase.Odb;
using NDatabase.Odb.Core.Query.Criteria;
using NDatabase.Odb.Impl.Core.Layers.Layer3.Engine;
using NDatabase.Odb.Impl.Core.Query.Criteria;
using NUnit.Framework;
using Test.Odb.Test.VO.Login;

namespace Test.Odb.Test.Cache
{
	[TestFixture]
    public class TestCache : ODBTest
	{
		public static int NbObjects = 300;

		[SetUp]
		public override void SetUp()
		{
			Thread.Sleep(100);
			// Configuration.setUseModifiedClass(true);
			DeleteBase("cache.neodatis");
			IOdb odb = Open("cache.neodatis");
			for (int i = 0; i < NbObjects; i++)
			{
				odb.Store(new VO.Login.Function("function " + (i + i)));
				odb.Store(new User("olivier " + i, "olivier@neodatis.com "
					 + i, new Profile("profile " + i, new VO.Login.Function
					("inner function " + i))));
			}
			odb.Close();
		}

		[Test]
        public virtual void Test1()
		{
			IOdb odb = Open("cache.neodatis");
			IObjects<VO.Login.Function> l = odb.GetObjects<VO.Login.Function> (new CriteriaQuery(Where.Equal("name", "function 10")));
			AssertFalse(l.Count==0);
			// Cache must have only one object : The function
			AssertEquals(l.Count, Dummy.GetEngine
				(odb).GetSession(true).GetCache().GetNumberOfObjects());
			odb.Close();
		}

		[Test]
        public virtual void Test2()
		{
			IOdb odb = Open("cache.neodatis");
			IObjects<User> l = odb.GetObjects<User>(new CriteriaQuery(Where.Equal("name", "olivier 10")));
			AssertFalse(l.Count==0);
			// Cache must have 3 times the number of Users in list l (check the
			// setup method to understand this)
			AssertEquals(l.Count * 3, Dummy.GetEngine
				(odb).GetSession(true).GetCache().GetNumberOfObjects());
			odb.Close();
		}

		[TearDown]
		public override void TearDown()
		{
			DeleteBase("cache.neodatis");
		}
	}
}
