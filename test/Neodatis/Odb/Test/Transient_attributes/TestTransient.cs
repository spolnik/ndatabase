using NDatabase.Odb;
using NUnit.Framework;

namespace Test.Odb.Test.Transient_attributes
{
	[TestFixture]
    public class TestTransient : ODBTest
	{
		[Test]
        public virtual void Test1()
		{
			string baseName = GetBaseName();
			IOdb odb = Open(baseName);
			VoWithTransientAttribute vo = new VoWithTransientAttribute
				("vo1");
			odb.Store(vo);
			odb.Close();
			odb = Open(baseName);
			IObjects<VoWithTransientAttribute> vos = odb.GetObjects<VoWithTransientAttribute>();
			odb.Close();
			Println(vos.GetFirst().GetName());
			AssertEquals(1, vos.Count);
			AssertEquals("vo1", vos.GetFirst().GetName());
		}
	}
}
