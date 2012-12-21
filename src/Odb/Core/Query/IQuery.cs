using System;
using NDatabase2.Odb.Core.Query.Criteria;

namespace NDatabase2.Odb.Core.Query
{
    public interface IQuery
    {
        IObjectSet<T> Execute<T>() where T : class;
        IObjectSet<T> Execute<T>(bool inMemory) where T : class;
        IObjectSet<T> Execute<T>(bool inMemory, int startIndex, int endIndex) where T : class;

        /// <summary>
        ///   To order by the result of a query in descendent order
        /// </summary>
        /// <param name="fields"> A comma separated field list </param>
        /// <returns> this </returns>
        IQuery OrderByDesc(string fields);

        /// <summary>
        ///   To order by the result of a query in ascendent order
        /// </summary>
        /// <param name="fields"> A comma separated field list </param>
        /// <returns> this </returns>
        IQuery OrderByAsc(string fields);

        /// <summary>
        ///   Returns true if the query has an order by clause
        /// </summary>
        /// <returns> true if has an order by flag </returns>
        bool HasOrderBy();

        /// <summary>
        ///   Returns the field names of the order by
        /// </summary>
        /// <returns> The array of fields of the order by </returns>
        string[] GetOrderByFieldNames();

        /// <returns> the type of the order by - ORDER_BY_NONE,ORDER_BY_DESC,ORDER_BY_ASC </returns>
        OrderByConstants GetOrderByType();

        /// <summary>
        ///   used with isForSingleOid == true, to indicate we are working on a single object with a specific oid
        /// </summary>
        /// <returns> </returns>
        OID GetOidOfObjectToQuery();

        bool Match(object @object);

        Type UnderlyingType { get; }

        IQuery Descend(string attributeName);

        IConstraint LessOrEqual<TItem>(TItem value) where TItem : IComparable;
        IConstraint GreaterThan<TItem>(TItem value) where TItem : IComparable;
        IConstraint GreaterOrEqual<TItem>(TItem value) where TItem : IComparable;
        IConstraint LessThan<TItem>(TItem value) where TItem : IComparable;
        IConstraint Contain(object value);
        IConstraint SizeEq(int size);
        IConstraint SizeNe(int size);
        IConstraint SizeGt(int size);
        IConstraint SizeGe(int size);
        IConstraint SizeLt(int size);
        IConstraint SizeLe(int size);

        long Count();
        IConstraint Constrain(object value);
    }
}
