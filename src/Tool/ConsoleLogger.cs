using System;

namespace NDatabase.Tool
{
    internal sealed class ConsoleLogger : ILogger
    {
        public void Warning(string message)
        {
            var originalForegroundColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkYellow;

            Console.Out.WriteLine(message);

            Console.ForegroundColor = originalForegroundColor;
        }

        public void Debug(string message)
        {
            Console.Out.WriteLine(message);
        }

        public void Info(string message)
        {
            Console.Out.WriteLine(message);
        }

        public void Error(string message)
        {
            var originalForegroundColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;

            Console.Out.WriteLine(message);

            Console.ForegroundColor = originalForegroundColor;
        }

        public void Error(string message, Exception t)
        {
            var originalForegroundColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;

            Console.Out.WriteLine(message);
            Console.Out.WriteLine(t.ToString());

            Console.ForegroundColor = originalForegroundColor;
        }
    }
}