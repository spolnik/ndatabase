using NDatabase2.Odb.Core.Layers.Layer3;

namespace Test.NDatabase.Odb.Test.IO
{
    public class MockFileIdentification : IFileIdentification
    {
        #region IFileIdentification Members

        public string Id
        {
            get { return "mock"; }
        }

        public string FileName
        {
            get { return "mock"; }
        }

        public bool IsNew()
        {
            return false;
        }

        public string Directory
        {
            get { return string.Empty; }
        }

        #endregion

        public bool IsLocal()
        {
            return false;
        }
    }
}
