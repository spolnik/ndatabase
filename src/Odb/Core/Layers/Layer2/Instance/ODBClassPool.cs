using System;
using System.Collections.Generic;
using NDatabase2.Tool.Wrappers.Map;

namespace NDatabase2.Odb.Core.Layers.Layer2.Instance
{
    /// <summary>
    ///   A simple class pool, to optimize instance creation
    /// </summary>
    public static class OdbClassPool
    {
        private static readonly IDictionary<string, Type> ClassMap = new OdbHashMap<string, Type>();
        private static readonly object Access = new object();

        public static void Reset()
        {
            ClassMap.Clear();
        }

        public static Type GetClass(string className)
        {
            lock (Access)
            {
                Type clazz;
                var success = ClassMap.TryGetValue(className, out clazz);
                if (success)
                    return clazz;

                try
                {
                    clazz = Type.GetType(className);
                }
                catch (Exception e)
                {
                    throw new OdbRuntimeException(NDatabaseError.ClassPoolCreateClass.AddParameter(className), e);
                }

                ClassMap[className] = clazz;
                return clazz;
            }
        }
    }
}
