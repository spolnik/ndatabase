using System;
using System.Text;
using NDatabase2.Odb.Core.Layers.Layer2.Meta;
using NDatabase2.Odb.Core.Layers.Layer2.Meta.Compare;

namespace NDatabase2.Odb.Core.Query.Criteria.Evaluations
{
    internal abstract class AEvaluation : IEvaluation
    {
        protected readonly string AttributeName;
        protected readonly object TheObject;

        protected AEvaluation(object theObject, string attributeName)
        {
            TheObject = theObject;
            AttributeName = attributeName;
        }

        #region IEvaluation Members

        public abstract bool Evaluate(object candidate);

        #endregion

        protected bool IsNative()
        {
            return TheObject == null || OdbType.IsNative(TheObject.GetType());
        }

        protected object AsAttributeValuesMapValue(object valueToMatch)
        {
            // If it is a AttributeValuesMap, then gets the real value from the map
            var attributeValues = valueToMatch as AttributeValuesMap;

            return attributeValues != null
                       ? attributeValues[AttributeName]
                       : valueToMatch;
        }
    }

    internal class ComparisonEvaluation : AEvaluation
    {
        private readonly int _comparisonType;

        public ComparisonEvaluation(object theObject, string attributeName, IQuery query, int comparisonType) 
            : base(theObject, attributeName)
        {
            if (!(theObject is IComparable))
                throw new ArgumentException("Value need to implement IComparable", "theObject");

            _comparisonType = comparisonType;
        }

        public override bool Evaluate(object candidate)
        {
            if (candidate == null && TheObject == null)
                return true;

            candidate = AsAttributeValuesMapValue(candidate);

            if (candidate == null)
                return false;

            if (!(candidate is IComparable))
            {
                throw new OdbRuntimeException(
                    NDatabaseError.QueryComparableCriteriaAppliedOnNonComparable.AddParameter(
                        candidate.GetType().FullName));
            }

            var comparable1 = (IComparable)candidate;
            var comparable2 = (IComparable)TheObject;

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
            buffer.Append(AttributeName).Append(" ").Append(GetOperator()).Append(" ").Append(TheObject);
            return buffer.ToString();
        }

        private string GetOperator()
        {
            switch (_comparisonType)
            {
                case ComparisonCirerion.ComparisonTypeGt:
                    return ">";
                case ComparisonCirerion.ComparisonTypeGe:
                    return ">=";
                case ComparisonCirerion.ComparisonTypeLt:
                    return "<";
                case ComparisonCirerion.ComparisonTypeLe:
                    return "<=";}
            return "?";
        }

        internal AttributeValuesMap GetValues()
        {
            return new AttributeValuesMap { { AttributeName, TheObject } };
        }
    }
}