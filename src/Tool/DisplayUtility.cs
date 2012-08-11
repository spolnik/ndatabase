using System;
using System.Collections;
using System.Text;

namespace NDatabase.Tool
{
    public class DisplayUtility
    {
        public static string ByteArrayToString(byte[] bytes)
        {
            var buffer = new StringBuilder();
            foreach (var value in bytes)
                buffer.Append((int) value).Append(" ");

            return buffer.ToString();
        }

        public static string LongArrayToString(long[] longs)
        {
            var buffer = new StringBuilder();
            foreach (var value in longs)
                buffer.Append(value).Append(" ");

            return buffer.ToString();
        }

        public static string ObjectArrayToString(object[] objects)
        {
            var buffer = new StringBuilder();
            foreach (var value in objects)
                buffer.Append(value).Append(" ");

            return buffer.ToString();
        }

        public static void Display(string title, ICollection list)
        {
            Console.Out.WriteLine("***" + title);
            
            var i = 1;
            foreach (var item in list)
            {
                Console.Out.WriteLine("{0}={1}", i, item);
                i++;
            }
        }

        public static string ListToString(IList list)
        {
            var buffer = new StringBuilder();
            for (var i = 0; i < list.Count; i++)
                buffer.AppendFormat("{0}={1}", (i + 1), list[i]).Append("\n");

            return buffer.ToString();
        }
    }
}