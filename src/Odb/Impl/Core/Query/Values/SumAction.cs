using System;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Query.Execution;
using NDatabase.Odb.Core.Query.Values;
using NDatabase.Tool.Wrappers;

namespace NDatabase.Odb.Impl.Core.Query.Values
{
    [Serializable]
    public class SumAction : AbstractQueryFieldAction
    {
        private Decimal _sum;

        public SumAction(string attributeName, string alias) : base(attributeName, alias, false)
        {
            _sum = new Decimal(0);
        }

        public override void Execute(OID oid, AttributeValuesMap values)
        {
            var number = (Decimal) values[AttributeName];
            _sum = Decimal.Add(_sum, ValuesUtil.Convert(number));
        }

        public virtual Decimal GetSum()
        {
            return _sum;
        }

        public override object GetValue()
        {
            return _sum;
        }

        public override void End()
        {
        }

        public override void Start()
        {
        }

        public override IQueryFieldAction Copy()
        {
            return new SumAction(AttributeName, Alias);
        }
    }
}
