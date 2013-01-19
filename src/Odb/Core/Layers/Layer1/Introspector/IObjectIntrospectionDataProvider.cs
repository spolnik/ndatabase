using System;
using NDatabase.Odb.Core.Layers.Layer2.Meta;

namespace NDatabase.Odb.Core.Layers.Layer1.Introspector
{
    internal interface IObjectIntrospectionDataProvider
    {
        ClassInfo GetClassInfo(Type type);
        void Clear();
        NonNativeObjectInfo EnrichWithOid(NonNativeObjectInfo nnoi, object o);
    }
}