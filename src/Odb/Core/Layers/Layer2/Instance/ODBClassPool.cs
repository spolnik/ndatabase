using System;
using System.Collections.Generic;
using NDatabase.Tool.Wrappers.Map;

namespace NDatabase.Odb.Core.Layers.Layer2.Instance
{
    /// <summary>
    ///   A simple class pool, to optimize instance creation
    /// </summary>
    public sealed class OdbClassPool : IClassPool
    {
        private static readonly IDictionary<string, Type> ClassMap = new OdbHashMap<string, Type>();

        #region IClassPool Members

        public void Reset()
        {
            ClassMap.Clear();
        }

        public Type GetClass(string className)
        {
            lock (this)
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

        #endregion
    }
}
