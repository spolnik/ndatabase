using NDatabase.Tool;
using NDatabase.Tool.Wrappers.IO;

namespace NDatabase.Odb.Core.Layers.Layer3
{
    /// <summary>
    ///   Database Parameters for local database access
    /// </summary>
    /// <author>osmadja</author>
    public sealed class IOFileParameter : IBaseIdentification
    {
        private bool _canWrite;
        private string _fileName;

        public IOFileParameter(string name, bool write)
        {
            _fileName = name;
            _canWrite = write;
        }

        #region IBaseIdentification Members

        public bool CanWrite()
        {
            return _canWrite;
        }

        public string GetDirectory()
        {
            return new OdbFile(_fileName).GetDirectory();
        }

        public string GetIdentification()
        {
            return GetCleanFileName();
        }

        public bool IsNew()
        {
            return !IOUtil.ExistFile(_fileName);
        }

        #endregion

        public void SetCanWrite(bool canWrite)
        {
            _canWrite = canWrite;
        }

        public string GetFileName()
        {
            return _fileName;
        }

        public void SetFileName(string fileName)
        {
            _fileName = fileName;
        }

        public override string ToString()
        {
            return _fileName;
        }

        public string GetCleanFileName()
        {
            return new OdbFile(_fileName).GetCleanFileName();
        }
    }
}
