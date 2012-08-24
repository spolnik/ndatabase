using System;
using NDatabase.Odb;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Impl.Core.Query.Values;

namespace Test.Odb.Test.Query.Values
{
    [Serializable]
    public class TestCustomQueryFieldAction : CustomQueryFieldAction
    {
        private Decimal myValue;

        public TestCustomQueryFieldAction()
        {
            myValue = new Decimal(0);
        }

        public override void Execute(OID oid, AttributeValuesMap values)
        {
            var n = ValuesUtil.Convert(Convert.ToDecimal(values[oid]));
            var multiply = decimal.Multiply(new Decimal(2), Convert.ToDecimal(n.ToString()));
            myValue = decimal.Add(multiply, myValue);
        }

        public override object GetValue()
        {
            return myValue;
        }

        public override bool IsMultiRow()
        {
            return false;
        }

        public override void Start()
        {
        }

        public override void End()
        {
        }
    }
}
