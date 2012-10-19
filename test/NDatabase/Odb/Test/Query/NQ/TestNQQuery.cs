using NDatabase2.Odb.Core.Query.NQ;
using NUnit.Framework;
using Test.NDatabase.Odb.Test.VO.Login;

namespace Test.NDatabase.Odb.Test.Query.NQ
{
    [TestFixture]
    public class TestNqQuery : ODBTest
    {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            DeleteBase("get.neodatis");
            // println("TestNQQuery.setUp");
            var odb = Open("get.neodatis");
            for (var i = 0; i < NbObjects; i++)
            {
                odb.Store(new VO.Login.Function("function " + i));
                odb.Store(new User("olivier " + i, "olivier@neodatis.org " + "1",
                                   new Profile("profile " + i, new VO.Login.Function("inner function " + i))));
                odb.Store(new User("olivier " + i, "olivier@neodatis.org " + "2",
                                   new Profile("profile " + i, new VO.Login.Function("inner function " + i))));
                odb.Store(new User("olivier " + i, "olivier@neodatis.org " + "3",
                                   new Profile("profile " + i, new VO.Login.Function("inner function " + i))));
            }
            odb.Close();
        }

        [TearDown]
        public override void TearDown()
        {
            DeleteBase("get.neodatis");
        }

        #endregion

        public static int NbObjects = 10;

        private sealed class SimpleNativeQuery70 : SimpleNativeQuery<VO.Login.Function>
        {
            public override bool Match(VO.Login.Function function)
            {
                return true;
            }
        }

        private sealed class SimpleNativeQuery89 : SimpleNativeQuery<VO.Login.Function>
        {
            public override bool Match(VO.Login.Function function)
            {
                return function.GetName().Equals("function 5");
            }
        }

        private sealed class SimpleNativeQuery105 : SimpleNativeQuery<User>
        {
            public override bool Match(User user)
            {
                return user.GetProfile().GetName().Equals("profile 5");
            }
        }

        private sealed class SimpleNativeQuery121 : SimpleNativeQuery<User>
        {
            public override bool Match(User user)
            {
                return user.GetProfile().GetName().StartsWith("profile");
            }
        }

        private sealed class SimpleNativeQuery139 : SimpleNativeQuery<User>
        {
            public override bool Match(User user)
            {
                return user.GetProfile().GetName().StartsWith("profile");
            }
        }

        private sealed class SimpleNativeQuery158 : SimpleNativeQuery<User>
        {
            public override bool Match(User user)
            {
                return user.GetProfile().GetName().StartsWith("profile") &&
                       user.GetEmail().StartsWith("olivier@neodatis.org 1");
            }
        }

        private sealed class SimpleNativeQuery183 : SimpleNativeQuery<User>
        {
            public override bool Match(User user)
            {
                return user.GetProfile().GetName().StartsWith("profile") &&
                       user.GetEmail().StartsWith("olivier@neodatis.org 2");
            }
        }

        private sealed class SimpleNativeQuery208 : SimpleNativeQuery<User>
        {
            public override bool Match(User user)
            {
                return user.GetProfile().GetName().StartsWith("profile");
            }
        }

        private sealed class SimpleNativeQuery235 : SimpleNativeQuery<User>
        {
            public override bool Match(User user)
            {
                return user.GetProfile().GetName().StartsWith("profile");
            }
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test1()
        {
            var odb = Open("get.neodatis");
            odb.Query<VO.Login.Function>(true);
            odb.Close();
            odb = Open("get.neodatis");
            // println("TestNQQuery.test1:"+odb.getObjects (Function.class,true));
            var l = odb.Query<VO.Login.Function>(new SimpleNativeQuery70());
            odb.Close();
            AssertFalse(l.Count == 0);
            AssertEquals(NbObjects * 4, l.Count);
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test2()
        {
            var odb = Open("get.neodatis");
            // println("++++TestNQQuery.test2:"+odb.getObjects
            // (Function.class,true));
            var l = odb.Query<VO.Login.Function>(new SimpleNativeQuery89());
            odb.Close();
            AssertFalse(l.Count == 0);
            AssertEquals(1, l.Count);
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test3()
        {
            var odb = Open("get.neodatis");
            var l = odb.Query<User>(new SimpleNativeQuery105());
            odb.Close();
            AssertFalse(l.Count == 0);
            AssertEquals(3, l.Count);
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test4()
        {
            var odb = Open("get.neodatis");
            var query = new SimpleNativeQuery121();
            var l = odb.Query<User>(query, true, 0, 5);
            odb.Close();
            AssertFalse(l.Count == 0);
            AssertEquals(5, l.Count);
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test5()
        {
            var odb = Open("get.neodatis");
            var query = new SimpleNativeQuery139();
            var l = odb.Query<User>(query, true, 5, 6);
            odb.Close();
            AssertFalse(l.Count == 0);
            AssertEquals(1, l.Count);
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test6Ordering()
        {
            var odb = Open("get.neodatis");
            var query = new SimpleNativeQuery158();
            query.OrderByAsc("name");
            var l = odb.Query<User>(query, true);
            var i = 0;
            AssertFalse(l.Count == 0);
            while (l.HasNext())
            {
                var user = l.Next();
                AssertEquals("olivier " + i, user.GetName());
                // println(user.getName());
                i++;
            }
            odb.Close();
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test7Ordering()
        {
            var odb = Open("get.neodatis");
            var query = new SimpleNativeQuery183();
            query.OrderByDesc("name");
            var l = odb.Query<User>(query, true);
            var i = l.Count - 1;
            AssertFalse(l.Count == 0);
            while (l.HasNext())
            {
                var user = l.Next();
                AssertEquals("olivier " + i, user.GetName());
                // println(user.getName());
                i--;
            }
            odb.Close();
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test8Ordering()
        {
            var odb = Open("get.neodatis");
            var query = new SimpleNativeQuery208();
            query.OrderByAsc("name,email");
            var l = odb.Query<User>(query, true);
            AssertFalse(l.Count == 0);
            var i = 0;
            while (l.HasNext())
            {
                var user = l.Next();
                // println(user.getName() + " / " + user.getEmail());
                AssertEquals("olivier " + i / 3, user.GetName());
                AssertEquals("olivier@neodatis.org " + ((i % 3) + 1), user.GetEmail());
                i++;
            }
            odb.Close();
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test9Ordering()
        {
            var odb = Open("get.neodatis");
            var query = new SimpleNativeQuery235();
            query.OrderByDesc("name,email");
            var l = odb.Query<User>(query, true);
            var i = l.Count - 1;
            AssertFalse(l.Count == 0);
            while (l.HasNext())
            {
                var user = l.Next();
                // println(user.getName() + " / " + user.getEmail());
                AssertEquals("olivier " + i / 3, user.GetName());
                AssertEquals("olivier@neodatis.org " + ((i % 3) + 1), user.GetEmail());
                i--;
            }
            odb.Close();
        }
    }
}
