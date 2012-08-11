using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Layers.Layer3.Engine;
using NDatabase.Odb.Core.Transaction;

namespace NDatabase.Odb.Impl.Core.Layers.Layer3.Engine
{
    public class LocalFileSystemInterface : FileSystemInterface
    {
        private readonly ISession _session;

        public LocalFileSystemInterface(string name, ISession session, string fileName, bool canWrite, bool canLog,
                                        int bufferSize) : base(name, fileName, canWrite, canLog, bufferSize)
        {
            _session = session;
        }

        public LocalFileSystemInterface(string name, ISession session, IBaseIdentification parameters, bool canLog,
                                        int bufferSize) : base(name, parameters, canLog, bufferSize)
        {
            _session = session;
        }

        public override ISession GetSession()
        {
            return _session;
        }
    }
}
