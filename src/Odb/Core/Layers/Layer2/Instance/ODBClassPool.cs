using System;
using System.Collections.Generic;
using NDatabase.Tool;
using NDatabase.Exceptions;

namespace NDatabase.Odb.Core.Layers.Layer2.Instance
{
    /// <summary>
    ///   A simple class pool, to optimize instance creation
    /// </summary>
    internal static class OdbClassPool
    {
        private static readonly Dictionary<string, Type> ClassMap = new Dictionary<string, Type>();

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
