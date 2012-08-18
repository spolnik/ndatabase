using NDatabase.Odb;
using NDatabase.Tool;
using NDatabase.Tool.Wrappers.IO;
using NUnit.Framework;

namespace Test.Odb.Test.Cache
{
	[TestFixture]
    public class TestHashCode : ODBTest
	{
		/// <summary>a problem reported by glsender - 1875544</summary>
		
		[Test]
        public virtual void Test1()
		{
			string baseName = GetBaseName();
			IOdb odb = Open(baseName);
			MyObjectWithMyHashCode my = null;
			// creates 1000 objects
			for (int i = 0; i < 1000; i++)
			{
				my = new MyObjectWithMyHashCode(System.Convert.ToInt64(1000
					));
				odb.Store(my);
			}
			odb.Close();
			odb = Open(baseName);
			IObjects<MyObjectWithMyHashCode> objects = odb.GetObjects<MyObjectWithMyHashCode>();
			AssertEquals(1000, objects.Count);
			while (objects.HasNext())
			{
				my = (MyObjectWithMyHashCode)objects.Next();
				odb.Delete(my);
			}
			odb.Close();
			odb = Open(baseName);
            objects = odb.GetObjects<MyObjectWithMyHashCode>();
			odb.Close();
			OdbFile.DeleteFile(baseName);
			AssertEquals(0, objects.Count);
		}

		/// <summary>a problem reported by glsender</summary>
		
		[Test]
        public virtual void Test2()
		{
			string baseName = GetBaseName();
			IOdb odb = Open(baseName);
			MyObjectWithMyHashCode2 my = null;
			// creates 1000 objects
			for (int i = 0; i < 1000; i++)
			{
				my = new MyObjectWithMyHashCode2(System.Convert.ToInt64(1000
					));
				odb.Store(my);
			}
			odb.Close();
			odb = Open(baseName);
            IObjects<MyObjectWithMyHashCode2> objects = odb.GetObjects<MyObjectWithMyHashCode2>();
			AssertEquals(1000, objects.Count);
			while (objects.HasNext())
			{
				my = (MyObjectWithMyHashCode2)objects.Next();
				odb.Delete(my);
			}
			odb.Close();
			odb = Open(baseName);
			objects = odb.GetObjects<MyObjectWithMyHashCode2>();
			odb.Close();
			OdbFile.DeleteFile(baseName);
			AssertEquals(0, objects.Count);
		}
	}
}
