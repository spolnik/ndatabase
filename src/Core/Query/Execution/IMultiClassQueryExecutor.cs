using NDatabase.Core.Layers.Layer2.Meta;
using NDatabase.Core.Layers.Layer3;

namespace NDatabase.Core.Query.Execution
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
