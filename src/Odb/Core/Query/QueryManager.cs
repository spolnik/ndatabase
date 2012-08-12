using NDatabase.Odb.Core.Layers.Layer2.Instance;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Query.Execution;
using NDatabase.Odb.Core.Query.NQ;
using NDatabase.Odb.Impl.Core.Query.Criteria;
using NDatabase.Odb.Impl.Core.Query.NQ;
using NDatabase.Odb.Impl.Core.Query.Values;

namespace NDatabase.Odb.Core.Query
{
    public class QueryManager
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
                return CriteriaQueryManager.Match(criteriaQuery, @object);

            throw new OdbRuntimeException(NDatabaseError.QueryTypeNotImplemented.AddParameter(query.GetType().FullName));
        }

        public static string GetFullClassName(IQuery query)
        {
            var nativeQuery = query as NativeQuery;
            if (nativeQuery != null)
                return NativeQueryManager.GetClass(nativeQuery);

            var simpleNativeQuery = query as SimpleNativeQuery;
            if (simpleNativeQuery != null)
                return NativeQueryManager.GetFullClassName(simpleNativeQuery);

            if (typeof (CriteriaQuery) == query.GetType() || typeof (ValuesCriteriaQuery) == query.GetType())
                return CriteriaQueryManager.GetFullClassName((CriteriaQuery) query);

            throw new OdbRuntimeException(NDatabaseError.QueryTypeNotImplemented.AddParameter(query.GetType().FullName));
        }

        public static bool NeedsInstanciation(IQuery query)
        {
            if (query is NativeQuery)
                return true;

            if (query is SimpleNativeQuery)
                return true;

            if (typeof (CriteriaQuery) == query.GetType() || typeof (ValuesCriteriaQuery) == query.GetType())
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
            if (query.IsPolymorphic())
                return GetMultiClassQueryExecutor(query, engine, instanceBuilder);
            return GetSingleClassQueryExecutor(query, engine, instanceBuilder);
        }

        /// <summary>
        ///   Return a single class query executor (polymorphic = false)
        /// </summary>
        /// <param name="query"> </param>
        /// <param name="engine"> </param>
        /// <param name="instanceBuilder"> </param>
        /// <returns> </returns>
        protected static IQueryExecutor GetSingleClassQueryExecutor(IQuery query, IStorageEngine engine,
                                                                    IInstanceBuilder instanceBuilder)
        {
            if (query is ValuesCriteriaQuery)
                return new ValuesCriteriaQueryExecutor(query, engine);

            if (query is CriteriaQuery)
                return new CriteriaQueryExecutor(query, engine);

            if (query is NativeQuery)
                return new NativeQueryExecutor(query, engine, instanceBuilder);

            if (query is SimpleNativeQuery)
                return new NativeQueryExecutor(query, engine, instanceBuilder);

            throw new OdbRuntimeException(NDatabaseError.QueryTypeNotImplemented.AddParameter(query.GetType().FullName));
        }

        /// <summary>
        ///   Returns a multi class query executor (polymorphic = true)
        /// </summary>
        /// <param name="query"> </param>
        /// <param name="engine"> </param>
        /// <param name="instanceBuilder"> </param>
        /// <returns> </returns>
        protected static IQueryExecutor GetMultiClassQueryExecutor(IQuery query, IStorageEngine engine,
                                                                   IInstanceBuilder instanceBuilder)
        {
            if (typeof (CriteriaQuery) == query.GetType())
                return new MultiClassGenericQueryExecutor(new CriteriaQueryExecutor(query, engine));

            if (typeof (ValuesCriteriaQuery) == query.GetType())
                return new MultiClassGenericQueryExecutor(new ValuesCriteriaQueryExecutor(query, engine));

            if (query is NativeQuery)
                return new MultiClassGenericQueryExecutor(new NativeQueryExecutor(query, engine, instanceBuilder));

            if (query is SimpleNativeQuery)
                return new MultiClassGenericQueryExecutor(new NativeQueryExecutor(query, engine, instanceBuilder));

            throw new OdbRuntimeException(NDatabaseError.QueryTypeNotImplemented.AddParameter(query.GetType().FullName));
        }
    }
}
