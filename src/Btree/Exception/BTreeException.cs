namespace NDatabase2.Btree.Exception
{
    public class BTreeException : System.Exception
    {
        public BTreeException()
        {
        }

        public BTreeException(string message, System.Exception cause) : base(message, cause)
        {
        }

        public BTreeException(string message) : base(message)
        {
        }
    }
}