using System;

namespace NDatabase2.Odb.Core.Layers.Layer3.IO
{
    /// <summary>
    ///   The basic IO interface for basic IO operation like reading and writing bytes
    /// </summary>
    public interface IOdbFileStream : IDisposable
    {
        /// <summary>
        ///  Sets the current position of this stream to the given value
        /// </summary>
        /// <param name="position">offset</param>
        void Seek(long position);

        void Write(byte value);

        void Write(byte[] buffer, int size);

        long Read(byte[] buffer, int size);

        int Read();

        /// <summary>
        /// Gets the length in bytes of the stream
        /// </summary>
        long Length { get; }
    }
}
