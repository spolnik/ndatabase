using System;

namespace NDatabase.Odb.Core.Query.Linq
{
    public class LinqQueryException : OdbRuntimeException
    {
        internal LinqQueryException(string message)
            : base(NDatabaseError.BtreeError.AddParameter(message))
        {
        }

        internal LinqQueryException(string message, Exception cause)
            : base(NDatabaseError.BtreeError.AddParameter(message), cause)
        {
        }
    }
}