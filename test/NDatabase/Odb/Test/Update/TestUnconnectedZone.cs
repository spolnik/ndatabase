using NDatabase.Odb.Core.Layers.Layer3.Engine;
using NDatabase.Tool.Wrappers;
using NUnit.Framework;

namespace Test.NDatabase.Odb.Test.Update
{
    [TestFixture]
    public class TestUnconnectedZone : ODBTest
    {
        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test1()
        {
            DeleteBase("unconnected");
            var odb = Open("unconnected");
            var oid = odb.Store(new VO.Login.Function("f1"));
            odb.Close();
            Println("Oid=" + oid);
            odb = Open("unconnected");
            var f2 = (VO.Login.Function) odb.GetObjectFromId(oid);
            f2.SetName("New Function");
            odb.Store(f2);
            var storageEngine = Dummy.GetEngine(odb);
            // retrieve the class info to check connected and unconnected zone
            var fullClassName = OdbClassUtil.GetFullName(typeof (VO.Login.Function));
            var classInfo = storageEngine.GetSession(true).GetMetaModel().GetClassInfo(fullClassName, true);
            odb.Close();
            AssertEquals(1, classInfo.GetCommitedZoneInfo().GetNbObjects());
            AssertNotNull(classInfo.GetCommitedZoneInfo().First);
            AssertNotNull(classInfo.GetCommitedZoneInfo().Last);
            AssertEquals(0, classInfo.GetUncommittedZoneInfo().GetNbObjects());
            AssertNull(classInfo.GetUncommittedZoneInfo().First);
            AssertNull(classInfo.GetUncommittedZoneInfo().Last);
            odb = Open("unconnected");
            AssertEquals(1, odb.GetObjects<VO.Login.Function>().Count);
            odb.Close();
        }
    }
}
