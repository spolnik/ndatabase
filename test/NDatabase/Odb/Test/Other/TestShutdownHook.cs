using NUnit.Framework;
using Test.Odb.Test.VO.Attribute;

namespace Test.Odb.Test.Other
{
    [TestFixture]
    public class TestShutdownHook : ODBTest
    {
        [Test]
        public virtual void Test1()
        {
            DeleteBase("hook.neodatis");
            var obase = Open("hook.neodatis");
            obase.GetObjects<TestClass>();
            obase.Store(new TestClass());
            obase.Close();
            DeleteBase("hook.neodatis");
        }
    }
}
