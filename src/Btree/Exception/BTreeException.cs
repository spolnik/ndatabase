using NDatabase.Odb;
using NDatabase.Odb.Core;

namespace NDatabase.Btree.Exception
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
