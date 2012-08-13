using NUnit.Framework;
using Test.Neodatis.Odb.Test.Enum;
using Test.Odb.Test;

namespace Enum
{
    [TestFixture]
    public class TestEnum : ODBTest
    {
        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void TestEnum1()
        {
            var baseName = GetBaseName();
            var odb = Open(baseName);
            var e = new ClassWithEnum("enum1", ObjectType.Medium);
            odb.Store(e);
            odb.Close();
            odb = Open(baseName);
            var objects = odb.GetObjects<ClassWithEnum>();
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
            var objects = odb.GetObjects<ClassWithEnum>();
            var cwe = objects.GetFirst();
            cwe.SetObjectType(ObjectType.Small);
            odb.Store(cwe);
            odb.Close();
            odb = Open(baseName);
            objects = odb.GetObjects<ClassWithEnum>();
            AssertEquals(1, objects.Count);

            cwe = objects.GetFirst();
            odb.Close();
            AssertEquals(ObjectType.Small, cwe.GetObjectType());
        }
    }
}
