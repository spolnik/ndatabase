using System.Collections.Generic;
using System.Linq;
using NDatabase2.Odb;
using NDatabase2.Tool.Wrappers;
using NUnit.Framework;
using Test.NDatabase.Odb.Test.VO.Login;

namespace Test.NDatabase.Odb.Test.Query.Criteria
{
    [TestFixture]
    public class TestCriteriaQuery3 : ODBTest
    {
        private void Init(string baseName)
        {
            DeleteBase(baseName);
            var odb = Open(baseName);
            var start = OdbTime.GetCurrentTimeInTicks();
            var size = 10;
            for (var i = 0; i < size; i++)
            {
                var u = new User("user" + i, "email" + i,
                                 new Profile("profile" + i, new VO.Login.Function("function " + i)));
                odb.Store(u);
            }
            var user = new User("big user", "big email",
                                new Profile("big profile", new VO.Login.Function("big function 1")));
            user.GetProfile().AddFunction(new VO.Login.Function("big function 2"));
            user.GetProfile().AddFunction(new VO.Login.Function("big function 3"));
            user.GetProfile().AddFunction(new VO.Login.Function("big function 4"));
            odb.Store(user);
            user = new User("user no function", "email no function", new Profile("profile no function"));
            odb.Store(user);
            odb.Close();
        }

        [Test]
        public virtual void Test1()
        {
            var baseName = GetBaseName();
            Init(baseName);
            var odb = Open(baseName);
            var query = odb.CreateCriteriaQuery<User>();
            query.Descend("profile.name").Equal("profile2");
            var l = query.Execute<User>();
            AssertEquals(1, l.Count());
            odb.Close();
        }

        [Test]
        public virtual void TestCriteriaQueryQueryWithObject()
        {
            var baseName = GetBaseName();
            Init(baseName);
            var odb = Open(baseName);
            var p0 = new Profile("profileCust0");
            p0.AddFunction(null);
            p0.AddFunction(new VO.Login.Function("f1"));
            p0.AddFunction(new VO.Login.Function("f2"));
            var p1 = new Profile("profileCust1");
            p1.AddFunction(null);
            p1.AddFunction(new VO.Login.Function("f12"));
            p1.AddFunction(new VO.Login.Function("f22"));
            var user = new User("The user", "themail", p0);
            var user2 = new User("The user2", "themail2", p1);
            odb.Store(user);
            odb.Store(user2);
            odb.Close();
            odb = Open(baseName);
            var criteriaQuery = odb.CreateCriteriaQuery<Profile>();
            criteriaQuery.Descend("name").Equal("profileCust0");
            var pp = criteriaQuery.Execute<Profile>().GetFirst();

            var query = odb.CreateCriteriaQuery<User>();
            query.Descend("profile").Equal(pp);
            var l = query.Execute<User>();
            AssertEquals(1, l.Count());
            user = l.GetFirst();
            AssertEquals("The user", user.GetName());
            odb.Close();
        }

        [Test]
        public virtual void TestCriteriaQueryQueryWithValueInList()
        {
            var baseName = GetBaseName();
            Init(baseName);
            var odb = Open(baseName);
            var p0 = new Profile("profile0");
            p0.AddFunction(null);
            p0.AddFunction(new VO.Login.Function("f1"));
            p0.AddFunction(new VO.Login.Function("f2"));
            var p1 = new Profile("profile1");
            p1.AddFunction(null);
            p1.AddFunction(new VO.Login.Function("f12"));
            p1.AddFunction(new VO.Login.Function("f22"));
            var user = new User("The user", "themail", p0);
            var user2 = new User("The user2", "themail2", p1);
            odb.Store(user);
            odb.Store(user2);
            odb.Close();
            odb = Open(baseName);
            var criteriaQuery = odb.CreateCriteriaQuery<VO.Login.Function>();
            criteriaQuery.Descend("name").Equal("f2");
            var f2bis = criteriaQuery.Execute<VO.Login.Function>().GetFirst();
            var query = odb.CreateCriteriaQuery<User>();
            query.Descend("profile.functions").Contain(f2bis);
            var l = query.Execute<User>();
            AssertEquals(1, l.Count());
            user = l.GetFirst();
            AssertEquals("The user", user.GetName());
            odb.Close();
        }

        [Test]
        public virtual void TestCriteriaQueryQueryWithValueInList2()
        {
            var baseName = GetBaseName();
            Init(baseName);
            var odb = Open(baseName);
            var p0 = new Profile("profile0");
            p0.AddFunction(new VO.Login.Function("f1"));
            p0.AddFunction(new VO.Login.Function("f2"));
            var p1 = new Profile("profile1");
            p0.AddFunction(new VO.Login.Function("f12"));
            p0.AddFunction(new VO.Login.Function("f22"));
            var user = new User("The user", "themail", p0);
            var user2 = new User("The user2", "themail2", p1);
            odb.Store(user);
            odb.Store(user2);
            odb.Close();
            odb = Open(baseName);
            var criteriaQuery = odb.CreateCriteriaQuery<VO.Login.Function>();
            criteriaQuery.Descend("name").Equal("f2");
            var f2bis = criteriaQuery.Execute<VO.Login.Function>().GetFirst();
            var query = odb.CreateCriteriaQuery<Profile>();
            query.Descend("functions").Contain(f2bis);
            var l = query.Execute<Profile>();
            AssertEquals(1, l.Count());
            p1 = l.GetFirst();
            AssertEquals("profile0", p1.GetName());
            odb.Close();
        }

        [Test]
        public virtual void TestCriteriaQueryQueryWithValueInList2_with_null_object()
        {
            var baseName = GetBaseName();
            Init(baseName);
            var odb = Open(baseName);
            var p0 = new Profile("profile0");
            p0.AddFunction(new VO.Login.Function("f1"));
            p0.AddFunction(new VO.Login.Function("f2"));
            p0.AddFunction(new VO.Login.Function("f12"));
            p0.AddFunction(new VO.Login.Function("f22"));

            var p1 = new Profile("profile1");

            var user = new User("The user", "themail", p0);
            var user2 = new User("The user2", "themail2", p1);
            odb.Store(user);
            odb.Store(user2);
            odb.Close();
            odb = Open(baseName);
            var f2bis = new VO.Login.Function("f2");
            var query = odb.CreateCriteriaQuery<Profile>();
            query.Descend("functions").Contain<object>(null);
            var l = query.Execute<Profile>();
            //One from test, one from init
            AssertEquals(2, l.Count());
            p1 = l.First(x => x.GetName().Equals("profile1"));
            AssertEquals("profile1", p1.GetName());
            odb.Close();
        }

        [Test]
        public virtual void TestCriteriaQueryQueryWithValueInList3()
        {
            var baseName = GetBaseName();
            Init(baseName);
            var odb = Open(baseName);
            var p0 = new Profile("profile0");
            p0.AddFunction(null);
            p0.AddFunction(null);
            p0.AddFunction(null);
            var p1 = new Profile("profile1");
            p1.AddFunction(null);
            p1.AddFunction(null);
            p1.AddFunction(new VO.Login.Function("f22"));
            var user = new User("The user", "themail", p0);
            var user2 = new User("The user2", "themail2", p1);
            odb.Store(user);
            odb.Store(user2);
            odb.Close();
            odb = Open(baseName);
            var criteriaQuery = odb.CreateCriteriaQuery<VO.Login.Function>();
            criteriaQuery.Descend("name").Equal("f22");
            var f2bis = criteriaQuery.Execute<VO.Login.Function>().GetFirst();
            var query = odb.CreateCriteriaQuery<User>();
            query.Descend("profile.functions").Contain(f2bis);
            var l = query.Execute<User>();
            AssertEquals(1, l.Count());
            user = l.GetFirst();
            AssertEquals("The user2", user.GetName());
            odb.Close();
        }

        [Test]
        public virtual void TestCriteriaQueryQueryWithValueInList4()
        {
            var baseName = GetBaseName();
            Init(baseName);
            var odb = Open(baseName);
            IList<string> strings = new List<string>();
            var c = new ClassWithListOfString("name", strings);
            c.GetStrings().Add("s1");
            c.GetStrings().Add("s2");
            c.GetStrings().Add("s3");
            IList<string> strings2 = new List<string>();
            var c2 = new ClassWithListOfString("name", strings2);
            c2.GetStrings().Add("s1");
            c2.GetStrings().Add("s2");
            c2.GetStrings().Add("s3");
            odb.Store(c);
            odb.Store(c2);
            odb.Close();
            odb = Open(baseName);
            var query = odb.CreateCriteriaQuery<ClassWithListOfString>();
            query.Descend("strings").Contain("s2222");
            var l = query.Execute<ClassWithListOfString>();
            AssertEquals(0, l.Count());
            odb.Close();
        }

        [Test]
        public virtual void TestCriteriaQueryQueryWithValueInList5()
        {
            var baseName = GetBaseName();
            Init(baseName);
            var odb = Open(baseName);
            IList<string> strings = new List<string>();
            var c = new ClassWithListOfString("name", strings);
            c.GetStrings().Add("s1");
            c.GetStrings().Add(null);
            c.GetStrings().Add("s3");
            IList<string> strings2 = new List<string>();
            var c2 = new ClassWithListOfString("name", null);
            odb.Store(c);
            odb.Store(c2);
            odb.Close();
            odb = Open(baseName);
            var query = odb.CreateCriteriaQuery<ClassWithListOfString>();
            query.Descend("strings").Contain<object>(null);
            var l = query.Execute<ClassWithListOfString>();
            odb.Close();
            AssertEquals(1, l.Count());
        }

        [Test]
        public virtual void TestCriteriaQueryQueryWithValueInList6()
        {
            var baseName = GetBaseName();
            Init(baseName);
            var odb = Open(baseName);
            IList<string> strings = new List<string>();
            var c = new ClassWithListOfString("name", strings);
            c.GetStrings().Add("s1");
            c.GetStrings().Add(null);
            c.GetStrings().Add("s3");
            IList<string> strings2 = new List<string>();
            var c2 = new ClassWithListOfString("name", null);
            odb.Store(c);
            odb.Store(c2);
            odb.Close();
            odb = Open(baseName);
            var query = odb.CreateCriteriaQuery<ClassWithListOfString>();
            query.Descend("strings").Contain("s4");
            var l = query.Execute<ClassWithListOfString>();
            odb.Close();
            AssertEquals(0, l.Count());
        }

        [Test]
        public virtual void TestListSize0()
        {
            var baseName = GetBaseName();
            Init(baseName);
            IOdb odb = null;
            try
            {
                odb = Open(baseName);
                var query = odb.CreateCriteriaQuery<User>();
                query.Descend("profile.functions").SizeEq(0);
                var l = query.Execute<User>();
                AssertEquals(1, l.Count());
                var u = l.GetFirst();
                AssertEquals("profile no function", u.GetProfile().GetName());
            }
            finally
            {
                if (odb != null)
                    odb.Close();
            }
        }

        [Test]
        public virtual void TestListSize1()
        {
            IOdb odb = null;
            try
            {
                var baseName = GetBaseName();
                Init(baseName);
                odb = Open(baseName);
                var query = odb.CreateCriteriaQuery<User>();
                query.Descend("profile.functions").SizeEq(1);
                var l = query.Execute<User>();
                AssertEquals(10, l.Count());
            }
            finally
            {
                if (odb != null)
                    odb.Close();
            }
        }

        [Test]
        public virtual void TestListSize4()
        {
            IOdb odb = null;
            try
            {
                var baseName = GetBaseName();
                Init(baseName);
                odb = Open(baseName);
                var query = odb.CreateCriteriaQuery<User>();
                query.Descend("profile.functions").SizeEq(4);
                var l = query.Execute<User>();
                AssertEquals(1, l.Count());
                var u = l.GetFirst();
                AssertEquals("big profile", u.GetProfile().GetName());
                AssertEquals(4, u.GetProfile().GetFunctions().Count);
            }
            finally
            {
                if (odb != null)
                    odb.Close();
            }
        }

        [Test]
        public virtual void TestListSizeGt2()
        {
            IOdb odb = null;
            try
            {
                var baseName = GetBaseName();
                Init(baseName);
                odb = Open(baseName);
                var query = odb.CreateCriteriaQuery<User>();
                query.Descend("profile.functions").SizeGt(2);
                var l = query.Execute<User>();
                AssertEquals(1, l.Count());
            }
            finally
            {
                if (odb != null)
                    odb.Close();
            }
        }

        [Test]
        public virtual void TestListSizeNotEqulTo1()
        {
            IOdb odb = null;
            try
            {
                var baseName = GetBaseName();
                Init(baseName);
                odb = Open(baseName);
                var query = odb.CreateCriteriaQuery<User>();
                query.Descend("profile.functions").SizeNe(1);
                var l = query.Execute<User>();
                AssertEquals(2, l.Count());
            }
            finally
            {
                if (odb != null)
                    odb.Close();
            }
        }
    }
}
