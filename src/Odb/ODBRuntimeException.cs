using System;
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
                string.Format("{0}\nError:{1}", Message1, error), t)
        {
        }

        public OdbRuntimeException(IError error)
            : base(
                string.Format("{0}\nError:{1}", Message1, error))
        {
        }

        public OdbRuntimeException(IError error, string message)
            : base(
                string.Format("{0}\nError:{1}\nStackTrace:{2}", Message1, error, message))
        {
        }

        public OdbRuntimeException(Exception e, string message)
            : base(
                string.Format("{0}\nStackTrace:{1}", Message1, message), e)
        {
        }
    }
}
