using System;
using System.Collections.Generic;
using System.Reflection;
using NDatabase.Odb.Core;
using NDatabase.Odb.Core.Layers.Layer2.Instance;
using NDatabase.Tool.Wrappers.Map;

namespace NDatabase.Odb.Impl.Core.Layers.Layer2.Instance
{
    /// <summary>
    ///   A simple class pool, to optimize instance creation
    /// </summary>
    public sealed class OdbClassPool : IClassPool
    {
        private static readonly IDictionary<string, Type> ClassMap = new OdbHashMap<string, Type>();

        private static readonly IDictionary<string, ConstructorInfo> ConstrutorsMap =
            new OdbHashMap<string, ConstructorInfo>();

        #region IClassPool Members

        public void Reset()
        {
            ClassMap.Clear();
            ConstrutorsMap.Clear();
        }

        public Type GetClass(string className)
        {
            lock (this)
            {
                Type clazz;
                ClassMap.TryGetValue(className, out clazz);
                if (clazz == null)
                {
                    try
                    {
                        clazz = Type.GetType(className);
                    }
                    catch (Exception e)
                    {
                        throw new OdbRuntimeException(NDatabaseError.ClassPoolCreateClass.AddParameter(className), e);
                    }
                    ClassMap[className] = clazz;
                }
                return clazz;
            }
        }

        public ConstructorInfo GetConstrutor(string className)
        {
            ConstructorInfo constructorInfo;
            ConstrutorsMap.TryGetValue(className, out constructorInfo);
            return constructorInfo;
        }

        public void AddConstrutor(string className, ConstructorInfo constructor)
        {
            ConstrutorsMap[className] = constructor;
        }

        #endregion
    }
}
