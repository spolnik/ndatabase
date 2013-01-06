using System;

namespace NDatabase.Odb
{
    /// <summary>
    /// Object ID interface
    /// </summary>
    public interface OID : IComparable<OID>, IComparable
    {
        /// <summary>
        /// Underlying long number - oid
        /// </summary>
        long ObjectId { get; }
    }
}