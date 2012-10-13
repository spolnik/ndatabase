using System;
using System.Globalization;
using NDatabase2.Odb.Core.Query.Values;
using NUnit.Framework;
using Test.NDatabase.Odb.Test.VO.Attribute;
using Test.NDatabase.Odb.Test.VO.Login;

namespace Test.NDatabase.Odb.Test.Query.Values
{
    [TestFixture]
    public class TestGetValuesGroupBy : ODBTest
    {
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test1()
        {
            DeleteBase("values2.test1.odb");
            var odb = Open("values2.test1.odb");
            var tc1 = new TestClass();
            tc1.SetInt1(45);
            odb.Store(tc1);
            var tc2 = new TestClass();
            tc2.SetInt1(45);
            odb.Store(tc2);
            var tc3 = new TestClass();
            tc3.SetInt1(46);
            odb.Store(tc3);
            odb.Close();
            odb = Open("values2.test1.odb");
            var vq = new ValuesCriteriaQuery<TestClass>().Sum("int1", "sum of int1").GroupBy("int1");
            vq.OrderByAsc("int1");
            var values = odb.GetValues<TestClass>(vq);
            AssertEquals(2, values.Count);
            Println(values);
            var ov = values.NextValues();
            AssertEquals(new Decimal(90), ov.GetByAlias("sum of int1"));
            ov = values.NextValues();
            AssertEquals(new Decimal(46), ov.GetByAlias("sum of int1"));
            odb.Close();
            AssertEquals(2, values.Count);
        }

        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test2()
        {
            DeleteBase("values2.test2.odb");
            var odb = Open("values2.test2.odb");
            var tc1 = new TestClass();
            tc1.SetInt1(45);
            odb.Store(tc1);
            var tc2 = new TestClass();
            tc2.SetInt1(45);
            odb.Store(tc2);
            var tc3 = new TestClass();
            tc3.SetInt1(46);
            odb.Store(tc3);
            odb.Close();
            odb = Open("values2.test2.odb");
            var vq =
                new ValuesCriteriaQuery<TestClass>().Sum("int1", "sum of int1").Count("count").GroupBy("int1");
            vq.OrderByAsc("int1");
            var values = odb.GetValues<TestClass>(vq);
            Println(values);
            var ov = values.NextValues();
            AssertEquals(new Decimal(90), ov.GetByAlias("sum of int1"));
            AssertEquals(new Decimal(2), ov.GetByAlias("count"));
            ov = values.NextValues();
            AssertEquals(new Decimal(46), ov.GetByAlias("sum of int1"));
            AssertEquals(new Decimal(1), ov.GetByAlias("count"));
            odb.Close();
            AssertEquals(2, values.Count);
        }

        /// <summary>
        ///   Retrieving the name of the profile, the number of user for that profile
        ///   and their average login number grouped by the name of the profile
        /// </summary>
        /// <exception cref="System.IO.IOException">System.IO.IOException</exception>
        /// <exception cref="System.Exception">System.Exception</exception>
        [Test]
        public virtual void Test3()
        {
            DeleteBase("values2.test3.odb");
            var odb = Open("values2.test3.odb");
            var p1 = new Profile("profile1", new VO.Login.Function("f1"));
            var p2 = new Profile("profile2", new VO.Login.Function("f2"));
            User u1 = new User2("user1", "user@neodatis.org", p1, 1);
            User u2 = new User2("user2", "user@neodatis.org", p1, 2);
            User u3 = new User2("user3", "user@neodatis.org", p1, 3);
            User u4 = new User2("user4", "user@neodatis.org", p2, 4);
            User u5 = new User2("user5", "user@neodatis.org", p2, 5);
            odb.Store(u1);
            odb.Store(u2);
            odb.Store(u3);
            odb.Store(u4);
            odb.Store(u5);
            odb.Close();
            odb = Open("values2.test3.odb");
            var q =
                new ValuesCriteriaQuery<User2>().Field("profile.name").Count("count").Avg("nbLogins", "avg").
                    GroupBy("profile.name");
            q.OrderByAsc("name");
            var values = odb.GetValues<User2>(q);
            Println(values);
            var ov = values.NextValues();
            AssertEquals(2, values.Count);
            AssertEquals("profile1", ov.GetByAlias("profile.name"));
            AssertEquals(Convert.ToDecimal("3"), ov.GetByAlias("count"));
            AssertEquals(Convert.ToDecimal("2.00", CultureInfo.InvariantCulture), ov.GetByAlias("avg"));
            odb.Close();
            AssertEquals(2, values.Count);
        }
    }
}
