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

        internal OdbRuntimeException(IError error, Exception t)
            : base(
                string.Format("{0}\nError:{1}", Message1, error), t)
        {
        }

        internal OdbRuntimeException(IError error)
            : base(
                string.Format("{0}\nError:{1}", Message1, error))
        {
        }

        internal OdbRuntimeException(IError error, string message)
            : base(
                string.Format("{0}\nError:{1}\nStackTrace:{2}", Message1, error, message))
        {
        }

        internal OdbRuntimeException(Exception e, string message)
            : base(
                string.Format("{0}\nStackTrace:{1}", Message1, message), e)
        {
        }
    }
}
