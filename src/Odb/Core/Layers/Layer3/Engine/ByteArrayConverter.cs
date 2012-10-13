using System;
using System.Text;
using NDatabase2.Odb.Core.Layers.Layer2.Meta;

namespace NDatabase2.Odb.Core.Layers.Layer3.Engine
{
    /// <summary>
    ///   Converts array of bytes into native objects and native objects into array of bytes
    /// </summary>
    public static class ByteArrayConverter
    {
        private static readonly byte[] BytesForTrue = new byte[] {1};

        private static readonly byte[] BytesForFalse = new byte[] {0};

        private static readonly int IntSize = OdbType.Integer.Size;

        private static readonly int IntSizeX2 = OdbType.Integer.Size * 2;

        public static byte[] BooleanToByteArray(bool b)
        {
            return b
                       ? BytesForTrue
                       : BytesForFalse;
        }

        public static bool ByteArrayToBoolean(byte[] bytes)
        {
            return ByteArrayToBoolean(bytes, 0);
        }

        public static bool ByteArrayToBoolean(byte[] bytes, int offset)
        {
            return bytes[offset] != 0;
        }

        public static byte[] ShortToByteArray(short s)
        {
            return BitConverter.GetBytes(s);
        }

        public static short ByteArrayToShort(byte[] bytes)
        {
            return BitConverter.ToInt16(bytes, 0);
        }

        public static byte[] CharToByteArray(char c)
        {
            return BitConverter.GetBytes(c);
        }

        public static char ByteArrayToChar(byte[] bytes)
        {
            return BitConverter.ToChar(bytes, 0);
        }

        public static int GetNumberOfBytesOfAString(String s)
        {
            return Encoding.UTF8.GetBytes(s).Length + IntSizeX2;
        }

        /// <param name="s">Input</param>
        /// <param name="totalSpace"> The total space of the string (can be bigger that the real string size - to support later in place update) </param>
        /// <returns> The byte array that represent the string </returns>
        public static byte[] StringToByteArray(String s, int totalSpace)
        {
            var stringBytes = Encoding.UTF8.GetBytes(s);

            var size = stringBytes.Length + IntSizeX2;

            var totalSize = totalSpace < size
                                ? size
                                : totalSpace;

            var totalSizeBytes = IntToByteArray(totalSize);
            var stringRealSize = IntToByteArray(stringBytes.Length);

            var bytes2 = new byte[totalSize + IntSizeX2];

            for (var i = 0; i < 4; i++)
                bytes2[i] = totalSizeBytes[i];

            for (var i = 4; i < 8; i++)
                bytes2[i] = stringRealSize[i - 4];

            for (var i = 0; i < stringBytes.Length; i++)
                bytes2[i + 8] = stringBytes[i];

            return bytes2;
        }

        /// <returns> The String represented by the byte array </returns>
        public static String ByteArrayToString(byte[] bytes)
        {
            var realSize = ByteArrayToInt(bytes, IntSize);
            return Encoding.UTF8.GetString(bytes, IntSizeX2, realSize);
        }

        public static byte[] DecimalToByteArray(Decimal bigDecimal)
        {
            var bits = Decimal.GetBits(bigDecimal);

            return GetBytes(bits[0], bits[1], bits[2], bits[3]);
        }

        private static byte[] GetBytes(int lo, int mid, int hi, int flags)
        {
            var buffer = new byte[16];
            buffer[0] = (byte) lo;
            buffer[1] = (byte) (lo >> 8);
            buffer[2] = (byte) (lo >> 16);
            buffer[3] = (byte) (lo >> 24);

            buffer[4] = (byte) mid;
            buffer[5] = (byte) (mid >> 8);
            buffer[6] = (byte) (mid >> 16);
            buffer[7] = (byte) (mid >> 24);

            buffer[8] = (byte) hi;
            buffer[9] = (byte) (hi >> 8);
            buffer[10] = (byte) (hi >> 16);
            buffer[11] = (byte) (hi >> 24);

            buffer[12] = (byte) flags;
            buffer[13] = (byte) (flags >> 8);
            buffer[14] = (byte) (flags >> 16);
            buffer[15] = (byte) (flags >> 24);

            return buffer;
        }

        public static Decimal ByteArrayToDecimal(byte[] buffer)
        {
            var lo = (buffer[0]) | (buffer[1] << 8) | (buffer[2] << 16) | (buffer[3] << 24);
            var mid = (buffer[4]) | (buffer[5] << 8) | (buffer[6] << 16) | (buffer[7] << 24);
            var hi = (buffer[8]) | (buffer[9] << 8) | (buffer[10] << 16) | (buffer[11] << 24);
            var flags = (buffer[12]) | (buffer[13] << 8) | (buffer[14] << 16) | (buffer[15] << 24);

            return new Decimal(new[] {lo, mid, hi, flags});
        }

        public static byte[] IntToByteArray(int l)
        {
            return BitConverter.GetBytes(l);
        }

        public static int ByteArrayToInt(byte[] bytes)
        {
            return ByteArrayToInt(bytes, 0);
        }

        public static int ByteArrayToInt(byte[] bytes, int offset)
        {
            return BitConverter.ToInt32(bytes, offset);
        }

        public static byte[] LongToByteArray(long l)
        {
            return BitConverter.GetBytes(l);
        }

        public static long ByteArrayToLong(byte[] bytes)
        {
            return ByteArrayToLong(bytes, 0);
        }

        public static byte[] DateToByteArray(DateTime date)
        {
            return LongToByteArray(date.Ticks);
        }

        public static DateTime ByteArrayToDate(byte[] bytes)
        {
            var ticks = ByteArrayToLong(bytes);
            return new DateTime(ticks);
        }

        public static byte[] FloatToByteArray(float f)
        {
            return BitConverter.GetBytes(f);
        }

        public static float ByteArrayToFloat(byte[] bytes)
        {
            return BitConverter.ToSingle(bytes, 0);
        }

        public static byte[] DoubleToByteArray(double d)
        {
            return BitConverter.GetBytes(d);
        }

        public static double ByteArrayToDouble(byte[] bytes)
        {
            return BitConverter.ToDouble(bytes, 0);
        }

        public static long ByteArrayToLong(byte[] bytes, int offset)
        {
            return BitConverter.ToInt64(bytes, offset);
        }
    }
}
