using System;
using System.Text;
using NDatabase2.Odb.Core.Layers.Layer2.Meta;
using NDatabase2.Odb.Core.Layers.Layer2.Meta.Compare;

namespace NDatabase2.Odb.Core.Query.Criteria
{
    public static class ComparisonCirerion
    {
        public const int ComparisonTypeGt = 1;

        public const int ComparisonTypeGe = 2;

        public const int ComparisonTypeLt = 3;

        public const int ComparisonTypeLe = 4;
    }

    /// <summary>
    ///   A Criterion for greater than (gt),greater or equal(ge), less than (lt) and less or equal (le)
    /// </summary>
    public sealed class ComparisonCriterion<T> : AbstractCriterion where T : IComparable
    {
        private int _comparisonType;
        private T _criterionValue;

        public ComparisonCriterion(string attributeName, T value, int comparisonType)
            : base(attributeName)
        {
            Init(value, comparisonType);
        }

        private void Init(T value, int comparisonType)
        {
            _criterionValue = value;
            _comparisonType = comparisonType;
        }

        public override bool Match(object valueToMatch)
        {
            if (valueToMatch == null && _criterionValue == null)
                return true;

            valueToMatch = AsAttributeValuesMapValue(valueToMatch);

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
                case ComparisonCirerion.ComparisonTypeGt:
                {
                    return AttributeValueComparator.Compare(comparable1, comparable2) > 0;
                }

                case ComparisonCirerion.ComparisonTypeGe:
                {
                    return AttributeValueComparator.Compare(comparable1, comparable2) >= 0;
                }

                case ComparisonCirerion.ComparisonTypeLt:
                {
                    return AttributeValueComparator.Compare(comparable1, comparable2) < 0;
                }

                case ComparisonCirerion.ComparisonTypeLe:
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
                case ComparisonCirerion.ComparisonTypeGt:
                {
                    return ">";
                }

                case ComparisonCirerion.ComparisonTypeGe:
                {
                    return ">=";
                }

                case ComparisonCirerion.ComparisonTypeLt:
                {
                    return "<";
                }

                case ComparisonCirerion.ComparisonTypeLe:
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
    }
}
