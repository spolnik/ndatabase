using System;
using NDatabase.Odb.Core.Layers.Layer2.Instance;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Query.Criteria;
using NDatabase.Odb.Core.Query.Execution;
using NDatabase.Odb.Core.Query.NQ;
using NDatabase.Odb.Core.Query.Values;

namespace NDatabase.Odb.Core.Query
{
    internal static class QueryManager
    {
        public static bool Match(IQuery query, object @object)
        {
            var nativeQuery = query as NativeQuery;
            if (nativeQuery != null)
                return NativeQueryManager.Match(nativeQuery, @object);

            var simpleNativeQuery = query as SimpleNativeQuery;
            if (simpleNativeQuery != null)
                return NativeQueryManager.Match(simpleNativeQuery, @object);

            var criteriaQuery = query as CriteriaQuery;
            if (criteriaQuery != null)
                return criteriaQuery.Match((AbstractObjectInfo) @object);

            throw new OdbRuntimeException(NDatabaseError.QueryTypeNotImplemented.AddParameter(query.GetType().FullName));
        }

        public static Type GetUnderlyingType(IQuery query)
        {
            var nativeQuery = query as NativeQuery;
            if (nativeQuery != null)
                return nativeQuery.GetObjectType();

            var simpleNativeQuery = query as SimpleNativeQuery;
            if (simpleNativeQuery != null)
                return NativeQueryManager.GetUnderlyingType(simpleNativeQuery);

            var criteriaQuery = query as CriteriaQuery;
            if (criteriaQuery != null)
                return criteriaQuery.UnderlyingType;

            throw new OdbRuntimeException(NDatabaseError.QueryTypeNotImplemented.AddParameter(query.GetType().FullName));
        }

        public static bool NeedsInstanciation(IQuery query)
        {
            if (query is NativeQuery)
                return true;

            if (query is SimpleNativeQuery)
                return true;

            if (query is CriteriaQuery)
                return false;

            throw new OdbRuntimeException(NDatabaseError.QueryTypeNotImplemented.AddParameter(query.GetType().FullName));
        }

        public static bool IsCriteriaQuery(IQuery query)
        {
            return query is CriteriaQuery;
        }

        public static int[] GetOrderByAttributeIds(ClassInfo classInfo, IQuery query)
        {
            var fieldNames = query.GetOrderByFieldNames();
            var fieldIds = new int[fieldNames.Length];

            for (var i = 0; i < fieldNames.Length; i++)
                fieldIds[i] = classInfo.GetAttributeId(fieldNames[i]);

            return fieldIds;
        }

        /// <summary>
        ///   Returns a query executor according to the query type
        /// </summary>
        /// <param name="query"> </param>
        /// <param name="engine"> </param>
        /// <param name="instanceBuilder"> </param>
        /// <returns> </returns>
        public static IQueryExecutor GetQueryExecutor(IQuery query, IStorageEngine engine,
                                                      IInstanceBuilder instanceBuilder)
        {
            return GetMultiClassQueryExecutor(query, engine, instanceBuilder);
        }

        /// <summary>
        ///   Returns a multi class query executor (polymorphic = true)
        /// </summary>
        /// <param name="query"> </param>
        /// <param name="engine"> </param>
        /// <param name="instanceBuilder"> </param>
        /// <returns> </returns>
        private static IQueryExecutor GetMultiClassQueryExecutor(IQuery query, IStorageEngine engine,
                                                                   IInstanceBuilder instanceBuilder)
        {
            if (query is ValuesCriteriaQuery)
                return new MultiClassGenericQueryExecutor(new ValuesCriteriaQueryExecutor(query, engine));

            if (query is CriteriaQuery)
                return new MultiClassGenericQueryExecutor(new CriteriaQueryExecutor(query, engine));

            if (query is NativeQuery)
                return new MultiClassGenericQueryExecutor(new NativeQueryExecutor(query, engine, instanceBuilder));

            if (query is SimpleNativeQuery)
                return new MultiClassGenericQueryExecutor(new NativeQueryExecutor(query, engine, instanceBuilder));

            throw new OdbRuntimeException(NDatabaseError.QueryTypeNotImplemented.AddParameter(query.GetType().FullName));
        }
    }
}
