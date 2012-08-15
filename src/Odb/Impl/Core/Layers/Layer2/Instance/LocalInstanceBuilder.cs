using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Transaction;

namespace NDatabase.Odb.Impl.Core.Layers.Layer2.Instance
{
    public class LocalInstanceBuilder : AbstractInstanceBuilder
    {
        private readonly ISession _session;

        public LocalInstanceBuilder(IStorageEngine engine) : base(engine)
        {
            _session = engine.GetSession(true);
        }

        protected override ISession GetSession()
        {
            return _session;
        }
    }
}
