using System;
using System.Collections.Generic;
using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Query.Criteria;
using NDatabase.Odb.Core.Query.Execution;

namespace NDatabase.Odb.Core.Query
{
    internal interface IInternalQuery : IQuery
    {
        IQueryExecutionPlan GetExecutionPlan();
        void SetExecutionPlan(IQueryExecutionPlan plan);

        IStorageEngine GetStorageEngine();
        void SetStorageEngine(IStorageEngine storageEngine);

        void Add(IConstraint criterion);

        /// <summary>
        ///   To indicate if a query must be executed on a single object with the specific OID.
        /// </summary>
        /// <remarks>
        ///   To indicate if a query must be executed on a single object with the specific OID. Used for ValuesQeuries
        /// </remarks>
        /// <returns> </returns>
        bool IsForSingleOid();

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

        Type UnderlyingType { get; }
    }
}