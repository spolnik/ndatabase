using System;
using System.Reflection;

namespace NDatabase.Tool.Wrappers
{
    public static class OdbClassUtil
    {
        public static string GetClassName(string fullClassName)
        {
            var index = fullClassName.LastIndexOf('.');

            return index == -1
                       ? fullClassName // primitive type
                       : GetClassName(fullClassName, index);
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

        public static String GetFullName(Type type)
        {
            var name = type.Assembly.GetName();
            var publicKey = name.GetPublicKey();
            var isSignedAsm = publicKey.Length > 0;

            var index = type.Assembly.FullName.IndexOf(',');
            
            return string.Format("{0},{1}", type.FullName, isSignedAsm
                                                               ? type.Assembly.FullName
                                                               : type.Assembly.FullName.Substring(0, index));
        }
    }
}
