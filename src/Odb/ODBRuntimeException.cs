using System;
using System.Threading;
using NDatabase.Odb.Core;

namespace NDatabase.Odb
{
    /// <summary>
    ///   Generic ODB Runtime exception : Used to report all problems.
    /// </summary>
    [Serializable]
    public class OdbRuntimeException : Exception
    {
        private const string Url = "http://ndatabase.codeplex.com/workitem/list/basic";

        private static readonly string Message1 =
            string.Format(
                "\nNDatabase has thrown an Exception, please help us filling a bug report at {0} with the following error message",
                Url);

        public OdbRuntimeException(IError error, Exception t)
            : base(
                string.Format("{0}\nVersion={1} , Build={2}, Date={3}, Thread={4}\nError:{5}", Message1,
                              Release.ReleaseNumber, Release.ReleaseBuild, Release.ReleaseDate,
                              Thread.CurrentThread.Name, error), t)
        {
        }

        public OdbRuntimeException(IError error)
            : base(
                string.Format("{0}\nVersion={1} , Build={2}, Date={3}, Thread={4}\nError:{5}", Message1,
                              Release.ReleaseNumber, Release.ReleaseBuild, Release.ReleaseDate,
                              Thread.CurrentThread.Name, error))
        {
        }

        public OdbRuntimeException(IError error, string message)
            : base(
                string.Format("{0}\nVersion={1} , Build={2}, Date={3}, Thread={4}\nError:{5}\nStackTrace:{6}", Message1,
                              Release.ReleaseNumber, Release.ReleaseBuild, Release.ReleaseDate,
                              Thread.CurrentThread.Name, error, message))
        {
        }

        // FIXME add a submit a bug link to SF

        public OdbRuntimeException(Exception e, string message)
            : base(
                string.Format("{0}\nVersion={1} , Build={2}, Date={3}, Thread={4}\nStackTrace:{5}", Message1,
                              Release.ReleaseNumber, Release.ReleaseBuild, Release.ReleaseDate,
                              Thread.CurrentThread.Name, message), e)
        {
        }
    }
}
