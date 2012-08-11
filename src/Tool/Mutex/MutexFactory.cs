using System.Collections.Generic;
using NDatabase.Tool.Wrappers.Map;

namespace NDatabase.Tool.Mutex
{
    /// <summary>
    ///   A mutex factory
    /// </summary>
    /// <author>osmadja</author>
    public static class MutexFactory
    {
        private static readonly IDictionary<string, Mutex> Mutexs = new OdbHashMap<string, Mutex>();

        private static bool _debug;

        public static Mutex Get(string name)
        {
            lock (typeof (MutexFactory))
            {
                var mutex = Mutexs[name];

                if (mutex == null)
                {
                    mutex = new Mutex(name);
                    mutex.SetDebug(_debug);
                    Mutexs.Add(name, mutex);
                }

                return mutex;
            }
        }

        public static void SetDebug(bool debugValue)
        {
            _debug = debugValue;
        }
    }
}
