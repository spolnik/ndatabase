using System;
using System.Collections;

namespace NDatabase2.Odb.Core.Query.Criteria
{
    /// <summary>
    ///   A criterio to test collection or array size
    /// </summary>
    internal sealed class CollectionSizeCriterion : AConstraint
    {
        public const int SizeEq = 1;
        public const int SizeNe = 2;
        public const int SizeGt = 3;
        public const int SizeGe = 4;
        public const int SizeLt = 5;
        public const int SizeLe = 6;

        private readonly int _sizeType;

        public CollectionSizeCriterion(IQuery query, string attributeName, int size, int sizeType)
            : base(query, attributeName, size)
        {
            _sizeType = sizeType;
        }

        public override bool Match(object valueToMatch)
        {
            valueToMatch = AsAttributeValuesMapValue(valueToMatch);
            var size = (int) TheObject;

            if (valueToMatch == null)
            {
                // Null list are considered 0-sized list
                if (_sizeType == SizeEq && size == 0)
                    return true;
                if ((_sizeType == SizeLe && size >= 0) || (_sizeType == SizeLt && size > 0))
                    return true;
                return _sizeType == SizeNe && size != 0;
            }

            var collection = valueToMatch as ICollection;
            if (collection != null)
                return MatchSize(collection.Count, size, _sizeType);

            var clazz = valueToMatch.GetType();
            if (clazz.IsArray)
            {
                var arrayLength = ((Array) valueToMatch).GetLength(0);
                return MatchSize(arrayLength, size, _sizeType);
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
    }
}
