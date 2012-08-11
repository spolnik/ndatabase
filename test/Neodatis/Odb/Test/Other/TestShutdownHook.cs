using NDatabase.Odb;
using NUnit.Framework;
using Test.Odb.Test.VO.Attribute;

namespace Test.Odb.Test.Other
{
	[TestFixture]
    public class TestShutdownHook : ODBTest
	{
		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void Test1()
		{
			DeleteBase("hook.neodatis");
			IOdb obase = Open("hook.neodatis");
			obase.GetObjects<TestClass>();
			obase.Store(new TestClass());
			obase.Close();
			DeleteBase("hook.neodatis");
		}
	}
}
