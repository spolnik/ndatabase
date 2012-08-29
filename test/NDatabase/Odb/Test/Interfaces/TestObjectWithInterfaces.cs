using NUnit.Framework;
using Test.NDatabase.Odb.Test.VO.Interfaces;

namespace Test.NDatabase.Odb.Test.Interfaces
{
    [TestFixture]
    public class TestObjectWithInterfaces : ODBTest
    {
        [Test]
        public virtual void TestInsert()
        {
            DeleteBase("tinterfaces.neodatis");
            var odb = Open("tinterfaces.neodatis");
            var owi = new ObjectWithInterfaces("Ol√° chico");
            odb.Store(owi);
            odb.Commit();
            odb.Close();
        }

        [Test]
        public virtual void TestInsertAndSelect()
        {
            DeleteBase("tinterfaces.neodatis");
            var odb = Open("tinterfaces.neodatis");
            var owi = new ObjectWithInterfaces("Ol√° chico");
            odb.Store(owi);
            odb.Close();
            odb = Open("tinterfaces.neodatis");
            var os = odb.GetObjects<ObjectWithInterfaces>();
            AssertEquals(1, os.Count);
            odb.Close();
        }

        [Test]
        public virtual void TestStoreObjectByInterfaces()
        {
            var baseName = GetBaseName();
            var odb = Open(baseName);
            object o = new MyObject("f");
            odb.Store(o);
            odb.Close();
            odb = Open(baseName);
            var os = odb.GetObjects<MyObject>();
            AssertEquals(1, os.Count);
            odb.Close();
            // deleteBase(baseName);
            Println(baseName);
        }
    }
}
