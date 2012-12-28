using System;

namespace NDatabase.Odb
{
    public interface OID : IComparable<OID>, IComparable
    {
        long ObjectId { get; }
    }
}