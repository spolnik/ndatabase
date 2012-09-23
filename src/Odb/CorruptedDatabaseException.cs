using NDatabase.Odb.Core;

namespace NDatabase.Odb
{
    /// <summary>
    ///   An exception thrown by ODB when a corrupted block is found
    /// </summary>
    internal sealed class CorruptedDatabaseException : OdbRuntimeException
    {
        internal CorruptedDatabaseException(IError error) : base(error)
        {
        }
    }
}
