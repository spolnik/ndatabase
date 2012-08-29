using NDatabase.Odb.Core.Query.NQ;
using NUnit.Framework;
using Test.NDatabase.Odb.Test.VO.Login;

namespace Test.NDatabase.Odb.Test.Query.NQ
{
    [TestFixture]
    public class TestNQQuery2 : ODBTest
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

        // println("NbFunctions " + odb.count(Function.class));
        // println("NbUsers " + odb.count(User.class));

        internal sealed class _SimpleNativeQuery_70 : SimpleNativeQuery
        {
            public bool Match(VO.Login.Function function)
            {
                return true;
            }
        }

        internal sealed class _SimpleNativeQuery_89 : SimpleNativeQuery
        {
            public bool Match(VO.Login.Function function)
            {
                return function.GetName().Equals("function 5");
            }
        }

        private sealed class _SimpleNativeQuery_105 : SimpleNativeQuery
        {
            public bool Match(User user)
            {
                return user.GetProfile().GetName().Equals("profile 5");
            }
        }

        internal sealed class _SimpleNativeQuery_121 : SimpleNativeQuery
        {
            public bool Match(User user)
            {
                return user.GetProfile().GetName().StartsWith("profile");
            }
        }

        private sealed class _SimpleNativeQuery_139 : SimpleNativeQuery
        {
            public bool Match(User user)
            {
                return user.GetProfile().GetName().StartsWith("profile");
            }
        }

        private sealed class _SimpleNativeQuery_158 : SimpleNativeQuery
        {
            public bool Match(User user)
            {
                return user.GetProfile().GetName().StartsWith("profile") &&
                       user.GetEmail().StartsWith("olivier@neodatis.org 1");
            }
        }

        private sealed class _SimpleNativeQuery_183 : SimpleNativeQuery
        {
            public bool Match(User user)
            {
                return user.GetProfile().GetName().StartsWith("profile") &&
                       user.GetEmail().StartsWith("olivier@neodatis.org 2");
            }
        }

        private sealed class _SimpleNativeQuery_208 : SimpleNativeQuery
        {
            public bool Match(User user)
            {
                return user.GetProfile().GetName().StartsWith("profile");
            }
        }

        private sealed class _SimpleNativeQuery_235 : SimpleNativeQuery
        {
            public bool Match(User user)
            {
                return user.GetProfile().GetName().StartsWith("profile");
            }
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test1()
        {
            var odb = Open("get.neodatis");
            odb.GetObjects<VO.Login.Function>(true);
            odb.Close();
            odb = Open("get.neodatis");
            // println("TestNQQuery.test1:"+odb.getObjects (Function.class,true));
            var l = odb.GetObjects<VO.Login.Function>(new _SimpleNativeQuery_70());
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
            var l = odb.GetObjects<VO.Login.Function>(new _SimpleNativeQuery_89());
            odb.Close();
            AssertFalse(l.Count == 0);
            AssertEquals(1, l.Count);
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test3()
        {
            var odb = Open("get.neodatis");
            var l = odb.GetObjects<User>(new _SimpleNativeQuery_105());
            odb.Close();
            AssertFalse(l.Count == 0);
            AssertEquals(3, l.Count);
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test4()
        {
            var odb = Open("get.neodatis");
            SimpleNativeQuery query = new _SimpleNativeQuery_121();
            var l = odb.GetObjects<User>(query, true, 0, 5);
            odb.Close();
            AssertFalse(l.Count == 0);
            AssertEquals(5, l.Count);
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test5()
        {
            var odb = Open("get.neodatis");
            SimpleNativeQuery query = new _SimpleNativeQuery_139();
            var l = odb.GetObjects<User>(query, true, 5, 6);
            odb.Close();
            AssertFalse(l.Count == 0);
            AssertEquals(1, l.Count);
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test6Ordering()
        {
            var odb = Open("get.neodatis");
            SimpleNativeQuery query = new _SimpleNativeQuery_158();
            query.OrderByAsc("name");
            var l = odb.GetObjects<User>(query, true);
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
            SimpleNativeQuery query = new _SimpleNativeQuery_183();
            query.OrderByDesc("name");
            var l = odb.GetObjects<User>(query, true);
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
            SimpleNativeQuery query = new _SimpleNativeQuery_208();
            query.OrderByAsc("name,email");
            var l = odb.GetObjects<User>(query, true);
            var i = 0;
            AssertFalse(l.Count == 0);
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
            SimpleNativeQuery query = new _SimpleNativeQuery_235();
            query.OrderByDesc("name,email");
            var l = odb.GetObjects<User>(query, true);
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
