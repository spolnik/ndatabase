using System;
using NDatabase.Odb.Core.Layers.Layer3;

namespace NDatabase.Tool
{
    public sealed class ConsoleLogger : ILogger
    {
        private readonly IStorageEngine _engine;
        private int _i;

        public ConsoleLogger(IStorageEngine engine)
        {
            _engine = engine;
            _i = 0;
        }

        public ConsoleLogger()
        {
            _i = 0;
        }

        #region ILogger Members

        public void Debug(object o)
        {
            Console.Out.WriteLine(o);
        }

        public void Error(object o)
        {
            const string header =
                "An internal error occured,please email the error stack trace displayed below to odb.support@neodatis.org";
            Console.Out.WriteLine(header);
            Console.Out.WriteLine(o);
        }

        public void Error(object o, Exception throwable)
        {
            const string header =
                "An internal error occured,please email the error stack trace displayed below to odb.support@neodatis.org";
            Console.Out.WriteLine(header);
            Console.Out.WriteLine(o);
            Console.Out.WriteLine(throwable.ToString());
        }

        public void Info(object o)
        {
            if (_i % 20 == 0)
            {
                if (_engine != null)
                    Console.Out.WriteLine(_engine.GetSession(true).GetCache().ToString());
            }

            Console.Out.WriteLine(o);
            _i++;
        }

        #endregion
    }
}
