using NDatabase.Odb.Core.Layers.Layer1.Introspector;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Layers.Layer3.Engine;
using NDatabase.Odb.Core.Transaction;
using NDatabase.Odb.Core.Trigger;

namespace NDatabase.Odb.Impl.Core.Layers.Layer3.Engine
{
    public sealed class LocalStorageEngine : AbstractStorageEngine
    {
        private ISession _session;

        public LocalStorageEngine(IBaseIdentification parameters) : base(parameters)
        {
        }

        public override ISession BuildDefaultSession()
        {
            _session = OdbConfiguration.GetCoreProvider().GetLocalSession(this);
            return _session;
        }

        public override ISession GetSession(bool throwExceptionIfDoesNotExist)
        {
            return _session;
        }

        public override ClassInfoList AddClasses(ClassInfoList classInfoList)
        {
            return GetObjectWriter().AddClasses(classInfoList);
        }

        public override IObjectIntrospector BuildObjectIntrospector()
        {
            return CoreProvider.GetLocalObjectIntrospector(this);
        }

        public override IObjectWriter BuildObjectWriter()
        {
            return CoreProvider.GetObjectWriter(this);
        }

        public override IObjectReader BuildObjectReader()
        {
            return CoreProvider.GetObjectReader(this);
        }

        public override ITriggerManager BuildTriggerManager()
        {
            return CoreProvider.GetLocalTriggerManager(this);
        }
    }
}
