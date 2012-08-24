using System;
using NDatabase.Odb;
using NDatabase.Odb.Core.Query;
using NDatabase.Odb.Core.Query.Criteria;
using NDatabase.Odb.Impl.Core.Query.Criteria;
using NDatabase.Odb.Impl.Core.Query.Values;
using NDatabase.Tool.Wrappers;
using NUnit.Framework;
using Test.Odb.Test.VO.Login;

namespace Test.Odb.Test.Query.Values
{
    [TestFixture]
    public class TestPerfObjectValuesVsCriteriaQuery : ODBTest
    {
        #region Setup/Teardown

        [SetUp]
        public virtual void Populate()
        {
            var odb = Open("perfOValuesVsCriteria");
            var nbProfiles = 200;
            var nbUsers = 500000;
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
                if (i % 10000 == 0)
                    Println(i);
            }
            odb.Close();
        }

        #endregion

        private int GetProfileIndex(int nbProfiles)
        {
            return OdbRandom.GetRandomInteger() * nbProfiles;
        }

        /// <exception cref="System.Exception"></exception>
        public static void Main2(string[] args)
        {
            var t = new TestPerfObjectValuesVsCriteriaQuery();
            // t.populate();
            t.T1estA();
        }

        /// <exception cref="System.Exception"></exception>
        public virtual void T1est()
        {
            var odb = Open("perfOValuesVsCriteria");
            OdbConfiguration.MonitorMemory(true);
            IQuery q = new CriteriaQuery(typeof (User2));
            Decimal b = odb.Count(new CriteriaQuery(typeof (User2)));
            Println(b);
            Console.Out.WriteLine(q.GetExecutionPlan().GetDetails());
            AssertEquals(Convert.ToDecimal("500000"), b);
            odb.Close();
        }

        /// <exception cref="System.Exception"></exception>
        public virtual void T1estA()
        {
            var odb = Open("perfOValuesVsCriteria");
            OdbConfiguration.MonitorMemory(true);
            IQuery q = new CriteriaQuery(typeof (User2));
            var objects = odb.GetObjects<User2>(q, false);
            Println(objects.Count);
            Console.Out.WriteLine(q.GetExecutionPlan().GetDetails());
            AssertEquals(2000000, objects.Count);
            odb.Close();
        }

        /// <exception cref="System.Exception"></exception>
        public virtual void T1est1()
        {
            var odb = Open("perfOValuesVsCriteria");
            OdbConfiguration.MonitorMemory(true);
            var q = new ValuesCriteriaQuery(typeof (User2), Where.Equal("nbLogins", 100)).Field("name");
            var v = odb.GetValues(q);
            Println(v.Count);
            Console.Out.WriteLine(q.GetExecutionPlan().GetDetails());
            AssertEquals(2000000, v.Count);
            odb.Close();
        }
    }
}
