using System;
using System.IO;

namespace NDatabase.Tool.Wrappers.IO
{
    public sealed class OdbFile
    {
        private readonly string _fileName;

        public OdbFile(String fileName)
        {
            _fileName = Path.GetFullPath(fileName);
        }

        private String GetDirectory()
        {
            return Path.GetDirectoryName(_fileName);
        }

        public bool Exists()
        {
            return File.Exists(_fileName);
        }

        public OdbFile GetParentFile()
        {
            var di = new DirectoryInfo(GetDirectory());

            //TODO: to NDatabase error
            if (di.Parent == null)
                throw new Exception("OdbFile Error. Parent file is null!");

            return new OdbFile(di.Parent.FullName);
        }

        public void Mkdirs()
        {
            var di = new DirectoryInfo(GetDirectory());
            //TODO check if it creates all sub directories
            di.Create();
        }

        private bool Delete()
        {
            var fileExists = Exists();
            if (!fileExists)
                return false;

            File.Delete(_fileName);
            return !Exists();
        }

        public static bool DeleteFile(string fileName)
        {
            var file = new OdbFile(fileName);
            return file.Delete();
        }
    }
}
