using NDatabase2.Odb.Core.Layers.Layer2.Meta;
using NDatabase2.Odb.Core.Layers.Layer3;
using NDatabase2.Odb.Core.Query.Criteria;
using NDatabase2.Odb.Core.Query.Execution;
using NDatabase2.Odb.Core.Query.Values;

namespace NDatabase2.Odb.Core.Query
{
    internal static class QueryManager
    {
        public static int[] GetOrderByAttributeIds(ClassInfo classInfo, IInternalQuery query)
        {
            var fieldNames = query.GetOrderByFieldNames();
            var fieldIds = new int[fieldNames.Count];

            for (var i = 0; i < fieldNames.Count; i++)
                fieldIds[i] = classInfo.GetAttributeId(fieldNames[i]);

            return fieldIds;
        }

        /// <summary>
        ///   Returns a multi class query executor (polymorphic = true)
        /// </summary>
        public static IQueryExecutor GetQueryExecutor(IQuery query, IStorageEngine engine)
        {
            if (query is ValuesCriteriaQuery)
                return new MultiClassGenericQueryExecutor(new ValuesCriteriaQueryExecutor(query, engine));

            if (query is CriteriaQuery)
                return new MultiClassGenericQueryExecutor(new CriteriaQueryExecutor(query, engine));

            throw new OdbRuntimeException(NDatabaseError.QueryTypeNotImplemented.AddParameter(query.GetType().FullName));
        }
    }
}
