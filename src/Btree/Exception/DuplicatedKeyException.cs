namespace NDatabase.Btree.Exception
{
    public sealed class DuplicatedKeyException : BTreeException
    {
        internal DuplicatedKeyException(string message) : base(message)
        {
        }
    }
}