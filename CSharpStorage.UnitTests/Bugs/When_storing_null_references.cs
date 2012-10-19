using NDatabase2.Odb;
using NUnit.Framework;

namespace NDatabase.UnitTests.Bugs
{
    public class When_storing_null_references
    {
        class A
        {
            public B Value { get; set; }
        }

        class B
        {
            public string Value { get; set; } 
        }


        [Test] 
        public void It_should_store_object_with_null_as_the_object_value()
        {
            NDb.Delete("nullreftest.ndb");

            using (var odb = NDb.Open("nullreftest.ndb"))
            {
                var a = new A {Value = null};

                odb.Store(a);
            }

            using (var odb = NDb.OpenLast())
            {
                var a = odb.Query<A>().GetFirst();

                a.Value = new B();

                odb.Store(a);
            }

            using (var odb = NDb.OpenLast())
            {
                var a = odb.Query<A>().GetFirst();

                a.Value.Value = "Value";

                odb.Store(a);
            }

            using (var odb = NDb.OpenLast())
            {
                var a = odb.Query<A>().GetFirst();

                Assert.That(a.Value.Value, Is.EqualTo("Value"));
            }
        }
    }
}