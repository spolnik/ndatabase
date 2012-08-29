using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Impl.Core.Layers.Layer3.Engine;
using NUnit.Framework;

namespace Test.NDatabase.Odb.Test.Other
{
    /// <author>olivier</author>
    [TestFixture]
    public class TestGetDatabaseName : ODBTest
    {
        [Test]
        public virtual void Test1()
        {
            var baseName = "name.neodatis";
            DeleteBase(baseName);

            IStorageEngine engine;
            using (var odb = Open(baseName))
            {
                engine = Dummy.GetEngine(odb);
            }

            var s = engine.GetBaseIdentification().Id;

            AssertEquals(baseName, s);
        }

        [Test]
        public virtual void Test2()
        {
            var baseName = "name.neodatis";
            DeleteBase(baseName);

            string s;
            using (var odb = Open(baseName))
            {
                s = odb.GetName();
            }
            AssertEquals(baseName, s);
        }
    }
}
