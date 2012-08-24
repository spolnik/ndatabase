using System.Text;

namespace NDatabase.Tool
{
    internal static class DisplayUtility
    {
        internal static string ByteArrayToString(byte[] bytes)
        {
            var buffer = new StringBuilder();
            foreach (var value in bytes)
                buffer.Append((int) value).Append(" ");

            return buffer.ToString();
        }

        internal static string LongArrayToString(long[] longs)
        {
            var buffer = new StringBuilder();
            foreach (var value in longs)
                buffer.Append(value).Append(" ");

            return buffer.ToString();
        }
    }
}