using NDatabase.Odb.Core.Layers.Layer1.Introspector;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Layers.Layer3.Engine;
using NDatabase.Odb.Core.Transaction;
using NDatabase.Odb.Core.Trigger;

namespace IO
{
    public class MockStorageEngine : AbstractStorageEngine
    {
        private IObjectReader mockObjectReader;
        private IObjectWriter mockObjectWriter;
        protected ISession session;

        public MockStorageEngine() : base(new MockBaseIdentification())
        {
        }

        public override IObjectWriter GetObjectWriter()
        {
            return mockObjectWriter;
        }

        public override IObjectReader GetObjectReader()
        {
            return mockObjectReader;
        }

        protected override MetaModel GetMetaModel()
        {
            return session.GetMetaModel();
        }

        public override ISession GetSession(bool throwExceptionIfDoesNotExist)
        {
            return session;
        }

        public override ISession BuildDefaultSession()
        {
            session = new MockSession("mock");
            return session;
        }

        public override ClassInfoList AddClasses(ClassInfoList classInfoList)
        {
            session.GetMetaModel().AddClasses(classInfoList);
            return classInfoList;
        }

        public override IObjectIntrospector BuildObjectIntrospector()
        {
            return null;
        }

        public override IObjectReader BuildObjectReader()
        {
            mockObjectReader = new MockObjectReader(this);
            return mockObjectReader;
        }

        public override IObjectWriter BuildObjectWriter()
        {
            mockObjectWriter = new MockObjectWriter(this);
            return mockObjectWriter;
        }

        public override ITriggerManager BuildTriggerManager()
        {
            return null;
        }
    }
}