using NDatabase2.Odb.Core.Query;
using NDatabase2.Odb.Core.Query.NQ;
using NUnit.Framework;
using Test.NDatabase.Odb.Test.VO.Human;

namespace Test.NDatabase.Odb.Test.Query.NQ
{
    [TestFixture]
    public class TestPolyMorphic : ODBTest
    {
        internal sealed class SimpleNativeQuery31 : SimpleNativeQuery<Animal>
        {
            public override bool Match(Animal animal)
            {
                return true;
            }
        }

        internal sealed class SimpleNativeQuery60 : SimpleNativeQuery<Human>
        {
            public override bool Match(Human human)
            {
                return true;
            }
        }

        internal sealed class SimpleNativeQuery91 : SimpleNativeQuery<Animal>
        {
            public override bool Match(Animal @object)
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
            IQuery q = new SimpleNativeQuery31();

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
            IQuery q = new SimpleNativeQuery60();

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
            IQuery q = new SimpleNativeQuery91();

            var objects = odb.GetObjects<Animal>(q);
            odb.Close();
            DeleteBase("multi");
            AssertEquals(size * 3, objects.Count);
        }
    }
}
