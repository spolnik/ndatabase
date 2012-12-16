using System;
using NDatabase2.Odb.Core.Query;
using NDatabase2.Odb.Core.Query.Criteria;
using NDatabase2.Odb.Core.Query.Values;
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
            IQuery q = new CriteriaQuery<object>();

            var os = odb.Query<object>(q);
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
            IQuery q = new CriteriaQuery<Human>();

            var os = odb.Query<Human>(q);
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
            var q = new ValuesCriteriaQuery<object>().Field("specie");

            var os = odb.GetValues<object>(q);
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
            var q = new ValuesCriteriaQuery<Human>().Field("specie");

            var os = odb.GetValues<Human>(q);
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
            var q = new ValuesCriteriaQuery<Man>().Field("specie");

            var os = odb.GetValues<Man>(q);
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
            var q = new CriteriaQuery<object>();

            Decimal nb = odb.Count(q);
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
            var q = new CriteriaQuery<object>();

            Decimal nb = odb.Count(q);
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
            var q = new CriteriaQuery<object>();
            q.Equal("specie", "man");

            Decimal nb = odb.Count(q);
            Println(nb);
            odb.Close();
            AssertEquals(new Decimal(1 * size), nb);
            DeleteBase(baseName);
        }
    }
}
