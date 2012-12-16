using System;
using System.Collections;
using NDatabase2.Odb;
using NDatabase2.Odb.Core.Query;
using NDatabase2.Odb.Core.Query.Criteria;
using NDatabase2.Tool.Wrappers;
using NUnit.Framework;
using Test.NDatabase.Odb.Test.VO.Login;

namespace Test.NDatabase.Odb.Test.Update
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
            var l = odb.Query<VO.Login.Function>();
            AssertEquals(2 * NbObjects, l.Count);
            odb.Close();
        }

        [TearDown]
        public override void TearDown()
        {
            DeleteBase(FileName);
        }

        #endregion

        public static int NbObjects = 50;

        public static string FileName = "update.neodatis";

        [Test]
        public virtual void Test1()
        {
            var odb = Open(FileName);
            IQuery query = odb.CreateCriteriaQuery<VO.Login.Function>();
            query.Equal("name", "function 10");
            var l = query.Execute<VO.Login.Function>();
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
            query = odb.CreateCriteriaQuery<VO.Login.Function>();
            query.Equal("name", "function 10");
            l = query.Execute<VO.Login.Function>();

            query = odb.CreateCriteriaQuery<VO.Login.Function>();
            query.Equal("name", newName);
            AssertTrue(size == l.Count + 1);
            l = query.Execute<VO.Login.Function>();

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
            var nbProfiles = odb.Query<Profile>().Count;
            IQuery query = odb.CreateCriteriaQuery<User>();
            query.Equal("profile.name", "profile 10");
            var l = query.Execute<User>();
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
            query = odb.CreateCriteriaQuery<User>();
            query.Equal("profile.name", "profile 10");
            l = query.Execute<User>();
            AssertTrue(l.Count == size - 1);
            
            var l2 = odb.Query<Profile>(false);
            AssertEquals(nbProfiles, l2.Count);
            odb.Close();
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test3()
        {
            var odb = Open(FileName);
            IQuery pquery = odb.CreateCriteriaQuery<Profile>();
            pquery.Equal("name", "profile 10");
            var nbProfiles = odb.CreateCriteriaQuery<Profile>().Count();
            long nbProfiles10 = pquery.Execute<Profile>().Count;
            IQuery query = odb.CreateCriteriaQuery<User>();
            query.Equal("profile.name", "profile 10");
            var l = query.Execute<User>();
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
            pquery = odb.CreateCriteriaQuery<Profile>();
            pquery.Equal("name", "profile 10");
            query = odb.CreateCriteriaQuery<User>();
            query.Equal("profile.name", "profile 10");
            l = query.Execute<User>();
            AssertEquals(l.Count + 1, size);
            AssertEquals(nbProfiles10, pquery.Execute<Profile>().Count + 1);
            
            var l2 = odb.Query<Profile>(false);
            AssertEquals(nbProfiles, l2.Count);
            odb.Close();
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test4()
        {
            DeleteBase(FileName);
            var odb = Open(FileName);
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
            var l = odb.Query<VO.Login.Function>(true);
            l.Next();
            l.Next();
            odb.Store(l.Next());
            odb.Close();
            odb = Open(FileName);
            AssertEquals(15, odb.CreateCriteriaQuery<VO.Login.Function>().Count());
            odb.Close();
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test5()
        {
            DeleteBase(FileName);
            
            using (var odb = Open(FileName))
            {
                IList list = new ArrayList();
                for (var i = 0; i < 15; i++)
                {
                    var function = new VO.Login.Function("function " + i);
                    odb.Store(function);
                    list.Add(function);
                }
            }

            using (var odb = Open(FileName))
            {
                IQuery query = odb.CreateCriteriaQuery<VO.Login.Function>();
                query.Like("name", "%9").Or(query.Like("name", "%8"));
                var l = odb.Query<VO.Login.Function>(query, false);
                AssertEquals(2, l.Count);
                l.Next();
                odb.Store(l.Next());
            }
            
            using (var odb = Open(FileName))
            {
                AssertEquals(15, odb.CreateCriteriaQuery<VO.Login.Function>().Count());
            }
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test6()
        {
            MyObject mo;
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
                mo2 = odb.Query<MyObject>().GetFirst();
                mo2.SetDate(new DateTime(mo.GetDate().Ticks + 10));
                mo2.SetSize(mo.GetSize() + 1);
                odb.Store(mo2);
            }

            using (var odb = Open(FileName))
            {
                var mo3 = odb.Query<MyObject>().GetFirst();
                AssertEquals(mo3.GetDate().Ticks, mo2.GetDate().Ticks);
                AssertTrue(mo3.GetDate().Ticks > mo.GetDate().Ticks);
                AssertTrue(mo3.GetSize() == mo.GetSize() + 1);
            }
        }

        // println("before:" + mo.getDate().getTime() + " - " + mo.getSize());
        // println("after:" + mo3.getDate().getTime() + " - " + mo3.getSize());
        /// <summary>
        ///   When an object an a collection attribute, and this colllection is changed
        ///   (adding one object)
        /// </summary>
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
            var user2 = odb.Query<User>().GetFirst();
            user2.GetProfile().AddFunction(new VO.Login.Function("new Function"));
            odb.Store(user2);
            odb.Close();
            odb = Open(FileName);
            var user3 = odb.Query<User>().GetFirst();
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
            var user2 = odb.Query<User>().GetFirst();
            user2.SetProfile(null);
            odb.Store(user2);
            odb.Close();
            odb = Open(FileName);
            var user3 = odb.Query<User>().GetFirst();
            AssertNull(user3.GetProfile());
            odb.Close();
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void TestDirectSave()
        {
            DeleteBase("btree.neodatis");
            var odb = Open("btree.neodatis");
            var function = new VO.Login.Function("f1");
            odb.Store(function);
            for (var i = 0; i < 2; i++)
            {
                function.SetName(function.GetName() + function.GetName() + function.GetName() + function.GetName());
                odb.Store(function);
            }
            var engine = odb.GetStorageEngine();

            var fullClassName = OdbClassUtil.GetFullName(typeof (VO.Login.Function));

            var ci = engine.GetSession(true).GetMetaModel().GetClassInfo(fullClassName, true);
            Println(ci);
            AssertEquals(null, ci.CommitedZoneInfo.First);
            AssertEquals(null, ci.CommitedZoneInfo.Last);
            AssertEquals(1, ci.UncommittedZoneInfo.GetNumberbOfObjects());
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
            var user2 = odb.Query<User>().GetFirst();
            user2.SetProfile(profile2);
            odb.Store(user2);
            odb.Close();
            odb = Open(FileName);
            user2 = odb.Query<User>().GetFirst();
            AssertEquals("new operator", user2.GetProfile().GetName());
            AssertEquals(2, odb.Query<Profile>().Count);
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
            var query = odb.CreateCriteriaQuery<Profile>();
            query.Equal("name", "new operator");
            profile2 = query.Execute<Profile>().GetFirst();
            var user2 = odb.Query<User>().GetFirst();
            user2.SetProfile(profile2);
            odb.Store(user2);
            odb.Close();
            odb = Open(FileName);
            user2 = odb.Query<User>().GetFirst();
            AssertEquals("new operator", user2.GetProfile().GetName());
            AssertEquals(2, odb.Query<Profile>().Count);
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
            var user2 = odb.Query<User>().GetFirst();
            user2.SetProfile(profile2);
            odb.Store(user2);
            odb.Close();
            odb = Open(FileName);
            user2 = odb.Query<User>().GetFirst();
            AssertEquals("new operator", user2.GetProfile().GetName());
            AssertEquals(2, odb.Query<Profile>().Count);
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
            var user2 = odb.Query<User>().GetFirst();
            user2.SetProfile(profile2);
            odb.Store(user2);
            odb.Close();
            odb = Open(FileName);
            user2 = odb.Query<User>().GetFirst();
            AssertEquals("new operator", user2.GetProfile().GetName());
            AssertEquals(1, odb.Query<Profile>().Count);
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
            var query = odb.CreateCriteriaQuery<VO.Login.Function>();
            query.Equal("name", "f1");
            var functions = query.Execute<VO.Login.Function>();
            var f1 = functions.GetFirst();
            // Create a profile with the loaded function
            var profile = new Profile("test", f1);
            odb.Store(profile);
            odb.Close();
            odb = Open(baseName);
            var profiles = odb.Query<Profile>();
            functions = odb.Query<VO.Login.Function>();
            odb.Close();
            DeleteBase(baseName);
            AssertEquals(1, functions.Count);
            AssertEquals(1, profiles.Count);
        }
    }
}
