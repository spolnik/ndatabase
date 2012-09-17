using System;
using System.Diagnostics;
using System.Text;
using NDatabase.Odb.Core.Transaction;

namespace NDatabase.Odb.Impl.Tool
{
    public static class MemoryMonitor
    {
        private static readonly PerformanceCounter Memory = new PerformanceCounter("Memory", "Available MBytes");

        public static void DisplayCurrentMemory(string label, bool all)
        {
            var buffer = new StringBuilder();

            buffer.Append(label).Append(":Free=").Append(Memory.NextValue()).Append("k / Total=").Append("?").Append("k");

            if (all)
                buffer.Append(" - Cache Usage = ").Append(Cache.Usage());

            Console.Out.WriteLine(buffer.ToString());
        }
    }
}
