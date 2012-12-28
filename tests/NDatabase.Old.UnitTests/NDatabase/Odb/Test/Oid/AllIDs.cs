using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Layers.Layer3.Engine;
using NUnit.Framework;

namespace Test.NDatabase.Odb.Test.Oid
{
    public class AllIDs : ODBTest
    {
        public static string FileName = "ids.neodatis";

        [Test]
        public virtual void Test1()
        {
            DeleteBase(FileName);

            IFileIdentification parameter = new FileIdentification(FileName);

            var engine = (IStorageEngine) new StorageEngine(parameter);
            var function1 = new VO.Login.Function("login");
            engine.Store(function1);

            var function2 = new VO.Login.Function("login2");
            engine.Store(function2);

            engine.Commit();
            engine.Close();

            engine = new StorageEngine(parameter);
            var l = engine.GetAllObjectIds();
            AssertEquals(2, l.Count);
            engine.Close();
            DeleteBase(FileName);
        }
    }
}
