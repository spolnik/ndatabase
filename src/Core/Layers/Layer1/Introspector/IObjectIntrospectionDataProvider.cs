using System;
using NDatabase.Core.Layers.Layer2.Meta;

namespace NDatabase.Core.Layers.Layer1.Introspector
{
    internal interface IObjectIntrospectionDataProvider
    {
        ClassInfo GetClassInfo(Type type);
        void Clear();
        NonNativeObjectInfo EnrichWithOid(NonNativeObjectInfo nnoi, object o);
    }
}