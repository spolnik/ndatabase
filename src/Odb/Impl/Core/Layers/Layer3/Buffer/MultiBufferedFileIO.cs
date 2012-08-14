using System;
using System.Globalization;
using System.IO;
using System.Threading;
using NDatabase.Odb.Core;
using NDatabase.Tool;
using NDatabase.Tool.Wrappers;
using NDatabase.Tool.Wrappers.IO;

namespace NDatabase.Odb.Impl.Core.Layers.Layer3.Buffer
{
    /// <summary>
    ///   A buffer manager that can manage more than one buffer.
    /// </summary>
    /// <remarks>
    ///   A buffer manager that can manage more than one buffer. Number of buffers can be configured using Configuration.setNbBuffers().
    /// </remarks>
    /// <author>osmadja</author>
    public class MultiBufferedFileIO : MultiBufferedIO
    {
        private const string MultiBufLogId = "MultiBufferedFileIO";

        private OdbFileIO _fileWriter;

        private string _wholeFileName;

        public MultiBufferedFileIO(int nbBuffers, string name, string fileName, bool canWrite, int bufferSize)
            : base(nbBuffers, name, bufferSize, canWrite)
        {
            Init(fileName, canWrite);
        }

        private void Init(string fileName, bool canWrite)
        {
            _wholeFileName = fileName;

            try
            {
                if (OdbConfiguration.IsDebugEnabled(MultiBufLogId))
                    DLogger.Info(string.Format("Opening datatbase file : {0}", Path.GetFullPath(_wholeFileName)));

                _fileWriter = BuildFileWriter(canWrite);
                SetIoDeviceLength(_fileWriter.Length());
            }
            catch (Exception e)
            {
                throw new OdbRuntimeException(NDatabaseError.InternalError, e);
            }
        }

        /// <exception cref="System.IO.IOException"></exception>
        protected virtual OdbFileIO BuildFileWriter(bool canWrite)
        {
            return new OdbFileIO(_wholeFileName, canWrite);
        }

        public override void GoToPosition(long position)
        {
            try
            {
                if (position < 0)
                    throw new OdbRuntimeException(NDatabaseError.NegativePosition.AddParameter(position));
                _fileWriter.Seek(position);
            }
            catch (IOException e)
            {
                long l = -1;
                try
                {
                    l = _fileWriter.Length();
                }
                catch (IOException)
                {
                }
                throw new OdbRuntimeException(NDatabaseError.GoToPosition.AddParameter(position).AddParameter(l), e);
            }
        }

        public override long GetLength()
        {
            return GetIoDeviceLength();
        }

        public override void InternalWrite(byte b)
        {
            try
            {
                _fileWriter.Write(b);
            }
            catch (IOException e)
            {
                throw new OdbRuntimeException(e, "Error while writing a byte");
            }
        }

        public override void InternalWrite(byte[] bs, int size)
        {
            try
            {
                _fileWriter.Write(bs, 0, size);
            }
            catch (IOException e)
            {
                throw new OdbRuntimeException(e, "Error while writing an array of byte");
            }
        }

        public override byte InternalRead()
        {
            try
            {
                var data = _fileWriter.Read();
                if (data == -1)
                    throw new IOException("Enf of file");

                return (byte) data;
            }
            catch (IOException e)
            {
                throw new OdbRuntimeException(e, "Error while reading a byte");
            }
        }

        public override long InternalRead(byte[] array, int size)
        {
            // FIXME raf.read only returns int not long
            try
            {
                return _fileWriter.Read(array, 0, size);
            }
            catch (IOException e)
            {
                throw new OdbRuntimeException(e, "Error while reading an array of byte");
            }
        }

        public override void CloseIO()
        {
            try
            {
                if (OdbConfiguration.IsDebugEnabled(MultiBufLogId))
                    DLogger.Debug("Closing file with size " + _fileWriter.Length());
                
                _fileWriter.Close();
            }
            catch (IOException e)
            {
                DLogger.Error(e.ToString());
            }
            _fileWriter = null;
            if (IsForTransaction() && AutomaticDeleteIsEnabled())
            {
                var b = OdbFile.DeleteFile(_wholeFileName);
                if (!b)
                    throw new OdbRuntimeException(NDatabaseError.CanNotDeleteFile.AddParameter(_wholeFileName));
            }
        }

        public override bool Delete()
        {
            return OdbFile.DeleteFile(_wholeFileName);
        }
    }
}
