using System;
using NDatabase.Odb;
using NDatabase.Odb.Core.Query;
using NDatabase.Odb.Core.Query.Criteria;
using NDatabase.Odb.Impl.Core.Query.Criteria;
using NDatabase.Odb.Impl.Core.Query.Values;
using NDatabase.Tool.Wrappers;
using NUnit.Framework;
using Test.Odb.Test;
using Test.Odb.Test.VO.Login;

namespace Query.Values
{
    [TestFixture]
    public class TestPerfObjectValuesVsCriteriaQueryWithIndexes : ODBTest
    {
        private static int GetProfileIndex(int nbProfiles)
        {
            return Math.Abs(OdbRandom.GetRandomInteger() * nbProfiles) % nbProfiles;
        }

        [SetUp]
        public virtual void Populate()
        {
            DeleteBase("perfOValuesVsCriteriaIndex1");
            var odb = Open("perfOValuesVsCriteriaIndex1");
            var atts = new[] {"name"};
            try
            {
                odb.GetClassRepresentation(typeof (User2)).AddUniqueIndexOn("Index", atts, true);
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
                profiles[i] = new Profile("profile " + (i + profileStart), new Function("function Profile" + i));
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

        [Test]
        public virtual void T1est()
        {
            var odb = Open("perfOValuesVsCriteriaIndex1");
            OdbConfiguration.MonitorMemory(true);
            var q = new CriteriaQuery(typeof (User2));
            Decimal b = odb.Count(q);
            Println(b);
            AssertEquals(Convert.ToDecimal("500"), b);
            odb.Close();
        }

        [Test]
        public virtual void T1est1()
        {
            var odb = Open("perfOValuesVsCriteriaIndex1");
            OdbConfiguration.MonitorMemory(true);
            var q = new ValuesCriteriaQuery(typeof (User2), Where.Equal("name", "user1599")).Field("name");
            var v = odb.GetValues(q);
            Println(v.Count);
            AssertEquals(1, v.Count);
            odb.Close();
        }

        [Test]
        public virtual void T1estA()
        {
            var odb = Open("perfOValuesVsCriteriaIndex1");
            OdbConfiguration.MonitorMemory(true);
            IQuery q = new CriteriaQuery(typeof (User2), Where.Equal("name", "user1599"));
            var objects = odb.GetObjects<User2>(q, false);
            Println(objects.Count);
            AssertEquals(1, objects.Count);
            objects = odb.GetObjects<User2>(q, false);
            Println(objects.Count);
            AssertEquals(1, objects.Count);
            odb.Close();
        }
    }
}
