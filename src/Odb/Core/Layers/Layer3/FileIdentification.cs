using System.IO;

namespace NDatabase.Odb.Core.Layers.Layer3
{
    /// <summary>
    ///   Database Parameters for local database access
    /// </summary>
    internal sealed class FileIdentification : IFileIdentification
    {
        private readonly string _fileName;

        internal FileIdentification(string name)
        {
            _fileName = name;
        }

        #region IFileIdentification Members

        public string Directory
        {
            get
            {
                var fullPath = Path.GetFullPath(_fileName);
                return Path.GetDirectoryName(fullPath);
            }
        }

        public string Id
        {
            get { return GetCleanFileName(); }
        }

        public bool IsNew()
        {
            return !File.Exists(_fileName);
        }

        public string FileName
        {
            get { return _fileName; }
        }

        #endregion

        public override string ToString()
        {
            return _fileName;
        }

        private string GetCleanFileName()
        {
            return Path.GetFileName(_fileName);
        }
    }
}