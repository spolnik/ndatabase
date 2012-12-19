using NDatabase2.Odb.Core.Layers.Layer3;
using NDatabase2.Odb.Core.Query.Criteria;
using NDatabase2.Odb.Core.Query.Execution;

namespace NDatabase2.Odb.Core.Query
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
    }
}