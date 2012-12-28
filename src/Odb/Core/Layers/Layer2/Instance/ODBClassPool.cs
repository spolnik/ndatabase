using System;
using System.Collections.Concurrent;

namespace NDatabase.Odb.Core.Layers.Layer2.Instance
{
    /// <summary>
    ///   A simple class pool, to optimize instance creation
    /// </summary>
    public static class OdbClassPool
    {
        private static readonly ConcurrentDictionary<string, Type> ClassMap = new ConcurrentDictionary<string, Type>();

        public static void Reset()
        {
            ClassMap.Clear();
        }

        public static Type GetClass(string className)
        {
            return ClassMap.GetOrAdd(className, GetType);
        }

        private static Type GetType(string className)
        {
            try
            {
                return Type.GetType(className);
            }
            catch (Exception e)
            {
                throw new OdbRuntimeException(NDatabaseError.ClassPoolCreateClass.AddParameter(className), e);
            }
        }
    }
}
