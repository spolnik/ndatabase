using System;
using System.Text;
using NDatabase2.Odb.Core.Layers.Layer2.Meta;
using NDatabase2.Odb.Core.Layers.Layer2.Meta.Compare;

namespace NDatabase2.Odb.Core.Query.Criteria.Evaluations
{
    internal class EqualsEvaluation : AEvaluation
    {
        public EqualsEvaluation(object theObject, string attributeName)
            : base(theObject, attributeName)
        {
        }

        public override bool Evaluate(object candidate)
        {
            candidate = AsAttributeValuesMapValue(candidate);

            if (candidate == null && TheObject == null)
                return true;

            if (candidate == null || TheObject == null)
                return false;

            if (AttributeValueComparator.IsNumber(candidate) && AttributeValueComparator.IsNumber(TheObject))
                return AttributeValueComparator.Compare((IComparable) candidate, (IComparable) TheObject) == 0;

            return Equals(candidate, TheObject);
        }

        public override string ToString()
        {
            var buffer = new StringBuilder();
            buffer.Append(AttributeName).Append(" = ").Append(TheObject);
            return buffer.ToString();
        }

        public AttributeValuesMap GetValues()
        {
            return new AttributeValuesMap {{AttributeName, TheObject}};
        }
    }
}