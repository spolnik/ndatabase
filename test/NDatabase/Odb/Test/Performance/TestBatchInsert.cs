using System;
using NDatabase.Odb.Core.Query.Criteria;
using NDatabase.Odb.Impl.Tool;
using NUnit.Framework;

namespace Test.NDatabase.Odb.Test.Performance
{
    [TestFixture]
    public class TestBatchInsert : ODBTest
    {
//        public static int TestSize = 2000000;

        #region Setup/Teardown

        [SetUp]
        public virtual void T1est1()
        {
            //OdbConfiguration.setUseCache(false);
            DeleteBase(OdbFileName);
            //OdbConfiguration.set
            var odb = Open(OdbFileName);
            for (var i = 0; i < TestSize; i++)
            {
                odb.Store(GetSimpleObjectInstance(i));
                if (i % 10000 == 0)
                {
                    MemoryMonitor.DisplayCurrentMemory(i + " objects", false);
                    odb.Close();
                    odb = Open(OdbFileName);
                }
            }
            odb.Close();
        }

        #endregion

        public static int TestSize = 200;

        public static readonly string OdbFileName = "perf-batch.neodatis";

        private SimpleObject GetSimpleObjectInstance(int i)
        {
            var so = new SimpleObject();
            so.SetDate(new DateTime());
            so.SetDuration(i);
            so.SetName("Bonjour, comment allez vous?" + i);
            return so;
        }

        [Test]
        public virtual void TestSelect()
        {
            var odb = Open(OdbFileName);
            var functions =
                odb.GetObjects<SimpleObject>(new CriteriaQuery<SimpleObject>(Where.Equal("name", "Bonjour, comment allez vous?100")));
            odb.Close();
            AssertEquals(1, functions.Count);
        }
    }
}
