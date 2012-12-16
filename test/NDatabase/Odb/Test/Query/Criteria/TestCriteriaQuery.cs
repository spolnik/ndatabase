using System;
using System.Threading;
using NDatabase2.Odb;
using NDatabase2.Odb.Core.Query;
using NDatabase2.Odb.Core.Query.Criteria;
using NUnit.Framework;

namespace Test.NDatabase.Odb.Test.Query.Criteria
{
    [TestFixture]
    public class TestCriteriaQuery : ODBTest
    {
        public void SetUp(string baseName)
        {
            base.SetUp();
            DeleteBase(baseName);
            var odb = Open(baseName);
            for (var i = 0; i < 50; i++)
                odb.Store(new VO.Login.Function("function " + i));
            odb.Close();
        }

        public void TearDown(string baseName)
        {
            DeleteBase(baseName);
        }

        [Test]
        public virtual void Test1()
        {
            var baseName = GetBaseName();
            SetUp(baseName);
            var odb = Open(baseName);
            var aq =
                odb.CreateCriteriaQuery<VO.Login.Function>();

            aq.Equal("name", "function 2").Or(aq.Equal("name", "function 3"));

            var l = odb.Query<VO.Login.Function>(aq, true, -1, -1);
            AssertEquals(2, l.Count);
            var f = l.GetFirst();
            AssertEquals("function 2", f.GetName());
            Println(l);
            odb.Close();
            Console.ReadLine();
        }

        [Test]
        public virtual void Test2()
        {
            var baseName = GetBaseName();
            SetUp(baseName);
            var odb = Open(baseName);
            var aq = odb.CreateCriteriaQuery<VO.Login.Function>();
            aq.Equal("name", "function 2").Not();
            var l = odb.Query<VO.Login.Function>(aq, true, -1, -1);
            AssertEquals(49, l.Count);
            var f = l.GetFirst();
            AssertEquals("function 0", f.GetName());
            odb.Close();
        }

        [Test]
        public virtual void Test3()
        {
            var baseName = GetBaseName();
            SetUp(baseName);
            var odb = Open(baseName);
            var aq =
                odb.CreateCriteriaQuery<VO.Login.Function>();

            aq.Equal("name", "function 2").Or(aq.Equal("name", "function 3")).Not();

            var l = odb.Query<VO.Login.Function>(aq, true, -1, -1);
            AssertEquals(48, l.Count);
            var f = l.GetFirst();
            AssertEquals("function 0", f.GetName());
            odb.Close();
        }

        [Test]
        public virtual void Test4Sort()
        {
            var baseName = GetBaseName();
            SetUp(baseName);
            var d = OdbConfiguration.GetDefaultIndexBTreeDegree();
            try
            {
                OdbConfiguration.SetDefaultIndexBTreeDegree(40);
                var odb = Open(baseName);
                var aq =
                    odb.CreateCriteriaQuery<VO.Login.Function>();

                aq.Equal("name", "function 2").Or(aq.Equal("name", "function 3")).Not();

                aq.OrderByDesc("name");
                
                var l = odb.Query<VO.Login.Function>(aq, true, -1, -1);
                odb.Close();

                AssertEquals(48, l.Count);
                var f = l.GetFirst();
                AssertEquals("function 9", f.GetName());
            }
            finally
            {
                OdbConfiguration.SetDefaultIndexBTreeDegree(d);
            }
        }

        [Test]
        public virtual void TestDate1()
        {
            var baseName = GetBaseName();
            SetUp(baseName);
            var odb = Open(baseName);
            var myDates = new MyDates();
            var d1 = new DateTime();
            Thread.Sleep(100);

            var d2 = new DateTime();
            Thread.Sleep(100);
            var d3 = new DateTime();
            myDates.SetDate1(d1);
            myDates.SetDate2(d3);
            myDates.SetI(5);
            odb.Store(myDates);
            odb.Close();
            odb = Open(baseName);

            IQuery query = odb.CreateCriteriaQuery<MyDates>();
            query.LessOrEqual("date1", d2).And(Where.GreaterOrEqual("date2", d2)).And(query.Equal("i", 5));
            var objects = query.Execute<MyDates>();
            AssertEquals(1, objects.Count);
            odb.Close();
        }

        [Test]
        public virtual void TestEqual2()
        {
            var baseName = GetBaseName();
            SetUp(baseName);
            var odb = Open(baseName);
            var aq = odb.CreateCriteriaQuery<VO.Login.Function>();
            aq.Equal("name", "FuNcTiOn 1");
            aq.OrderByDesc("name");
            var l = odb.Query<VO.Login.Function>(aq, true, -1, -1);
            AssertEquals(0, l.Count);
            odb.Close();
        }

        [Test]
        public virtual void TestILike()
        {
            var baseName = GetBaseName();
            SetUp(baseName);
            var odb = Open(baseName);
            var aq = odb.CreateCriteriaQuery<VO.Login.Function>();
            aq.InvariantLike("name", "FUNc%");
            aq.OrderByDesc("name");
            var l = odb.Query<VO.Login.Function>(aq, true, -1, -1);
            AssertEquals(50, l.Count);
            odb.Close();
        }

        [Test]
        public virtual void TestIequal()
        {
            var baseName = GetBaseName();
            SetUp(baseName);
            var odb = Open(baseName);
            var aq = odb.CreateCriteriaQuery<VO.Login.Function>();
            aq.InvariantEqual("name", "FuNcTiOn 1");
            aq.OrderByDesc("name");
            var l = odb.Query<VO.Login.Function>(aq, true, -1, -1);
            AssertEquals(1, l.Count);
            odb.Close();
        }

        [Test]
        public virtual void TestLike()
        {
            var baseName = GetBaseName();
            SetUp(baseName);
            var odb = Open(baseName);
            var aq = odb.CreateCriteriaQuery<VO.Login.Function>();
            aq.Like("name", "func%");
            aq.OrderByDesc("name");
            var l = odb.Query<VO.Login.Function>(aq, true, -1, -1);
            AssertEquals(50, l.Count);
            odb.Close();
        }

        [Test]
        public virtual void TestLike2()
        {
            var baseName = GetBaseName();
            SetUp(baseName);
            var odb = Open(baseName);
            var aq = odb.CreateCriteriaQuery<VO.Login.Function>();
            aq.Like("name", "FuNc%");
            aq.OrderByDesc("name");
            var l = odb.Query<VO.Login.Function>(aq, true, -1, -1);
            AssertEquals(0, l.Count);
            odb.Close();
        }
    }
}
