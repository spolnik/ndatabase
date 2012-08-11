using System;
using NDatabase.Odb.Core;
using NDatabase.Tool.Wrappers;

namespace NDatabase.Odb
{
    /// <summary>
    ///   Generic ODB Runtime exception : Used to report all problems.
    /// </summary>
    /// <remarks>
    ///   Generic ODB Runtime exception : Used to report all problems.
    /// </remarks>
    /// <author>osmadja</author>
    [Serializable]
    public class OdbRuntimeException : Exception
    {
        private const string Url = "https://sourceforge.net/tracker/?func=add&group_id=179124&atid=887885";

        private static readonly string Message1 =
            string.Format(
                "\nNDatabase has thrown an Exception, please help us filling a bug report at {0} with the following error message",
                Url);

        public OdbRuntimeException(IError error, Exception t)
            : base(
                string.Format("{0}\nVersion={1} , Build={2}, Date={3}, Thread={4}\nError:{5}", Message1,
                              Release.ReleaseNumber, Release.ReleaseBuild, Release.ReleaseDate,
                              OdbThreadUtil.GetCurrentThreadName(), error), t)
        {
        }

        public OdbRuntimeException(IError error)
            : base(
                string.Format("{0}\nVersion={1} , Build={2}, Date={3}, Thread={4}\nError:{5}", Message1,
                              Release.ReleaseNumber, Release.ReleaseBuild, Release.ReleaseDate,
                              OdbThreadUtil.GetCurrentThreadName(), error))
        {
        }

        public OdbRuntimeException(IError error, string message)
            : base(
                string.Format("{0}\nVersion={1} , Build={2}, Date={3}, Thread={4}\nError:{5}\nStackTrace:{6}", Message1,
                              Release.ReleaseNumber, Release.ReleaseBuild, Release.ReleaseDate,
                              OdbThreadUtil.GetCurrentThreadName(), error, message))
        {
        }

        // FIXME add a submit a bug link to SF

        public OdbRuntimeException(Exception e, string message)
            : base(
                string.Format("{0}\nVersion={1} , Build={2}, Date={3}, Thread={4}\nStackTrace:{5}", Message1,
                              Release.ReleaseNumber, Release.ReleaseBuild, Release.ReleaseDate,
                              OdbThreadUtil.GetCurrentThreadName(), message), e)
        {
        }

        public virtual void AddMessageHeader(string @string)
        {
        }
    }
}
