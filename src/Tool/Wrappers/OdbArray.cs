using System;

namespace NDatabase.Tool.Wrappers
{
    internal static class OdbArray
    {
        internal static void SetValue(object array, int index, object value)
        {
            ((Array) array).SetValue(value, index);
        }

        internal static int GetArrayLength(object array)
        {
            var realArray = (Array) array;
            return realArray.GetLength(0);
        }

        internal static Object GetArrayElement(object array, int index)
        {
            var realArray = (Array) array;
            return realArray.GetValue(index);
        }
    }
}