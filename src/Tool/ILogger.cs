using System;

namespace NDatabase2.Tool
{
    public interface ILogger
    {
        void Warning(string message);

        void Debug(string message);

        void Info(string message);

        void Error(string message);

        void Error(string message, Exception t);
    }
}