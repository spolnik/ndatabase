using System;
using System.Text;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Impl.Core.Layers.Layer2.Meta.Compare;

namespace NDatabase.Odb.Core.Query.Criteria
{
    /// <summary>
    ///   A Criterion for greater than (gt),greater or equal(ge), less than (lt) and less or equal (le)
    /// </summary>
    /// <author>olivier s</author>
    [Serializable]
    public sealed class ComparisonCriterion : AbstractCriterion
    {
        public const int ComparisonTypeGt = 1;

        public const int ComparisonTypeGe = 2;

        public const int ComparisonTypeLt = 3;

        public const int ComparisonTypeLe = 4;

        private int _comparisonType;
        private object _criterionValue;

        public ComparisonCriterion(string attributeName, string criterionValue, int comparisonType)
            : base(attributeName)
        {
            Init(criterionValue, comparisonType);
        }

        public ComparisonCriterion(string attributeName, int value, int comparisonType) : base(attributeName)
        {
            Init(value, comparisonType);
        }

        public ComparisonCriterion(string attributeName, short value, int comparisonType) : base(attributeName)
        {
            Init(value, comparisonType);
        }

        public ComparisonCriterion(string attributeName, byte value, int comparisonType) : base(attributeName)
        {
            Init(value, comparisonType);
        }

        public ComparisonCriterion(string attributeName, float value, int comparisonType) : base(attributeName)
        {
            Init(value, comparisonType);
        }

        public ComparisonCriterion(string attributeName, double value, int comparisonType) : base(attributeName)
        {
            Init(value, comparisonType);
        }

        public ComparisonCriterion(string attributeName, long value, int comparisonType) : base(attributeName)
        {
            Init(value, comparisonType);
        }

        public ComparisonCriterion(string attributeName, object value, int comparisonType) : base(attributeName)
        {
            Init(value, comparisonType);
        }

        public ComparisonCriterion(string attributeName, bool value, int comparisonType) : base(attributeName)
        {
            Init(value, comparisonType);
        }

        private void Init(object value, int comparisonType)
        {
            _criterionValue = value;
            _comparisonType = comparisonType;
        }

        public override bool Match(object valueToMatch)
        {
            if (valueToMatch == null && _criterionValue == null)
                return true;

            // If it is a AttributeValuesMap, then gets the real value from the map 
            if (valueToMatch is AttributeValuesMap)
            {
                var attributeValues = (AttributeValuesMap) valueToMatch;
                valueToMatch = attributeValues.GetAttributeValue(AttributeName);
            }

            if (valueToMatch == null)
                return false;

            if (!(valueToMatch is IComparable))
            {
                throw new OdbRuntimeException(
                    NDatabaseError.QueryComparableCriteriaAppliedOnNonComparable.AddParameter(
                        valueToMatch.GetType().FullName));
            }

            var comparable1 = (IComparable) valueToMatch;
            var comparable2 = (IComparable) _criterionValue;

            switch (_comparisonType)
            {
                case ComparisonTypeGt:
                {
                    return AttributeValueComparator.Compare(comparable1, comparable2) > 0;
                }

                case ComparisonTypeGe:
                {
                    return AttributeValueComparator.Compare(comparable1, comparable2) >= 0;
                }

                case ComparisonTypeLt:
                {
                    return AttributeValueComparator.Compare(comparable1, comparable2) < 0;
                }

                case ComparisonTypeLe:
                {
                    return AttributeValueComparator.Compare(comparable1, comparable2) <= 0;
                }
            }

            throw new OdbRuntimeException(NDatabaseError.QueryUnknownOperator.AddParameter(_comparisonType));
        }

        public override string ToString()
        {
            var buffer = new StringBuilder();
            buffer.Append(AttributeName).Append(" ").Append(GetOperator()).Append(" ").Append(_criterionValue);
            return buffer.ToString();
        }

        private string GetOperator()
        {
            switch (_comparisonType)
            {
                case ComparisonTypeGt:
                {
                    return ">";
                }

                case ComparisonTypeGe:
                {
                    return ">=";
                }

                case ComparisonTypeLt:
                {
                    return "<";
                }

                case ComparisonTypeLe:
                {
                    return "<=";
                }
            }
            return "?";
        }

        public override AttributeValuesMap GetValues()
        {
            return new AttributeValuesMap {{AttributeName, _criterionValue}};
        }

        public override void Ready()
        {
        }
    }
}
