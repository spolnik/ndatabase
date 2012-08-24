using NUnit.Framework;
using Test.Odb.Test.VO.Arraycollectionmap;

namespace Test.Odb.Test.Session
{
    [TestFixture]
    public class TestSession : ODBTest
    {
        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test1()
        {
            DeleteBase("session.neodatis");
            var odb = Open("session.neodatis");
            odb.Close();
            var odb2 = Open("session.neodatis");
            var l = odb2.GetObjects<PlayerWithList>(true);
            AssertEquals(0, l.Count);
            odb2.Close();
            DeleteBase("session.neodatis");
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test2()
        {
            DeleteBase("session.neodatis");
            var odb = Open("session.neodatis");
            var f = new VO.Login.Function("f1");
            odb.Store(f);
            odb.Commit();
            f.SetName("f1 -1");
            odb.Store(f);
            odb.Close();
            odb = Open("session.neodatis");
            var os = odb.GetObjects<VO.Login.Function>();
            AssertEquals(1, os.Count);
            var f2 = os.GetFirst();
            odb.Close();
            DeleteBase("session.neodatis");
            AssertEquals("f1 -1", f2.GetName());
        }
    }
}
