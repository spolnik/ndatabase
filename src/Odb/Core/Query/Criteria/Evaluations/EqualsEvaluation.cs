using System;
using System.Text;
using NDatabase2.Odb.Core.Layers.Layer2.Meta;
using NDatabase2.Odb.Core.Layers.Layer2.Meta.Compare;

namespace NDatabase2.Odb.Core.Query.Criteria.Evaluations
{
    internal class EqualsEvaluation : AEvaluation
    {
        private readonly bool _isCaseSensitive;

        public EqualsEvaluation(object theObject, string attributeName, bool isCaseSensitive = true)
            : base(theObject, attributeName)
        {
            _isCaseSensitive = isCaseSensitive;
        }

        public override bool Evaluate(object candidate)
        {
            candidate = AsAttributeValuesMapValue(candidate);

            if (candidate == null && TheObject == null)
                return true;

            if (AttributeValueComparator.IsNumber(candidate) && AttributeValueComparator.IsNumber(TheObject))
                return AttributeValueComparator.Compare((IComparable) candidate, (IComparable) TheObject) == 0;

            // if case sensitive (default value), just call the equals on the objects
            if (_isCaseSensitive)
            {
                if (IsNative())
                    return candidate != null && Equals(candidate, TheObject);
            }

            // Case insensitive (iequal) only works on String or Character!
            var typeOfValueToMatch = candidate.GetType();

            var canUseCaseInsensitive = TheObject != null &&
                                         ((TheObject is string && typeOfValueToMatch == typeof (string)) ||
                                          (TheObject is char && typeOfValueToMatch == typeof (char)));
            if (!canUseCaseInsensitive)
            {
                throw new OdbRuntimeException(
                    NDatabaseError.QueryAttributeTypeNotSupportedInIequalExpression.AddParameter(
                        typeOfValueToMatch.FullName));
            }

            // Cast to string to make the right comparison using the
            // equalsIgnoreCase
            var s1 = (string) candidate;
            var s2 = TheObject as string;

            return String.Compare(s1, s2, StringComparison.OrdinalIgnoreCase) == 0;
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