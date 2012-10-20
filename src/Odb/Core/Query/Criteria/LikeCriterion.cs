using System;
using NDatabase2.Tool.Wrappers;

namespace NDatabase2.Odb.Core.Query.Criteria
{
    public sealed class LikeCriterion : AbstractCriterion
    {
        private readonly string _criterionValue;
        private readonly bool _isCaseSensitive;

        public LikeCriterion(string attributeName, string criterionValue, bool isCaseSensiive) : base(attributeName)
        {
            _criterionValue = criterionValue;
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
            if (_criterionValue.IndexOf("%", StringComparison.Ordinal) != -1)
            {
                regExp = _criterionValue.Replace("%", "(.)*");

                return _isCaseSensitive
                           ? OdbString.Matches(regExp, value)
                           : OdbString.Matches(regExp.ToLower(), value.ToLower());
            }
            if (_isCaseSensitive)
            {
                regExp = string.Format("(.)*{0}(.)*", _criterionValue);
                return OdbString.Matches(regExp, value);
            }
            regExp = string.Format("(.)*{0}(.)*", _criterionValue.ToLower());
            return OdbString.Matches(regExp, value.ToLower());
        }
    }
}
