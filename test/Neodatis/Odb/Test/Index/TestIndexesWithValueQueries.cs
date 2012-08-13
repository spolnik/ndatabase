using NDatabase.Odb;
using NDatabase.Odb.Core.Query;
using NDatabase.Odb.Core.Query.Criteria;
using NDatabase.Odb.Impl.Core.Query.Values;
using NUnit.Framework;
using Test.Odb.Test;
using Test.Odb.Test.VO.Login;

namespace Index
{
	/// <author>olivier</author>
	[TestFixture]
    public class TestIndexesWithValueQueries : ODBTest
	{
		[Test]
        public virtual void Test1()
		{
			string baseName = GetBaseName();
			IOdb odb = null;
			int size = 10000;
			try
			{
				odb = Open(baseName);
				odb.GetClassRepresentation(typeof(Function)).AddIndexOn
					("index1", new string[] { "name" }, true);
				for (int i = 0; i < size; i++)
				{
					odb.Store(new Function("function " + i));
				}
				odb.Close();
				odb = Open(baseName);
				// build a value query to retrieve only the name of the function
				IValuesQuery vq = new ValuesCriteriaQuery
					(typeof(Function), Where
					.Equal("name", "function " + (size - 1))).Field("name");
				var values = odb.GetValues(vq);
				AssertEquals(1, values.Count);
				Println(vq.GetExecutionPlan().GetDetails());
				AssertEquals(true, vq.GetExecutionPlan().UseIndex());
			}
			finally
			{
				if (odb != null)
				{
					odb.Close();
				}
			}
		}
	}
}
