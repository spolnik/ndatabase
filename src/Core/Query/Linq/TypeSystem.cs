using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NDatabase.Core.Query.Linq
{
    internal static class TypeSystem
    {
        internal static Type GetElementType(Type seqType)
        {
            var iEnumerable = FindIEnumerable(seqType);

            return iEnumerable == null
                       ? seqType
                       : iEnumerable.GenericTypeArguments[0];
        }

        private static Type FindIEnumerable(Type seqType)
        {
            if (seqType == null || seqType == typeof(string))
                return null;

            if (seqType.IsArray)
                return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());

            if (seqType.IsGenericParameter)
            {
                foreach (var typeArguments in seqType.GenericTypeArguments)
                {
                    var iEnumerable = typeof(IEnumerable<>).MakeGenericType(typeArguments);

                    if (iEnumerable.GetTypeInfo().IsAssignableFrom(seqType.GetTypeInfo()))
                        return iEnumerable;
                }
            }

            var interfaces = seqType.GetTypeInfo().ImplementedInterfaces.ToArray();

            if (interfaces.Length > 0)
            {
                foreach (var iEnumerable in interfaces.Select(FindIEnumerable).Where(iEnumerable => iEnumerable != null))
                    return iEnumerable;
            }

            return seqType.GetTypeInfo().BaseType != null && seqType.GetTypeInfo().BaseType != typeof(object)
                       ? FindIEnumerable(seqType.GetTypeInfo().BaseType)
                       : null;
        }
    }
}