using System;
using System.Globalization;
using System.Text;
using NDatabase.Odb.Core;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer3.Engine;

namespace NDatabase.Odb.Impl.Core.Layers.Layer3.Engine
{
    /// <summary>
    ///   Converts array of bytes into native objects and native objects into array of bytes
    /// </summary>
    public sealed class ByteArrayConverter : IByteArrayConverter
    {
        private const byte ByteForTrue = 1;
        private const byte ByteForFalse = 0;

        private static readonly byte[] BytesForTrue = new byte[] {1};

        private static readonly byte[] BytesForFalse = new byte[] {0};

        private static int _intSize;

        private static int _intSizeX2;

        /// <summary>
        ///   The encoding used for string to byte conversion
        /// </summary>
        private string _encoding;

        private bool _hasEncoding;

        #region IByteArrayConverter Members

        /// <summary>
        ///   Two Phase Init method
        /// </summary>
        public void Init2()
        {
            _intSize = OdbType.Integer.GetSize();
            _intSizeX2 = _intSize * 2;
            SetDatabaseCharacterEncoding(OdbConfiguration.GetDatabaseCharacterEncoding());
        }

        public void SetDatabaseCharacterEncoding(string databaseCharacterEncoding)
        {
            _encoding = databaseCharacterEncoding;
            if (_encoding == null || _encoding.Equals(StorageEngineConstant.NoEncoding))
                _hasEncoding = false;
            else
                _hasEncoding = true;
        }

        public byte[] BooleanToByteArray(bool b)
        {
            if (b)
                return BytesForTrue;
            return BytesForFalse;
        }

        public void BooleanToByteArray(bool b, byte[] arrayWhereToWrite, int offset)
        {
            if (b)
                arrayWhereToWrite[offset] = ByteForTrue;
            else
                arrayWhereToWrite[offset] = ByteForFalse;
        }

        public bool ByteArrayToBoolean(byte[] bytes, int offset)
        {
            return bytes[offset] != 0;
        }

        public byte[] ShortToByteArray(short s)
        {
            return BitConverter.GetBytes(s);
        }

        public short ByteArrayToShort(byte[] bytes)
        {
            return BitConverter.ToInt16(bytes, 0);
        }

        public byte[] CharToByteArray(char c)
        {
            return BitConverter.GetBytes(c);
        }

        public char ByteArrayToChar(byte[] bytes)
        {
            return BitConverter.ToChar(bytes, 0);
        }

        public int GetNumberOfBytesOfAString(String s, bool useEncoding)
        {
            if (useEncoding && _hasEncoding)
            {
                try
                {
                    return Encoding.UTF8.GetBytes(s).Length + OdbType.Integer.GetSize() * 2;
                }
                catch (Exception)
                {
                    throw new OdbRuntimeException(NDatabaseError.UnsupportedEncoding.AddParameter(_encoding));
                }
            }
#if SILVERLIGHT
            var bytes = new AsciiEncoding().GetBytes(s);
#else
            var bytes = Encoding.ASCII.GetBytes(s);
#endif
            return bytes.Length;
        }


        /// <summary>
        /// </summary>
        /// <param name="s"> </param>
        /// <param name="withSize"> if true, returns an array with an initial int with its size </param>
        /// <param name="totalSpace"> The total space of the string (can be bigger that the real string size - to support later in place update) </param>
        /// <param name="withEncoding"> </param>
        /// <returns> The byte array that represent the string </returns>
        /// <throws>UnsupportedEncodingException</throws>
        public byte[] StringToByteArray(String s, bool withSize, int totalSpace, bool withEncoding)
        {
            byte[] bytes;
            if (withEncoding && _hasEncoding)
            {
                try
                {
                    bytes = Encoding.UTF8.GetBytes(s);
                }
                catch (Exception)
                {
                    throw new OdbRuntimeException(NDatabaseError.UnsupportedEncoding.AddParameter(_encoding));
                }
            }
            else
            {
#if SILVERLIGHT
                bytes = new AsciiEncoding().GetBytes(s);
#else
                bytes = Encoding.ASCII.GetBytes(s);
#endif
            }

            if (!withSize)
                return bytes;
            int totalSize;

            if (totalSpace == - 1)
            {
                // we always store a string with X the size to enable in place update for bigger string later
                totalSize = OdbConfiguration.GetStringSpaceReserveFactor() * bytes.Length +
                            2 * OdbType.Integer.GetSize();
            }
            else
                totalSize = totalSpace;

            var totalSizeBytes = IntToByteArray(totalSize);
            var stringRealSize = IntToByteArray(bytes.Length);

            var bytes2 = new byte[totalSize + _intSizeX2];

            for (var i = 0; i < 4; i++)
                bytes2[i] = totalSizeBytes[i];

            for (var i = 4; i < 8; i++)
                bytes2[i] = stringRealSize[i - 4];

            for (var i = 0; i < bytes.Length; i++)
                bytes2[i + 8] = bytes[i];

            return bytes2;
        }

        /// <summary>
        /// </summary>
        /// <param name="bytes"> </param>
        /// <param name="hasSize"> If hasSize is true, the first four bytes are the size of the string </param>
        /// <param name="useEncoding"> </param>
        /// <returns> The String represented by the byte array </returns>
        /// <throws>UnsupportedEncodingException</throws>
        public String ByteArrayToString(byte[] bytes, bool hasSize, bool useEncoding)
        {
            if (hasSize)
            {
                var realSize = ByteArrayToInt(bytes, _intSize);

                return Encoding.UTF8.GetString(bytes, _intSizeX2, realSize);
            }

            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

        public byte[] BigDecimalToByteArray(Decimal bigDecimal, bool withSize)
        {
            return StringToByteArray(bigDecimal.ToString(CultureInfo.InvariantCulture), withSize, - 1, false);
        }

        public Decimal ByteArrayToBigDecimal(byte[] bytes, bool hasSize)
        {
            return Decimal.Parse(ByteArrayToString(bytes, hasSize, false), NumberStyles.Any, CultureInfo.InvariantCulture);
        }

        public byte[] IntToByteArray(int l)
        {
            return BitConverter.GetBytes(l);
        }

        public void IntToByteArray(int l, byte[] arrayWhereToWrite, int offset)
        {
            int i;
            var bytes = BitConverter.GetBytes(l);

            for (i = 0; i < 4; i++)
                arrayWhereToWrite[offset + i] = bytes[i];
        }

        public int ByteArrayToInt(byte[] bytes, int offset)
        {
            return BitConverter.ToInt32(bytes, offset);
        }

        public byte[] LongToByteArray(long l)
        {
            return BitConverter.GetBytes(l);
        }

        public void LongToByteArray(long l, byte[] arrayWhereToWrite, int offset)
        {
            int i;
            var bytes = BitConverter.GetBytes(l);
            for (i = 0; i < 8; i++)
                arrayWhereToWrite[offset + i] = bytes[i];
        }

        public long ByteArrayToLong(byte[] bytes, int offset)
        {
            return BitConverter.ToInt64(bytes, offset);
        }

        public byte[] DateToByteArray(DateTime date)
        {
            var ticks = date.Ticks;
            return LongToByteArray(ticks);
        }

        public DateTime ByteArrayToDate(byte[] bytes)
        {
            var ticks = ByteArrayToLong(bytes, 0);
            return new DateTime(ticks);
        }

        public byte[] FloatToByteArray(float f)
        {
            return BitConverter.GetBytes(f);
        }

        public float ByteArrayToFloat(byte[] bytes)
        {
            return BitConverter.ToSingle(bytes, 0);
        }

        public byte[] DoubleToByteArray(double d)
        {
            return BitConverter.GetBytes(d);
        }

        public double ByteArrayToDouble(byte[] bytes)
        {
            return BitConverter.ToDouble(bytes, 0);
        }

        public void TestEncoding(string encoding)
        {
            var s = "test encoding";
            Encoding.GetEncoding(encoding).GetBytes(s);
        }

        #endregion

#if SILVERLIGHT
        /// <summary>
        /// Silverlight doesn't have an ASCII encoder, so here is one:
        /// </summary>
        public class AsciiEncoding : Encoding
        {
            public override int GetMaxByteCount(int charCount)
            {
                return charCount;
            }
            public override int GetMaxCharCount(int byteCount)
            {
                return byteCount;
            }
            public override int GetByteCount(char[] chars, int index, int count)
            {
                return count;
            }

            public override int GetCharCount(byte[] bytes)
            {
                return bytes.Length;
            }
            public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
            {
                for (int i = 0; i < charCount; i++)
                {
                    bytes[byteIndex + i] = (byte)chars[charIndex + i];
                }
                return charCount;
            }
            public override int GetCharCount(byte[] bytes, int index, int count)
            {
                return count;
            }
            public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
            {
                for (int i = 0; i < byteCount; i++)
                {
                    chars[charIndex + i] = (char)bytes[byteIndex + i];
                }
                return byteCount;
            }
        }
#endif
    }
}
