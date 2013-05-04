using NDatabase.Api;
using NUnit.Framework;

namespace NDatabase.Client.UnitTests.Queries.Values
{
    public class TestCase_working_with_values_query
    {
        private const string DbName = "values_query.ndb";
        private const int Limit = 10;

        [SetUp]
        public void SetUp()
        {
            OdbFactory.Delete(DbName);

            using (var odb = OdbFactory.Open(DbName))
            {
                for (var i = 1; i <= Limit; i++)
                    odb.Store(new Result("Result" + i, i));
            }
        }

        [Test]
        public void using_sum_on_stored_objects()
        {
            IValues values;

            using (var odb = OdbFactory.Open(DbName))
            {
                values = odb.ValuesQuery<Result>().Sum("_value", "sum").Execute();
            }

            var objectValues = values.NextValues();
            Assert.That(objectValues.GetByAlias("sum"), Is.EqualTo(55m));
        }
    }

    public class Result
    {
        private readonly string _name;
        private readonly int _value;

        public Result(string name, int value)
        {
            _name = name;
            _value = value;
        }

        public string GetName()
        {
            return _name;
        }

        public int GetValue()
        {
            return _value;
        }
    }
}