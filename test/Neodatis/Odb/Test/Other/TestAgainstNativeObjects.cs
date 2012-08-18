using NDatabase.Odb;
using NUnit.Framework;

namespace Test.Odb.Test.Other
{
	[TestFixture]
    public class TestAgainstNativeObjects : ODBTest
	{
		
		[Test]
        public virtual void Test1()
		{
			DeleteBase("native.neodatis");
			IOdb @base = Open("native.neodatis");
			try
			{
				@base.Store("olivier");
			}
			catch (OdbRuntimeException)
			{
				@base.Close();
				DeleteBase("native.neodatis");
				return;
			}
			@base.Close();
			Fail("Allow native object direct persistence");
			DeleteBase("native.neodatis");
		}

		
		[Test]
        public virtual void Test2()
		{
			DeleteBase("native.neodatis");
			IOdb @base = Open("native.neodatis");
			try
			{
				string[] array = new string[] { "olivier", "joao", "peter" };
				@base.Store(array);
			}
			catch (OdbRuntimeException)
			{
				@base.Close();
				DeleteBase("native.neodatis");
				return;
			}
			@base.Close();
			Fail("Allow native object direct persistence");
			DeleteBase("native.neodatis");
		}
	}
}
