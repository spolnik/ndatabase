using NDatabase.Odb.Core.Query;
using NDatabase.Odb.Core.Query.NQ;
using NUnit.Framework;
using Test.Odb.Test.VO.Human;

namespace Test.Odb.Test.Query.NQ
{
    [TestFixture]
    public class TestPolyMorphic : ODBTest
    {
        internal sealed class _SimpleNativeQuery_31 : SimpleNativeQuery
        {
            public bool Match(Animal animal)
            {
                return true;
            }
        }

        internal sealed class _SimpleNativeQuery_60 : SimpleNativeQuery
        {
            public bool Match(Human human)
            {
                return true;
            }
        }

        internal sealed class _SimpleNativeQuery_91 : SimpleNativeQuery
        {
            public bool Match(Animal @object)
            {
                return @object.GetName().StartsWith("my ");
            }
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test1()
        {
            DeleteBase("multi");
            var odb = Open("multi");
            odb.Store(new Animal("dog", "M", "my dog"));
            odb.Store(new Animal("cat", "F", "my cat"));
            odb.Store(new Man("Joe"));
            odb.Store(new Woman("Karine"));
            odb.Close();
            odb = Open("multi");
            IQuery q = new _SimpleNativeQuery_31();

            var os = odb.GetObjects<Animal>(q);
            Println(os);
            odb.Close();
            AssertEquals(4, os.Count);
            DeleteBase("multi");
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test2()
        {
            DeleteBase("multi");
            var odb = Open("multi");
            odb.Store(new Animal("dog", "M", "my dog"));
            odb.Store(new Animal("cat", "F", "my cat"));
            odb.Store(new Man("Joe"));
            odb.Store(new Woman("Karine"));
            odb.Close();
            odb = Open("multi");
            IQuery q = new _SimpleNativeQuery_60();

            var os = odb.GetObjects<Human>(q);
            Println(os);
            odb.Close();
            AssertEquals(2, os.Count);
            DeleteBase("multi");
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test8()
        {
            var size = 3000;
            DeleteBase("multi");
            var odb = Open("multi");
            for (var i = 0; i < size; i++)
            {
                odb.Store(new Animal("dog", "M", "my dog" + i));
                odb.Store(new Animal("cat", "F", "my cat" + i));
                odb.Store(new Man("Joe" + i));
                odb.Store(new Woman("my Karine" + i));
            }
            odb.Close();
            odb = Open("multi");
            IQuery q = new _SimpleNativeQuery_91();

            var objects = odb.GetObjects<Animal>(q);
            odb.Close();
            DeleteBase("multi");
            AssertEquals(size * 3, objects.Count);
        }
    }
}
