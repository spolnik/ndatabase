namespace NDatabase.Odb.Core.Layers.Layer3
{
    /// <summary>
    ///   The basic IO interface for basic IO operation like reading and writing bytes
    /// </summary>
    /// <author>olivier</author>
    public interface IO
    {
        void Init(string fileName, bool canWrite, string password);

        void Seek(long pos);

        void Close();

        void Write(byte b);

        void Write(byte[] bytes, int offset, int size);

        long Read(byte[] bytes, int offset, int size);

        int Read();

        long Length();

        bool LockFile();

        bool UnlockFile();

        bool IsLocked();
    }
}
