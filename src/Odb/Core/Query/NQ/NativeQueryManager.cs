using System;
using System.Collections.Generic;
using System.Reflection;
using NDatabase.Tool.Wrappers;
using NDatabase.Tool.Wrappers.Map;

namespace NDatabase.Odb.Core.Query.NQ
{
    internal static class NativeQueryManager
    {
        private const string MatchMethodName = "Match";

        private static readonly IDictionary<IQuery, MethodInfo> MethodsCache = new OdbHashMap<IQuery, MethodInfo>();

        public static string GetClass(NativeQuery query)
        {
            return OdbClassUtil.GetFullName(query.GetObjectType());
        }

        public static Type GetUnderlyingType(SimpleNativeQuery query)
        {
            var clazz = query.GetType();
            var methods = GetMethods(clazz);
            
            foreach (var method in methods)
            {
                var attributes = GetAttributeTypes(method);

                if (!method.Name.Equals(MatchMethodName) || attributes.Length != 1)
                    continue;

                clazz = attributes[0];
                MethodsCache.Add(query, method);
                return clazz;
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

        private static IEnumerable<MethodInfo> GetMethods(IReflect type)
        {
            return type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        private static Type[] GetAttributeTypes(MethodInfo method)
        {
            var parameterInfoList = method.GetParameters();
            var types = new Type[parameterInfoList.Length];

            for (var i = 0; i < parameterInfoList.Length; i++)
                types[i] = parameterInfoList[i].ParameterType;

            return types;
        }
    }
}
