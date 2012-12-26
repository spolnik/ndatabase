using System;
using System.Collections;
using NDatabase2.Odb.Core.Layers.Layer2.Meta;

namespace NDatabase2.Odb.Core.Query.Criteria.Evaluations
{
    internal class ContainsEvaluation : AEvaluation
    {
        /// <summary>
        ///   For criteria query on objects, we use the oid of the object instead of the object itself.
        /// </summary>
        /// <remarks>
        ///   For criteria query on objects, we use the oid of the object instead of the object itself. 
        ///   So comparison will be done with OID It is faster and avoid the need of the object 
        ///   (class) having to implement Serializable in client server mode
        /// </remarks>
        private readonly OID _oid;

        private IInternalQuery _query;

        public ContainsEvaluation(object theObject, string attributeName, IQuery query) 
            : base(theObject, attributeName)
        {
            _query = (IInternalQuery) query;

            if (IsNative())
                return;

            // For non native object, we just need the oid of it
            _oid = _query.GetStorageEngine().GetObjectId(TheObject, false);
        }

        public override bool Evaluate(object candidate)
        {
            candidate = AsAttributeValuesMapValue(candidate);

            if (candidate == null && TheObject == null && _oid == null)
                return true;

            if (candidate == null)
                return false;

            if (candidate is OID)
            {
                var oid = (OID) candidate;
                candidate = _query.GetStorageEngine().GetObjectFromOid(oid);
            }

            if (candidate is IDictionary)
            {
                // The value in the map, just take the object with the attributeName
                var map = (IDictionary)candidate;
                candidate = map[AttributeName];

                // The value valueToMatch was redefined, so we need to re-make some
                // tests
                if (candidate == null && TheObject == null && _oid == null)
                    return true;

                if (candidate == null)
                    return false;
            }

            var collection = candidate as ICollection;
            if (collection != null)
                return CheckIfCollectionContainsValue(collection);

            var clazz = candidate.GetType();

            if (clazz.IsArray)
                return CheckIfArrayContainsValue(candidate);

            var candidateAsString = candidate as string;
            if (candidateAsString != null)
                return CheckIfStringContainsValue(candidateAsString);

            throw new OdbRuntimeException(
                NDatabaseError.QueryContainsCriterionTypeNotSupported.AddParameter(candidate.GetType().FullName));
        }

        private bool CheckIfStringContainsValue(string candidate)
        {
            var value = TheObject as string;
            return value != null && candidate.Contains(value);
        }

        private bool CheckIfCollectionContainsValue(IEnumerable collection)
        {
            var genericTypeDefinition = collection.GetType().GetGenericArguments()[0];

            // If the object to compared is native
            if (IsNative())
            {
                if (genericTypeDefinition == typeof(AbstractObjectInfo))
                {
                    foreach (AbstractObjectInfo abstractObjectInfo in collection)
                    {
                        if (abstractObjectInfo == null && TheObject == null)
                            return true;

                        if (abstractObjectInfo != null && TheObject == null)
                            return false;

                        if (abstractObjectInfo != null && TheObject.Equals(abstractObjectInfo.GetObject()))
                            return true;
                    }
                }
                else
                {
                    foreach (var item in collection)
                    {
                        if (item == null && TheObject == null)
                            return true;

                        if (item != null && TheObject == null)
                            return false;

                        if (TheObject.Equals(item))
                            return true;
                    }
                    
                }

                return false;
            }


            if (genericTypeDefinition == typeof(AbstractObjectInfo))
            {
                foreach (AbstractObjectInfo abstractObjectInfo in collection)
                {
                    if (abstractObjectInfo.IsNull() && TheObject == null && _oid == null)
                        return true;

                    if (_oid == null)
                        continue;

                    if (!abstractObjectInfo.IsNonNativeObject())
                        continue;

                    var nnoi1 = (NonNativeObjectInfo)abstractObjectInfo;
                    var isEqual = nnoi1.GetOid() != null && _oid != null && nnoi1.GetOid().Equals(_oid);

                    if (isEqual)
                        return true;
                }
            }
            else
            {
                foreach (var item in collection)
                {
                    if (item == null && TheObject == null && _oid == null)
                        return true;

                    if (Equals(item, TheObject))
                        return true;

                    if (_oid == null || item == null)
                        continue;

                    if (!OdbType.IsNative(item.GetType()))
                        continue;

                    if (Equals(item, _oid))
                        return true;
                }
            }

            return false;
        }

        private bool CheckIfArrayContainsValue(object valueToMatch)
        {
            var arrayLength = ((Array)valueToMatch).GetLength(0);
            for (var i = 0; i < arrayLength; i++)
            {
                var element = ((Array)valueToMatch).GetValue(i);
                if (element == null && TheObject == null)
                    return true;

                var abstractObjectInfo = (AbstractObjectInfo)element;
                if (abstractObjectInfo != null && abstractObjectInfo.GetObject() != null &&
                    abstractObjectInfo.GetObject().Equals(TheObject))
                    return true;
            }
            return false;
        }
    }
}