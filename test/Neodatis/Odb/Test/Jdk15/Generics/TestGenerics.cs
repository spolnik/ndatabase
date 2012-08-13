using NUnit.Framework;
using Test.Odb.Test;
using Test.Odb.Test.VO.Login;

namespace Generics
{
    /// <author>olivier</author>
    [TestFixture]
    public class TestGenerics : ODBTest
    {
        [Test]
        public virtual void TestGetObects()
        {
            var baseName = GetBaseName();
            var odb = Open(baseName);
            odb.Store(new Function("Test"));
            var functions = odb.GetObjects<Function>();
            var f = functions.GetFirst();
            odb.Close();
            AssertEquals(1, functions.Count);
        }

        [Test]
        public virtual void TestGetObects2()
        {
            var baseName = GetBaseName();
            var odb = Open(baseName);
            odb.Store(new Function("Test"));
            var functions = odb.GetObjects<Function>();
            var f = functions.Next();
            odb.Close();
            AssertEquals(1, functions.Count);
        }

        [Test]
        public virtual void TestGetObects3()
        {
            var baseName = GetBaseName();
            var odb = Open(baseName);
            odb.Store(new Function("Test"));
            var functions = odb.GetObjects<Function>();
            var iterator = functions.GetEnumerator();
            var f = iterator.Current;
            odb.Close();
            AssertEquals(1, functions.Count);
        }
    }
}
