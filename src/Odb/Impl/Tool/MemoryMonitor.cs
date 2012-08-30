using System;
#if !SILVERLIGHT
using System.Diagnostics;
#endif
using System.Text;
using NDatabase.Odb.Impl.Core.Transaction;

namespace NDatabase.Odb.Impl.Tool
{
    public static class MemoryMonitor
    {
#if !SILVERLIGHT
        private static readonly PerformanceCounter Memory = new PerformanceCounter("Memory", "Available MBytes");
#endif
        public static void DisplayCurrentMemory(string label, bool all)
        {
            var buffer = new StringBuilder();
#if !SILVERLIGHT
            buffer.Append(label).Append(":Free=").Append(Memory.NextValue()).Append("k / Total=").Append("?").Append("k");
#endif
            if (all)
                buffer.Append(" - Cache Usage = ").Append(Cache.Usage());

            Console.Out.WriteLine(buffer.ToString());
        }
    }
}
