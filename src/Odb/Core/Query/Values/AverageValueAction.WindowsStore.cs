using System;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Query.Execution;

namespace NDatabase.Odb.Core.Query.Values
{
    /// <summary>
    ///   An action to compute the average value of a field
    /// </summary>
    internal sealed class AverageValueAction : AbstractQueryFieldAction
    {
        private Decimal _average;
        private int _nbValues;
        private Decimal _totalValue;

        public AverageValueAction(string attributeName, string alias) : base(attributeName, alias, false)
        {
            _totalValue = new Decimal(0);
            _nbValues = 0;
            AttributeName = attributeName;
        }

        public override void Execute(OID oid, AttributeValuesMap values)
        {
            var n = Convert.ToDecimal(values[AttributeName]);
            _totalValue = Decimal.Add(_totalValue, ValuesUtil.Convert(n));
            _nbValues++;
        }

        public override object GetValue()
        {
            return _average;
        }

        public override void End()
        {
            var result = Decimal.Divide(_totalValue, _nbValues);
            _average = Decimal.Ceiling(result);
        }

        public override void Start()
        {
        }

        public override IQueryFieldAction Copy()
        {
            return new AverageValueAction(AttributeName, Alias);
        }
    }
}
