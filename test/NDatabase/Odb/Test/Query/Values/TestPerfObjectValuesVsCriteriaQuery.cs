using System;
using NDatabase.Odb;
using NDatabase.Odb.Core.Query;
using NDatabase.Odb.Core.Query.Criteria;
using NDatabase.Odb.Core.Query.Values;
using NDatabase.Tool.Wrappers;
using NUnit.Framework;
using Test.NDatabase.Odb.Test.VO.Login;

namespace Test.NDatabase.Odb.Test.Query.Values
{
    [TestFixture]
    public class TestPerfObjectValuesVsCriteriaQuery : ODBTest
    {
        #region Setup/Teardown

        [SetUp]
        public virtual void Populate()
        {
            OdbFactory.Delete("perfOValuesVsCriteria");

            var odb = Open("perfOValuesVsCriteria");
            var nbProfiles = 20;
            var nbUsers = 50;
            var profiles = new Profile[nbProfiles];
            var users = new User2[nbUsers];
            // First creates profiles
            for (var i = 0; i < nbProfiles; i++)
            {
                profiles[i] = new Profile("profile " + i, new VO.Login.Function("function Profile" + i));
                odb.Store(profiles[i]);
            }
            // Then creates users
            for (var i = 0; i < nbUsers; i++)
            {
                users[i] = new User2("user" + i, "user mail" + i, profiles[GetProfileIndex(nbProfiles)], i);
                odb.Store(users[i]);
                if (i % 10 == 0)
                    Println(i);
            }
            odb.Close();
        }

        #endregion

        private int GetProfileIndex(int nbProfiles)
        {
            return Math.Abs(OdbRandom.GetRandomInteger() * nbProfiles) % nbProfiles;
        }

        
        [Test]
        public virtual void T1est()
        {
            var odb = Open("perfOValuesVsCriteria");
            
            Decimal b = odb.Count(new CriteriaQuery(typeof (User2)));
            Println(b);
            
            AssertEquals(Convert.ToDecimal("50"), b);
            odb.Close();
        }

        [Test]
        public virtual void T1estA()
        {
            var odb = Open("perfOValuesVsCriteria");
            
            IQuery q = new CriteriaQuery(typeof(Profile));
            var objects = odb.GetObjects<Profile>(q, false);
            Println(objects.Count);
            Console.Out.WriteLine(q.GetExecutionPlan().GetDetails());
            AssertEquals(20, objects.Count);
            odb.Close();
        }

        [Test]
        public virtual void T1est1()
        {
            var odb = Open("perfOValuesVsCriteria");
            
            var q = new ValuesCriteriaQuery(typeof (User2), Where.Equal("nbLogins", 10)).Field("name");
            var v = odb.GetValues(q);
            Println(v.Count);
            Console.Out.WriteLine(q.GetExecutionPlan().GetDetails());
            AssertEquals(1, v.Count);
            odb.Close();
        }
    }
}
