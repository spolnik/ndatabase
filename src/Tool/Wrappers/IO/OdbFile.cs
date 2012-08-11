using System;
using System.IO;

namespace NDatabase.Tool.Wrappers.IO
{
    public class OdbFile
    {
        private readonly string _fileName;

        public OdbFile(String fileName)
        {
            _fileName = fileName;
            // Converts path (with directory info)
            _fileName = GetFullPath();
        }

        public String GetDirectory()
        {
            return Path.GetDirectoryName(_fileName);
        }

        public String GetCleanFileName()
        {
            return Path.GetFileName(_fileName);
        }

        public String GetFullPath()
        {
            return Path.GetFullPath(_fileName);
        }

        public bool Exists()
        {
            return File.Exists(_fileName);
        }

        public void Clear()
        {
        }

        public OdbFile GetParentFile()
        {
            var di = new DirectoryInfo(GetDirectory());

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

        public bool Delete()
        {
            var fileExists = Exists();
            if (!fileExists)
                return false;

            File.Delete(_fileName);
            return !Exists();
        }

        public void Create()
        {
            File.Create(_fileName);
        }

        public void Release()
        {
        }
    }
}