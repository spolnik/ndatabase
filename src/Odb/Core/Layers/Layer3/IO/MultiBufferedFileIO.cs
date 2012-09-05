using System;
using System.IO;
using System.Text;
using NDatabase.Tool;
using NDatabase.Tool.Wrappers;

namespace NDatabase.Odb.Core.Layers.Layer3.IO
{
    /// <summary>
    ///   Class allowing buffering for IO This class is used to give 
    ///   a transparent access to buffered file io
    /// </summary>
    internal sealed class MultiBufferedFileIO : IMultiBufferedFileIO
    {
        private const bool IsReading = true;
        private const bool IsWriting = false;

        private const string LogId = "MultiBufferedIO";

        private IMultiBuffer _buffer;

        private int _currentBufferIndex;

        private long _currentPositionWhenUsingBuffer;

        /// <summary>
        ///   The length of the io device
        /// </summary>
        private long _ioDeviceLength;

        /// <summary>
        ///   A boolean value to check if read write are using buffer
        /// </summary>
        private bool _isUsingBuffer;

        private int _nextBufferIndex;

        private INonBufferedFileIO _nonBufferedFileIO;
        private int[] _overlappingBuffers;

        public MultiBufferedFileIO(string fileName, int bufferSize)
        {
            _buffer = new MultiBuffer(bufferSize);
            _currentPositionWhenUsingBuffer = -1;

            _overlappingBuffers = new int[MultiBuffer.NumberOfBuffers];

            NumberOfFlush = 0;

            _isUsingBuffer = true;
            _nextBufferIndex = 0;

            _nonBufferedFileIO = new NonBufferedFileIO(fileName);

            try
            {
                if (OdbConfiguration.IsDebugEnabled(LogId))
                    DLogger.Info(string.Format("Opening datatbase file : {0}", Path.GetFullPath(fileName)));

                _ioDeviceLength = _nonBufferedFileIO.Length;
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
            get
            {
                return _isUsingBuffer
                           ? _ioDeviceLength
                           : _nonBufferedFileIO.Length;
            }
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
                return _isUsingBuffer
                           ? _currentPositionWhenUsingBuffer
                           : _nonBufferedFileIO.CurrentPositionForDirectWrite;
            }
        }

        public void SetCurrentWritePosition(long currentPosition)
        {
            SetCurrentPosition(currentPosition, () => { });
        }

        public void SetCurrentReadPosition(long currentPosition)
        {
            SetCurrentPosition(currentPosition, () => ManageBufferForNewPosition(currentPosition, IsReading, 1));
        }

        public void WriteByte(byte b)
        {
            if (!_isUsingBuffer)
            {
                _nonBufferedFileIO.WriteByte(b);
                return;
            }

            var bufferIndex = _buffer.GetBufferIndexForPosition(_currentPositionWhenUsingBuffer, 1);
            if (bufferIndex == -1)
                bufferIndex = ManageBufferForNewPosition(_currentPositionWhenUsingBuffer, IsWriting, 1);

            var positionInBuffer = (int) (_currentPositionWhenUsingBuffer - _buffer.BufferPositions[bufferIndex].Start);

            _buffer.SetByte(bufferIndex, positionInBuffer, b);

            _currentPositionWhenUsingBuffer++;
            if (_currentPositionWhenUsingBuffer > _ioDeviceLength)
                _ioDeviceLength = _currentPositionWhenUsingBuffer;
        }

        public byte[] ReadBytes(int size)
        {
            if (!_isUsingBuffer)
                return _nonBufferedFileIO.ReadBytes(size);

            var bytes = new byte[size];

            // If the size to read is smaller than the buffer size
            if (size <= _buffer.Size)
                return ReadBytes(bytes, 0, size);

            // else the read have to use various buffers
            if (OdbConfiguration.IsDebugEnabled(LogId))
                DLogger.Debug(string.Format("Data is larger than buffer size {0} > {1} : cutting the data", bytes.Length,
                                            _buffer.Size));

            var nbBuffersNeeded = bytes.Length / _buffer.Size + 1;
            var currentStart = 0;
            var currentEnd = _buffer.Size;

            for (var i = 0; i < nbBuffersNeeded; i++)
            {
                ReadBytes(bytes, currentStart, currentEnd);
                currentStart += _buffer.Size;

                if (currentEnd + _buffer.Size < bytes.Length)
                    currentEnd += _buffer.Size;
                else
                    currentEnd = bytes.Length;
            }

            return bytes;
        }

        public byte ReadByte()
        {
            if (!_isUsingBuffer)
                return _nonBufferedFileIO.ReadByte();

            var bufferIndex = ManageBufferForNewPosition(_currentPositionWhenUsingBuffer, IsReading, 1);
            var positionInBuffer = (int) (_currentPositionWhenUsingBuffer - _buffer.BufferPositions[bufferIndex].Start);

            var result = _buffer.Buffers[bufferIndex][positionInBuffer];
            _currentPositionWhenUsingBuffer++;

            return result;
        }

        public void WriteBytes(byte[] bytes)
        {
            if (bytes.Length > _buffer.Size)
            {
                if (OdbConfiguration.IsDebugEnabled(LogId))
                    DLogger.Debug(string.Format("Data is larger than buffer size {0} > {1} : cutting the data",
                                                bytes.Length, _buffer.Size));

                var nbBuffersNeeded = bytes.Length / _buffer.Size + 1;
                var currentStart = 0;
                var currentEnd = _buffer.Size;

                for (var i = 0; i < nbBuffersNeeded; i++)
                {
                    WriteBytes(bytes, currentStart, currentEnd);
                    currentStart += _buffer.Size;

                    if (currentEnd + _buffer.Size < bytes.Length)
                        currentEnd += _buffer.Size;
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
            _nonBufferedFileIO.Dispose();
            _nonBufferedFileIO = null;
        }

        public void EnableAutomaticDelete(bool yesOrNo)
        {
            _nonBufferedFileIO.EnableAutomaticDelete(yesOrNo);
        }

        public void Dispose()
        {
            Close();
        }

        #endregion

        private void SetCurrentPosition(long currentPosition, Action additionalAction)
        {
            if (_isUsingBuffer)
            {
                if (_currentPositionWhenUsingBuffer == currentPosition)
                    return;
                _currentPositionWhenUsingBuffer = currentPosition;
                additionalAction();
            }
            else
                _nonBufferedFileIO.SetCurrentPosition(currentPosition);
        }

        private int ManageBufferForNewPosition(long newPosition, bool isReading, int size)
        {
            var bufferIndex = _buffer.GetBufferIndexForPosition(newPosition, size);
            if (bufferIndex != -1)
            {
                NbBufferOk++;
                return bufferIndex;
            }

            NbBufferNotOk++;
            // checks if there is any overlapping buffer
            _overlappingBuffers = GetOverlappingBuffers(newPosition, _buffer.Size);
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
            if (isReading && newPosition >= length)
            {
                var message = string.Format("End Of File reached - position = {0} : Length = {1}", newPosition, length);
                DLogger.Error(message);
                throw new OdbRuntimeException(
                    NDatabaseError.EndOfFileReached.AddParameter(newPosition).AddParameter(length));
            }

            // The buffer must be initialized with real data, so the first thing we
            // must do is read data from file and puts it in the array
            long nread = _buffer.Size;

            // if new position is in the file
            if (newPosition < length)
            {
                // We are in the file, we are updating content. to create the
                // buffer, we first read the content of the file
                // Actually loads data from the file to the buffer
                nread = _nonBufferedFileIO.Read(newPosition, _buffer.Buffers[bufferIndex], _buffer.Size);
                _buffer.SetCreationDate(bufferIndex, OdbTime.GetCurrentTimeInTicks());
            }
            else
                _nonBufferedFileIO.GoToPosition(newPosition);

            // If we are in READ, sets the size equal to what has been read
            var endPosition = isReading
                                  ? newPosition + nread
                                  : newPosition + _buffer.Size;

            _buffer.SetPositions(bufferIndex, newPosition, endPosition);
            _currentPositionWhenUsingBuffer = newPosition;

            if (OdbConfiguration.IsDebugEnabled(LogId))
            {
                DLogger.Debug(string.Format("Creating buffer {0} : [{1},{2}]", bufferIndex,
                                            _buffer.BufferPositions[bufferIndex].Start,
                                            _buffer.BufferPositions[bufferIndex].End));
            }

            return bufferIndex;
        }

        private void Flush(int bufferIndex)
        {
            var buffer = _buffer.Buffers[bufferIndex];

            if (buffer != null && _buffer.HasBeenUsedForWrite(bufferIndex))
            {
                // the +1 is because the maxPositionInBuffer is a position and the
                // parameter is a length
                var bufferSizeToFlush = _buffer.MaxPositionInBuffer[bufferIndex] + 1;
                _nonBufferedFileIO.Flush(_buffer.BufferPositions[bufferIndex].Start, buffer, bufferSizeToFlush);

                NumberOfFlush++;
                TotalFlushSize += bufferSizeToFlush;

                if (OdbConfiguration.IsDebugEnabled(LogId))
                {
                    DLogger.Debug(string.Format("Flushing buffer {0} : [{1}:{2}] - flush size={3}  flush number = {4}",
                                                bufferIndex, _buffer.BufferPositions[bufferIndex].Start,
                                                _buffer.BufferPositions[bufferIndex].End, bufferSizeToFlush,
                                                NumberOfFlush));
                }

                _buffer.ClearBuffer(bufferIndex);
            }
            else
            {
                if (OdbConfiguration.IsDebugEnabled(LogId))
                {
                    DLogger.Debug(string.Format("Flushing buffer {0} : [{1}:{2}] - Nothing to flush!", bufferIndex,
                                                _buffer.BufferPositions[bufferIndex].Start,
                                                _buffer.BufferPositions[bufferIndex].End));
                }

                _buffer.ClearBuffer(bufferIndex);
            }
        }

        private void Clear()
        {
            FlushAll();
            _buffer.Clear();
            _buffer = null;
            _overlappingBuffers = null;
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
                var start2 = _buffer.BufferPositions[i].Start;
                var end2 = _buffer.BufferPositions[i].End;

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
            var bufferIndex = ManageBufferForNewPosition(_currentPositionWhenUsingBuffer, IsReading, size);

            var start = (int) (_currentPositionWhenUsingBuffer - _buffer.BufferPositions[bufferIndex].Start);
            var buffer = _buffer.Buffers[bufferIndex];

            Array.Copy(buffer, start, bytes, startIndex, size);
            _currentPositionWhenUsingBuffer += size;

            return bytes;
        }

        public void WriteBytes(byte[] bytes, int startIndex, int endIndex)
        {
            if (!_isUsingBuffer)
            {
                _nonBufferedFileIO.WriteBytes(bytes, bytes.Length);
                return;
            }

            var lengthToCopy = endIndex - startIndex;

            var bufferIndex = ManageBufferForNewPosition(_currentPositionWhenUsingBuffer, IsWriting, lengthToCopy);
            var positionInBuffer = (int) (_currentPositionWhenUsingBuffer - _buffer.BufferPositions[bufferIndex].Start);

            _buffer.WriteBytes(bufferIndex, bytes, startIndex, positionInBuffer, lengthToCopy);
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
                buffer.Append(i).Append(":[").Append(_buffer.BufferPositions[i].Start).Append(",").Append(
                    _buffer.BufferPositions[i].End).Append("] : write=").Append(_buffer.HasBeenUsedForWrite(i)).Append(
                        " - when=").Append(_buffer.GetCreationDate(i));

                if (i + 1 < MultiBuffer.NumberOfBuffers)
                    buffer.Append("\n");
            }

            return buffer.ToString();
        }
    }
}