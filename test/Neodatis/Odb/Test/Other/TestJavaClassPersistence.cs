using System;
using NDatabase.Odb;
using NUnit.Framework;

namespace Test.Odb.Test.Other
{
	[TestFixture]
    public class TestJavaClassPersistence : ODBTest
	{
		public static readonly string DbName = "class.neodatis";

		
		[Test]
        public virtual void Test1()
		{
			DeleteBase(DbName);
			IOdb odb = Open(DbName);
			odb.Store(new System.Exception("test"));
			odb.Close();
			odb = Open(DbName);
			IObjects<Exception> l = odb.GetObjects<Exception>();
			odb.Close();
			DeleteBase(DbName);
		}
	}
}
