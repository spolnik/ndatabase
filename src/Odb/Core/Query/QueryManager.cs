using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Query.Criteria;
using NDatabase.Odb.Core.Query.Execution;
using NDatabase.Odb.Core.Query.Values;
using NDatabase2.Odb;
using NDatabase2.Odb.Core;
using NDatabase2.Odb.Core.Layers.Layer3;

namespace NDatabase.Odb.Core.Query
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

            if (query is SodaQuery)
                return new MultiClassGenericQueryExecutor(new CriteriaQueryExecutor(query, engine));

            throw new OdbRuntimeException(NDatabaseError.QueryTypeNotImplemented.AddParameter(query.GetType().FullName));
        }
    }
}
