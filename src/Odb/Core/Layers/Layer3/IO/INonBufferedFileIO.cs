using System;

namespace NDatabase.Odb.Core.Layers.Layer3.IO
{
    internal interface INonBufferedFileIO : IDisposable
    {
        long Length { get; }

        long CurrentPositionForDirectWrite { get; }

        void SetCurrentPosition(long currentPosition);

        void GoToPosition(long position);

        void WriteByte(byte b);

        byte[] ReadBytes(int size);

        byte ReadByte();

        void EnableAutomaticDelete(bool yesOrNo);

        void WriteBytes(byte[] bytes, int length);

        void Flush(long position, byte[] buffer, int bufferSizeToFlush);

        long Read(long position, byte[] buffer, int size);
    }
}
