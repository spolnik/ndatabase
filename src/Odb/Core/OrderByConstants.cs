using System;

namespace NDatabase.Odb.Core
{
    /// <summary>
    ///   Some constants used for ordering queries and creating ordered collection iterators
    /// </summary>
    /// <author>osmadja</author>
    [Serializable]
    public class OrderByConstants
    {
        private const int OrderByNoneType = 0;

        private const int OrderByDescType = 1;

        private const int OrderByAscType = 2;

        public static readonly OrderByConstants OrderByNone = new OrderByConstants(OrderByNoneType);

        public static readonly OrderByConstants OrderByDesc = new OrderByConstants(OrderByDescType);

        public static readonly OrderByConstants OrderByAsc = new OrderByConstants(OrderByAscType);

        private readonly int _type;

        private OrderByConstants(int type)
        {
            _type = type;
        }

        public virtual bool IsOrderByDesc()
        {
            return _type == OrderByDescType;
        }

        public virtual bool IsOrderByAsc()
        {
            return _type == OrderByAscType;
        }

        public virtual bool IsOrderByNone()
        {
            return _type == OrderByNoneType;
        }

        public override string ToString()
        {
            switch (_type)
            {
                case OrderByAscType:
                {
                    return "order by asc";
                }

                case OrderByDescType:
                {
                    return "order by desc";
                }

                case OrderByNoneType:
                {
                    return "no order by";
                }
            }
            return "?";
        }
    }
}
