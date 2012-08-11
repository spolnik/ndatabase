using System;
using System.Collections;
using NDatabase.Odb;
using NDatabase.Odb.Core.Query;
using NDatabase.Odb.Core.Query.Criteria;
using NDatabase.Odb.Core.Query.NQ;
using NDatabase.Odb.Impl.Core.Layers.Layer3.Engine;
using NDatabase.Odb.Impl.Core.Query.Criteria;
using NDatabase.Tool.Wrappers;
using NUnit.Framework;
using Test.Odb.Test.VO.Login;

namespace Test.Odb.Test.Update
{
    [TestFixture]
    public class TestUpdate : ODBTest
    {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            DeleteBase(FileName);
            var odb = Open(FileName);
            for (var i = 0; i < NbObjects; i++)
            {
                odb.Store(new VO.Login.Function("function " + (i + i)));
                odb.Store(new User("olivier " + i, "olivier@neodatis.com " + i,
                                   new Profile("profile " + i, new VO.Login.Function("inner function " + i))));
            }
            odb.Close();
            odb = Open(FileName);
            var l = odb.GetObjects<VO.Login.Function>();
            AssertEquals(2 * NbObjects, l.Count);
            odb.Close();
        }

        #endregion

        public static int NbObjects = 50;

        public static string FileName = "update.neodatis";

        private sealed class _SimpleNativeQuery_134 : SimpleNativeQuery
        {
            private readonly string newName;

            public _SimpleNativeQuery_134(string newName)
            {
                this.newName = newName;
            }

            public bool Match(User user)
            {
                return user.GetProfile().GetName().Equals(newName);
            }
        }

        private sealed class _SimpleNativeQuery_179 : SimpleNativeQuery
        {
            private readonly string newName;

            public _SimpleNativeQuery_179(string newName)
            {
                this.newName = newName;
            }

            public bool Match(User user)
            {
                return user.GetProfile().GetName().Equals(newName);
            }
        }

        [TearDown]
        public override void TearDown()
        {
            OdbConfiguration.SetMaxNumberOfObjectInCache(300000);
            DeleteBase(FileName);
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test1()
        {
            var odb = Open(FileName);
            IQuery query = new CriteriaQuery(Where.Equal("name", "function 10"));
            var l = odb.GetObjects<VO.Login.Function>(query);
            var size = l.Count;
            AssertFalse(l.Count == 0);
            var f = l.GetFirst();
            var id = odb.GetObjectId(f);
            AssertEquals("function 10", f.GetName());
            var newName = OdbTime.GetCurrentTimeInTicks().ToString();
            f.SetName(newName);
            odb.Store(f);
            odb.Close();
            odb = Open(FileName);
            l = odb.GetObjects<VO.Login.Function>(query);
            query = new CriteriaQuery(Where.Equal("name", newName));
            AssertTrue(size == l.Count + 1);
            l = odb.GetObjects<VO.Login.Function>(query);
            AssertFalse(l.Count == 0);
            AssertEquals(1, l.Count);
            AssertEquals(id, odb.GetObjectId(l.GetFirst()));
            odb.Close();
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test2()
        {
            var odb = Open(FileName);
            var nbProfiles = odb.GetObjects<Profile>().Count;
            IQuery query = new CriteriaQuery(Where.Equal("profile.name", "profile 10"));
            var l = odb.GetObjects<User>(query);
            var size = l.Count;
            AssertFalse(l.Count == 0);
            var u = l.GetFirst();
            AssertEquals("profile 10", u.GetProfile().GetName());
            var p2 = u.GetProfile();
            var newName = OdbTime.GetCurrentTimeInTicks().ToString() + "-";
            p2.SetName(newName);
            odb.Store(p2);
            odb.Close();
            odb = Open(FileName);
            l = odb.GetObjects<User>(query);
            AssertTrue(l.Count == size - 1);
            query = new _SimpleNativeQuery_134(newName);
            l = odb.GetObjects<User>(query);
            AssertFalse(l.Count == 0);
            var l2 = odb.GetObjects<Profile>(false);
            AssertEquals(nbProfiles, l2.Count);
            odb.Close();
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test3()
        {
            var odb = Open(FileName);
            IQuery pquery = new CriteriaQuery(Where.Equal("name", "profile 10"));
            var nbProfiles = odb.Count(new CriteriaQuery(typeof(Profile)));
            long nbProfiles10 = odb.GetObjects<Profile>(pquery).Count;
            IQuery query = new CriteriaQuery(Where.Equal("profile.name", "profile 10"));
            var l = odb.GetObjects<User>(query);
            var size = l.Count;
            AssertFalse(l.Count == 0);
            var u = l.GetFirst();
            AssertEquals("profile 10", u.GetProfile().GetName());
            var newName = OdbTime.GetCurrentTimeInTicks().ToString() + "+";
            var p2 = u.GetProfile();
            p2.SetName(newName);
            odb.Store(u);
            odb.Close();
            odb = Open(FileName);
            l = odb.GetObjects<User>(query);
            AssertEquals(l.Count + 1, size);
            AssertEquals(nbProfiles10, odb.GetObjects<Profile>(pquery).Count + 1);
            query = new _SimpleNativeQuery_179(newName);
            l = odb.GetObjects<User>(query);
            AssertEquals(1, l.Count);
            var l2 = odb.GetObjects<Profile>(false);
            AssertEquals(nbProfiles, l2.Count);
            odb.Close();
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test4()
        {
            DeleteBase(FileName);
            var odb = Open(FileName);
            OdbConfiguration.SetMaxNumberOfObjectInCache(10);
            try
            {
                IList list = new ArrayList();
                for (var i = 0; i < 15; i++)
                {
                    var function = new VO.Login.Function("function " + i);
                    try
                    {
                        odb.Store(function);
                    }
                    catch (Exception e)
                    {
                        odb.Rollback();
                        odb.Close();
                        AssertTrue(e.Message.IndexOf("Cache is full!") != -1);
                        return;
                    }
                    list.Add(function);
                }
                odb.Close();
                odb = Open(FileName);
                var l = odb.GetObjects<VO.Login.Function>(true);
                l.Next();
                l.Next();
                odb.Store(l.Next());
                odb.Close();
                odb = Open(FileName);
                AssertEquals(15, odb.Count(new CriteriaQuery(typeof(VO.Login.Function))));
                odb.Close();
            }
            finally
            {
                OdbConfiguration.SetMaxNumberOfObjectInCache(300000);
            }
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test5()
        {
            try
            {
                DeleteBase(FileName);
                var odb = Open(FileName);
                IList list = new ArrayList();
                for (var i = 0; i < 15; i++)
                {
                    var function = new VO.Login.Function("function " + i);
                    odb.Store(function);
                    list.Add(function);
                }
                odb.Close();
                OdbConfiguration.SetMaxNumberOfObjectInCache(15);
                odb = Open(FileName);
                IQuery query = new CriteriaQuery(Where.Or().Add(Where.Like("name", "%9")).Add(Where.Like("name", "%8")));
                var l = odb.GetObjects<VO.Login.Function>(query, false);
                AssertEquals(2, l.Count);
                l.Next();
                odb.Store(l.Next());
                odb.Close();
                odb = Open(FileName);
                AssertEquals(15, odb.Count(new CriteriaQuery(typeof(VO.Login.Function))));
                odb.Close();
            }
            finally
            {
                OdbConfiguration.SetMaxNumberOfObjectInCache(300000);
            }
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test6()
        {
            MyObject mo = null;
            DeleteBase(FileName);
            using (var odb = Open(FileName))
            {
                mo = new MyObject(15, "oli");
                mo.SetDate(new DateTime());
                odb.Store(mo);
            }

            MyObject mo2;
            using (var odb = Open(FileName))
            {
                mo2 = odb.GetObjects<MyObject>().GetFirst();
                mo2.SetDate(new DateTime(mo.GetDate().Ticks + 10));
                mo2.SetSize(mo.GetSize() + 1);
                odb.Store(mo2);
            }

            using (var odb = Open(FileName))
            {
                var mo3 = odb.GetObjects<MyObject>().GetFirst();
                AssertEquals(mo3.GetDate().Ticks, mo2.GetDate().Ticks);
                AssertTrue(mo3.GetDate().Ticks > mo.GetDate().Ticks);
                AssertTrue(mo3.GetSize() == mo.GetSize() + 1);
            }
        }

        // println("before:" + mo.getDate().getTime() + " - " + mo.getSize());
        // println("after:" + mo3.getDate().getTime() + " - " + mo3.getSize());
        /// <summary>
        ///   When an object an a collection attribute, and this colllection is changed
        ///   (adding one object),no update in place is possible for instance.
        /// </summary>
        /// <remarks>
        ///   When an object an a collection attribute, and this colllection is changed
        ///   (adding one object),no update in place is possible for instance.
        /// </remarks>
        /// <exception cref="System.Exception">System.Exception</exception>
        [Test]
        public virtual void Test7()
        {
            DeleteBase(FileName);
            var odb = Open(FileName);
            var function = new VO.Login.Function("login");
            var profile = new Profile("operator", function);
            var user = new User("olivier smadja", "olivier@neodatis.com", profile);
            odb.Store(user);
            odb.Close();
            odb = Open(FileName);
            var user2 = odb.GetObjects<User>().GetFirst();
            user2.GetProfile().AddFunction(new VO.Login.Function("new Function"));
            odb.Store(user2);
            odb.Close();
            odb = Open(FileName);
            var user3 = odb.GetObjects<User>().GetFirst();
            AssertEquals(2, user3.GetProfile().GetFunctions().Count);
            var f1 = user3.GetProfile().GetFunctions()[0];
            var f2 = user3.GetProfile().GetFunctions()[1];
            AssertEquals("login", f1.GetName());
            AssertEquals("new Function", f2.GetName());
            odb.Close();
        }

        /// <summary>
        ///   setting one attribute to null
        /// </summary>
        /// <exception cref="System.Exception">System.Exception</exception>
        [Test]
        public virtual void Test8()
        {
            DeleteBase(FileName);
            var odb = Open(FileName);
            var function = new VO.Login.Function("login");
            var profile = new Profile("operator", function);
            var user = new User("olivier smadja", "olivier@neodatis.com", profile);
            odb.Store(user);
            odb.Close();
            odb = Open(FileName);
            var user2 = odb.GetObjects<User>().GetFirst();
            user2.SetProfile(null);
            odb.Store(user2);
            odb.Close();
            odb = Open(FileName);
            var user3 = odb.GetObjects<User>().GetFirst();
            AssertNull(user3.GetProfile());
            odb.Close();
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void TestDirectSave()
        {
            OdbConfiguration.SetSaveHistory(true);
            DeleteBase("btree.neodatis");
            var odb = Open("btree.neodatis");
            var function = new VO.Login.Function("f1");
            odb.Store(function);
            for (var i = 0; i < 2; i++)
            {
                function.SetName(function.GetName() + function.GetName() + function.GetName() + function.GetName());
                odb.Store(function);
            }
            var engine = Dummy.GetEngine(odb);

            var fullClassName = OdbClassUtil.GetFullName(typeof (VO.Login.Function));

            var ci = engine.GetSession(true).GetMetaModel().GetClassInfo(fullClassName, true);
            Println(ci);
            AssertEquals(null, ci.GetCommitedZoneInfo().First);
            AssertEquals(null, ci.GetCommitedZoneInfo().Last);
            AssertEquals(1, ci.GetUncommittedZoneInfo().GetNbObjects());
            odb.Close();
        }

        /// <summary>
        ///   Test updaing a non native attribute with a new non native object
        /// </summary>
        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void TestUpdateObjectReference()
        {
            DeleteBase(FileName);
            var odb = Open(FileName);
            var function = new VO.Login.Function("login");
            var profile = new Profile("operator", function);
            var user = new User("olivier smadja", "olivier@neodatis.com", profile);
            odb.Store(user);
            odb.Close();
            var profile2 = new Profile("new operator", function);
            odb = Open(FileName);
            var user2 = odb.GetObjects<User>().GetFirst();
            user2.SetProfile(profile2);
            odb.Store(user2);
            odb.Close();
            odb = Open(FileName);
            user2 = odb.GetObjects<User>().GetFirst();
            AssertEquals("new operator", user2.GetProfile().GetName());
            AssertEquals(2, odb.GetObjects<Profile>().Count);
            odb.Close();
        }

        /// <summary>
        ///   Test updaing a non native attribute with an already existing non native
        ///   object - with commit
        /// </summary>
        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void TestUpdateObjectReference2()
        {
            DeleteBase(FileName);
            var odb = Open(FileName);
            var function = new VO.Login.Function("login");
            var profile = new Profile("operator", function);
            var user = new User("olivier smadja", "olivier@neodatis.com", profile);
            odb.Store(user);
            odb.Close();
            var profile2 = new Profile("new operator", function);
            odb = Open(FileName);
            odb.Store(profile2);
            odb.Close();
            odb = Open(FileName);
            profile2 = odb.GetObjects<Profile>(new CriteriaQuery(Where.Equal("name", "new operator"))).GetFirst();
            var user2 = odb.GetObjects<User>().GetFirst();
            user2.SetProfile(profile2);
            odb.Store(user2);
            odb.Close();
            odb = Open(FileName);
            user2 = odb.GetObjects<User>().GetFirst();
            AssertEquals("new operator", user2.GetProfile().GetName());
            AssertEquals(2, odb.GetObjects<Profile>().Count);
            odb.Close();
        }

        /// <summary>
        ///   Test updating a non native attribute with an already existing non native
        ///   object without comit
        /// </summary>
        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void TestUpdateObjectReference3()
        {
            DeleteBase(FileName);
            var odb = Open(FileName);
            var function = new VO.Login.Function("login");
            var profile = new Profile("operator", function);
            var user = new User("olivier smadja", "olivier@neodatis.com", profile);
            odb.Store(user);
            odb.Close();
            var profile2 = new Profile("new operator", function);
            odb = Open(FileName);
            odb.Store(profile2);
            var user2 = odb.GetObjects<User>().GetFirst();
            user2.SetProfile(profile2);
            odb.Store(user2);
            odb.Close();
            odb = Open(FileName);
            user2 = odb.GetObjects<User>().GetFirst();
            AssertEquals("new operator", user2.GetProfile().GetName());
            AssertEquals(2, odb.GetObjects<Profile>().Count);
            odb.Close();
        }

        /// <summary>
        ///   Test updating a non native attribute than wall null with an already
        ///   existing non native object without comit
        /// </summary>
        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void TestUpdateObjectReference4()
        {
            DeleteBase(FileName);
            var odb = Open(FileName);
            var function = new VO.Login.Function("login");
            var user = new User("olivier smadja", "olivier@neodatis.com", null);
            odb.Store(user);
            odb.Close();
            var profile2 = new Profile("new operator", function);
            odb = Open(FileName);
            odb.Store(profile2);
            var user2 = odb.GetObjects<User>().GetFirst();
            user2.SetProfile(profile2);
            odb.Store(user2);
            odb.Close();
            odb = Open(FileName);
            user2 = odb.GetObjects<User>().GetFirst();
            AssertEquals("new operator", user2.GetProfile().GetName());
            AssertEquals(1, odb.GetObjects<Profile>().Count);
            odb.Close();
        }

        [Test]
        public virtual void TestUpdateRelation()
        {
            var baseName = GetBaseName();
            var odb = Open(baseName);
            // first create a function
            var f = new VO.Login.Function("f1");
            odb.Store(f);
            odb.Close();
            odb = Open(baseName);
            // reloads the function
            var functions = odb.GetObjects<VO.Login.Function>(new CriteriaQuery(Where.Equal("name", "f1")));
            var f1 = functions.GetFirst();
            // Create a profile with the loaded function
            var profile = new Profile("test", f1);
            odb.Store(profile);
            odb.Close();
            odb = Open(baseName);
            var profiles = odb.GetObjects<Profile>();
            functions = odb.GetObjects<VO.Login.Function>();
            odb.Close();
            DeleteBase(baseName);
            AssertEquals(1, functions.Count);
            AssertEquals(1, profiles.Count);
        }
    }
}
