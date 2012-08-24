using System;
using NUnit.Framework;

namespace Test.Odb.Test.IO
{
    [TestFixture]
    public class TestUseAfterClose : ODBTest
    {
        [Test]
        public virtual void Test()
        {
            DeleteBase("t111A.neodatis");
            var odb = Open("t111A.neodatis");
            odb.Close();
            try
            {
                odb.Store(new VO.Login.Function("login"));
            }
            catch (Exception e)
            {
                AssertTrue(e.Message.IndexOf("has already been closed") != -1);
            }
        }

        [Test]
        public virtual void TestReOpenWithoutClose()
        {
            DeleteBase("t111C.neodatis");
            var odb = Open("t111C.neodatis");
            try
            {
                odb = Open("t111C.neodatis");
                Assert.Fail();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Assert.Pass();
            }
            odb.Close();
        }

        [Test]
        public virtual void TestTwoCloses()
        {
            DeleteBase("t111B.neodatis");
            var odb = Open("t111B.neodatis");
            odb.Close();
            try
            {
                odb.Close();
                Assert.Fail();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Assert.Pass();
            }
        }
    }
}
