using System;
using System.Collections.Generic;

namespace NDatabase2.Tool.Wrappers
{
    public static class OdbClassUtil
    {
        private static readonly Dictionary<string, string> CacheByFullClassName = new Dictionary<string, string>();
        private static readonly Dictionary<Type, string> CacheByType = new Dictionary<Type, string>();

        public static string GetClassName(string fullClassName)
        {
            string value;
            var success = CacheByFullClassName.TryGetValue(fullClassName, out value);

            if (success)
                return value;

            var index = fullClassName.LastIndexOf('.');

            var className = index == -1
                                ? fullClassName // primitive type
                                : GetClassName(fullClassName, index);
            
            CacheByFullClassName.Add(fullClassName, className);
            return className;
        }

        private static string GetClassName(string fullClassName, int index)
        {
            var startIndex = index + 1;
            return fullClassName.Substring(startIndex, fullClassName.Length - startIndex);
        }

        public static string GetNamespace(string fullClassName)
        {
            var index = fullClassName.LastIndexOf('.');
            return index == -1
                       ? string.Empty
                       : fullClassName.Substring(0, index);
        }

        public static String GetFullName<T>()
        {
            return GetFullName(typeof (T));
        }

        public static String GetFullName(Type type)
        {
            string value;
            var success = CacheByType.TryGetValue(type, out value);

            if (success)
                return value;
            
            var name = type.Assembly.GetName();
            var publicKey = name.GetPublicKey();
            var isSignedAsm = publicKey.Length > 0;

            var index = type.Assembly.FullName.IndexOf(',');

            var fullName = string.Format("{0},{1}", type.FullName, isSignedAsm
                                                                       ? type.Assembly.FullName
                                                                       : type.Assembly.FullName.Substring(0, index));
            CacheByType.Add(type, fullName);
            return fullName;
        }
    }
}
