using System;

namespace NDatabase.Tool
{
    public interface ILogger
    {
        void Debug(object @object);

        void Info(object @object);

        void Error(object @object);

        void Error(object @object, Exception t);
    }
}