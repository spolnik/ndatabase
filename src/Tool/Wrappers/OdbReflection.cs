using System;
using System.Reflection;

namespace NDatabase.Tool.Wrappers
{
    public static class OdbReflection
    {
        public static MethodInfo[] GetMethods(Type type)
        {
            return type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        public static Type[] GetAttributeTypes(MethodInfo method)
        {
            var parameterInfoList = method.GetParameters();
            var types = new Type[parameterInfoList.Length];

            for (var i = 0; i < parameterInfoList.Length; i++)
                types[i] = parameterInfoList[i].ParameterType;

            return types;
        }
    }
}