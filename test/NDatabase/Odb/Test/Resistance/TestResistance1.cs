using System;
using NDatabase.Odb;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Query.Criteria;
using NDatabase.Tool.Wrappers;
using NUnit.Framework;
using Test.NDatabase.Odb.Test.VO.Login;
using Test.NDatabase.Tool;

namespace Test.NDatabase.Odb.Test.Resistance
{
    [TestFixture]
    public class TestResistance1 : ODBTest
    {
        private static readonly string FileName = "resistance1";

        /// <summary>
        ///   1) insert 10000 objects 2) update 5000 * 10 times 3) delete other 5000
        ///   4) check count : must be 5000 5) re-update 5000 * 10 times 6) delete the
        ///   other 5000 7) check count - must be zero
        /// </summary>
        /// <exception cref="System.Exception">System.Exception</exception>
        [Test]
        [Ignore("Long running test")]
        public virtual void Test1WithCommit()
        {
            var stopWatch = new StopWatch();
            stopWatch.Start();
            var size = 10000;
            var size2 = 5000;
            var nbFunctions = 2;
            DeleteBase(FileName);
            var odb = Open(FileName);
            var f1 = new VO.Login.Function("function 1");
            // Create Objects
            for (var i = 0; i < size; i++)
            {
                var p = new Profile("profile number " + i, f1);
                for (var j = 0; j < nbFunctions; j++)
                    p.AddFunction(new VO.Login.Function(" inner function of profile : number " + i + " - " + j));
                var user = new User("user name " + i, "user email " + i, p);
                odb.Store(user);
                if (i % 100 == 0)
                {
                }
            }
            // println("insert " + i);
            odb.Close();
            Println("created");
            // Updates 10 times the objects
            odb = Open(FileName);
            var objects = odb.GetObjects<User>();
            Println("got the object " + objects.Count);
            for (var k = 0; k < 10; k++)
            {
                objects.Reset();
                var start = OdbTime.GetCurrentTimeInMs();
                for (var i = 0; i < size2; i++)
                {
                    var user = objects.Next();
                    user.GetProfile().SetName(user.GetProfile().GetName() + "-updated");
                    odb.Store(user);
                    if (i % 100 == 0)
                    {
                    }
                }
                // println("update " + i + " - " + k);
                Println("Update " + k + " - " + (OdbTime.GetCurrentTimeInMs() - start) + " ms");
            }
            Println("updated");
            // Delete the rest of the objects
            for (var i = size2; i < size; i++)
            {
                odb.Delete(objects.Next());
                if (i % 100 == 0)
                    Println("delete " + i);
            }
            Println("deleted");
            odb.Close();
            // Check object count
            odb = Open(FileName);
            objects = odb.GetObjects<User>();
            AssertEquals(size2, objects.Count);
            // Check data of the objects
            var a = 0;
            while (objects.HasNext())
            {
                var user = objects.Next();
                AssertEquals("user name " + a, user.GetName());
                AssertEquals("user email " + a, user.GetEmail());
                AssertEquals(
                    "profile number " + a +
                    "-updated-updated-updated-updated-updated-updated-updated-updated-updated-updated",
                    user.GetProfile().GetName());
                a++;
            }
            Println("checked");
            for (var k = 0; k < 10; k++)
            {
                objects.Reset();
                for (var i = 0; i < size2; i++)
                {
                    var user = objects.Next();
                    user.GetProfile().SetName(user.GetProfile().GetName() + "-updated" + "-");
                    odb.Store(user);
                }
            }
            Println("re-updated");
            odb.Close();
            // delete objects
            odb = Open(FileName);
            objects = odb.GetObjects<User>();
            a = 0;
            while (objects.HasNext())
            {
                odb.Delete(objects.Next());
                a++;
            }
            AssertEquals(size2, a);
            odb.Close();
            odb = Open(FileName);
            AssertEquals(0, odb.GetObjects<User>().Count);
            AssertEquals(0, odb.Count(new CriteriaQuery<User>()));
            Println("deleted");
            odb.Close();
            stopWatch.End();
            Println("Total time 1 = " + stopWatch.GetDurationInMiliseconds());
            if (stopWatch.GetDurationInMiliseconds() > 90700)
                Fail("time is > than " + 90700 + " = " + stopWatch.GetDurationInMiliseconds());
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test1WithCommit2()
        {
            var stopWatch = new StopWatch();
            stopWatch.Start();
            var size = 2;
            var size2 = 1;
            var nbFunctions = 1;
            DeleteBase(FileName);
            var odb = Open(FileName);
            var f1 = new VO.Login.Function("function 1");
            Println(odb.Count(new CriteriaQuery<User>()));
            // Create Objects
            for (var i = 0; i < size; i++)
            {
                var p = new Profile("profile number " + i, f1);
                for (var j = 0; j < nbFunctions; j++)
                    p.AddFunction(new VO.Login.Function(" inner function of profile : number " + i + " - " + j));
                var user = new User("user name " + i, "user email " + i, p);
                odb.Store(user);
                if (i % 100 == 0)
                    Println("insert " + i);
            }
            odb.Close();
            Println("created");
            // Updates 10 times the objects
            odb = Open(FileName);
            var objects = odb.GetObjects<User>();
            Println("got the object " + objects.Count);
            for (var k = 0; k < 3; k++)
            {
                objects.Reset();
                var start = OdbTime.GetCurrentTimeInMs();
                for (var i = 0; i < size; i++)
                {
                    var user = objects.Next();
                    user.GetProfile().SetName(user.GetProfile().GetName() + "-updated");
                    odb.Store(user);
                    if (i % 100 == 0)
                        Println("update " + i + " - " + k);
                }
                Println("Update " + k + " - " + (OdbTime.GetCurrentTimeInMs() - start) + " ms");
            }
            Println("updated");
            Println("deleted");
            odb.Close();
            // Check object count
            odb = Open(FileName);
            objects = odb.GetObjects<User>();
            AssertEquals(objects.Count, size);
            // Check data of the objects
            var a = 0;
            while (objects.HasNext())
            {
                var user = objects.Next();
                AssertEquals("user name " + a, user.GetName());
                AssertEquals("user email " + a, user.GetEmail());
                AssertEquals("profile number " + a + "-updated-updated-updated", user.GetProfile().GetName());
                a++;
            }
            Println("checked");
            for (var k = 0; k < 10; k++)
            {
                objects.Reset();
                for (var i = 0; i < size2; i++)
                {
                    var user = objects.Next();
                    user.GetProfile().SetName(user.GetProfile().GetName() + "-updated" + "-");
                    odb.Store(user);
                }
            }
            Println("re-updated");
            odb.Close();
            // delete objects
            odb = Open(FileName);
            objects = odb.GetObjects<User>();
            a = 0;
            while (objects.HasNext())
            {
                odb.Delete(objects.Next());
                a++;
            }
            AssertEquals(size, a);
            odb.Close();
            odb = Open(FileName);
            AssertEquals(0, odb.GetObjects<User>().Count);
            AssertEquals(0, odb.Count(new CriteriaQuery<User>()));
            Println("deleted");
            odb.Close();
            stopWatch.End();
            Println("Total time 1 = " + stopWatch.GetDurationInMiliseconds());
            if (stopWatch.GetDurationInMiliseconds() > 90700)
                Fail("time is > than " + 90700 + " = " + stopWatch.GetDurationInMiliseconds());
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test1WithCommit3()
        {
            var stopWatch = new StopWatch();
            stopWatch.Start();
            DeleteBase(FileName);
            var odb = Open(FileName);
            var f1 = new VO.Login.Function("function 1");
            Println(odb.Count(new CriteriaQuery<User>()));
            var p = new Profile("profile number 0", f1);
            p.AddFunction(new VO.Login.Function("f1"));
            var user = new User("user name 0", "user email 0", p);
            odb.Store(user);
            var p2 = new Profile("profile number 0", f1);
            p2.AddFunction(new VO.Login.Function("f2"));
            var user2 = new User("user name 0", "user email 0", p2);
            odb.Store(user2);
            odb.Close();
            odb = Open(FileName);
            IObjects<User> objects = null;
            for (var k = 0; k < 2; k++)
            {
                Console.Out.WriteLine(":" + k);
                objects = odb.GetObjects<User>();
                while (objects.HasNext())
                {
                    user = objects.Next();
                    user.GetProfile().SetName(user.GetProfile().GetName() + "-updated");
                    Println(user.GetProfile().GetName());
                    odb.Store(user);
                }
            }
            odb.Close();
            odb = Open(FileName);
            objects = odb.GetObjects<User>();
            AssertEquals(2, objects.Count);
            odb.Close();
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test1WithCommit4()
        {
            var stopWatch = new StopWatch();
            stopWatch.Start();
            DeleteBase(FileName);
            var odb = Open(FileName);
            var f1 = new VO.Login.Function("function1");
            odb.Store(f1);
            var f2 = new VO.Login.Function("function2");
            odb.Store(f2);
            odb.Close();
            odb = Open(FileName);
            var objects = odb.GetObjects<VO.Login.Function>();
            VO.Login.Function f = null;
            Println("got the object " + objects.Count);
            for (var k = 0; k < 2; k++)
            {
                objects.Reset();
                while (objects.HasNext())
                {
                    f = objects.Next();
                    f.SetName(f.GetName() + "updated-");
                    odb.Store(f);
                }
            }
            odb.Close();
            odb = Open(FileName);
            objects = odb.GetObjects<VO.Login.Function>();
            odb.Close();
        }

        /// <summary>
        ///   1) insert 10000 objects 2) update 5000 * 10 times 3) delete other 5000
        ///   4) check count : must be 5000 5) re-update 5000 * 10 times 6) delete the
        ///   other 5000 7) check count - must be zero
        /// </summary>
        /// <exception cref="System.Exception">System.Exception</exception>
        [Test]
        [Ignore("Long running test")]
        public virtual void Test1WithoutCommit()
        {
            var stopWatch = new StopWatch();
            stopWatch.Start();
            var size = 10000;
            var size2 = 5000;
            var nbFunctions = 10;
            DeleteBase(FileName);
            var odb = Open(FileName);
            var f1 = new VO.Login.Function("function 1");
            // Create Objects
            for (var i = 0; i < size; i++)
            {
                var p = new Profile("profile number " + i, f1);
                for (var j = 0; j < nbFunctions; j++)
                    p.AddFunction(new VO.Login.Function(" inner function of profile : number " + i + " - " + j));
                var user = new User("user name " + i, "user email " + i, p);
                odb.Store(user);
            }
            Println("created");
            // Updates 10 times the objects
            var objects = odb.GetObjects<User>();
            for (var k = 0; k < 10; k++)
            {
                objects.Reset();
                for (var i = 0; i < size2; i++)
                {
                    var user = objects.Next();
                    user.GetProfile().SetName(user.GetProfile().GetName() + "-updated");
                    odb.Store(user);
                }
            }
            Println("updated");
            // Delete the rest of the objects
            for (var i = size2; i < size; i++)
                odb.Delete(objects.Next());
            Println("deleted");
            // Check object count
            objects = odb.GetObjects<User>();
            AssertEquals(size2, objects.Count);
            // Check data of the objects
            var a = 0;
            while (objects.HasNext())
            {
                var user = objects.Next();
                AssertEquals("user name " + a, user.GetName());
                AssertEquals("user email " + a, user.GetEmail());
                AssertEquals(
                    "profile number " + a +
                    "-updated-updated-updated-updated-updated-updated-updated-updated-updated-updated",
                    user.GetProfile().GetName());
                a++;
            }
            Println("checked");
            for (var k = 0; k < 10; k++)
            {
                objects.Reset();
                for (var i = 0; i < size2; i++)
                {
                    var user = objects.Next();
                    user.GetProfile().SetName(user.GetProfile().GetName() + "-updated" + "-");
                    odb.Store(user);
                }
            }
            Println("re-updated");
            objects = odb.GetObjects<User>();
            var engine = odb.GetStorageEngine();
            var uncommited =
                engine.GetSession(true).GetMetaModel().GetClassInfo(typeof (User).FullName, true).UncommittedZoneInfo;
            CIZoneInfo commited =
                engine.GetSession(true).GetMetaModel().GetClassInfo(typeof (User).FullName, true).CommitedZoneInfo;
            Println("Before commit : uncommited=" + uncommited);
            Println("Before commit : commited=" + commited);
            a = 0;
            while (objects.HasNext())
            {
                // println("a="+a);
                odb.Delete(objects.Next());
                a++;
            }
            AssertEquals(size2, a);
            AssertEquals(0, odb.GetObjects<User>().Count);
            AssertEquals(0, odb.Count(new CriteriaQuery<User>()));
            Println("deleted");
            odb.Close();
            stopWatch.End();
            Println("Total time 2 = " + stopWatch.GetDurationInMiliseconds());
            if (stopWatch.GetDurationInMiliseconds() > 108438)
                Fail("time is > than " + 108438 + " = " + stopWatch.GetDurationInMiliseconds());
        }
    }
}
