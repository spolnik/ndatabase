using System;

namespace NDatabase.Tool.Wrappers
{
    public static class OdbArray
    {
        public static void SetValue(object array, int index, object value)
        {
            ((Array) array).SetValue(value, index);
        }

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
    }
}