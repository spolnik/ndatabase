using System;
using System.Collections.Generic;
using NDatabase2.Odb;
using NDatabase2.Tool.Wrappers.Map;
using NUnit.Framework;
using Test.NDatabase.Odb.Test.VO.Login;

namespace Test.NDatabase.Odb.Test.Commit
{
    [TestFixture]
    public class TestCommit : ODBTest
    {
        private object GetInstance(int i)
        {
            var login = new VO.Login.Function("login" + i);
            var logout = new VO.Login.Function("logout" + i);
            IList<VO.Login.Function> list = new List<VO.Login.Function>();
            list.Add(login);
            list.Add(logout);
            var profile = new Profile("operator" + i, list);
            var user = new User("olivier" + i, "olivier@neodatis.com", profile);
            return user;
        }

        [Test]
        public virtual void TestInsertWithCommitsComplexObject()
        {
            DeleteBase("commits");
            IOdb odb = null;
            var size = 5300;
            var commitInterval = 400;
            try
            {
                odb = Open("commits");
                for (var i = 0; i < size; i++)
                {
                    odb.Store(GetInstance(i));
                    if (i % commitInterval == 0)
                        odb.Commit();
                    // println("commiting "+i);
                    if (i % 1000 == 0)
                        Println(i);
                }
            }
            finally
            {
                odb.Close();
            }
            odb = Open("commits");
            var users = odb.Query<User>();
            var profiles = odb.Query<Profile>();
            var functions = odb.Query<VO.Login.Function>();
            var nbUsers = users.Count;
            var nbProfiles = profiles.Count;
            var nbFunctions = functions.Count;
            odb.Close();
            DeleteBase("commits");
            Println("Nb users=" + nbUsers);
            Println("Nb profiles=" + nbProfiles);
            Println("Nb functions=" + nbFunctions);
            AssertEquals(size, nbUsers);
            AssertEquals(size, nbProfiles);
            AssertEquals(size * 2, nbFunctions);
        }

        [Test]
        public virtual void TestInsertWithCommitsSimpleObject()
        {
            DeleteBase("commits");
            IOdb odb = null;
            var size = 10000;
            var commitInterval = 1000;
            try
            {
                odb = Open("commits");
                for (var i = 0; i < size; i++)
                {
                    odb.Store(new VO.Login.Function("function " + i));
                    if (i % commitInterval == 0)
                    {
                        odb.Commit();
                        Console.WriteLine(i);
                    }
                }
            }
            finally
            {
                // println("commiting "+i);
                odb.Close();
            }
            odb = Open("commits");
            var objects = odb.Query<VO.Login.Function>();
            var nbObjects = objects.Count;
            var map = new OdbHashMap<VO.Login.Function, int>();
            VO.Login.Function function = null;
            var j = 0;
            while (objects.HasNext())
            {
                function = objects.Next();
                var ii = map[function];
                if (ii != 0)
                    Println(j + ":" + function.GetName() + " already exist at " + ii);
                else
                    map.Add(function, j);
                j++;
            }
            odb.Close();
            DeleteBase("commits");
            Println("Nb objects=" + nbObjects);
            AssertEquals(size, nbObjects);
        }
    }
}
