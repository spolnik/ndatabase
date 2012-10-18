using NDatabase2.Odb;
using NDatabase2.Odb.Core;

namespace NDatabase2.Btree.Exception
{
    public class BTreeException : OdbRuntimeException
    {
        public BTreeException(string message, System.Exception cause)
            : base(NDatabaseError.BtreeError.AddParameter(message), cause)
        {
        }

        public BTreeException(string message)
            : base(NDatabaseError.BtreeError.AddParameter(message))
        {
        }
    }
}
