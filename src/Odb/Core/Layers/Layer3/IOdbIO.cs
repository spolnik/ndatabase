using System;

namespace NDatabase.Odb.Core.Layers.Layer3
{
    /// <summary>
    ///   The basic IO interface for basic IO operation like reading and writing bytes
    /// </summary>
    public interface IOdbIO : IDisposable
    {
        void Seek(long pos);

        void Close();

        void Write(byte b);

        void Write(byte[] buffer, int size);

        long Read(byte[] buffer, int size);

        int Read();

        long Length { get; }
    }
}
