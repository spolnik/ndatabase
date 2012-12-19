using NDatabase2.Odb;
using NDatabase2.Odb.Core.Query;
using NDatabase2.Odb.Core.Query.Values;
using NUnit.Framework;
using System.Linq;

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
            odb.IndexManagerFor<VO.Login.Function>().AddIndexOn("index1", new[] {"name"});
            for (var i = 0; i < size; i++)
                odb.Store(new VO.Login.Function("function " + i));

            odb.Close();
            odb = Open(baseName);
            // build a value query to retrieve only the name of the function
            var vq =
                odb.ValuesQuery<VO.Login.Function>().Field("name");

            vq.Descend("name").Equal("function " + (size - 1));

            var values = odb.GetValues(vq);
            AssertEquals(1, values.Count());
            Println(((IInternalQuery)vq).GetExecutionPlan().GetDetails());
            AssertEquals(true, ((IInternalQuery)vq).GetExecutionPlan().UseIndex());
            odb.Close();

            DeleteBase(baseName);
        }
    }
}
