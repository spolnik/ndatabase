using System;
using System.IO;

namespace NDatabase.Odb.Core.Layers.Layer3.IO
{
    internal sealed class OdbFileStream : IOdbFileStream
    {
        private const int DefaultBufferSize = 4096*2;

        private readonly FileStream _fileAccess;

        internal OdbFileStream(string wholeFileName)
        {
            try
            {
#if SILVERLIGHT
                _fileAccess = new FileStream(wholeFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read,
                                             DefaultBufferSize);
#else

                _fileAccess = new FileStream(wholeFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read,
                                             DefaultBufferSize, FileOptions.RandomAccess);
#endif
                //TODO: _fileAccess.SetLength(1024 * 20);
            }
            catch (IOException e)
            {
                throw new OdbRuntimeException(NDatabaseError.FileNotFoundOrItIsAlreadyUsed.AddParameter(wholeFileName), e);
            }
            catch(Exception ex)
            {
                throw new OdbRuntimeException(NDatabaseError.InternalError.AddParameter("Error during opening FileStream"), ex);
            }
        }

        #region IO Members

        /// <summary>
        /// Gets the length in bytes of the stream
        /// </summary>
        public long Length
        {
            get { return _fileAccess.Length; }
        }

        /// <summary>
        ///  Sets the current position of this stream to the given value
        /// </summary>
        /// <param name="position">offset</param>
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
            catch (Exception ex)
            {
                var parameter = string.Format("Error during seek operation, position: {0}", position);
                throw new OdbRuntimeException(NDatabaseError.InternalError.AddParameter(parameter), ex);
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
