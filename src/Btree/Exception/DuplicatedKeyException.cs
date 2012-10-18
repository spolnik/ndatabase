namespace NDatabase2.Btree.Exception
{
    public sealed class DuplicatedKeyException : BTreeException
    {
        public DuplicatedKeyException(string message) : base(message)
        {
        }
    }
}