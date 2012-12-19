using System;
using NDatabase2.Odb.Core.Query;
using NUnit.Framework;
using Test.NDatabase.Odb.Test.VO.Human;

namespace Test.NDatabase.Odb.Test.Query.Criteria
{
    [TestFixture]
    public class TestPolyMorphic : ODBTest
    {
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
            IQuery q = odb.Query<object>();

            var os = q.Execute<object>();
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
            IQuery q = odb.Query<Human>();

            var os = q.Execute<Human>();
            Println(os);
            odb.Close();
            AssertEquals(2, os.Count);
            DeleteBase("multi");
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test3()
        {
            DeleteBase("multi");
            var odb = Open("multi");
            odb.Store(new Animal("dog", "M", "my dog"));
            odb.Store(new Animal("cat", "F", "my cat"));
            odb.Store(new Man("Joe"));
            odb.Store(new Woman("Karine"));
            odb.Close();
            odb = Open("multi");
            var q = odb.ValuesQuery<object>().Field("specie");

            var os = odb.GetValues(q);
            Println(os);
            odb.Close();
            AssertEquals(4, os.Count);
            DeleteBase("multi");
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test4()
        {
            DeleteBase("multi");
            var odb = Open("multi");
            odb.Store(new Animal("dog", "M", "my dog"));
            odb.Store(new Animal("cat", "F", "my cat"));
            odb.Store(new Man("Joe"));
            odb.Store(new Woman("Karine"));
            odb.Close();
            odb = Open("multi");
            var q = odb.ValuesQuery<Human>().Field("specie");

            var os = odb.GetValues(q);
            Println(os);
            odb.Close();
            AssertEquals(2, os.Count);
            DeleteBase("multi");
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test5()
        {
            DeleteBase("multi");
            var odb = Open("multi");
            odb.Store(new Animal("dog", "M", "my dog"));
            odb.Store(new Animal("cat", "F", "my cat"));
            odb.Store(new Man("Joe"));
            odb.Store(new Woman("Karine"));
            odb.Close();
            odb = Open("multi");
            var q = odb.ValuesQuery<Man>().Field("specie");

            var os = odb.GetValues(q);
            Println(os);
            odb.Close();
            AssertEquals(1, os.Count);
            DeleteBase("multi");
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test6()
        {
            DeleteBase("multi");
            var odb = Open("multi");
            odb.Store(new Animal("dog", "M", "my dog"));
            odb.Store(new Animal("cat", "F", "my cat"));
            odb.Store(new Man("Joe"));
            odb.Store(new Woman("Karine"));
            odb.Close();
            odb = Open("multi");
            var q = odb.Query<object>();

            Decimal nb = q.Count();
            Println(nb);
            odb.Close();
            AssertEquals(new Decimal(4), nb);
            DeleteBase("multi");
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test7()
        {
            var size = 3000;
            var baseName = GetBaseName();
            var odb = Open(baseName);
            for (var i = 0; i < size; i++)
            {
                odb.Store(new Animal("dog", "M", "my dog"));
                odb.Store(new Animal("cat", "F", "my cat"));
                odb.Store(new Man("Joe" + i));
                odb.Store(new Woman("Karine" + i));
            }
            odb.Close();
            odb = Open(baseName);
            var q = odb.Query<object>();

            Decimal nb = q.Count();
            Println(nb);
            odb.Close();
            AssertEquals(new Decimal(4 * size), nb);
            DeleteBase(baseName);
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test8()
        {
            var size = 3000;
            var baseName = GetBaseName();
            var odb = Open(baseName);
            for (var i = 0; i < size; i++)
            {
                odb.Store(new Animal("dog" + i, "M", "my dog" + i));
                odb.Store(new Animal("cat" + i, "F", "my cat" + i));
                odb.Store(new Man("Joe" + i));
                odb.Store(new Woman("Karine" + i));
            }
            odb.Close();
            odb = Open(baseName);
            var q = odb.Query<object>();
            q.Descend("specie").Equal("man");

            Decimal nb = q.Count();
            Println(nb);
            odb.Close();
            AssertEquals(new Decimal(1 * size), nb);
            DeleteBase(baseName);
        }
    }
}
