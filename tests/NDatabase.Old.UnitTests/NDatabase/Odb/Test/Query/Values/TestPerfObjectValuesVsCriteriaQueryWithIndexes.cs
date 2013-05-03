using System;
using NDatabase.Api.Query;
using NDatabase.Tool.Wrappers;
using NUnit.Framework;
using Test.NDatabase.Odb.Test.VO.Login;

namespace Test.NDatabase.Odb.Test.Query.Values
{
    [TestFixture]
    public class TestPerfObjectValuesVsCriteriaQueryWithIndexes : ODBTest
    {
        #region Setup/Teardown

        [SetUp]
        public virtual void Populate()
        {
            DeleteBase("perfOValuesVsCriteriaIndex1");
            var odb = Open("perfOValuesVsCriteriaIndex1");
            var atts = new[] {"name"};
            try
            {
                odb.IndexManagerFor<User2>().AddUniqueIndexOn("Index", atts);
            }
            catch (Exception)
            {
            }
            // TODO: handle exception
            var nbProfiles = 200;
            var nbUsers = 500;
            var profiles = new Profile[nbProfiles];
            var users = new User2[nbUsers];
            var userStart = 1500;
            var profileStart = 600;
            // First creates profiles
            for (var i = 0; i < nbProfiles; i++)
            {
                profiles[i] = new Profile("profile " + (i + profileStart), new VO.Login.Function("function Profile" + i));
                odb.Store(profiles[i]);
            }
            // Then creates users
            for (var i = 0; i < nbUsers; i++)
            {
                users[i] = new User2("user" + (i + userStart), "user mail" + i, profiles[GetProfileIndex(nbProfiles)], i);
                odb.Store(users[i]);
                if (i % 100 == 0)
                    Println(i);
            }
            odb.Close();
        }

        #endregion

        private static int GetProfileIndex(int nbProfiles)
        {
            return Math.Abs(OdbRandom.GetRandomInteger() * nbProfiles) % nbProfiles;
        }

        [Test]
        public virtual void T1est()
        {
            var odb = Open("perfOValuesVsCriteriaIndex1");
            
            var q = odb.Query<User2>();
            Decimal b = q.Count();
            Println(b);
            AssertEquals(Convert.ToDecimal("500"), b);
            odb.Close();
        }

        [Test]
        public virtual void T1est1()
        {
            var odb = Open("perfOValuesVsCriteriaIndex1");
            
            IQuery q = odb.Query<User2>();
            q.Descend("name").Constrain((object) "user1599").Equal();
            var objects = q.Execute<User2>(false);
            Println(objects.Count);
            AssertEquals(1, objects.Count);
            objects = q.Execute<User2>(false);
            Println(objects.Count);
            AssertEquals(1, objects.Count);
            odb.Close();
        }

        [Test]
        public virtual void T1estA()
        {
            var odb = Open("perfOValuesVsCriteriaIndex1");
            
            var q = odb.ValuesQuery<User2>().Field("name");
            q.Descend("name").Constrain((object) "user1599").Equal();
            var v = odb.GetValues(q);
            Println(v.Count);
            AssertEquals(1, v.Count);
            odb.Close();
        }
    }
}
