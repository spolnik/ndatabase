using System;
using System.Collections;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Tool.Wrappers;

namespace NDatabase.Odb.Core.Query.Criteria
{
    /// <summary>
    ///   A criterio to test collection or array size
    /// </summary>
    /// <author>olivier s</author>
    
    public sealed class CollectionSizeCriterion : AbstractCriterion
    {
        public const int SizeEq = 1;
        public const int SizeNe = 2;
        public const int SizeGt = 3;
        public const int SizeGe = 4;
        public const int SizeLt = 5;
        public const int SizeLe = 6;

        private readonly int _size;
        private readonly int _sizeType;

        public CollectionSizeCriterion(string attributeName, int size, int sizeType) : base(attributeName)
        {
            // The size that the collection must have
            _size = size;
            _sizeType = sizeType;
        }

        public override bool Match(object valueToMatch)
        {
            // If it is a AttributeValuesMap, then gets the real value from the map
            if (valueToMatch is AttributeValuesMap)
            {
                var attributeValues = (AttributeValuesMap) valueToMatch;
                valueToMatch = attributeValues.GetAttributeValue(AttributeName);
            }
            if (valueToMatch == null)
            {
                // Null list are considered 0-sized list
                if (_sizeType == SizeEq && _size == 0)
                    return true;
                if ((_sizeType == SizeLe && _size >= 0) || (_sizeType == SizeLt && _size > 0))
                    return true;
                if (_sizeType == SizeNe && _size != 0)
                    return true;
                return false;
            }
            var collection = valueToMatch as ICollection;
            if (collection != null)
                return MatchSize(collection.Count, _size, _sizeType);

            var clazz = valueToMatch.GetType();
            if (clazz.IsArray)
            {
                var arrayLength = ((Array) valueToMatch).GetLength(0);
                return MatchSize(arrayLength, _size, _sizeType);
            }

            throw new OdbRuntimeException(NDatabaseError.QueryBadCriteria.AddParameter(valueToMatch.GetType().FullName));
        }

        private static bool MatchSize(int collectionSize, int requestedSize, int sizeType)
        {
            switch (sizeType)
            {
                case SizeEq:
                    return collectionSize == requestedSize;
                case SizeNe:
                    return collectionSize != requestedSize;
                case SizeGt:
                    return collectionSize > requestedSize;
                case SizeGe:
                    return collectionSize >= requestedSize;
                case SizeLt:
                    return collectionSize < requestedSize;
                case SizeLe:
                    return collectionSize <= requestedSize;
            }

            throw new OdbRuntimeException(NDatabaseError.QueryCollectionSizeCriteriaNotSupported.AddParameter(sizeType));
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
