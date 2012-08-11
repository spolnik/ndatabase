using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer3;

namespace NDatabase.Odb.Core.Query.Execution
{
    public interface IMultiClassQueryExecutor : IQueryExecutor
    {
        /// <summary>
        ///   Used to indicate if the execute method must call start and end method of the queryResultAction.
        /// </summary>
        /// <remarks>
        ///   Used to indicate if the execute method must call start and end method of the queryResultAction. The default is yes. For MultiClass Query executor, it is set to false to avoid to reset the result
        /// </remarks>
        /// <returns> true or false to indicate if start and end method of queryResultAction must be executed </returns>
        bool ExecuteStartAndEndOfQueryAction();

        void SetExecuteStartAndEndOfQueryAction(bool yes);

        IStorageEngine GetStorageEngine();

        IQuery GetQuery();

        /// <summary>
        ///   The class on which to execute the query
        /// </summary>
        void SetClassInfo(ClassInfo ci);
    }
}
