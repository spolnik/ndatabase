using NDatabase.Odb.Core.Lookup;
using NUnit.Framework;

namespace Test.NDatabase.Odb.Test.Lookup
{
    /// <author>olivier</author>
    [TestFixture]
    public class TestLookup : ODBTest
    {
        [Test]
        public virtual void Test1()
        {
            ILookup lookup = new LookupImpl();
            lookup.Set("oid1", "Ol√° chico");
            var s = (string) lookup.Get("oid1");
            AssertEquals("Ol√° chico", s);
        }

        [Test]
        public virtual void Test2()
        {
            var lookup = LookupFactory.Get("test");
            lookup.Set("oid1", "Ol√° chico");
            lookup = LookupFactory.Get("test");
            var s = (string) lookup.Get("oid1");
            AssertEquals("Ol√° chico", s);
        }
    }
}
