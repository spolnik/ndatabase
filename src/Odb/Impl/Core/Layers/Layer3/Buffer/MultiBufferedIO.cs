using System;
using System.Text;
using NDatabase.Odb.Core;
using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Tool;
using NDatabase.Tool.Wrappers;
using NDatabase.Tool.Wrappers.IO;

namespace NDatabase.Odb.Impl.Core.Layers.Layer3.Buffer
{
    /// <summary>
    ///   Abstract class allowing buffering for IO This class is used to give a transparent access to buffered io : File, socket The DefaultFileIO and DefaultSocketIO inherits from AbstractIO
    /// </summary>
    /// <author>olivier s</author>
    public abstract class MultiBufferedIO : IBufferedIO
    {
        private const int Read = 1;

        private const int Write = 2;

        public static readonly string LogId = "MultiBufferedIO";

        /// <summary>
        ///   The size of the buffer
        /// </summary>
        private readonly int _bufferSize;

        private readonly string _name;

        private readonly int _nbBuffers;

        private int _currentBufferIndex;

        private long _currentPositionForDirectWrite;
        private long _currentPositionWhenUsingBuffer;

        private bool _enableAutomaticDelete;

        /// <summary>
        ///   The length of the io device
        /// </summary>
        private long _ioDeviceLength;

        /// <summary>
        ///   A boolean value to check if read write are using buffer
        /// </summary>
        private bool _isUsingBuffer;

        private MultiBufferVO _multiBuffer;

        private int _nextBufferIndex;
        private int[] _overlappingBuffers;

        protected MultiBufferedIO(int nbBuffers, string name, int bufferSize, bool canWrite)
        {
            _nbBuffers = nbBuffers;
            _multiBuffer = new MultiBufferVO(nbBuffers, bufferSize);
            _bufferSize = bufferSize;
            _currentPositionWhenUsingBuffer = -1;
            _currentPositionForDirectWrite = -1;
            _overlappingBuffers = new int[nbBuffers];
            NumberOfFlush = 0;
            _isUsingBuffer = true;
            _name = name;
            _enableAutomaticDelete = true;
            _nextBufferIndex = 0;
        }

        /// <summary>
        ///   Internal counter of flush
        /// </summary>
        public static long NumberOfFlush { get; set; }

        public static long TotalFlushSize { get; set; }

        public static int NbFlushForOverlap { get; set; }

        public static int NbBufferOk { get; set; }

        public static int NbBufferNotOk { get; set; }

        #region IBufferedIO Members

        public abstract void GoToPosition(long position);

        public abstract long GetLength();

        public virtual int ManageBufferForNewPosition(long newPosition, int readOrWrite, int size)
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
            if (_nbBuffers > 1 && _overlappingBuffers[1] != -1 && bufferIndex == _currentBufferIndex)
                bufferIndex = _overlappingBuffers[1];
            if (bufferIndex == -1)
            {
                bufferIndex = _nextBufferIndex;
                _nextBufferIndex = (_nextBufferIndex + 1) % _nbBuffers;
                if (bufferIndex == _currentBufferIndex)
                {
                    bufferIndex = _nextBufferIndex;
                    _nextBufferIndex = (_nextBufferIndex + 1) % _nbBuffers;
                }
                Flush(bufferIndex);
            }
            _currentBufferIndex = bufferIndex;
            
            var length = GetLength();
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
                nread = InternalRead(_multiBuffer.Buffers[bufferIndex], _bufferSize);
                _multiBuffer.SetCreationDate(bufferIndex, OdbTime.GetCurrentTimeInTicks());
            }
            else
                GoToPosition(newPosition);
            // If we are in READ, sets the size equal to what has been read
            var endPosition = readOrWrite == Read
                                  ? newPosition + nread
                                  : newPosition + _bufferSize;

            _multiBuffer.SetPositions(bufferIndex, newPosition, endPosition, 0);
            _currentPositionWhenUsingBuffer = newPosition;

            if (OdbConfiguration.IsDebugEnabled(LogId))
            {
                DLogger.Debug(string.Format("Creating buffer {0}-{1} : [{2},{3}]", _name, bufferIndex,
                                            _multiBuffer.BufferStartPosition[bufferIndex],
                                            _multiBuffer.BufferEndPosition[bufferIndex]));
            }
            return bufferIndex;
        }

        public virtual bool IsUsingbuffer()
        {
            return _isUsingBuffer;
        }

        public virtual void SetUseBuffer(bool useBuffer)
        {
            // If we are using buffer, and the new useBuffer indicator if false
            // Then we need to flush all buffers
            if (_isUsingBuffer && !useBuffer)
                FlushAll();
            _isUsingBuffer = useBuffer;
        }

        public virtual long GetCurrentPosition()
        {
            if (!_isUsingBuffer)
                return _currentPositionForDirectWrite;
            return _currentPositionWhenUsingBuffer;
        }

        public virtual void SetCurrentWritePosition(long currentPosition)
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

        public virtual void SetCurrentReadPosition(long currentPosition)
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

        public virtual void WriteByte(byte b)
        {
            if (!_isUsingBuffer)
            {
                GoToPosition(_currentPositionForDirectWrite);
                InternalWrite(b);
                _currentPositionForDirectWrite++;
                return;
            }
            var bufferIndex = _multiBuffer.GetBufferIndexForPosition(_currentPositionWhenUsingBuffer, 1);
            if (bufferIndex == -1)
                bufferIndex = ManageBufferForNewPosition(_currentPositionWhenUsingBuffer, Write, 1);

            var positionInBuffer =
                (int) (_currentPositionWhenUsingBuffer - _multiBuffer.BufferStartPosition[bufferIndex]);

            _multiBuffer.SetByte(bufferIndex, positionInBuffer, b);
            _currentPositionWhenUsingBuffer++;
            if (_currentPositionWhenUsingBuffer > _ioDeviceLength)
                _ioDeviceLength = _currentPositionWhenUsingBuffer;
        }

        public virtual byte[] ReadBytesOld(int size)
        {
            var bytes = new byte[size];
            for (var i = 0; i < size; i++)
                bytes[i] = ReadByte();
            return bytes;
        }

        public virtual byte[] ReadBytes(int size)
        {
            var bytes = new byte[size];
            if (!_isUsingBuffer)
            {
                // If there is no buffer, simply read data
                GoToPosition(_currentPositionForDirectWrite);
                var realSize = InternalRead(bytes, size);
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

        public virtual byte ReadByte()
        {
            if (!_isUsingBuffer)
            {
                GoToPosition(_currentPositionForDirectWrite);
                var b = InternalRead();
                _currentPositionForDirectWrite++;
                return b;
            }
            var bufferIndex = ManageBufferForNewPosition(_currentPositionWhenUsingBuffer, Read, 1);
            var byt = _multiBuffer.GetByte(bufferIndex,
                                            (int)
                                            (_currentPositionWhenUsingBuffer -
                                             _multiBuffer.BufferStartPosition[bufferIndex]));
            _currentPositionWhenUsingBuffer++;
            return byt;
        }

        public virtual void WriteBytes(byte[] bytes)
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

        public virtual void FlushAll()
        {
            for (var i = 0; i < _nbBuffers; i++)
                Flush(i);
        }

        public virtual void Flush(int bufferIndex)
        {
            var buffer = _multiBuffer.Buffers[bufferIndex];
            if (buffer != null && _multiBuffer.HasBeenUsedForWrite(bufferIndex))
            {
                GoToPosition(_multiBuffer.BufferStartPosition[bufferIndex]);
                // the +1 is because the maxPositionInBuffer is a position and the
                // parameter is a length
                var bufferSizeToFlush = _multiBuffer.MaxPositionInBuffer[bufferIndex] + 1;
                InternalWrite(buffer, bufferSizeToFlush);
                NumberOfFlush++;
                TotalFlushSize += bufferSizeToFlush;
                if (OdbConfiguration.IsDebugEnabled(LogId))
                {
                    DLogger.Debug("Flushing buffer " + _name + "-" + bufferIndex + " : [" +
                                  _multiBuffer.BufferStartPosition[bufferIndex] + ":" +
                                  _multiBuffer.BufferEndPosition[bufferIndex] + "] - flush size=" + bufferSizeToFlush +
                                  "  flush number = " + NumberOfFlush);
                }
                _multiBuffer.ClearBuffer(bufferIndex);
            }
            else
            {
                if (OdbConfiguration.IsDebugEnabled(LogId))
                {
                    DLogger.Debug("Flushing buffer " + _name + "-" + bufferIndex + " : [" +
                                  _multiBuffer.BufferStartPosition[bufferIndex] + ":" +
                                  _multiBuffer.BufferEndPosition[bufferIndex] + "] - Nothing to flush!");
                }
                _multiBuffer.ClearBuffer(bufferIndex);
            }
        }

        public virtual long GetIoDeviceLength()
        {
            return _ioDeviceLength;
        }

        public virtual void SetIoDeviceLength(long ioDeviceLength)
        {
            _ioDeviceLength = ioDeviceLength;
        }

        public virtual void Close()
        {
            Clear();
            CloseIO();
        }

        public virtual void Clear()
        {
            FlushAll();
            _multiBuffer.Clear();
            _multiBuffer = null;
            _overlappingBuffers = null;
        }

        public virtual bool IsForTransaction()
        {
            return _name != null && _name.Equals("transaction");
        }

        public virtual void EnableAutomaticDelete(bool yesOrNo)
        {
            _enableAutomaticDelete = yesOrNo;
        }

        public virtual bool AutomaticDeleteIsEnabled()
        {
            return _enableAutomaticDelete;
        }

        public abstract bool Delete();

        #endregion

        public abstract void InternalWrite(byte b);

        public abstract void InternalWrite(byte[] bs, int size);

        public abstract byte InternalRead();

        public abstract long InternalRead(byte[] array, int size);

        public abstract void CloseIO();

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
            var indexes = new int[_nbBuffers];
            var index = 0;
            for (var i = 0; i < _nbBuffers; i++)
            {
                var start2 = _multiBuffer.BufferStartPosition[i];
                var end2 = _multiBuffer.BufferEndPosition[i];
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
            for (var i = index; i < _nbBuffers; i++)
                indexes[i] = -1;
            return indexes;
        }

        public virtual byte[] ReadBytes(byte[] bytes, int startIndex, int endIndex)
        {
            var size = endIndex - startIndex;
            var bufferIndex = ManageBufferForNewPosition(_currentPositionWhenUsingBuffer, Read, size);
            var start = (int) (_currentPositionWhenUsingBuffer - _multiBuffer.BufferStartPosition[bufferIndex]);
            var buffer = _multiBuffer.Buffers[bufferIndex];
            Array.Copy(buffer, start, bytes, startIndex, size);
            _currentPositionWhenUsingBuffer += size;
            return bytes;
        }

        public virtual void WriteBytes(byte[] bytes, int startIndex, int endIndex)
        {
            if (!_isUsingBuffer)
            {
                GoToPosition(_currentPositionForDirectWrite);
                InternalWrite(bytes, bytes.Length);
                _currentPositionForDirectWrite += bytes.Length;
                return;
            }
            var lengthToCopy = endIndex - startIndex;

            var bufferIndex = ManageBufferForNewPosition(_currentPositionWhenUsingBuffer, Write, lengthToCopy);
            var positionInBuffer =
                (int) (_currentPositionWhenUsingBuffer - _multiBuffer.BufferStartPosition[bufferIndex]);
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

        /// <returns> Returns the numberOfFlush. </returns>
        public virtual long GetNumberOfFlush()
        {
            return NumberOfFlush;
        }

        public override string ToString()
        {
            var buffer = new StringBuilder();
            buffer.Append("Buffers=").Append("currbuffer=").Append(_currentBufferIndex).Append(" : \n");
            for (var i = 0; i < _nbBuffers; i++)
            {
                buffer.Append(i).Append(":[").Append(_multiBuffer.BufferStartPosition[i]).Append(",").Append(
                    _multiBuffer.BufferEndPosition[i]).Append("] : write=").Append(_multiBuffer.HasBeenUsedForWrite(i)).
                    Append(" - when=").Append(_multiBuffer.GetCreationDate(i));
                if (i + 1 < _nbBuffers)
                    buffer.Append("\n");
            }
            return buffer.ToString();
        }
    }
}
