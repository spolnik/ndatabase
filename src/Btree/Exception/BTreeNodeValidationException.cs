using NDatabase2.Odb;
using NDatabase2.Odb.Core;

namespace NDatabase2.Btree.Exception
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