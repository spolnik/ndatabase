using System;
using NDatabase.Meta;

namespace NDatabase.Core.Introspector
{
    internal interface IObjectIntrospectionDataProvider
    {
        ClassInfo GetClassInfo(Type type);
        void Clear();
        NonNativeObjectInfo EnrichWithOid(NonNativeObjectInfo nnoi, object o);
    }
}