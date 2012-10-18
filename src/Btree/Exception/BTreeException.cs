using NDatabase2.Odb;
using NDatabase2.Odb.Core;

namespace NDatabase2.Btree.Exception
{
    public class BTreeException : OdbRuntimeException
    {
        internal BTreeException(string message, System.Exception cause)
            : base(NDatabaseError.BtreeError.AddParameter(message), cause)
        {
        }

        internal BTreeException(string message)
            : base(NDatabaseError.BtreeError.AddParameter(message))
        {
        }
    }
}
