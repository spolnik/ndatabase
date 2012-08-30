using System;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Query.Criteria;

namespace NDatabase.Odb.Impl.Core.Query.Criteria
{
    
    public sealed class IsNullCriterion : AbstractCriterion
    {
        public IsNullCriterion(string attributeName) : base(attributeName)
        {
        }

        public override bool Match(object valueToMatch)
        {
            // If it is a AttributeValuesMap, then gets the real value from the map
            if (valueToMatch is AttributeValuesMap)
            {
                var attributeValues = (AttributeValuesMap) valueToMatch;
                valueToMatch = attributeValues[AttributeName];
            }
            return valueToMatch == null;
        }

        public override AttributeValuesMap GetValues()
        {
            return new AttributeValuesMap();
        }

        public override void Ready()
        {
        }
    }
}
