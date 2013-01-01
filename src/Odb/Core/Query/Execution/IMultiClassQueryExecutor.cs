using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer3;

namespace NDatabase.Odb.Core.Query.Execution
{
    internal interface IMultiClassQueryExecutor : IQueryExecutor
    {
        void SetExecuteStartAndEndOfQueryAction(bool yes);

        IStorageEngine GetStorageEngine();

        IInternalQuery GetQuery();

        /// <summary>
        ///   The class on which to execute the query
        /// </summary>
        void SetClassInfo(ClassInfo ci);
    }
}
