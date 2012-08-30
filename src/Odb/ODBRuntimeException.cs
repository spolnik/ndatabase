using System;
using System.Threading;
using NDatabase.Odb.Core;

namespace NDatabase.Odb
{
    /// <summary>
    ///   Generic ODB Runtime exception : Used to report all problems.
    /// </summary>
    
    public class OdbRuntimeException : Exception
    {
        private static readonly string Message1 =
            string.Format("\nNDatabase has thrown an Exception");

        public OdbRuntimeException(IError error, Exception t)
            : base(
                string.Format("{0}\n Thread={1}\nError:{2}", Message1,
                              Thread.CurrentThread.Name, error), t)
        {
        }

        public OdbRuntimeException(IError error)
            : base(
                string.Format("{0}\nThread={1}\nError:{2}", Message1,
                              Thread.CurrentThread.Name, error))
        {
        }

        public OdbRuntimeException(IError error, string message)
            : base(
                string.Format("{0}\nThread={1}\nError:{2}\nStackTrace:{3}", Message1,
                              Thread.CurrentThread.Name, error, message))
        {
        }

        public OdbRuntimeException(Exception e, string message)
            : base(
                string.Format("{0}\nThread={1}\nStackTrace:{2}", Message1,
                              Thread.CurrentThread.Name, message), e)
        {
        }
    }
}
