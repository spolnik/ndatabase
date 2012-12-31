using System;

namespace NDatabase.Odb.Core.Layers.Layer1.Introspector
{
    /// <summary>
    /// Use when you don't want to serialize the field. 
    /// </summary>
    /// <remarks>
    /// In such case, mark the attribute with <code>[NonPersistent]</code>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class NonPersistentAttribute : Attribute
    {
    }
}