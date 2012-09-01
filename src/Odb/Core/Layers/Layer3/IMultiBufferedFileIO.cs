using System;

namespace NDatabase.Odb.Core.Layers.Layer3
{
    /// <summary>
    ///   The interface for buffered IO
    /// </summary>
    public interface IMultiBufferedFileIO : IDisposable
    {
        long Length { get; }
        
        long CurrentPosition { get; }

        void SetCurrentWritePosition(long currentPosition);

        void SetCurrentReadPosition(long currentPosition);

        void SetUseBuffer(bool useBuffer);

        void WriteByte(byte b);

        byte ReadByte();

        void WriteBytes(byte[] bytes);

        byte[] ReadBytes(int size);

        void FlushAll();

        void Close();

        void EnableAutomaticDelete(bool yesOrNo);
    }
}