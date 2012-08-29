using System.Text;
using NUnit.Framework;

namespace Test.NDatabase.Odb.Test.Buffer
{
    [TestFixture]
    public class TestBigData : ODBTest
    {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp()
        {
            DeleteBase("big-data.neodatis");
        }

        [TearDown]
        public override void TearDown()
        {
            DeleteBase("big-data.neodatis");
        }

        #endregion

        [Test]
        public void Test1()
        {
            var odb = Open("big-data.neodatis");
            var buffer = new StringBuilder();
            for (var i = 0; i < 30000; i++)
                buffer.Append('a');
            var function = new VO.Login.Function(buffer.ToString());
            odb.Store(function);
            odb.Close();
            odb = Open("big-data.neodatis");
            var f2 = odb.GetObjects<VO.Login.Function>().GetFirst();
            AssertEquals(30000, f2.GetName().Length);
            odb.Close();
            odb = Open("big-data.neodatis");
            f2 = odb.GetObjects<VO.Login.Function>().GetFirst();
            f2.SetName(f2.GetName() + "ola chico");
            var newSize = f2.GetName().Length;
            odb.Store(f2);
            odb.Close();
            odb = Open("big-data.neodatis");
            f2 = odb.GetObjects<VO.Login.Function>().GetFirst();
            AssertEquals(newSize, f2.GetName().Length);
            AssertEquals(buffer + "ola chico", f2.GetName());
            odb.Close();
        }
    }
}
