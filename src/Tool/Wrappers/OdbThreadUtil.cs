using System;
using System.Threading;

namespace NDatabase.Tool.Wrappers
{
    public static class OdbThreadUtil
    {
        public static String GetCurrentThreadName()
        {
            return Thread.CurrentThread.Name;
        }

        public static void Sleep(long timeout)
        {
            var t = (int) timeout;
            Thread.Sleep(t);
        }
    }
}
