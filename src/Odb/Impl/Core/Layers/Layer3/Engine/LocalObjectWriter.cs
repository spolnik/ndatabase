using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Layers.Layer3.Engine;
using NDatabase.Odb.Core.Transaction;

namespace NDatabase.Odb.Impl.Core.Layers.Layer3.Engine
{
    public sealed class LocalObjectWriter : AbstractObjectWriter
    {
        private readonly ISession _session;

        public LocalObjectWriter(IStorageEngine engine) : base(engine)
        {
            _session = engine.GetSession(true);
        }

        public override ISession GetSession()
        {
            return _session;
        }

        public override IFileSystemInterface BuildFsi()
        {
            return new LocalFileSystemInterface("local-data", GetSession(), StorageEngine.GetBaseIdentification(), true,
                                                OdbConfiguration.GetDefaultBufferSizeForData());
        }
    }
}
