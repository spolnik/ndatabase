using System;
using NDatabase.Odb.Core;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Query.Criteria;
using NDatabase.Tool.Wrappers;

namespace NDatabase.Odb.Impl.Core.Query.Criteria
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

            // If it is a AttributeValuesMap, then gets the real value from the map
            if (valueToMatch is AttributeValuesMap)
            {
                var attributeValues = (AttributeValuesMap) valueToMatch;
                valueToMatch = attributeValues[AttributeName];
            }

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

                if (_isCaseSensitive)
                    return OdbString.Matches(regExp, value);

                return OdbString.Matches(regExp.ToLower(), value.ToLower());
            }
            if (_isCaseSensitive)
            {
                regExp = string.Format("(.)*{0}(.)*", _criterionValue);
                return OdbString.Matches(regExp, value);
            }
            regExp = string.Format("(.)*{0}(.)*", _criterionValue.ToLower());
            return OdbString.Matches(regExp, value.ToLower());
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
