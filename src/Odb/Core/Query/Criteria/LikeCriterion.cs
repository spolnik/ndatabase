using System;
using NDatabase2.Tool.Wrappers;

namespace NDatabase2.Odb.Core.Query.Criteria
{
    internal sealed class LikeCriterion : AConstraint
    {
        private readonly bool _isCaseSensitive;

        public LikeCriterion(IQuery query, string attributeName, string criterionValue, bool isCaseSensiive) 
            : base(query, attributeName, criterionValue)
        {
            _isCaseSensitive = isCaseSensiive;
        }

        public override bool Match(object valueToMatch)
        {
            string regExp;
            if (valueToMatch == null)
                return false;

            valueToMatch = AsAttributeValuesMapValue(valueToMatch);

            if (valueToMatch == null)
                return false;

            // Like operator only work with String
            if (!(valueToMatch is string))
            {
                throw new OdbRuntimeException(
                    NDatabaseError.QueryAttributeTypeNotSupportedInLikeExpression.AddParameter(
                        valueToMatch.GetType().FullName));
            }

            var value = (string) valueToMatch;
            var criterionValue = (string) TheObject;

            if (criterionValue.IndexOf("%", StringComparison.Ordinal) != -1)
            {
                regExp = criterionValue.Replace("%", "(.)*");

                return _isCaseSensitive
                           ? OdbString.Matches(regExp, value)
                           : OdbString.Matches(regExp.ToLower(), value.ToLower());
            }
            if (_isCaseSensitive)
            {
                regExp = string.Format("(.)*{0}(.)*", criterionValue);
                return OdbString.Matches(regExp, value);
            }
            regExp = string.Format("(.)*{0}(.)*", criterionValue.ToLower());

            return OdbString.Matches(regExp, value.ToLower());
        }
    }
}
