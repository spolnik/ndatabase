using System.Collections;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Tool.Wrappers;

namespace NDatabase.Odb.Core.Query.Criteria
{
    
    public sealed class ContainsCriterion : AbstractCriterion
    {
        private object _criterionValue;

        private bool _objectIsNative;

        /// <summary>
        ///   For criteria query on objects, we use the oid of the object instead of the object itself.
        /// </summary>
        /// <remarks>
        ///   For criteria query on objects, we use the oid of the object instead of the object itself. So comparison will be done with OID It is faster and avoid the need of the object (class) having to implement Serializable in client server mode
        /// </remarks>
        private OID _oid;

        public ContainsCriterion(string attributeName, string criterionValue) : base(attributeName)
        {
            Init(criterionValue);
        }

        public ContainsCriterion(string attributeName, int value) : base(attributeName)
        {
            Init(value);
        }

        public ContainsCriterion(string attributeName, short value) : base(attributeName)
        {
            Init(value);
        }

        public ContainsCriterion(string attributeName, byte value) : base(attributeName)
        {
            Init(value);
        }

        public ContainsCriterion(string attributeName, float value) : base(attributeName)
        {
            Init(value);
        }

        public ContainsCriterion(string attributeName, double value) : base(attributeName)
        {
            Init(value);
        }

        public ContainsCriterion(string attributeName, long value) : base(attributeName)
        {
            Init(value);
        }

        public ContainsCriterion(string attributeName, object value) : base(attributeName)
        {
            Init(value);
        }

        public ContainsCriterion(string attributeName, bool value) : base(attributeName)
        {
            Init(value);
        }

        private void Init(object value)
        {
            _criterionValue = value;
            _objectIsNative = _criterionValue == null || OdbType.IsNative(_criterionValue.GetType());
        }

        public override bool Match(object valueToMatch)
        {
            if (valueToMatch == null && _criterionValue == null && _oid == null)
                return true;

            if (valueToMatch == null)
                return false;

            if (valueToMatch is IDictionary)
            {
                // The value in the map, just take the object with the attributeName
                var map = (IDictionary) valueToMatch;
                valueToMatch = map[AttributeName];

                // The value valueToMatch was redefined, so we need to re-make some
                // tests
                if (valueToMatch == null && _criterionValue == null && _oid == null)
                    return true;

                if (valueToMatch == null)
                    return false;
            }

            var collection = valueToMatch as ICollection;
            if (collection != null)
                return CheckIfCollectionContainsValue(collection);

            var clazz = valueToMatch.GetType();

            if (clazz.IsArray)
                return CheckIfArrayContainsValue(valueToMatch);

            throw new OdbRuntimeException(
                NDatabaseError.QueryContainsCriterionTypeNotSupported.AddParameter(valueToMatch.GetType().FullName));
        }

        private bool CheckIfCollectionContainsValue(IEnumerable collection)
        {
            var engine = GetQuery().GetStorageEngine();
            if (engine == null)
                throw new OdbRuntimeException(NDatabaseError.QueryEngineNotSet);

            // If the object to compared is native
            if (_objectIsNative)
            {
                foreach (AbstractObjectInfo abstractObjectInfo in collection)
                {
                    if (abstractObjectInfo == null && _criterionValue == null)
                        return true;

                    if (abstractObjectInfo != null && _criterionValue == null)
                        return false;

                    if (abstractObjectInfo != null && _criterionValue.Equals(abstractObjectInfo.GetObject()))
                        return true;
                }

                return false;
            }
            
            foreach (AbstractObjectInfo abstractObjectInfo in collection)
            {
                if (abstractObjectInfo.IsNull() && _criterionValue == null && _oid == null)
                    return true;

                Ready();
                if (_oid == null)
                    continue;

                if (!abstractObjectInfo.IsNonNativeObject())
                    continue;

                var nnoi1 = (NonNativeObjectInfo)abstractObjectInfo;
                var isEqual = nnoi1.GetOid() != null && _oid != null && nnoi1.GetOid().Equals(_oid);

                if (isEqual)
                    return true;
            }

            return false;
        }

        private bool CheckIfArrayContainsValue(object valueToMatch)
        {
            var arrayLength = OdbArray.GetArrayLength(valueToMatch);
            for (var i = 0; i < arrayLength; i++)
            {
                var element = OdbArray.GetArrayElement(valueToMatch, i);
                if (element == null && _criterionValue == null)
                    return true;

                var abstractObjectInfo = (AbstractObjectInfo) element;
                if (abstractObjectInfo != null && abstractObjectInfo.GetObject() != null &&
                    abstractObjectInfo.GetObject().Equals(_criterionValue))
                    return true;
            }
            return false;
        }

        public override AttributeValuesMap GetValues()
        {
            return new AttributeValuesMap();
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
        }
    }
}
