using System;
using NDatabase.Odb;
using NDatabase.Odb.Core.Query.Values;
using NUnit.Framework;
using Test.NDatabase.Odb.Test.VO.Attribute;
using Test.NDatabase.Odb.Test.VO.Login;
using Test.NDatabase.Tool;

namespace Test.NDatabase.Odb.Test.Query.Values
{
    [TestFixture]
    public class TestGetValues : ODBTest
    {
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test1()
        {
            DeleteBase("valuesA");
            var odb = Open("valuesA");
            odb.Store(new VO.Login.Function("f1"));
            odb.Close();
            odb = Open("valuesA");
            var valuesQuery = odb.ValuesQuery<VO.Login.Function>();
            valuesQuery.Field("name");
            var values = odb.GetValues(valuesQuery);
            Println(values);
            var ov = values.NextValues();
            odb.Close();
            AssertEquals("f1", ov.GetByAlias("name"));
            AssertEquals("f1", ov.GetByIndex(0));
        }

        /// <summary>
        ///   Custom
        /// </summary>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test10()
        {
            DeleteBase("valuesA2");
            var odb = Open("valuesA2");
            var t = new StopWatch();
            t.Start();
            var size = 100;
            long sum = 0;
            for (var i = 0; i < size; i++)
            {
                var tc1 = new TestClass();
                tc1.SetInt1(i);
                odb.Store(tc1);
                sum += i;
            }
            odb.Close();
            t.End();
            Println(" time for insert = " + t.GetDurationInMiliseconds());
            odb = Open("valuesA2");
            t.Start();
            ICustomQueryFieldAction custom = new TestCustomQueryFieldAction();
            var valuesQuery = odb.ValuesQuery<TestClass>().Custom("int1", "custom of int1", custom);
            var values =
                odb.GetValues(valuesQuery);
            t.End();
            var ov = values.NextValues();
            var c = (Decimal) ov.GetByAlias("custom of int1");
            Println(c);
            Println(" time for count = " + t.GetDurationInMiliseconds());
            odb.Close();
        }

        // assertEquals(bsum.divide(new
        // Decimal(size),2,Decimal.ROUND_HALF_DOWN), avg);
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test16()
        {
            DeleteBase("valuesA3");
            IOdb odb = null;
            var t = new StopWatch();
            var size = 100;
            for (var j = 0; j < 10; j++)
            {
                t.Start();
                odb = Open("valuesA3");
                for (var i = 0; i < size; i++)
                {
                    var tc1 = new TestClass();
                    tc1.SetInt1(45);
                    odb.Store(tc1);
                }
                odb.Close();
                t.End();
                Println(" time for insert = " + t.GetDurationInMiliseconds());
            }
            odb = Open("valuesA3");
            t.Start();
            var valuesQuery = odb.ValuesQuery<TestClass>().Count("nb objects");
            var values = odb.GetValues(valuesQuery);
            t.End();
            Println(values);
            Println(" time for count = " + t.GetDurationInMiliseconds());
            var ov = values.NextValues();
            odb.Close();
            AssertEquals(size * 10, ov.GetByAlias("nb objects"));
            AssertEquals(1, values.Count);
        }

        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test17()
        {
            DeleteBase("valuesA4");
            var odb = Open("valuesA4");
            odb.Store(new User("user1", "email1", new Profile("profile name", new VO.Login.Function("f111"))));
            odb.Close();
            odb = Open("valuesA4");
            var valuesQuery = odb.ValuesQuery<User>().Field("name").Field("profile");
            var values = odb.GetValues(valuesQuery);
            Println(values);
            var ov = values.NextValues();
            odb.Close();
            AssertEquals("user1", ov.GetByAlias("name"));
            AssertEquals("user1", ov.GetByIndex(0));
            var p2 = (Profile) ov.GetByAlias("profile");
            AssertEquals("profile name", p2.GetName());
        }

        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test2()
        {
            DeleteBase("valuesA21");
            var odb = Open("valuesA21");
            odb.Store(new VO.Login.Function("f1"));
            odb.Close();
            odb = Open("valuesA21");
            var valuesQuery = odb.ValuesQuery<VO.Login.Function>().Field("name", "Alias of the field");
            var values =
                odb.GetValues(valuesQuery);
            Println(values);
            var ov = values.NextValues();
            odb.Close();
            AssertEquals("f1", ov.GetByAlias("Alias of the field"));
            AssertEquals("f1", ov.GetByIndex(0));
        }

        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test3()
        {
            DeleteBase("valuesA3");
            var odb = Open("valuesA3");
            odb.Store(new User("user1", "email1", new Profile("profile name", new VO.Login.Function("f111"))));
            odb.Close();
            odb = Open("valuesA3");
            var valuesQuery = odb.ValuesQuery<User>().Field("name").Field("profile.name");
            var values = odb.GetValues(valuesQuery);
            Println(values);
            var ov = values.NextValues();
            odb.Close();
            AssertEquals("user1", ov.GetByAlias("name"));
            AssertEquals("user1", ov.GetByIndex(0));
            AssertEquals("profile name", ov.GetByAlias("profile.name"));
            AssertEquals("profile name", ov.GetByIndex(1));
        }

        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test4()
        {
            DeleteBase("valuesA4");
            var odb = Open("valuesA4");
            var tc1 = new TestClass();
            tc1.SetInt1(45);
            odb.Store(tc1);
            var tc2 = new TestClass();
            tc2.SetInt1(5);
            odb.Store(tc2);
            odb.Close();
            odb = Open("valuesA4");
            var valuesQuery = odb.ValuesQuery<TestClass>().Sum("int1", "sum of int1").Count("nb objects");
            var values =
                odb.GetValues(valuesQuery);
            Println(values);
            var ov = values.NextValues();
            odb.Close();
            AssertEquals(50, ov.GetByAlias("sum of int1"));
            AssertEquals(2, ov.GetByAlias("nb objects"));
            AssertEquals(1, values.Count);
        }

        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test5()
        {
            DeleteBase("valuesA5");
            var odb = Open("valuesA5");
            var size = 1000;
            for (var i = 0; i < size; i++)
            {
                var tc1 = new TestClass();
                tc1.SetInt1(45);
                odb.Store(tc1);
            }
            odb.Close();
            odb = Open("valuesA5");
            var valuesQuery = odb.ValuesQuery<TestClass>().Count("nb objects");
            var values = odb.GetValues(valuesQuery);
            Println(values);
            var ov = values.NextValues();
            odb.Close();
            AssertEquals(size, ov.GetByAlias("nb objects"));
            AssertEquals(1, values.Count);
        }

        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test6()
        {
            DeleteBase("valuesA6");
            var odb = Open("valuesA6");
            var size = 1000;
            for (var i = 0; i < size; i++)
            {
                var tc1 = new TestClass();
                tc1.SetInt1(i);
                odb.Store(tc1);
            }
            odb.Close();
            odb = Open("valuesA6");
            var valuesCriteriaQuery = odb.ValuesQuery<TestClass>();
            valuesCriteriaQuery.Descend("int1").Constrain((object) 2).Equal();

            var valuesQuery = valuesCriteriaQuery.Count("nb objects");
            var values =
                odb.GetValues(valuesQuery);
            Println(values);
            var ov = values.NextValues();
            odb.Close();
            AssertEquals(1, ov.GetByAlias("nb objects"));
            AssertEquals(1, values.Count);
        }

        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test7()
        {
            DeleteBase("valuesA7");
            var odb = Open("valuesA7");
            var size = 1000;
            for (var i = 0; i < size; i++)
            {
                var tc1 = new TestClass();
                tc1.SetInt1(i);
                odb.Store(tc1);
            }
            odb.Close();
            odb = Open("valuesA7");
            var query = odb.Query<TestClass>();
            query.Descend("int1").Constrain((object) 2).Equal();
            decimal nb = query.Count();
            Println(nb);
            odb.Close();
            AssertEquals(1, nb);
        }

        /// <summary>
        ///   Max and average
        /// </summary>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test8()
        {
            DeleteBase("valuesA8");
            var odb = Open("valuesA8");
            var size = 1000;
            long sum = 0;
            for (var i = 0; i < size; i++)
            {
                var tc1 = new TestClass();
                tc1.SetInt1(i);
                odb.Store(tc1);
                sum += i;
            }
            odb.Close();
            odb = Open("valuesA8");
            var valuesQuery =
                odb.ValuesQuery<TestClass>().Max("int1", "max of int1").Avg("int1", "avg of int1").Sum("int1",
                                                                                                       "sum of int1");
            var values = odb.GetValues(valuesQuery);
            var ov = values.NextValues();
            var max = (Decimal) ov.GetByAlias("max of int1");
            var avg = (Decimal) ov.GetByAlias("avg of int1");
            var bsum = (Decimal) ov.GetByAlias("sum of int1");
            Println(max);
            Println(avg);
            Println(bsum);
            odb.Close();
            AssertEquals(new Decimal(sum), bsum);
            AssertEquals(new Decimal(size - 1), max);
            AssertEquals(bsum / size, avg);
        }

        /// <summary>
        ///   Min
        /// </summary>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test9()
        {
            DeleteBase("valuesA9");
            var odb = Open("valuesA9");
            var size = 1000;
            long sum = 0;
            for (var i = 0; i < size; i++)
            {
                var tc1 = new TestClass();
                tc1.SetInt1(i);
                odb.Store(tc1);
                sum += i;
            }
            odb.Close();
            odb = Open("valuesA9");
            var valuesQuery =
                odb.ValuesQuery<TestClass>().Min("int1", "min of int1").Avg("int1", "avg of int1").Sum("int1",
                                                                                                       "sum of int1");
            var values = odb.GetValues(valuesQuery);
            var ov = values.NextValues();
            var min = (Decimal) ov.GetByAlias("min of int1");
            var avg = (Decimal) ov.GetByAlias("avg of int1");
            var bsum = (Decimal) ov.GetByAlias("sum of int1");
            Println(min);
            Println(avg);
            Println(bsum);
            odb.Close();
            AssertEquals(new Decimal(sum), bsum);
            AssertEquals(new Decimal(0), min);
            AssertEquals(bsum / 1000, avg);
        }
    }
}
