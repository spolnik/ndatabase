using System;

namespace NDatabase2.Odb
{
    public interface OID : IComparable<OID>, IComparable
    {
        long ObjectId { get; }
    }
}