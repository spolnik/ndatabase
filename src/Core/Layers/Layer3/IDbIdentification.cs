using NDatabase.Core.Layers.Layer3.IO;

namespace NDatabase.Core.Layers.Layer3
{
    /// <summary>
    ///   An interface to get info about database parameters
    /// </summary>
    internal interface IDbIdentification
    {
        string Id { get; }
        string Directory { get; }
        string FileName { get; }

        bool IsNew();
        void EnsureDirectories();

        IMultiBufferedFileIO GetIO(int bufferSize);

        IDbIdentification GetTransactionIdentification(long creationDateTime, string sessionId);
    }
}