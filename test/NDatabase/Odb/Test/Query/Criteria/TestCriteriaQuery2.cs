using System;
using NDatabase.Odb.Core.Query.Criteria;
using NDatabase.Odb.Impl.Core.Query.Criteria;
using NDatabase.Tool.Wrappers;
using NUnit.Framework;
using Test.Odb.Test.VO.Attribute;

namespace Test.Odb.Test.Query.Criteria
{
    [TestFixture]
    public class TestCriteriaQuery2 : ODBTest
    {
        public void SetUp(string BaseName)
        {
            base.SetUp();
            DeleteBase(BaseName);
            var odb = Open(BaseName);
            var start = OdbTime.GetCurrentTimeInTicks();
            var size = 50;
            for (var i = 0; i < size; i++)
            {
                var testClass = new TestClass();
                testClass.SetBigDecimal1(new Decimal(i));
                testClass.SetBoolean1(i % 3 == 0);
                testClass.SetChar1((char) (i % 5));
                testClass.SetDate1(new DateTime(start + i));
                testClass.SetDouble1(((double) (i % 10)) / size);
                testClass.SetInt1(size - i);
                testClass.SetString1("test class " + i);
                odb.Store(testClass);
            }
            // println(testClass.getDouble1() + " | " + testClass.getString1() +
            // " | " + testClass.getInt1());
            odb.Close();
        }

        [Test]
        public virtual void Test1()
        {
            var BaseName = GetBaseName();
            SetUp(BaseName);
            var odb = Open(BaseName);
            var aq =
                new CriteriaQuery(
                    Where.Or().Add(Where.Equal("string1", "test class 1")).Add(Where.Equal("string1", "test class 3")));
            aq.OrderByAsc("string1");
            var l = odb.GetObjects<TestClass>(aq, true, -1, -1);
            odb.Close();

            AssertEquals(2, l.Count);
            var testClass = l.GetFirst();
            AssertEquals("test class 1", testClass.GetString1());
        }

        [Test]
        public virtual void Test2()
        {
            var BaseName = GetBaseName();
            SetUp(BaseName);
            var odb = Open(BaseName);
            var aq = new CriteriaQuery(Where.Not(Where.Equal("string1", "test class 2")));
            var l = odb.GetObjects<TestClass>(aq, true, -1, -1);
            AssertEquals(49, l.Count);
            var testClass = l.GetFirst();
            AssertEquals("test class 0", testClass.GetString1());
            odb.Close();
        }

        [Test]
        public virtual void Test3()
        {
            var BaseName = GetBaseName();
            SetUp(BaseName);
            var odb = Open(BaseName);
            var aq =
                new CriteriaQuery(
                    Where.Not(
                        Where.Or().Add(Where.Equal("string1", "test class 0")).Add(Where.Equal("bigDecimal1",
                                                                                               new Decimal(5)))));
            var l = odb.GetObjects<TestClass>(aq, true, -1, -1);
            AssertEquals(48, l.Count);
            var testClass = l.GetFirst();
            AssertEquals("test class 1", testClass.GetString1());
            odb.Close();
        }

        [Test]
        public virtual void Test4Sort()
        {
            var BaseName = GetBaseName();
            SetUp(BaseName);
            var odb = Open(BaseName);
            var aq =
                new CriteriaQuery(
                    Where.Not(
                        Where.Or().Add(Where.Equal("string1", "test class 2")).Add(Where.Equal("string1", "test class 3"))));
            aq.OrderByDesc("double1,int1");
            var l = odb.GetObjects<TestClass>(aq, true, -1, -1);
            // println(l);
            AssertEquals(48, l.Count);
            var testClass = l.GetFirst();
            AssertEquals("test class 9", testClass.GetString1());
            odb.Close();
        }

        [Test]
        public virtual void Test5Sort()
        {
            var BaseName = GetBaseName();
            SetUp(BaseName);
            var odb = Open(BaseName);
            var aq =
                new CriteriaQuery(
                    Where.Not(
                        Where.Or().Add(Where.Equal("string1", "test class 2")).Add(Where.Equal("string1", "test class 3"))));
            // aq.orderByDesc("double1,boolean1,int1");
            aq.OrderByDesc("double1,int1");
            var l = odb.GetObjects<TestClass>(aq, true, -1, -1);
            AssertEquals(48, l.Count);
            var testClass = l.GetFirst();
            AssertEquals("test class 9", testClass.GetString1());
            odb.Close();
        }

        [Test]
        public virtual void Test6Sort()
        {
            var BaseName = GetBaseName();
            SetUp(BaseName);
            var odb = Open(BaseName);
            ICriterion c =
                Where.Or().Add(Where.Equal("string1", "test class 2")).Add(Where.Equal("string1", "test class 3")).Add(
                    Where.Equal("string1", "test class 4")).Add(Where.Equal("string1", "test class 5"));
            var aq = new CriteriaQuery(c);
            aq.OrderByDesc("boolean1,int1");
            var l = odb.GetObjects<TestClass>(aq, true, -1, -1);
            AssertEquals(4, l.Count);
            var testClass = l.GetFirst();
            AssertEquals("test class 3", testClass.GetString1());
            odb.Close();
        }
    }
}
