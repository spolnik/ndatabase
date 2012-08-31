using System;

namespace NDatabase.Odb.Core.Layers.Layer3.IO
{
    internal sealed class MultiBuffer : IMultiBuffer
    {
        ///<summary>
        ///  The number of buffers.
        ///</summary>
        internal static readonly int NumberOfBuffers = 5;

        internal static readonly int DefaultBufferSizeForData = 4096 * 4;

        internal static readonly int DefaultBufferSizeForTransaction = 4096 * 4;

        ///<summary>
        ///  The buffer size.
        ///</summary>
        private readonly int _bufferSize;

        private readonly long[] _creations;

        ///<summary>
        ///  To know if buffer has been used for write - to speedup flush.
        ///</summary>
        private bool[] _bufferHasBeenUsedForWrite;

        internal MultiBuffer(int bufferSize)
        {
            _bufferSize = bufferSize;
            Buffers = new byte[NumberOfBuffers][];

            for (var x = 0; x < NumberOfBuffers; x++)
                Buffers[x] = new byte[bufferSize];

            BufferPositions = new BufferPosition[NumberOfBuffers];
            MaxPositionInBuffer = new int[NumberOfBuffers];
            _creations = new long[NumberOfBuffers];
            _bufferHasBeenUsedForWrite = new bool[NumberOfBuffers];
        }

        #region IMultiBuffer Members

        public BufferPosition[] BufferPositions { get; private set; }

        public byte[][] Buffers { get; private set; }

        ///<summary>
        ///  The max position in the buffer, used to optimize the flush - to flush only new data and not all the buffer
        ///</summary>
        public int[] MaxPositionInBuffer { get; private set; }

        public void ClearBuffer(int bufferIndex)
        {
            var buffer = Buffers[bufferIndex];
            var maxPosition = MaxPositionInBuffer[bufferIndex];
            for (var i = 0; i < maxPosition; i++)
                buffer[i] = 0;

            BufferPositions[bufferIndex] = new BufferPosition();
            MaxPositionInBuffer[bufferIndex] = 0;
            _bufferHasBeenUsedForWrite[bufferIndex] = false;
        }

        public void SetByte(int bufferIndex, int positionInBuffer, byte value)
        {
            if (Buffers[bufferIndex] == null)
                Buffers[bufferIndex] = new byte[_bufferSize];

            Buffers[bufferIndex][positionInBuffer] = value;
            _bufferHasBeenUsedForWrite[bufferIndex] = true;

            if (positionInBuffer > MaxPositionInBuffer[bufferIndex])
                MaxPositionInBuffer[bufferIndex] = positionInBuffer;
        }

        public int GetBufferIndexForPosition(long position, int size)
        {
            var max = position + size;

            for (var i = 0; i < NumberOfBuffers; i++)
            {
                // Check if new position is in buffer
                if (max <= BufferPositions[i].End && position >= BufferPositions[i].Start)
                    return i;
            }

            return -1;
        }

        public void SetCreationDate(int bufferIndex, long currentTimeInMs)
        {
            _creations[bufferIndex] = currentTimeInMs;
        }

        public void SetPositions(int bufferIndex, long startPosition, long endPosition)
        {
            BufferPositions[bufferIndex].Start = startPosition;
            BufferPositions[bufferIndex].End = endPosition;

            MaxPositionInBuffer[bufferIndex] = 0;
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
            BufferPositions = null;
            MaxPositionInBuffer = null;
            _bufferHasBeenUsedForWrite = null;
        }

        public long GetCreationDate(int bufferIndex)
        {
            return _creations[bufferIndex];
        }

        #endregion

        #region Nested type: BufferPosition

        internal struct BufferPosition
        {
            ///<summary>
            ///  The current start position of the buffer
            ///</summary>
            public long Start { get; set; }

            ///<summary>
            ///  The current end position of the buffer
            ///</summary>
            public long End { get; set; }
        }

        #endregion
    }
}
