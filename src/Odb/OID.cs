using System;

namespace NDatabase.Odb
{
    public interface OID : IComparable
    {
        long ObjectId { get; }
    }
}