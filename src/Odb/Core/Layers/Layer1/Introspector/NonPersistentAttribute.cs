using System;

namespace NDatabase2.Odb.Core.Layers.Layer1.Introspector
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class NonPersistentAttribute : Attribute
    {
    }
}