using System;
using NDatabase.Tool;

namespace Test.Odb.Test.Log
{
    /// <author>olivier</author>
    public class MyLogger : ILogger
    {
        #region ILogger Members

        public virtual void Debug(object @object)
        {
        }

        public virtual void Error(object @object)
        {
        }

        public virtual void Error(object @object, Exception t)
        {
        }

        public virtual void Info(object @object)
        {
        }

        #endregion
    }
}
