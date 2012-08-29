using System;
using System.IO;

namespace NDatabase.Odb.Core.Layers.Layer3.IO
{
    internal sealed class OdbFileStream : IOdbIO
    {
        private readonly FileStream _fileAccess;
        internal const int DefaultBufferSize = 4096*2;

        internal OdbFileStream(string wholeFileName)
        {
            try
            {
                _fileAccess = new FileStream(wholeFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read,
                                             DefaultBufferSize, FileOptions.RandomAccess);
                //TODO: _fileAccess.SetLength(1024 * 20);
            }
            catch (Exception e)
            {
                //TODO: handle IOException in other way ?
                throw new OdbRuntimeException(NDatabaseError.FileNotFoundOrItIsAlreadyUsed.AddParameter(wholeFileName), e);
            }
        }

        #region IO Members

        public long Length
        {
            get { return _fileAccess.Length; }
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
                long l = -1;
                try
                {
                    l = _fileAccess.Length;
                }
                catch (IOException)
                {
                }

                throw new OdbRuntimeException(NDatabaseError.GoToPosition.AddParameter(position).AddParameter(l), e);
            }
        }

        public void Write(byte b)
        {
            _fileAccess.WriteByte(b);
        }

        public void Write(byte[] buffer, int size)
        {
            _fileAccess.Write(buffer, 0, size);
        }

        public int Read()
        {
            return _fileAccess.ReadByte();
        }

        public long Read(byte[] buffer, int size)
        {
            return _fileAccess.Read(buffer, 0, size);
        }

        public void Close()
        {
            _fileAccess.Close();
        }

        #endregion

        public void Dispose()
        {
            Close();
        }
    }
}
