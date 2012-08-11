using System;
using System.Collections.Generic;

namespace NDatabase.Tool
{
    /// <summary>
    ///   Simple logging class <p></p>
    /// </summary>
    public static class DLogger
    {
        private static readonly IList<ILogger> Iloggers = new List<ILogger>();

        public static void Register(ILogger logger)
        {
            Iloggers.Add(logger);
        }

        public static void Debug(object @object)
        {
            Console.Out.WriteLine(@object == null
                                      ? "null"
                                      : @object.ToString());
            foreach (var logger in Iloggers)
                logger.Debug(@object);
        }

        public static void Info(object @object)
        {
            Console.Out.WriteLine(@object == null
                                      ? "null"
                                      : @object.ToString());
            foreach (var logger in Iloggers)
                logger.Info(@object);
        }

        /// <param name="obj"> The obj to be logged </param>
        public static void Error(object obj)
        {
            Console.Out.WriteLine(obj == null
                                      ? "null"
                                      : obj.ToString());
            foreach (var logger in Iloggers)
                logger.Error(obj);
        }

        public static void Error(object obj, Exception t)
        {
            Console.Out.WriteLine(obj == null
                                      ? "null"
                                      : obj.ToString());
            Console.Out.WriteLine(t.ToString());
            foreach (var logger in Iloggers)
                logger.Error(obj, t);
        }
    }
}
