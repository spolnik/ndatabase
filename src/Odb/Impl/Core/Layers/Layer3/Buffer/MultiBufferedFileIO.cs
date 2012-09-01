using System;
using System.IO;
using System.Text;
using NDatabase.Odb.Core;
using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Layers.Layer3.IO;
using NDatabase.Tool;
using NDatabase.Tool.Wrappers;

namespace NDatabase.Odb.Impl.Core.Layers.Layer3.Buffer
{
    /// <summary>
    ///   Class allowing buffering for IO This class is used to give 
    ///   a transparent access to buffered file io
    /// </summary>
    public sealed class MultiBufferedFileIO : IMultiBufferedFileIO
    {
        private const int Read = 1;
        private const int Write = 2;

        public static readonly string LogId = "MultiBufferedIO";

        /// <summary>
        ///   The size of the buffer
        /// </summary>
        private readonly int _bufferSize;

        private readonly string _name;
        private readonly string _wholeFileName;

        private int _currentBufferIndex;

        private long _currentPositionForDirectWrite;
        private long _currentPositionWhenUsingBuffer;

        private bool _enableAutomaticDelete;
        private IOdbFileStream _fileWriter;

        /// <summary>
        ///   The length of the io device
        /// </summary>
        private long _ioDeviceLength;

        /// <summary>
        ///   A boolean value to check if read write are using buffer
        /// </summary>
        private bool _isUsingBuffer;

        private IMultiBuffer _multiBuffer;

        private int _nextBufferIndex;
        private int[] _overlappingBuffers;

        public MultiBufferedFileIO(string name, string fileName, int bufferSize)
        {
            _multiBuffer = new MultiBuffer(bufferSize);
            _bufferSize = bufferSize;
            _currentPositionWhenUsingBuffer = -1;
            _currentPositionForDirectWrite = -1;
            _overlappingBuffers = new int[MultiBuffer.NumberOfBuffers];

            NumberOfFlush = 0;

            _isUsingBuffer = true;
            _name = name;
            _enableAutomaticDelete = true;
            _nextBufferIndex = 0;
            _wholeFileName = fileName;

            try
            {
                if (OdbConfiguration.IsDebugEnabled(LogId))
                    DLogger.Info(string.Format("Opening datatbase file : {0}", Path.GetFullPath(_wholeFileName)));

                _fileWriter = new OdbFileStream(_wholeFileName);
                _ioDeviceLength = _fileWriter.Length;
            }
            catch (Exception e)
            {
                throw new OdbRuntimeException(NDatabaseError.InternalError, e);
            }
        }

        /// <summary>
        ///   Internal counter of flush
        /// </summary>
        public static long NumberOfFlush { get; private set; }

        public static long TotalFlushSize { get; private set; }

        public static int NbFlushForOverlap { get; private set; }

        public static int NbBufferOk { get; private set; }

        public static int NbBufferNotOk { get; private set; }

        #region IMultiBufferedFileIO Members

        public long Length
        {
            get { return _ioDeviceLength; }
        }

        public void SetUseBuffer(bool useBuffer)
        {
            // If we are using buffer, and the new useBuffer indicator if false
            // Then we need to flush all buffers
            if (_isUsingBuffer && !useBuffer)
                FlushAll();
            _isUsingBuffer = useBuffer;
        }

        public long CurrentPosition
        {
            get
            {
                if (!_isUsingBuffer)
                    return _currentPositionForDirectWrite;
                return _currentPositionWhenUsingBuffer;
            }
        }

        public void SetCurrentWritePosition(long currentPosition)
        {
            if (_isUsingBuffer)
            {
                if (_currentPositionWhenUsingBuffer == currentPosition)
                    return;
                _currentPositionWhenUsingBuffer = currentPosition;
            }
            else
            {
                //manageBufferForNewPosition(currentPosition, WRITE, 1);
                _currentPositionForDirectWrite = currentPosition;
                GoToPosition(currentPosition);
            }
        }

        public void SetCurrentReadPosition(long currentPosition)
        {
            if (_isUsingBuffer)
            {
                if (_currentPositionWhenUsingBuffer == currentPosition)
                    return;
                _currentPositionWhenUsingBuffer = currentPosition;
                ManageBufferForNewPosition(currentPosition, Read, 1);
            }
            else
            {
                _currentPositionForDirectWrite = currentPosition;
                GoToPosition(currentPosition);
            }
        }

        public void WriteByte(byte b)
        {
            if (!_isUsingBuffer)
            {
                GoToPosition(_currentPositionForDirectWrite);
                _fileWriter.Write(b);
                _currentPositionForDirectWrite++;
                return;
            }
            var bufferIndex = _multiBuffer.GetBufferIndexForPosition(_currentPositionWhenUsingBuffer, 1);
            if (bufferIndex == -1)
                bufferIndex = ManageBufferForNewPosition(_currentPositionWhenUsingBuffer, Write, 1);

            var positionInBuffer =
                (int) (_currentPositionWhenUsingBuffer - _multiBuffer.BufferPositions[bufferIndex].Start);

            _multiBuffer.SetByte(bufferIndex, positionInBuffer, b);

            _currentPositionWhenUsingBuffer++;
            if (_currentPositionWhenUsingBuffer > _ioDeviceLength)
                _ioDeviceLength = _currentPositionWhenUsingBuffer;
        }

        public byte[] ReadBytes(int size)
        {
            var bytes = new byte[size];
            if (!_isUsingBuffer)
            {
                // If there is no buffer, simply read data
                GoToPosition(_currentPositionForDirectWrite);
                var realSize = _fileWriter.Read(bytes, size);
                _currentPositionForDirectWrite += realSize;
                return bytes;
            }
            // If the size to read in smaller than the buffer size
            if (size <= _bufferSize)
                return ReadBytes(bytes, 0, size);
            // else the read have to use various buffers
            if (OdbConfiguration.IsDebugEnabled(LogId))
                DLogger.Debug(string.Format("Data is larger than buffer size {0} > {1} : cutting the data", bytes.Length,
                                            _bufferSize));

            var nbBuffersNeeded = bytes.Length / _bufferSize + 1;
            var currentStart = 0;
            var currentEnd = _bufferSize;
            for (var i = 0; i < nbBuffersNeeded; i++)
            {
                ReadBytes(bytes, currentStart, currentEnd);
                currentStart += _bufferSize;
                if (currentEnd + _bufferSize < bytes.Length)
                    currentEnd += _bufferSize;
                else
                    currentEnd = bytes.Length;
            }
            return bytes;
        }

        public byte ReadByte()
        {
            if (!_isUsingBuffer)
            {
                GoToPosition(_currentPositionForDirectWrite);
                var b = (byte) _fileWriter.Read();
                _currentPositionForDirectWrite++;
                return b;
            }
            var bufferIndex = ManageBufferForNewPosition(_currentPositionWhenUsingBuffer, Read, 1);
            var positionInBuffer =
                (int) (_currentPositionWhenUsingBuffer - _multiBuffer.BufferPositions[bufferIndex].Start);
            var result = _multiBuffer.Buffers[bufferIndex][positionInBuffer];
            _currentPositionWhenUsingBuffer++;
            return result;
        }

        public void WriteBytes(byte[] bytes)
        {
            if (bytes.Length > _bufferSize)
            {
                // throw new ODBRuntimeException(session,"The buffer has a size of "
                // + bufferSize + " but there exist data with " + bytes.length +
                // " size! - please set manually the odb data buffer to a greater value than "
                // + bytes.length +
                // " using Configuration.setDefaultBufferSizeForData(int)");
                if (OdbConfiguration.IsDebugEnabled(LogId))
                    DLogger.Debug(string.Format("Data is larger than buffer size {0} > {1} : cutting the data",
                                                bytes.Length, _bufferSize));

                var nbBuffersNeeded = bytes.Length / _bufferSize + 1;
                var currentStart = 0;
                var currentEnd = _bufferSize;
                for (var i = 0; i < nbBuffersNeeded; i++)
                {
                    WriteBytes(bytes, currentStart, currentEnd);
                    currentStart += _bufferSize;
                    if (currentEnd + _bufferSize < bytes.Length)
                        currentEnd += _bufferSize;
                    else
                        currentEnd = bytes.Length;
                }
            }
            else
                WriteBytes(bytes, 0, bytes.Length);
        }

        public void FlushAll()
        {
            for (var i = 0; i < MultiBuffer.NumberOfBuffers; i++)
                Flush(i);
        }

        public void Close()
        {
            Clear();
            CloseIO();
        }

        public void EnableAutomaticDelete(bool yesOrNo)
        {
            _enableAutomaticDelete = yesOrNo;
        }

        public void Dispose()
        {
            Close();
        }

        #endregion

        private void GoToPosition(long position)
        {
            _fileWriter.Seek(position);
        }

        private void CloseIO()
        {
            try
            {
                if (OdbConfiguration.IsDebugEnabled(LogId))
                    DLogger.Debug("Closing file with size " + _fileWriter.Length);

                _fileWriter.Close();
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
            if (!IsForTransaction() || !_enableAutomaticDelete)
                return;

            OdbFactory.Delete(_wholeFileName);
            if (File.Exists(_wholeFileName))
                throw new OdbRuntimeException(NDatabaseError.CanNotDeleteFile.AddParameter(_wholeFileName));
        }

        private int ManageBufferForNewPosition(long newPosition, int readOrWrite, int size)
        {
            var bufferIndex = _multiBuffer.GetBufferIndexForPosition(newPosition, size);
            if (bufferIndex != -1)
            {
                NbBufferOk++;
                return bufferIndex;
            }
            NbBufferNotOk++;
            // checks if there is any overlapping buffer
            _overlappingBuffers = GetOverlappingBuffers(newPosition, _bufferSize);
            // Choose the first overlaping buffer
            bufferIndex = _overlappingBuffers[0];
            if (MultiBuffer.NumberOfBuffers > 1 && _overlappingBuffers[1] != -1 && bufferIndex == _currentBufferIndex)
                bufferIndex = _overlappingBuffers[1];
            if (bufferIndex == -1)
            {
                bufferIndex = _nextBufferIndex;
                _nextBufferIndex = (_nextBufferIndex + 1) % MultiBuffer.NumberOfBuffers;
                if (bufferIndex == _currentBufferIndex)
                {
                    bufferIndex = _nextBufferIndex;
                    _nextBufferIndex = (_nextBufferIndex + 1) % MultiBuffer.NumberOfBuffers;
                }
                Flush(bufferIndex);
            }
            _currentBufferIndex = bufferIndex;

            var length = Length;
            if (readOrWrite == Read && newPosition >= length)
            {
                var message = string.Format("End Of File reached - position = {0} : Length = {1}", newPosition, length);
                DLogger.Error(message);
                throw new OdbRuntimeException(
                    NDatabaseError.EndOfFileReached.AddParameter(newPosition).AddParameter(length));
            }
            // The buffer must be initialized with real data, so the first thing we
            // must do is read data from file and puts it in the array
            long nread = _bufferSize;
            // if new position is in the file
            if (newPosition < length)
            {
                // We are in the file, we are updating content. to create the
                // buffer, we first read the content of the file
                GoToPosition(newPosition);
                // Actually loads data from the file to the buffer
                nread = _fileWriter.Read(_multiBuffer.Buffers[bufferIndex], _bufferSize);
                _multiBuffer.SetCreationDate(bufferIndex, OdbTime.GetCurrentTimeInTicks());
            }
            else
                GoToPosition(newPosition);
            // If we are in READ, sets the size equal to what has been read
            var endPosition = readOrWrite == Read
                                  ? newPosition + nread
                                  : newPosition + _bufferSize;

            _multiBuffer.SetPositions(bufferIndex, newPosition, endPosition);
            _currentPositionWhenUsingBuffer = newPosition;

            if (OdbConfiguration.IsDebugEnabled(LogId))
            {
                DLogger.Debug(string.Format("Creating buffer {0}-{1} : [{2},{3}]", _name, bufferIndex,
                                            _multiBuffer.BufferPositions[bufferIndex].Start,
                                            _multiBuffer.BufferPositions[bufferIndex].End));
            }
            return bufferIndex;
        }

        private void Flush(int bufferIndex)
        {
            var buffer = _multiBuffer.Buffers[bufferIndex];
            if (buffer != null && _multiBuffer.HasBeenUsedForWrite(bufferIndex))
            {
                GoToPosition(_multiBuffer.BufferPositions[bufferIndex].Start);
                // the +1 is because the maxPositionInBuffer is a position and the
                // parameter is a length
                var bufferSizeToFlush = _multiBuffer.MaxPositionInBuffer[bufferIndex] + 1;
                _fileWriter.Write(buffer, bufferSizeToFlush);
                NumberOfFlush++;
                TotalFlushSize += bufferSizeToFlush;
                if (OdbConfiguration.IsDebugEnabled(LogId))
                {
                    DLogger.Debug("Flushing buffer " + _name + "-" + bufferIndex + " : [" +
                                  _multiBuffer.BufferPositions[bufferIndex].Start + ":" +
                                  _multiBuffer.BufferPositions[bufferIndex].End + "] - flush size=" + bufferSizeToFlush +
                                  "  flush number = " + NumberOfFlush);
                }
                _multiBuffer.ClearBuffer(bufferIndex);
            }
            else
            {
                if (OdbConfiguration.IsDebugEnabled(LogId))
                {
                    DLogger.Debug("Flushing buffer " + _name + "-" + bufferIndex + " : [" +
                                  _multiBuffer.BufferPositions[bufferIndex].Start + ":" +
                                  _multiBuffer.BufferPositions[bufferIndex].End + "] - Nothing to flush!");
                }
                _multiBuffer.ClearBuffer(bufferIndex);
            }
        }

        private void Clear()
        {
            FlushAll();
            _multiBuffer.Clear();
            _multiBuffer = null;
            _overlappingBuffers = null;
        }

        private bool IsForTransaction()
        {
            return _name != null && _name.Equals("transaction");
        }

        /// <summary>
        ///   Check if a new buffer starting at position with a size ='size' would overlap with an existing buffer
        /// </summary>
        /// <param name="position"> </param>
        /// <param name="size"> </param>
        /// <returns> @ </returns>
        private int[] GetOverlappingBuffers(long position, int size)
        {
            var start1 = position;
            var end1 = position + size;
            var indexes = new int[MultiBuffer.NumberOfBuffers];
            var index = 0;
            for (var i = 0; i < MultiBuffer.NumberOfBuffers; i++)
            {
                var start2 = _multiBuffer.BufferPositions[i].Start;
                var end2 = _multiBuffer.BufferPositions[i].End;
                if ((start1 >= start2 && start1 < end2) || (start2 >= start1 && start2 < end1) ||
                    start2 <= start1 && end2 >= end1)
                {
                    // This buffer is overlapping the buffer
                    indexes[index++] = i;
                    // Flushes the buffer
                    Flush(i);
                    NbFlushForOverlap++;
                }
            }
            for (var i = index; i < MultiBuffer.NumberOfBuffers; i++)
                indexes[i] = -1;
            return indexes;
        }

        public byte[] ReadBytes(byte[] bytes, int startIndex, int endIndex)
        {
            var size = endIndex - startIndex;
            var bufferIndex = ManageBufferForNewPosition(_currentPositionWhenUsingBuffer, Read, size);
            var start = (int) (_currentPositionWhenUsingBuffer - _multiBuffer.BufferPositions[bufferIndex].Start);
            var buffer = _multiBuffer.Buffers[bufferIndex];
            Array.Copy(buffer, start, bytes, startIndex, size);
            _currentPositionWhenUsingBuffer += size;
            return bytes;
        }

        public void WriteBytes(byte[] bytes, int startIndex, int endIndex)
        {
            if (!_isUsingBuffer)
            {
                GoToPosition(_currentPositionForDirectWrite);
                _fileWriter.Write(bytes, bytes.Length);
                _currentPositionForDirectWrite += bytes.Length;
                return;
            }
            var lengthToCopy = endIndex - startIndex;

            var bufferIndex = ManageBufferForNewPosition(_currentPositionWhenUsingBuffer, Write, lengthToCopy);
            var positionInBuffer =
                (int) (_currentPositionWhenUsingBuffer - _multiBuffer.BufferPositions[bufferIndex].Start);
            // Here, the bytes.length seems to have an average value lesser that 70,
            // and in this
            // It is faster to copy using System.arraycopy
            // see org.neodatis.odb.test.performance.TestArrayCopy	
            _multiBuffer.WriteBytes(bufferIndex, bytes, startIndex, positionInBuffer, lengthToCopy);
            positionInBuffer = positionInBuffer + lengthToCopy - 1;
            _currentPositionWhenUsingBuffer += lengthToCopy;
            if (_currentPositionWhenUsingBuffer > _ioDeviceLength)
                _ioDeviceLength = _currentPositionWhenUsingBuffer;
        }

        public override string ToString()
        {
            var buffer = new StringBuilder();
            buffer.Append("Buffers=").Append("currbuffer=").Append(_currentBufferIndex).Append(" : \n");
            for (var i = 0; i < MultiBuffer.NumberOfBuffers; i++)
            {
                buffer.Append(i).Append(":[").Append(_multiBuffer.BufferPositions[i].Start).Append(",").Append(
                    _multiBuffer.BufferPositions[i].End).Append("] : write=").Append(_multiBuffer.HasBeenUsedForWrite(i))
                    .Append(" - when=").Append(_multiBuffer.GetCreationDate(i));
                if (i + 1 < MultiBuffer.NumberOfBuffers)
                    buffer.Append("\n");
            }
            return buffer.ToString();
        }
    }
}
