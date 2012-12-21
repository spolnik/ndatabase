using System;
using NDatabase2.Odb;
using NDatabase2.Odb.Core.Query;
using NDatabase2.Odb.Core.Query.Criteria;
using NDatabase2.Tool.Wrappers;
using NUnit.Framework;
using Test.NDatabase.Odb.Test.VO.Attribute;

namespace Test.NDatabase.Odb.Test.Query.Criteria
{
    [TestFixture]
    public class TestCriteriaQuery4 : ODBTest
    {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            DeleteBase(BaseName);
            var odb = Open(BaseName);
            var start = OdbTime.GetCurrentTimeInTicks();
            var size = 50;
            for (var i = 0; i < size; i++)
            {
                var tc = new TestClass();
                tc.SetBigDecimal1(new Decimal(i));
                tc.SetBoolean1(i % 3 == 0);
                tc.SetChar1((char) (i % 5));
                tc.SetDate1(new DateTime(1000 + start + i));
                tc.SetDouble1(((double) (i % 10)) / size);
                tc.SetInt1(size - i);
                tc.SetString1("test class " + i);
                odb.Store(tc);
            }
            var testClass = new TestClass();
            testClass.SetBigDecimal1(new Decimal(190.95));
            testClass.SetBoolean1(true);
            testClass.SetChar1('s');
            correctDate = new DateTime();
            testClass.SetDate1(correctDate);
            testClass.SetDouble1(190.99);
            testClass.SetInt1(190);
            testClass.SetString1("test class with values");
            odb.Store(testClass);
            var testClass2 = new TestClass();
            testClass2.SetBigDecimal1(0);
            testClass2.SetBoolean1(true);
            testClass2.SetChar1('s');
            correctDate = new DateTime();
            testClass2.SetDate1(correctDate);
            testClass2.SetDouble1(191.99);
            testClass2.SetInt1(1901);
            testClass2.SetString1("test class with null Decimal");
            odb.Store(testClass2);
            var testClass3 = new TestClass();
            odb.Store(testClass3);
            odb.Close();
        }

        [TearDown]
        public override void TearDown()
        {
            DeleteBase(BaseName);
        }

        #endregion

        private DateTime correctDate;

        public static readonly string BaseName = "soda-native-object.neodatis";

        [Test]
        public virtual void TestIsNotNull()
        {
            IOdb odb = null;
            try
            {
                odb = Open(BaseName);
                var query = odb.Query<TestClass>();
                query.Descend("bigDecimal1").Constrain(null).Equals().Not();
                var l = query.Execute<TestClass>();
                AssertEquals(53, l.Count);
            }
            finally
            {
                if (odb != null)
                    odb.Close();
            }
        }

        [Test]
        public virtual void TestIsNull()
        {
            IOdb odb = null;
            try
            {
                odb = Open(BaseName);
                var query = odb.Query<TestClass>();
                query.Descend("bigDecimal1").Constrain(null).Equals();
                var l = query.Execute<TestClass>();
                AssertEquals(0, l.Count);
            }
            finally
            {
                if (odb != null)
                    odb.Close();
            }
        }

        [Test]
        public virtual void TestSodaWithBoolean()
        {
            var odb = Open(BaseName);
            var query = odb.Query<TestClass>();
            query.Descend("boolean1").Constrain((object) true).Equals();
            var l = query.Execute<TestClass>();
            AssertTrue(l.Count > 1);
            query = odb.Query<TestClass>();
            query.Descend("boolean1").Constrain((object) true).Equals();
            l = query.Execute<TestClass>();
            AssertTrue(l.Count > 1);
            odb.Close();
        }

        [Test]
        public virtual void TestSodaWithDate()
        {
            var odb = Open(BaseName);

            var query =
                odb.Query<TestClass>();

            ((IConstraint) query.Descend("string1").Constrain((object) "test class with values").Equals()).And(query.Descend("date1").Constrain((object) new DateTime(correctDate.Millisecond)).Equals());

            var l = query.Execute<TestClass>();

            query =
                odb.Query<TestClass>();

            ((IConstraint) query.Descend("string1").Constrain((object) "test class with values").Equals()).And(query.Descend("date1").GreaterOrEqual(new DateTime(
                                                                                                                            correctDate.Millisecond)));
            l = query.Execute<TestClass>();
            if (l.Count != 1)
            {
                query = odb.Query<TestClass>();
                query.Descend("string1").Constrain((object) "test class with null Decimal").Equals();
                var l2 = query.Execute<TestClass>();
                Println(l2);
                Println(correctDate.Millisecond);
                l = query.Execute<TestClass>();
            }
            AssertEquals(1, l.Count);
            odb.Close();
        }

        [Test]
        public virtual void TestSodaWithDouble()
        {
            var odb = Open(BaseName);
            var query = odb.Query<TestClass>();
            query.Descend("double1").Constrain((object) 190.99).Equals();
            var l = query.Execute<TestClass>();
            AssertEquals(1, l.Count);
            query = odb.Query<TestClass>();
            query.Descend("double1").GreaterThan((double) 189);
            l = query.Execute<TestClass>();
            AssertTrue(l.Count >= 1);
            query = odb.Query<TestClass>();
            query.Descend("double1").LessThan((double) 191);
            l = query.Execute<TestClass>();
            AssertTrue(l.Count >= 1);
            odb.Close();
        }

        [Test]
        public virtual void TestSodaWithInt()
        {
            var odb = Open(BaseName);
            var query = odb.Query<TestClass>();
            query.Descend("int1").Constrain((object) 190).Equals();
            var l = query.Execute<TestClass>();
            AssertEquals(1, l.Count);
            query = odb.Query<TestClass>();
            query.Descend("int1").GreaterThan(189);
            l = query.Execute<TestClass>();
            AssertTrue(l.Count >= 1);
            query = odb.Query<TestClass>();
            query.Descend("int1").LessThan(191);
            l = query.Execute<TestClass>();
            AssertTrue(l.Count >= 1);
            odb.Close();
        }
    }
}
