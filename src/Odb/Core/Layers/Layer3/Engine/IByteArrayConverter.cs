using System;

namespace NDatabase.Odb.Core.Layers.Layer3.Engine
{
    public interface IByteArrayConverter : ITwoPhaseInit
    {
        byte[] BooleanToByteArray(bool b);

        bool ByteArrayToBoolean(byte[] bytes, int offset);

        byte[] ShortToByteArray(short s);

        short ByteArrayToShort(byte[] bytes);

        byte[] CharToByteArray(char c);

        char ByteArrayToChar(byte[] bytes);

        int GetNumberOfBytesOfAString(string s, bool useEncoding);

        /// <param name="s"> </param>
        /// <param name="withSize"> if true, returns an array with an initial int with its size </param>
        /// <param name="totalSpace"> The total space of the string (can be bigger that the real string size - to support later in place update) </param>
        /// <param name="withEncoding"> To specify if SPECIFIC encoding must be used </param>
        /// <returns> The byte array that represent the string </returns>
        byte[] StringToByteArray(string s, bool withSize, int totalSpace, bool withEncoding);

        /// <param name="bytes"> </param>
        /// <param name="hasSize"> If hasSize is true, the first four bytes are the size of the string </param>
        /// <param name="useEncoding"> </param>
        /// <returns> The String represented by the byte array </returns>
        string ByteArrayToString(byte[] bytes, bool hasSize, bool useEncoding);

        byte[] BigDecimalToByteArray(Decimal bigDecimal, bool withSize);

        Decimal ByteArrayToBigDecimal(byte[] bytes, bool hasSize);

        byte[] IntToByteArray(int l);

        /// <summary>
        ///   This method writes the byte directly to the array parameter
        /// </summary>
        void IntToByteArray(int l, byte[] arrayWhereToWrite, int offset);

        int ByteArrayToInt(byte[] bytes, int offset);

        byte[] LongToByteArray(long l);

        /// <summary>
        ///   This method writes the byte directly to the array parameter
        /// </summary>
        void LongToByteArray(long l, byte[] arrayWhereToWrite, int offset);

        long ByteArrayToLong(byte[] bytes, int offset);

        byte[] DateToByteArray(DateTime date);

        DateTime ByteArrayToDate(byte[] bytes);

        byte[] FloatToByteArray(float f);

        float ByteArrayToFloat(byte[] bytes);

        byte[] DoubleToByteArray(double d);

        double ByteArrayToDouble(byte[] bytes);

        void SetDatabaseCharacterEncoding(string databaseCharacterEncoding);

        /// <param name="b"> </param>
        /// <param name="arrayWhereToWrite"> </param>
        /// <param name="offset"> </param>
        void BooleanToByteArray(bool b, byte[] arrayWhereToWrite, int offset);

        void TestEncoding(string encoding);
    }
}
