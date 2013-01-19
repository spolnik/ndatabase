using System.Collections.Generic;
using System.Threading;
using NDatabase.Tool.Wrappers;

namespace NDatabase.Odb.Core.Layers.Layer3.Engine
{
    /// <summary>
    ///   A mutex to logically lock ODB database file
    /// </summary>
    internal sealed class FileMutex
    {
        private const int NumberOfRetryToOpenFile = 5;
        private static readonly FileMutex Instance = new FileMutex();

        private readonly IDictionary<string, string> _openFiles;

        private FileMutex()
        {
            _openFiles = new OdbHashMap<string, string>();
        }

        internal static FileMutex GetInstance()
        {
            lock (typeof (FileMutex))
            {
                return Instance;
            }
        }

        internal void ReleaseFile(string fileName)
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

        internal void OpenFile(string fileName)
        {
            var canOpenfile = CanOpenFile(fileName);

            if (canOpenfile)
                return;

            var nbRetry = 0;
            while (!CanOpenFile(fileName) && nbRetry < NumberOfRetryToOpenFile)
            {
                Thread.Sleep(100);

                nbRetry++;
            }
        }
    }
}
