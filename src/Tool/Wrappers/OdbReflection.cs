using System;
using System.Reflection;

namespace NDatabase.Tool.Wrappers
{
    public class OdbReflection
    {
        public static int GetArrayLength(object array)
        {
            var realArray = (Array) array;
            return realArray.GetLength(0);
        }

        public static Object GetArrayElement(object array, int index)
        {
            var realArray = (Array) array;
            return realArray.GetValue(index);
        }

        public static MethodInfo[] GetMethods(Type clazz)
        {
            return clazz.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
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