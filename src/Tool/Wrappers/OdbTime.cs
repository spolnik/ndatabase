using System;

namespace NDatabase.Tool.Wrappers
{
    public static class OdbTime
    {
        public static long GetCurrentTimeInTicks()
        {
            return DateTime.Now.Ticks;
        }

        public static long GetCurrentTimeInMs()
        {
            return (long) TimeSpan.FromTicks(GetCurrentTimeInTicks()).TotalMilliseconds;
        }
    }
}
