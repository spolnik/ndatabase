using System;

namespace NDatabase.Odb.Core.Layers.Layer3.IO
{
    internal sealed class MultiBufferVO
    {
        ///<summary>
        ///  The buffer size.
        ///</summary>
        private readonly int _bufferSize;

        private readonly long[] _creations;

        ///<summary>
        ///  The number of buffers.
        ///</summary>
        private readonly int _numberOfBuffers;

        ///<summary>
        ///  To know if buffer has been used for write - to speedup flush.
        ///</summary>
        private bool[] _bufferHasBeenUsedForWrite;

        internal MultiBufferVO(int numberOfBuffers, int bufferSize)
        {
            _numberOfBuffers = numberOfBuffers;
            _bufferSize = bufferSize;
            Buffers = new byte[numberOfBuffers][];

            for (var x = 0; x < numberOfBuffers; x++)
                Buffers[x] = new byte[bufferSize];

            BufferStartPosition = new long[numberOfBuffers];
            BufferEndPosition = new long[numberOfBuffers];
            MaxPositionInBuffer = new int[numberOfBuffers];
            _creations = new long[numberOfBuffers];
            _bufferHasBeenUsedForWrite = new bool[numberOfBuffers];
        }

        ///<summary>
        ///  The current end position of the buffer
        ///</summary>
        public long[] BufferEndPosition { get; set; }

        ///<summary>
        ///  The current start position of the buffer
        ///</summary>
        public long[] BufferStartPosition { get; set; }

        public byte[][] Buffers { get; set; }

        ///<summary>
        ///  The max position in the buffer, used to optimize the flush - to flush only new data and not all the buffer
        ///</summary>
        public int[] MaxPositionInBuffer { get; set; }

        public byte GetByte(int bufferIndex, int byteIndex)
        {
            return Buffers[bufferIndex][byteIndex];
        }

        public void ClearBuffer(int bufferIndex)
        {
            var buffer = Buffers[bufferIndex];
            var maxPosition = MaxPositionInBuffer[bufferIndex];
            for (var i = 0; i < maxPosition; i++)
                buffer[i] = 0;

            BufferStartPosition[bufferIndex] = 0;
            BufferEndPosition[bufferIndex] = 0;
            MaxPositionInBuffer[bufferIndex] = 0;
            _bufferHasBeenUsedForWrite[bufferIndex] = false;
        }

        public void SetByte(int bufferIndex, int positionInBuffer, byte b)
        {
            if (Buffers[bufferIndex] == null)
                Buffers[bufferIndex] = new byte[_bufferSize];

            Buffers[bufferIndex][positionInBuffer] = b;
            _bufferHasBeenUsedForWrite[bufferIndex] = true;

            if (positionInBuffer > MaxPositionInBuffer[bufferIndex])
                MaxPositionInBuffer[bufferIndex] = positionInBuffer;
        }

        public int GetBufferIndexForPosition(long position, int size)
        {
            var max = position + size;

            for (var i = 0; i < _numberOfBuffers; i++)
            {
                // Check if new position is in buffer
                if (max <= BufferEndPosition[i] && position >= BufferStartPosition[i])
                    return i;
            }

            return -1;
        }

        public void SetCreationDate(int bufferIndex, long currentTimeInMs)
        {
            _creations[bufferIndex] = currentTimeInMs;
        }

        public void SetPositions(int bufferIndex, long startPosition, long endPosition, int maxPosition)
        {
            BufferStartPosition[bufferIndex] = startPosition;
            BufferEndPosition[bufferIndex] = endPosition;
            MaxPositionInBuffer[bufferIndex] = maxPosition;
        }

        public void WriteBytes(int bufferIndex, byte[] bytes, int startIndex, int offsetWhereToCopy, int lengthToCopy)
        {
            Array.Copy(bytes, startIndex, Buffers[bufferIndex], offsetWhereToCopy, lengthToCopy);

            _bufferHasBeenUsedForWrite[bufferIndex] = true;

            var positionInBuffer = offsetWhereToCopy + lengthToCopy - 1;
            if (positionInBuffer > MaxPositionInBuffer[bufferIndex])
                MaxPositionInBuffer[bufferIndex] = positionInBuffer;
        }

        public bool HasBeenUsedForWrite(int bufferIndex)
        {
            return _bufferHasBeenUsedForWrite[bufferIndex];
        }

        public void Clear()
        {
            Buffers = null;
            BufferStartPosition = null;
            BufferEndPosition = null;
            MaxPositionInBuffer = null;
            _bufferHasBeenUsedForWrite = null;
        }

        public long GetCreationDate(int bufferIndex)
        {
            return _creations[bufferIndex];
        }
    }
}
