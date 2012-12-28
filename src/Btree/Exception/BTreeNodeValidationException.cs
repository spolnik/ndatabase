using NDatabase.Odb;
using NDatabase.Odb.Core;

namespace NDatabase.Btree.Exception
{
    public sealed class BTreeNodeValidationException : OdbRuntimeException
    {
        internal BTreeNodeValidationException(string message, System.Exception cause)
            : base(NDatabaseError.BtreeValidationError.AddParameter(message), cause)
        {
        }

        internal BTreeNodeValidationException(string message)
            : base(NDatabaseError.BtreeValidationError.AddParameter(message))
        {
        }
    }
}