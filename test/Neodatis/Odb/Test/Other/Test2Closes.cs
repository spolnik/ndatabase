using NDatabase.Odb;
using NUnit.Framework;
using Test.Odb.Test.VO.Attribute;

namespace Test.Odb.Test.Other
{
	[TestFixture]
    public class Test2Closes : ODBTest
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
			bool exception = false;
			try
			{
				obase.Close();
			}
			catch (System.Exception e)
			{
				exception = true;
				AssertTrue(e.Message.IndexOf("ODB session has already been closed") != -1);
			}
			AssertTrue(exception);
		}
	}
}
