using System;
using NUnit.Framework;

namespace Test.NDatabase.Odb.Test.Other
{
    [TestFixture]
    public class TestJavaClassPersistence : ODBTest
    {
        public static readonly string DbName = "class.neodatis";

        [Test]
        public virtual void Test1()
        {
            DeleteBase(DbName);
            var odb = Open(DbName);
            odb.Store(new Exception("test"));
            odb.Close();
            odb = Open(DbName);
            var l = odb.Query<Exception>();
            odb.Close();
            DeleteBase(DbName);
        }
    }
}
