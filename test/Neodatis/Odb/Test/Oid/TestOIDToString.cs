using NDatabase.Odb.Core.Oid;
using NDatabase.Odb.Impl.Core.Oid;
using NDatabase.Tool;
using NDatabase.Tool.Wrappers.IO;
using NUnit.Framework;

namespace Test.Odb.Test.Oid
{
    /// <author>olivier</author>
    [TestFixture]
    public class TestOIDToString : ODBTest
    {
        [Test]
        public virtual void Test1()
        {
            var baseName = GetBaseName();
            var odb = Open(baseName);
            var oid = odb.Store(new VO.Login.Function("My Function"));
            odb.Close();
            OdbFile.DeleteFile(baseName);
            var soid = oid.OidToString();
            var oid2 = OIDFactory.OidFromString(soid);
            AssertEquals(oid, oid2);
        }

        [Test]
        public virtual void Test2()
        {
            var oid = new OdbClassOID(10002);
            var soid = oid.OidToString();
            var oid2 = OIDFactory.OidFromString(soid);
            AssertEquals(oid, oid2);
        }

        [Test]
        public virtual void Test3()
        {
            var baseName = GetBaseName();
            var odb = Open(baseName);
            var oid = odb.Store(new VO.Login.Function("My Function"));
            oid = odb.Ext().ConvertToExternalOID(oid);
            odb.Close();
            OdbFile.DeleteFile(baseName);
            var soid = oid.OidToString();
            Println(soid);
            var oid2 = OIDFactory.OidFromString(soid);
            AssertEquals(oid, oid2);
        }

        [Test]
        public virtual void Test4()
        {
            var baseName = GetBaseName();
            var odb = Open(baseName);
            var oid = new ExternalClassOID(new OdbClassOID(19), odb.Ext().GetDatabaseId());
            var soid = oid.OidToString();
            var oid2 = OIDFactory.OidFromString(soid);
            AssertEquals(oid, oid2);
        }
    }
}
