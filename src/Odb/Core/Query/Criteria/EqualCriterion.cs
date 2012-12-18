using System;
using System.Text;
using NDatabase2.Odb.Core.Layers.Layer2.Meta;
using NDatabase2.Odb.Core.Layers.Layer2.Meta.Compare;

namespace NDatabase2.Odb.Core.Query.Criteria
{
    /// <summary>
    ///   A criterion to match equality
    /// </summary>
    internal sealed class EqualCriterion : AConstraint
    {
        private bool _isCaseSensitive;

        private bool _objectIsNative;

        /// <summary>
        ///   For criteria query on objects, we use the oid of the object instead of the object itself.
        /// </summary>
        /// <remarks>
        ///   For criteria query on objects, we use the oid of the object instead of the object itself. So comparison will be done with OID It is faster and avoid the need of the object (class) having to implement Serializable in client server mode
        /// </remarks>
        private OID _oid;

        public EqualCriterion(string attributeName, object value) 
            : base(attributeName, value)
        {
            Init(value);
        }

        /// <param name="attributeName"> </param>
        /// <param name="value"> </param>
        /// <param name="isCaseSensitive"> </param>
        private EqualCriterion(string attributeName, object value, bool isCaseSensitive) 
            : base(attributeName, value)
        {
            _isCaseSensitive = isCaseSensitive;
        }

        public static EqualCriterion CreateInvartiantStringEqualCriterion(string attributeName, string value,
                                                                                  bool isCaseSensitive)
        {
            return new EqualCriterion(attributeName, value, isCaseSensitive);
        }

        private void Init(object value)
        {
            _isCaseSensitive = true;
            _objectIsNative = TheObject == null || OdbType.IsNative(TheObject.GetType());
        }

        public override bool Match(object valueToMatch)
        {
            valueToMatch = AsAttributeValuesMapValue(valueToMatch);

            if (valueToMatch == null && TheObject == null && _oid == null)
                return true;

            if (AttributeValueComparator.IsNumber(valueToMatch) && AttributeValueComparator.IsNumber(TheObject))
                return AttributeValueComparator.Compare((IComparable)valueToMatch, (IComparable)TheObject) == 0;

            // if case sensitive (default value), just call the equals on the objects
            if (_isCaseSensitive)
            {
                if (_objectIsNative)
                    return valueToMatch != null && Equals(valueToMatch, TheObject);

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
            var typeOfValueToMatch = valueToMatch.GetType();

            var canUseCaseInsensitive = TheObject != null && ((TheObject is string && typeOfValueToMatch == typeof(string)) ||
                                                              (TheObject is char && typeOfValueToMatch == typeof(char)));
            if (!canUseCaseInsensitive)
            {
                throw new OdbRuntimeException(
                    NDatabaseError.QueryAttributeTypeNotSupportedInIequalExpression.AddParameter(
                        typeOfValueToMatch.FullName));
            }

            // Cast to string to make the right comparison using the
            // equalsIgnoreCase
            var s1 = (string) valueToMatch;
            var s2 = TheObject as string;
            return String.Compare(s1, s2, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public override string ToString()
        {
            var buffer = new StringBuilder();
            buffer.Append(AttributeName).Append(" = ").Append(TheObject);
            return buffer.ToString();
        }

        public override AttributeValuesMap GetValues()
        {
            var map = new AttributeValuesMap();

            if (_oid != null)
                map.SetOid(_oid);
            else
                map.Add(AttributeName, TheObject);

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

            var engine = ((IInternalQuery) GetQuery()).GetStorageEngine();
            if (engine == null)
                throw new OdbRuntimeException(NDatabaseError.ContainsQueryWithNoStorageEngine);

            // For non native object, we just need the oid of it
            _oid = engine.GetObjectId(TheObject, false);
        }
    }
}
