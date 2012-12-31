using System.IO;
using NDatabase.Tool;

namespace NDatabase.Odb.Core.Layers.Layer3.IO
{
    internal sealed class NonBufferedFileIO : INonBufferedFileIO
    {
        private readonly string _wholeFileName;

        private bool _enableAutomaticDelete;
        private IOdbFileStream _fileWriter;

        internal NonBufferedFileIO(string fileName)
        {
            CurrentPositionForDirectWrite = -1;

            _wholeFileName = fileName;
            _fileWriter = new OdbFileStream(_wholeFileName);
        }

        #region INonBufferedFileIO Members

        public long Length
        {
            get { return _fileWriter.Length; }
        }

        /// <summary>
        ///   Current position for direct write to IO
        /// </summary>
        public long CurrentPositionForDirectWrite { get; private set; }

        public void Dispose()
        {
            CloseIO();
        }

        public void SetCurrentPosition(long currentPosition)
        {
            CurrentPositionForDirectWrite = currentPosition;
            GoToPosition(currentPosition);
        }

        private void GoToPosition(long position)
        {
            _fileWriter.SetPosition(position);
        }

        public void WriteByte(byte b)
        {
            GoToPosition(CurrentPositionForDirectWrite);
            _fileWriter.Write(b);
            CurrentPositionForDirectWrite++;
        }

        public byte[] ReadBytes(int size)
        {
            GoToPosition(CurrentPositionForDirectWrite);

            var bytes = new byte[size];
            var realSize = _fileWriter.Read(bytes, size);

            CurrentPositionForDirectWrite += realSize;
            return bytes;
        }

        public byte ReadByte()
        {
            GoToPosition(CurrentPositionForDirectWrite);

            var b = (byte) _fileWriter.Read();
            CurrentPositionForDirectWrite++;

            return b;
        }

        public void EnableAutomaticDelete(bool yesOrNo)
        {
            _enableAutomaticDelete = yesOrNo;
        }

        public void WriteBytes(byte[] bytes, int length)
        {
            GoToPosition(CurrentPositionForDirectWrite);
            _fileWriter.Write(bytes, length);
            CurrentPositionForDirectWrite += length;
        }

        public long Read(long position, byte[] buffer, int size)
        {
            GoToPosition(position);
            return _fileWriter.Read(buffer, size);
        }

        #endregion

        private void CloseIO()
        {
            try
            {
                if (OdbConfiguration.IsLoggingEnabled())
                {
                    var length = _fileWriter.Length.ToString();
                    DLogger.Debug("Closing file with size " + length);
                }

                _fileWriter.Dispose();
            }
            catch (IOException e)
            {
                DLogger.Error(e.ToString());
            }

            _fileWriter = null;
            AutoDelete();
        }

        private void AutoDelete()
        {
            if (!_enableAutomaticDelete)
                return;

            OdbFactory.Delete(_wholeFileName);
        }
    }
}
