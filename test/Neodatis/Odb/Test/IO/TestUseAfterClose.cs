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
			}
			catch (System.Exception e)
			{
				AssertTrue(e.Message.IndexOf("has already been closed") != -1);
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
			}
			catch (System.Exception e)
			{
			    AssertTrue(
			        e.Message.IndexOf(
			            "file is locked by the current Virtual machine - check if the database has not been opened in the current VM!",
			            System.StringComparison.Ordinal) != -1);
			}
			odb.Close();
			DeleteBase("t111.neodatis");
		}
	}
}
