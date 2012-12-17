using NDatabase2.Odb.Core.Layers.Layer3;
using NDatabase2.Odb.Core.Query.Criteria;
using NDatabase2.Odb.Core.Query.Execution;

namespace NDatabase2.Odb.Core.Query
{
    internal interface IInternalQuery
    {
        IQueryExecutionPlan GetExecutionPlan();
        void SetExecutionPlan(IQueryExecutionPlan plan);

        IStorageEngine GetStorageEngine();
        void SetStorageEngine(IStorageEngine storageEngine);

        void Join(IConstraint criterion);
    }
}