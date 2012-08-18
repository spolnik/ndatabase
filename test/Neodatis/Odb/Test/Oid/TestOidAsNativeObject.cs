using NDatabase.Odb;
using NDatabase.Odb.Core.Oid;
using NUnit.Framework;

namespace Test.Odb.Test.Oid
{
	[TestFixture]
    public class TestOidAsNativeObject : ODBTest
	{
		
		[Test]
        public virtual void Test1()
		{
			ClassWithOid cwo = new ClassWithOid("test"
				, OIDFactory.BuildObjectOID(47));
			DeleteBase("native-oid");
			IOdb odb = Open("native-oid");
			odb.Store(cwo);
			odb.Close();
			odb = Open("native-oid");
			IObjects<ClassWithOid> objects = odb.GetObjects<ClassWithOid>();
			AssertEquals(1, objects.Count);
			ClassWithOid cwo2 = (ClassWithOid)objects
				.GetFirst();
			AssertEquals(47, cwo2.GetOid().ObjectId);
		}
	}
}
