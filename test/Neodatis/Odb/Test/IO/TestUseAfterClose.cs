using System;
using NDatabase.Odb;
using NUnit.Framework;
using Test.Odb.Test;
using Test.Odb.Test.VO.Login;

namespace IO
{
	[TestFixture]
    public class TestUseAfterClose : ODBTest
	{
		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void Test()
		{
			IOdb odb = Open("t111.neodatis");
			odb.Close();
			try
			{
				odb.Store(new Function("login"));
			}
			catch (System.Exception e)
			{
				AssertTrue(e.Message.IndexOf("has already been closed") != -1);
			}
			DeleteBase("t111.neodatis");
		}

		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void TestTwoCloses()
		{
			IOdb odb = Open("t111.neodatis");
			odb.Close();
			try
			{
				odb.Close();
                Assert.Fail();
			}
			catch (System.Exception e)
			{
			    Console.WriteLine(e.Message);
				Assert.Pass();
			}
			DeleteBase("t111.neodatis");
		}

		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void TestReOpenWithoutClose()
		{
			IOdb odb = Open("t111.neodatis");
			try
			{
				odb = Open("t111.neodatis");
                Assert.Fail();
			}
			catch (System.Exception e)
			{
                Console.WriteLine(e.Message);
			    Assert.Pass();
			}
			odb.Close();
			DeleteBase("t111.neodatis");
		}
	}
}
