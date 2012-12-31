namespace NDatabase.Btree.Exception
{
    /// <summary>
    /// Exception raised when error in BTrees will appear (Duplicated key)
    /// </summary>
    public sealed class DuplicatedKeyException : BTreeException
    {
        internal DuplicatedKeyException(string message) : base(message)
        {
        }
    }
}