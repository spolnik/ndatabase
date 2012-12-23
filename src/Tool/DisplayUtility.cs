using System.Collections.Generic;
using System.Text;

namespace NDatabase2.Tool
{
    internal static class DisplayUtility
    {
        internal static string ByteArrayToString(IEnumerable<byte> bytes)
        {
            var buffer = new StringBuilder();
            foreach (var value in bytes)
                buffer.Append((int) value).Append(" ");

            return buffer.ToString();
        }

        internal static string LongArrayToString(IEnumerable<long> longs)
        {
            var buffer = new StringBuilder();
            foreach (var value in longs)
                buffer.Append(value).Append(" ");

            return buffer.ToString();
        }
    }
}