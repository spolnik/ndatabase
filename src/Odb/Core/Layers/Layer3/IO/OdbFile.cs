using System;
using System.IO;

namespace NDatabase.Odb.Core.Layers.Layer3.IO
{
    internal sealed class OdbFile
    {
        private readonly string _fileName;

        internal OdbFile(String fileName)
        {
            _fileName = Path.GetFullPath(fileName);
        }

        private String GetDirectory()
        {
            return Path.GetDirectoryName(_fileName);
        }

        internal bool Exists()
        {
            return File.Exists(_fileName);
        }

        internal OdbFile GetParentFile()
        {
            var di = new DirectoryInfo(GetDirectory());

            //TODO: to NDatabase error
            if (di.Parent == null)
                throw new Exception("OdbFile Error. Parent file is null!");

            return new OdbFile(di.Parent.FullName);
        }

        internal void Mkdirs()
        {
            var di = new DirectoryInfo(GetDirectory());
            //TODO check if it creates all sub directories
            di.Create();
        }

        internal bool Delete()
        {
            var fileExists = Exists();
            if (!fileExists)
                return false;

            File.Delete(_fileName);

            return !Exists();
        }
    }
}
