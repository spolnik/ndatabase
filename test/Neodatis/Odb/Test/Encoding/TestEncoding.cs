using NDatabase.Odb;
using NUnit.Framework;

namespace Test.Odb.Test.Encoding
{
    [TestFixture]
    public class TestEncoding : ODBTest
    {
        [Test]
        public virtual void Test1()
        {
            string baseName = GetBaseName();
            Println(baseName);
            string currentEncoding = OdbConfiguration.GetDatabaseCharacterEncoding
                ();
            OdbConfiguration.SetDatabaseCharacterEncoding("iso-8859-5");
            IOdb odb = null;
            try
            {
                odb = Open(baseName);
                string nameWithCyrillicCharacters = "\u0410 \u0430 \u0431 \u0448 \u0429";
                var f = new VO.Login.Function(nameWithCyrillicCharacters
                    );
                OID oid = odb.Store(f);
                odb.Close();
                Println(f);
                odb = Open(baseName);
                var f2 = (VO.Login.Function) odb
                                        .GetObjectFromId(oid);
                odb.Close();
                AssertEquals(nameWithCyrillicCharacters, f2.GetName());
                AssertEquals('\u0410', f2.GetName()[0]);
                AssertEquals('\u0430', f2.GetName()[2]);
                AssertEquals('\u0431', f2.GetName()[4]);
                AssertEquals('\u0448', f2.GetName()[6]);
                AssertEquals('\u0429', f2.GetName()[8]);
            }
            finally
            {
                OdbConfiguration.SetDatabaseCharacterEncoding(currentEncoding);
            }
        }
    }
}