using System;

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
            var index = type.Assembly.FullName.IndexOf(',');
            return string.Format("{0},{1}", type.FullName, type.Assembly.FullName.Substring(0, index));
        }
    }
}
