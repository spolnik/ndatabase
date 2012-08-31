namespace NDatabase.Odb.Core.Layers.Layer3
{
    /// <summary>
    ///   The interface for buffered IO
    /// </summary>
    /// <author>osmadja</author>
    public interface IBufferedIO
    {
        long GetLength();

        void SetUseBuffer(bool useBuffer);

        long GetCurrentPosition();

        void SetCurrentWritePosition(long currentPosition);

        void SetCurrentReadPosition(long currentPosition);

        void WriteByte(byte b);

        byte[] ReadBytes(int size);

        byte ReadByte();

        void WriteBytes(byte[] bytes);

        void Flush(int bufferIndex);

        void FlushAll();

        long GetIoDeviceLength();

        void SetIoDeviceLength(long ioDeviceLength);

        void Close();

        void EnableAutomaticDelete(bool yesOrNo);
    }
}