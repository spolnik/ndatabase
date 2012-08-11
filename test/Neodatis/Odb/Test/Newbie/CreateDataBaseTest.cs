using NDatabase.Tool;
using NUnit.Framework;

namespace Test.Odb.Test.Newbie
{
	/// <summary>It is just a simple test to help the newbies</summary>
	/// <author>mayworm at <xmpp://mayworm@gmail.com></author>
	public class CreateDataBaseTest : ODBTest
	{
	    private const string NewbieOdb = "newbie.neodatis";

	    /// <summary>Test if a new database could be created</summary>
		[Test]
        public virtual void TestCreateDataBase()
		{
		    DeleteBase(NewbieOdb);
		    using (Open(NewbieOdb)) {}

		    var existFile = IOUtil.ExistFile(NewbieOdb);
		    AssertTrue("ODB data file couldn't created", existFile);
		    DeleteBase(NewbieOdb);
		}
	}
}
