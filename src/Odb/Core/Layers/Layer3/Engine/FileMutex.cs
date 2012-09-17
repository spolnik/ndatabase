using System.Collections.Generic;
using System.Threading;
using NDatabase.Tool.Wrappers.Map;

namespace NDatabase.Odb.Core.Layers.Layer3.Engine
{
    /// <summary>
    ///   A mutex to logically lock ODB database file
    /// </summary>
    public sealed class FileMutex
    {
        private static readonly FileMutex Instance = new FileMutex();

        private readonly IDictionary<string, string> _openFiles;

        private FileMutex()
        {
            _openFiles = new OdbHashMap<string, string>();
        }

        public static FileMutex GetInstance()
        {
            lock (typeof (FileMutex))
            {
                return Instance;
            }
        }

        public void ReleaseFile(string fileName)
        {
            lock (_openFiles)
            {
                _openFiles.Remove(fileName);
            }
        }

        private void LockFile(string fileName)
        {
            lock (_openFiles)
            {
                _openFiles.Add(fileName, fileName);
            }
        }

        private bool CanOpenFile(string fileName)
        {
            lock (_openFiles)
            {
                string value;
                _openFiles.TryGetValue(fileName, out value);
                var canOpen = value == null;
                if (canOpen)
                    LockFile(fileName);

                return canOpen;
            }
        }

        public bool OpenFile(string fileName)
        {
            var canOpenfile = CanOpenFile(fileName);

            if (canOpenfile)
                return true;

            if (OdbConfiguration.RetryIfFileIsLocked())
            {
                var nbRetry = 0;
                while (!CanOpenFile(fileName) && nbRetry < OdbConfiguration.GetNumberOfRetryToOpenFile())
                {
                    Thread.Sleep((int)OdbConfiguration.GetRetryTimeout());

                    nbRetry++;
                }
                if (nbRetry < OdbConfiguration.GetNumberOfRetryToOpenFile())
                    return true;
            }
            return false;
        }
    }
}
