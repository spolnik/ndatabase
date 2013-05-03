using NDatabase.Api.Query;
using NDatabase.Core.Layer3;
using NDatabase.Core.Query.Execution;
using NDatabase.Meta;

namespace NDatabase.Core.Query
{
    internal interface IQueryManager
    {
        int[] GetOrderByAttributeIds(ClassInfo classInfo, IInternalQuery query);

        /// <summary>
        ///   Returns a multi class query executor (polymorphic = true)
        /// </summary>
        IQueryExecutor GetQueryExecutor(IQuery query, IStorageEngine engine);
    }
}