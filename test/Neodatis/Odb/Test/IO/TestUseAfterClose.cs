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
            DeleteBase("t111A.neodatis");
			IOdb odb = Open("t111A.neodatis");
			odb.Close();
			try
			{
				odb.Store(new Function("login"));
			}
			catch (System.Exception e)
			{
				AssertTrue(e.Message.IndexOf("has already been closed") != -1);
			}
		}

		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void TestTwoCloses()
        {
            DeleteBase("t111B.neodatis");
			IOdb odb = Open("t111B.neodatis");
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
		}

		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void TestReOpenWithoutClose()
		{
            DeleteBase("t111C.neodatis");
			IOdb odb = Open("t111C.neodatis");
			try
			{
				odb = Open("t111C.neodatis");
                Assert.Fail();
			}
			catch (System.Exception e)
			{
                Console.WriteLine(e.Message);
			    Assert.Pass();
			}
			odb.Close();
		}
	}
}
