using NDatabase.Odb.Core.Query;
using NDatabase.Odb.Core.Query.Criteria;
using NDatabase.Odb.Impl.Core.Query.Criteria;
using NUnit.Framework;

namespace Test.Odb.Test.Query.Criteria
{
    [TestFixture]
    public class TestRelation : ODBTest
    {
        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void TestNullRelation()
        {
            DeleteBase("null-rel.neodatis");
            var odb = Open("null-rel.neodatis");
            odb.Store(new Class2());
            odb.Close();
            odb = Open("null-rel.neodatis");
            IQuery q = new CriteriaQuery(typeof (Class2), Where.IsNull("class1.name"));
            var os = odb.GetObjects<Class2>(q);
            odb.Close();
            AssertEquals(1, os.Count);
            var c2 = os.GetFirst();
            AssertEquals(null, c2.GetClass1());
        }
    }
}
