using System;
using System.Collections.Generic;
using System.Reflection;
using NDatabase.Odb.Core;
using NDatabase.Tool;

namespace NDatabase.Odb.Impl.Core.Layers.Layer1.Introspector
{
    internal static class SilverlightClassIntrospector
    {
        private static readonly Dictionary<Type, object> NullValues = new Dictionary<Type, object>();
        private const string LogId = "ClassIntrospector";

        static SilverlightClassIntrospector()
        {
            NullValues.Add(typeof(int), 0);
            NullValues.Add(typeof(uint), (uint)0);
            NullValues.Add(typeof(byte), (byte)0);
    	    NullValues.Add(typeof(short), (short)0);
    	    NullValues.Add(typeof(float), (float)0);
    	    NullValues.Add(typeof(double), (double)0);
            NullValues.Add(typeof(ulong), (ulong)0);
            NullValues.Add(typeof(long), (long)0);
    	    NullValues.Add(typeof(bool), false);
            NullValues.Add(typeof(char), (char)0);
            NullValues.Add(typeof(sbyte), (sbyte)0);
            NullValues.Add(typeof(decimal), (decimal)0);
            NullValues.Add(typeof(ushort), (ushort)0);
            NullValues.Add(typeof(DateTime), DateTime.MinValue);
            NullValues.Add(typeof(string), "dummy");
        }

        internal static object NewInstanceOf(Type clazz)
        {
            try
            {
                return Activator.CreateInstance(clazz);
            }
            catch
            {
                // else take the constructer with the smaller number of parameters
                // and call it will null values
                // @TODO Put this info in cache !
                if (OdbConfiguration.IsDebugEnabled(LogId))
                {
                    DLogger.Debug(
                        string.Format(
                            "{0} does not have default constructor! using a 'with parameter' constructor will null values",
                            clazz));
                }

                var constructors =
                    clazz.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public |
                                          BindingFlags.DeclaredOnly);

                if (clazz.IsAbstract || clazz.IsInterface)
                {
                    throw new OdbRuntimeException(
                        NDatabaseError.InternalError.AddParameter(
                            "Cannot create an instance of abstract class or interface"));
                }

                const int numberOfParameters = 1000;
                var bestConstructorIndex = 0;
                for (var i = 0; i < constructors.Length; i++)
                {
                    if (constructors[i].GetParameters().Length < numberOfParameters)
                        bestConstructorIndex = i;
                }
                
                var constructor = constructors[bestConstructorIndex];
                
                var nulls = new Object[constructor.GetParameters().Length];
                
                for (var i = 0; i < nulls.Length; i++)
                {
                    var parameterType = constructor.GetParameters()[i].ParameterType;

                    object val;
                    NullValues.TryGetValue(parameterType, out val);
                    nulls[i] = val;

                }
                Object objectRenamed;

                try
                {
                    objectRenamed = constructor.Invoke(nulls);
                }
                catch (Exception e2)
                {
                    throw new OdbRuntimeException(
                        NDatabaseError.InternalError.AddParameter(
                            string.Format("Cannot invoke constructor with default parameters for type{0}",
                                          clazz.FullName)), e2);
                }
                return objectRenamed;
            }
        }
    }
}
