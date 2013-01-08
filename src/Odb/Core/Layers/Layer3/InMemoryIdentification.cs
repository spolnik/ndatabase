using NDatabase.Odb.Core.Layers.Layer3.IO;

namespace NDatabase.Odb.Core.Layers.Layer3
{
    internal sealed class InMemoryIdentification : IDbIdentification
    {
        private string _inmemoryID = InMemoryName + ".ID";
        private const string InMemoryName = "InMemory";

        public string Id
        {
            get
            {
                return _inmemoryID;
            }
        }

        public string Directory
        {
            get { return string.Empty; }
        }

        public string FileName
        {
            get { return string.Empty; }
        }

        public bool IsNew()
        {
            return true;
        }

        public void EnsureDirectories()
        {
            // in memory
        }

        public IMultiBufferedFileIO GetIO(int bufferSize)
        {
            return new MultiBufferedFileIO(bufferSize);
        }

        public IDbIdentification GetTransactionIdentification(long creationDateTime, string sessionId)
        {
            var filename =
                string.Format("{0}-{1}-{2}.transaction", Id, creationDateTime, sessionId);

            return new InMemoryIdentification {_inmemoryID = filename};
        }
    }
}