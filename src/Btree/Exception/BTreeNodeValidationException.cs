using NDatabase2.Odb;
using NDatabase2.Odb.Core;

namespace NDatabase2.Btree.Exception
{
    public sealed class BTreeNodeValidationException : OdbRuntimeException
    {
        public BTreeNodeValidationException(string message, System.Exception cause)
            : base(NDatabaseError.BtreeValidationError.AddParameter(message), cause)
        {
        }

        public BTreeNodeValidationException(string message)
            : base(NDatabaseError.BtreeValidationError.AddParameter(message))
        {
        }
    }
}