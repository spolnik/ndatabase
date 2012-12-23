using System;

namespace NDatabase2.Odb.Core.Query.Linq
{
    public class LinqQueryException : Exception
    {
        internal LinqQueryException()
        {
        }

        internal LinqQueryException(string message)
            : base(message)
        {
        }

        internal LinqQueryException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}