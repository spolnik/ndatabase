using System;

namespace NDatabase.Odb.Impl.Core.Layers.Layer1.Introspector
{
    [AttributeUsage(AttributeTargets.Field|AttributeTargets.Property)]
    public sealed class NonPersistentAttribute : Attribute
    {
    }
}