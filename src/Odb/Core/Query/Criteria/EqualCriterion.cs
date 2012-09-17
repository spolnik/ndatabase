using System;
using System.Text;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer2.Meta.Compare;

namespace NDatabase.Odb.Core.Query.Criteria
{
    /// <summary>
    ///   A criterion to match equality
    /// </summary>
    /// <author>olivier s</author>
    
    public sealed class EqualCriterion : AbstractCriterion
    {
        private object _criterionValue;

        private bool _isCaseSensitive;

        private bool _objectIsNative;

        /// <summary>
        ///   For criteria query on objects, we use the oid of the object instead of the object itself.
        /// </summary>
        /// <remarks>
        ///   For criteria query on objects, we use the oid of the object instead of the object itself. So comparison will be done with OID It is faster and avoid the need of the object (class) having to implement Serializable in client server mode
        /// </remarks>
        private OID _oid;

        public EqualCriterion(string attributeName, int value) : base(attributeName)
        {
            Init(value);
        }

        public EqualCriterion(string attributeName, short value) : base(attributeName)
        {
            Init(value);
        }

        public EqualCriterion(string attributeName, byte value) : base(attributeName)
        {
            Init(value);
        }

        public EqualCriterion(string attributeName, float value) : base(attributeName)
        {
            Init(value);
        }

        public EqualCriterion(string attributeName, double value) : base(attributeName)
        {
            Init(value);
        }

        public EqualCriterion(string attributeName, long value) : base(attributeName)
        {
            Init(value);
        }

        /// <param name="attributeName"> </param>
        /// <param name="value"> </param>
        public EqualCriterion(string attributeName, object value) : base(attributeName)
        {
            Init(value);
        }

        /// <param name="attributeName"> </param>
        /// <param name="value"> </param>
        /// <param name="isCaseSensitive"> </param>
        public EqualCriterion(string attributeName, object value, bool isCaseSensitive) : base(attributeName)
        {
            _criterionValue = value;
            _isCaseSensitive = isCaseSensitive;
        }

        public EqualCriterion(string attributeName, string value, bool isCaseSensitive) : base(attributeName)
        {
            _criterionValue = value;
            _isCaseSensitive = isCaseSensitive;
        }

        public EqualCriterion(string attributeName, bool value) : base(attributeName)
        {
            Init(value);
        }

        private void Init(object value)
        {
            _criterionValue = value;
            _isCaseSensitive = true;
            _objectIsNative = _criterionValue == null || OdbType.IsNative(_criterionValue.GetType());
        }

        public override bool Match(object valueToMatch)
        {
            // If it is a AttributeValuesMap, then gets the real value from the map
            // AttributeValuesMap is used to optimize Criteria Query
            // (reading only values of the object that the query needs to be
            // evaluated instead of reading the entire object)
            if (valueToMatch is AttributeValuesMap)
            {
                var attributeValues = (AttributeValuesMap) valueToMatch;
                valueToMatch = attributeValues.GetAttributeValue(AttributeName);
            }

            if (valueToMatch == null && _criterionValue == null && _oid == null)
                return true;

            if (AttributeValueComparator.IsNumber(valueToMatch) && AttributeValueComparator.IsNumber(_criterionValue))
                return AttributeValueComparator.Compare((IComparable)valueToMatch, (IComparable)_criterionValue) == 0;

            // if case sensitive (default value), just call the equals on the
            // objects
            if (_isCaseSensitive)
            {
                if (_objectIsNative)
                    return valueToMatch != null && Equals(valueToMatch, _criterionValue);

                var objectOid = (OID) valueToMatch;
                if (_oid == null)
                {
                    Ready();

                    if (_oid == null)
                    {
                        // TODO Should we return false or thrown exception?
                        return false;    
                    }
                }

                // throw new
                // ODBRuntimeException(NDatabaseError.CRITERIA_QUERY_ON_UNKNOWN_OBJECT);
                return _oid.Equals(objectOid);
            }

            // && valueToMatch.equals(criterionValue);
            // Case insensitive (iequal) only works on String or Character!
            var canUseCaseInsensitive = (_criterionValue.GetType() == typeof (string) &&
                                         valueToMatch.GetType() == typeof (string)) ||
                                        (_criterionValue is char && valueToMatch.GetType() == typeof (char));
            if (!canUseCaseInsensitive)
            {
                throw new OdbRuntimeException(
                    NDatabaseError.QueryAttributeTypeNotSupportedInIequalExpression.AddParameter(
                        valueToMatch.GetType().FullName));
            }

            // Cast to string to make the right comparison using the
            // equalsIgnoreCase
            var s1 = (string) valueToMatch;
            var s2 = (string) _criterionValue;
            return String.Compare(s1, s2, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public override string ToString()
        {
            var buffer = new StringBuilder();
            buffer.Append(AttributeName).Append(" = ").Append(_criterionValue);
            return buffer.ToString();
        }

        public override AttributeValuesMap GetValues()
        {
            var map = new AttributeValuesMap();

            if (_criterionValue == null && _oid != null)
                map.SetOid(_oid);
            else
                map.Add(AttributeName, _criterionValue);

            return map;
        }

        public override bool CanUseIndex()
        {
            return true;
        }

        public override void Ready()
        {
            if (_objectIsNative)
                return;

            if (GetQuery() == null)
                throw new OdbRuntimeException(NDatabaseError.ContainsQueryWithNoQuery);

            var engine = GetQuery().GetStorageEngine();
            if (engine == null)
                throw new OdbRuntimeException(NDatabaseError.ContainsQueryWithNoStorageEngine);

            // For non native object, we just need the oid of it
            _oid = engine.GetObjectId(_criterionValue, false);
            _criterionValue = null;
        }
    }
}
