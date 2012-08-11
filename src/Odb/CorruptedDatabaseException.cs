using System;
using NDatabase.Odb.Core;

namespace NDatabase.Odb
{
    /// <summary>
    ///   An exception thrown by ODB when a corrupted block is found
    /// </summary>
    /// <author>olivier</author>
    [Serializable]
    public class CorruptedDatabaseException : OdbRuntimeException
    {
        public CorruptedDatabaseException(IError error, string message) : base(error, message)
        {
        }

        public CorruptedDatabaseException(IError error, Exception t) : base(error, t)
        {
        }

        public CorruptedDatabaseException(IError error) : base(error)
        {
        }
    }
}
