using System;
using System.IO;
using System.Threading.Tasks;
using NDatabase.Exceptions;
using Windows.Storage;

namespace NDatabase.Odb.Core.Layers.Layer3.IO
{
    internal static class OdbStore
    {
        internal static bool FileExists(string fileName)
        {
            return DoesFileExistAsync(fileName);
        }

        private static bool DoesFileExistAsync(string fileName)
        {
            try
            {
                var storageFolder = ApplicationData.Current.LocalFolder;
                storageFolder.GetFileAsync(fileName).GetResults();
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal static void DeleteFile(string fileName)
        {
            var storageFolder = ApplicationData.Current.LocalFolder;
            var deleteTask = storageFolder.DeleteAsync();
            deleteTask.AsTask().RunSynchronously();
        }
    }

    internal sealed class OdbFileStream : IOdbStream
    {
        private readonly string _wholeFileName;
        
        private long _position;

        internal OdbFileStream(string wholeFileName)
        {
            _wholeFileName = wholeFileName;
        }

        private async Task<StorageFile> OpenFile()
        {
            try
            {
                var storageFolder = ApplicationData.Current.LocalFolder;
                return await storageFolder.CreateFileAsync(_wholeFileName, CreationCollisionOption.OpenIfExists);
            }
            catch (IOException e)
            {
                throw new OdbRuntimeException(NDatabaseError.FileNotFoundOrItIsAlreadyUsed.AddParameter(_wholeFileName),
                                              e);
            }
            catch (Exception ex)
            {
                throw new OdbRuntimeException(
                    NDatabaseError.InternalError.AddParameter("Error during opening FileStream"), ex);
            }
        }

        #region IO Members

        /// <summary>
        ///     Gets the length in bytes of the stream
        /// </summary>
        public long Length
        {
            get
            {
                var file = OpenFile();
                using (var readStream = file.Result.OpenStreamForReadAsync().Result)
                {
                    return readStream.Length;
                }
            }
        }

        /// <summary>
        ///     Sets the current position of this stream to the given value
        /// </summary>
        /// <param name="position">offset</param>
        public void SetPosition(long position)
        {
            if (position < 0)
                throw new OdbRuntimeException(NDatabaseError.NegativePosition.AddParameter(position));

            _position = position;
        }

        public void Write(byte b)
        {
            try
            {
                var file = OpenFile();
                using (var writeStream = file.Result.OpenStreamForWriteAsync().Result)
                {
                    Seek(writeStream, _position);
                    writeStream.WriteByte(b);
                    _position = writeStream.Position;
                }
            }
            catch (IOException e)
            {
                throw new OdbRuntimeException(e, "Error while writing a byte");
            }
        }

        public void Write(byte[] buffer, int size)
        {
            try
            {
                var file = OpenFile();
                using (var writeStream = file.Result.OpenStreamForWriteAsync().Result)
                {
                    Seek(writeStream, _position);
                    writeStream.Write(buffer, 0, size);
                    _position = writeStream.Position;
                }
            }
            catch (IOException e)
            {
                throw new OdbRuntimeException(e, "Error while writing an array of byte");
            }
        }

        public int Read()
        {
            try
            {
                int data;
                var file = OpenFile();
                using (var readStream = file.Result.OpenStreamForReadAsync().Result)
                {
                    Seek(readStream, _position);
                    data = readStream.ReadByte();
                    if (data == -1)
                        throw new IOException("End of file");

                    _position = readStream.Position;
                }

                return (byte)data;
            }
            catch (IOException e)
            {
                throw new OdbRuntimeException(e, "Error while reading a byte");
            }
        }

        public int Read(byte[] buffer, int size)
        {
            try
            {
                int read;
                var file = OpenFile();
                using (var readStream = file.Result.OpenStreamForReadAsync().Result)
                {
                    Seek(readStream, _position);
                    read = readStream.Read(buffer, 0, size);
                    _position = readStream.Position;
                }
                return read;
            }
            catch (IOException e)
            {
                throw new OdbRuntimeException(e, "Error while reading an array of byte");
            }
        }

        private static void Seek(Stream writeStream, long position)
        {
            try
            {
                if (position < 0)
                    throw new OdbRuntimeException(NDatabaseError.NegativePosition.AddParameter(position));

                writeStream.Seek(position, SeekOrigin.Begin);
            }
            catch (IOException e)
            {
                long l = -1;
                try
                {
                    l = writeStream.Length;
                }
                catch (IOException)
                {
                }

                throw new OdbRuntimeException(NDatabaseError.GoToPosition.AddParameter(position).AddParameter(l), e);
            }
            catch (Exception ex)
            {
                var parameter = string.Concat("Error during seek operation, position: ", position.ToString());
                throw new OdbRuntimeException(NDatabaseError.InternalError.AddParameter(parameter), ex);
            }
        }

        #endregion

        public void Dispose()
        {
        }
    }
}