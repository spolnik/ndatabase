using NUnit.Framework;

namespace Test.NDatabase.Odb.Test.Enum
{
    [TestFixture]
    public class TestEnum : ODBTest
    {
        [Test]
        public virtual void TestEnum1()
        {
            var baseName = GetBaseName();
            var odb = Open(baseName);
            var e = new ClassWithEnum("enum1", ObjectType.Medium);
            odb.Store(e);
            odb.Close();
            odb = Open(baseName);
            var objects = odb.Query<ClassWithEnum>();
            odb.Close();
            AssertEquals(1, objects.Count);
        }

        [Test]
        public virtual void TestEnumUpdate()
        {
            var baseName = GetBaseName();
            var odb = Open(baseName);
            var e = new ClassWithEnum("enum1", ObjectType.Medium);
            odb.Store(e);
            odb.Close();
            odb = Open(baseName);
            var objects = odb.Query<ClassWithEnum>();
            var cwe = objects.GetFirst();
            cwe.SetObjectType(ObjectType.Small);
            odb.Store(cwe);
            odb.Close();
            odb = Open(baseName);
            objects = odb.Query<ClassWithEnum>();
            AssertEquals(1, objects.Count);

            cwe = objects.GetFirst();
            odb.Close();
            AssertEquals(ObjectType.Small, cwe.GetObjectType());
        }
    }
}
