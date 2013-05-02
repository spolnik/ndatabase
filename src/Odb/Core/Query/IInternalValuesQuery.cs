using System.Collections.Generic;
using NDatabase.Api.Query;
using NDatabase.Odb.Core.Query.Execution;

namespace NDatabase.Odb.Core.Query
{
    internal interface IInternalValuesQuery : IValuesQuery
    {
        IEnumerable<IQueryFieldAction> GetObjectActions();
        string[] GetGroupByFieldList();

        bool HasGroupBy();

        /// <summary>
        ///   To indicate if a query will return one row (for example, sum, average, max and min, or will return more than one row
        /// </summary>
        bool IsMultiRow();

        int ObjectActionsCount { get; }

        bool ReturnInstance();

        /// <summary>
        ///   Returns true if the query has an order by clause
        /// </summary>
        /// <returns> true if has an order by flag </returns>
        bool HasOrderBy();
    }
}