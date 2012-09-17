using System;
using System.Collections.Generic;
using System.Reflection;
using NDatabase.Tool.Wrappers;
using NDatabase.Tool.Wrappers.Map;

namespace NDatabase.Odb.Core.Query.NQ
{
    public static class NativeQueryManager
    {
        private const string MatchMethodName = "Match";

        private static readonly IDictionary<IQuery, MethodInfo> MethodsCache = new OdbHashMap<IQuery, MethodInfo>();

        public static string GetClass(NativeQuery query)
        {
            return OdbClassUtil.GetFullName(query.GetObjectType());
        }

        public static string GetFullClassName(SimpleNativeQuery query)
        {
            var clazz = query.GetType();
            var methods = OdbReflection.GetMethods(clazz);
            
            foreach (var method in methods)
            {
                var attributes = OdbReflection.GetAttributeTypes(method);

                if (method.Name.Equals(MatchMethodName) && attributes.Length == 1)
                {
                    clazz = attributes[0];
                    MethodsCache.Add(query, method);
                    return OdbClassUtil.GetFullName(clazz);
                }
            }

            throw new OdbRuntimeException(
                NDatabaseError.QueryNqMatchMethodNotImplemented.AddParameter(query.GetType().FullName));
        }

        public static bool Match(NativeQuery query, object o)
        {
            return query.Match(o);
        }

        public static bool Match(SimpleNativeQuery query, object o)
        {
            var method = MethodsCache[query];
            var @params = new[] {o};
            object result;
            try
            {
                result = method.Invoke(query, @params);
            }
            catch (Exception e)
            {
                throw new OdbRuntimeException(
                    NDatabaseError.QueryNqExceptionRaisedByNativeQueryExecution.AddParameter(query.GetType().FullName), e);
            }
            return ((bool) result);
        }
    }
}
