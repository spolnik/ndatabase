using System;

namespace NDatabase.Api
{

    /// <summary>
    /// Use when you want to enrich your class with OID.
    /// </summary>
    /// <remarks>
    /// You can apply it on fields of type: long or OID.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class EnrichWithOIDAttribute : Attribute
    {
    }
}