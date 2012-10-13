using System.Threading;
using NDatabase2.Odb.Core.Query.Criteria;
using NUnit.Framework;
using Test.NDatabase.Odb.Test.VO.Login;

namespace Test.NDatabase.Odb.Test.Cache
{
    [TestFixture]
    public class TestCache : ODBTest
    {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp()
        {
            Thread.Sleep(100);
            // Configuration.setUseModifiedClass(true);
            DeleteBase("cache.neodatis");
            var odb = Open("cache.neodatis");
            for (var i = 0; i < NbObjects; i++)
            {
                odb.Store(new VO.Login.Function("function " + (i + i)));
                odb.Store(new User("olivier " + i, "olivier@neodatis.com " + i,
                                   new Profile("profile " + i, new VO.Login.Function("inner function " + i))));
            }
            odb.Close();
        }

        [TearDown]
        public override void TearDown()
        {
            DeleteBase("cache.neodatis");
        }

        #endregion

        public static int NbObjects = 300;

        [Test]
        public virtual void Test1()
        {
            var odb = Open("cache.neodatis");
            var l = odb.GetObjects<VO.Login.Function>(new CriteriaQuery<VO.Login.Function>(Where.Equal("name", "function 10")));
            AssertFalse(l.Count == 0);
            // Cache must have only one object : The function
            
            odb.Close();
        }

        [Test]
        public virtual void Test2()
        {
            var odb = Open("cache.neodatis");
            var l = odb.GetObjects<User>(new CriteriaQuery<User>(Where.Equal("name", "olivier 10")));
            AssertFalse(l.Count == 0);
            // Cache must have 3 times the number of Users in list l (check the
            // setup method to understand this)
            
            odb.Close();
        }
    }
}
