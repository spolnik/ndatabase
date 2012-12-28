using System;
using System.Collections.Generic;
using NDatabase.Odb.Core.Query.Execution;
using NDatabase.Odb.Core.Query.Values;
using NDatabase.Tool.Wrappers.List;

namespace NDatabase.Odb.Core.Query
{
    internal interface IInternalValuesQuery : IValuesQuery
    {
        IOdbList<string> GetAllInvolvedFields();
        IEnumerable<IQueryFieldAction> GetObjectActions();
    }

    public interface IValuesQuery : IQuery
    {
        IValuesQuery Count(string alias);

        IValuesQuery Sum(string fieldName);

        IValuesQuery Sum(string fieldName, string alias);

        IValuesQuery Avg(string fieldName, string alias);

        IValuesQuery Avg(string fieldName);

        IValuesQuery Max(string fieldName, string alias);

        IValuesQuery Max(string fieldName);

        IValuesQuery Field(string fieldName);

        IValuesQuery Field(string fieldName, string alias);

        IValuesQuery Sublist(string attributeName, string alias, int fromIndex, int size, bool throwException);

        IValuesQuery Sublist(string attributeName, int fromIndex, int size, bool throwException);

        IValuesQuery Sublist(string attributeName, string alias, int fromIndex, int toIndex);

        IValuesQuery Sublist(string attributeName, int fromIndex, int toIndex);

        IValuesQuery Size(string attributeName);

        IValuesQuery Size(string attributeName, string alias);

        IValuesQuery GroupBy(string fieldList);

        string[] GetGroupByFieldList();

        bool HasGroupBy();

        /// <summary>
        ///   To indicate if a query will return one row (for example, sum, average, max and min, or will return more than one row
        /// </summary>
        bool IsMultiRow();

        /// <returns> </returns>
        bool ReturnInstance();

        /// <summary>
        ///   To indicate if query execution must build instances or return object representation, Default value is true(return instance)
        /// </summary>
        void SetReturnInstance(bool returnInstance);

        int ObjectActionsCount { get; }
        Type UnderlyingType { get; }
        IValuesQuery Min(string attributeName);
        IValuesQuery Min(string attributeName, string alias);
        IValuesQuery Custom(string attributeName, ICustomQueryFieldAction action);
        IValuesQuery Custom(string attributeName, string alias, ICustomQueryFieldAction action);

        /// <summary>
        ///   Returns true if the query has an order by clause
        /// </summary>
        /// <returns> true if has an order by flag </returns>
        bool HasOrderBy();

        /// <summary>
        ///   Returns the field names of the order by
        /// </summary>
        /// <returns> The array of fields of the order by </returns>
        IList<string> GetOrderByFieldNames();

        /// <returns> the type of the order by - ORDER_BY_NONE,ORDER_BY_DESC,ORDER_BY_ASC </returns>
        OrderByConstants GetOrderByType();

        /// <summary>
        ///   used with isForSingleOid == true, to indicate we are working on a single object with a specific oid
        /// </summary>
        /// <returns> </returns>
        OID GetOidOfObjectToQuery();
    }
}
