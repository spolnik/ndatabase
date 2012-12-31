using System;

namespace NDatabase.Reflection
{
    internal class ByteBuffer
    {
        private byte[] _buffer;
        private int _position;

        public ByteBuffer(byte[] buffer)
        {
            _buffer = buffer;
        }

        public byte[] Buffer
        {
            get { return _buffer; }
            set { _buffer = value; }
        }

        public int Position
        {
            get { return _position; }
            set { _position = value; }
        }

        private void CheckCanRead(int count)
        {
            if ((_position + count) > _buffer.Length)
                throw new ArgumentOutOfRangeException();
        }

        public byte ReadByte()
        {
            CheckCanRead(1);
            return _buffer[_position++];
        }

        public byte[] ReadBytes(int length)
        {
            CheckCanRead(length);
            var dst = new byte[length];
            System.Buffer.BlockCopy(_buffer, _position, dst, 0, length);
            _position += length;
            return dst;
        }

        public double ReadDouble()
        {
            if (!BitConverter.IsLittleEndian)
            {
                var array = ReadBytes(8);
                Array.Reverse(array);
                return BitConverter.ToDouble(array, 0);
            }
            CheckCanRead(8);
            var num = BitConverter.ToDouble(_buffer, _position);
            _position += 8;
            return num;
        }

        public short ReadInt16()
        {
            CheckCanRead(2);
            var num = (short) (_buffer[_position] | (_buffer[_position + 1] << 8));
            _position += 2;
            return num;
        }

        public int ReadInt32()
        {
            CheckCanRead(4);
            var num = ((_buffer[_position] | (_buffer[_position + 1] << 8)) | (_buffer[_position + 2] << 0x10)) |
                      (_buffer[_position + 3] << 0x18);
            _position += 4;
            return num;
        }

        public long ReadInt64()
        {
            CheckCanRead(8);
            var num =
                (uint)
                (((_buffer[_position] | (_buffer[_position + 1] << 8)) | (_buffer[_position + 2] << 0x10)) |
                 (_buffer[_position + 3] << 0x18));
            var num2 =
                (uint)
                (((_buffer[_position + 4] | (_buffer[_position + 5] << 8)) | (_buffer[_position + 6] << 0x10)) |
                 (_buffer[_position + 7] << 0x18));
            long num3 = (num2 << 0x20) | num;
            _position += 8;
            return num3;
        }

        public float ReadSingle()
        {
            if (!BitConverter.IsLittleEndian)
            {
                var array = ReadBytes(4);
                Array.Reverse(array);
                return BitConverter.ToSingle(array, 0);
            }
            CheckCanRead(4);
            var num = BitConverter.ToSingle(_buffer, _position);
            _position += 4;
            return num;
        }
    }
}