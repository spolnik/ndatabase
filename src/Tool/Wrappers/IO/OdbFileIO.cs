using System.IO;
using NDatabase.Odb;
using NDatabase.Odb.Core;

namespace NDatabase.Tool.Wrappers.IO
{
    public class OdbFileIO : Odb.Core.Layers.Layer3.IO
    {
        private FileStream _fileAccess;

        public OdbFileIO(string wholeFileName, bool canWrite, string password)
        {
            Init(wholeFileName, canWrite, password);
        }

        #region IO Members

        public long Length()
        {
            return _fileAccess.Length;
        }

        public void Seek(long position)
        {
            try
            {
                if (position < 0)
                    throw new OdbRuntimeException(NDatabaseError.NegativePosition.AddParameter(position));

                _fileAccess.Seek(position, SeekOrigin.Begin);
            }
            catch (IOException e)
            {
                throw new OdbRuntimeException(
                    NDatabaseError.GoToPosition.AddParameter(position).AddParameter(_fileAccess.Length), e);
            }
        }

        public void Write(byte b)
        {
            _fileAccess.WriteByte(b);
        }

        public void Write(byte[] bs, int offset, int size)
        {
            _fileAccess.Write(bs, offset, size);
        }

        public int Read()
        {
            return _fileAccess.ReadByte();
        }

        public long Read(byte[] array, int offset, int size)
        {
            return _fileAccess.Read(array, offset, size);
        }

        public void Close()
        {
            _fileAccess.Close();
        }

        public bool LockFile()
        {
            _fileAccess.Lock(0, 1);
            return true;
        }

        public bool UnlockFile()
        {
            _fileAccess.Unlock(0, 1);
            return true;
        }

        public bool IsLocked()
        {
            return _fileAccess != null;
        }

        public void Init(string fileName, bool canWrite, string password)
        {
            try
            {
                _fileAccess = new FileStream(fileName, FileMode.OpenOrCreate, canWrite
                                                                                  ? FileAccess.ReadWrite
                                                                                  : FileAccess.Read);
            }
            catch (IOException e)
            {
                throw new OdbRuntimeException(NDatabaseError.FileNotFound.AddParameter(fileName), e);
            }
        }

        #endregion
    }
}
