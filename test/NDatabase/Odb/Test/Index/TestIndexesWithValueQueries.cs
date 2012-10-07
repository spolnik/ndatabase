using NDatabase.Odb;
using NDatabase.Odb.Core.Query.Criteria;
using NDatabase.Odb.Core.Query.Values;
using NUnit.Framework;

namespace Test.NDatabase.Odb.Test.Index
{
    [TestFixture]
    public class TestIndexesWithValueQueries : ODBTest
    {
        [Test]
        public virtual void Test1()
        {
            var baseName = GetBaseName();
            const int size = 1000;
            IOdb odb = Open(baseName);
            odb.GetClassRepresentation<VO.Login.Function>().AddIndexOn("index1", new[] {"name"}, true);
            for (var i = 0; i < size; i++)
                odb.Store(new VO.Login.Function("function " + i));

            odb.Close();
            odb = Open(baseName);
            // build a value query to retrieve only the name of the function
            var vq =
                new ValuesCriteriaQuery(typeof (VO.Login.Function), Where.Equal("name", "function " + (size - 1))).Field
                    ("name");
            var values = odb.GetValues(vq);
            AssertEquals(1, values.Count);
            Println(vq.GetExecutionPlan().GetDetails());
            AssertEquals(true, vq.GetExecutionPlan().UseIndex());
            odb.Close();

            DeleteBase(baseName);
        }
    }
}
