using System;
using System.Threading;
using NDatabase.Odb;
using NDatabase.Odb.Core.Query;
using NDatabase.Odb.Core.Query.Criteria;
using NDatabase.Odb.Impl.Core.Query.Criteria;
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
                new CriteriaQuery(
                    Where.Or().Add(Where.Equal("name", "function 2")).Add(Where.Equal("name", "function 3")));
            var l = odb.GetObjects<VO.Login.Function>(aq, true, -1, -1);
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
            var aq = new CriteriaQuery(typeof (VO.Login.Function), Where.Not(Where.Equal("name", "function 2")));
            var l = odb.GetObjects<VO.Login.Function>(aq, true, -1, -1);
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
            var aq = new CriteriaQuery(typeof (VO.Login.Function),
                                       Where.Not(
                                           Where.Or().Add(Where.Equal("name", "function 2")).Add(Where.Equal("name",
                                                                                                             "function 3"))));
            var l = odb.GetObjects<VO.Login.Function>(aq, true, -1, -1);
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
                var aq = new CriteriaQuery(typeof (VO.Login.Function),
                                           Where.Not(
                                               Where.Or().Add(Where.Equal("name", "function 2")).Add(Where.Equal(
                                                   "name", "function 3"))));
                aq.OrderByDesc("name");
                // aq.orderByAsc("name");
                var l = odb.GetObjects<VO.Login.Function>(aq, true, -1, -1);
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
            IQuery query = new CriteriaQuery(typeof (MyDates),
                                             Where.And().Add(Where.Le("date1", d2)).Add(Where.Ge("date2", d2)).Add(
                                                 Where.Equal("i", 5)));
            var objects = odb.GetObjects<MyDates>(query);
            AssertEquals(1, objects.Count);
            odb.Close();
        }

        [Test]
        public virtual void TestEqual2()
        {
            var baseName = GetBaseName();
            SetUp(baseName);
            var odb = Open(baseName);
            var aq = new CriteriaQuery(typeof (VO.Login.Function), Where.Equal("name", "FuNcTiOn 1"));
            aq.OrderByDesc("name");
            var l = odb.GetObjects<VO.Login.Function>(aq, true, -1, -1);
            AssertEquals(0, l.Count);
            odb.Close();
        }

        [Test]
        public virtual void TestILike()
        {
            var baseName = GetBaseName();
            SetUp(baseName);
            var odb = Open(baseName);
            var aq = new CriteriaQuery(typeof (VO.Login.Function), Where.Ilike("name", "FUNc%"));
            aq.OrderByDesc("name");
            var l = odb.GetObjects<VO.Login.Function>(aq, true, -1, -1);
            AssertEquals(50, l.Count);
            odb.Close();
        }

        [Test]
        public virtual void TestIequal()
        {
            var baseName = GetBaseName();
            SetUp(baseName);
            var odb = Open(baseName);
            var aq = new CriteriaQuery(typeof (VO.Login.Function), Where.Iequal("name", "FuNcTiOn 1"));
            aq.OrderByDesc("name");
            var l = odb.GetObjects<VO.Login.Function>(aq, true, -1, -1);
            AssertEquals(1, l.Count);
            odb.Close();
        }

        [Test]
        public virtual void TestLike()
        {
            var baseName = GetBaseName();
            SetUp(baseName);
            var odb = Open(baseName);
            var aq = new CriteriaQuery(typeof (VO.Login.Function), Where.Like("name", "func%"));
            aq.OrderByDesc("name");
            var l = odb.GetObjects<VO.Login.Function>(aq, true, -1, -1);
            AssertEquals(50, l.Count);
            odb.Close();
        }

        [Test]
        public virtual void TestLike2()
        {
            var baseName = GetBaseName();
            SetUp(baseName);
            var odb = Open(baseName);
            var aq = new CriteriaQuery(typeof (VO.Login.Function), Where.Like("name", "FuNc%"));
            aq.OrderByDesc("name");
            var l = odb.GetObjects<VO.Login.Function>(aq, true, -1, -1);
            AssertEquals(0, l.Count);
            odb.Close();
        }
    }
}
