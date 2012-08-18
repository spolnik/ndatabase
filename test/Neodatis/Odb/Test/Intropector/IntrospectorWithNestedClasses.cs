using NDatabase.Odb;
using NUnit.Framework;
using Test.Odb.Test;

namespace Intropector
{
	public class IntrospectorWithNestedClasses : ODBTest
	{
		
		[Test]
        public virtual void Test1()
		{
			DeleteBase("test-nested");
			IOdb odb = Open("test-nested");
			Intropector.Class1 c1 = new Intropector.Class1
				("name1", "name2", "name3");
			OID oid = odb.Store(c1);
			odb.Close();
			odb = Open("test-nested");
			Intropector.Class1 c11 = (Intropector.Class1)
				odb.GetObjectFromId(oid);
			AssertEquals(c1.GetName1(), c11.GetName1());
			AssertEquals(c1.GetClass2().GetClass3().GetName3(), c11.GetClass2().GetClass3().GetName3
				());
		}
	}
}
