using NDatabase2.Odb.Core.Layers.Layer2.Instance;
using NDatabase2.Odb.Core.Layers.Layer2.Meta;
using NDatabase2.Odb.Core.Layers.Layer3;
using NDatabase2.Odb.Core.Query.Criteria;
using NDatabase2.Odb.Core.Query.Execution;
using NDatabase2.Odb.Core.Query.Values;

namespace NDatabase2.Odb.Core.Query
{
    internal static class QueryManager
    {
        public static int[] GetOrderByAttributeIds(ClassInfo classInfo, IQuery query)
        {
            var fieldNames = query.GetOrderByFieldNames();
            var fieldIds = new int[fieldNames.Length];

            for (var i = 0; i < fieldNames.Length; i++)
                fieldIds[i] = classInfo.GetAttributeId(fieldNames[i]);

            return fieldIds;
        }

        /// <summary>
        ///   Returns a multi class query executor (polymorphic = true)
        /// </summary>
        public static IQueryExecutor GetQueryExecutor<T>(IQuery query, IStorageEngine engine) where T : class
        {
            if (query is ValuesCriteriaQuery<T>)
                return new MultiClassGenericQueryExecutor(new ValuesCriteriaQueryExecutor<T>(query, engine));

            if (query is CriteriaQuery<T>)
                return new MultiClassGenericQueryExecutor(new CriteriaQueryExecutor<T>(query, engine));

            throw new OdbRuntimeException(NDatabaseError.QueryTypeNotImplemented.AddParameter(query.GetType().FullName));
        }
    }
}
