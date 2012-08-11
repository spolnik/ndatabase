using System;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Query.Execution;
using NDatabase.Odb.Core.Query.Values;
using NDatabase.Tool.Wrappers;

namespace NDatabase.Odb.Impl.Core.Query.Values
{
    /// <summary>
    ///   An action to compute the average value of a field
    /// </summary>
    /// <author>osmadja</author>
    [Serializable]
    public class AverageValueAction : AbstractQueryFieldAction
    {
        private readonly int _roundType;
        private readonly int _scale;
        private Decimal _average;
        private int _nbValues;
        private Decimal _totalValue;

        public AverageValueAction(string attributeName, string alias) : base(attributeName, alias, false)
        {
            _totalValue = new Decimal(0);
            _nbValues = 0;
            AttributeName = attributeName;
            _scale = OdbConfiguration.GetScaleForAverageDivision();
            _roundType = OdbConfiguration.GetRoundTypeForAverageDivision();
        }

        public override void Execute(OID oid, AttributeValuesMap values)
        {
            var n = (Decimal) values[AttributeName];
            _totalValue = NDatabaseNumber.Add(_totalValue, ValuesUtil.Convert(n));
            _nbValues++;
        }

        public override object GetValue()
        {
            return _average;
        }

        public override void End()
        {
            _average = NDatabaseNumber.Divide(_totalValue, _nbValues, _roundType, _scale);
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
