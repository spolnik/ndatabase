using System.Collections.Generic;
using NDatabase.Odb.Core;
using NDatabase.Odb.Core.Layers.Layer1.Introspector;
using NDatabase.Odb.Core.Layers.Layer2.Instance;
using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Layers.Layer3.Engine;
using NDatabase.Odb.Core.Layers.Layer3.IO;
using NDatabase.Odb.Core.Transaction;
using NDatabase.Odb.Core.Trigger;
using NDatabase.Odb.Impl.Core.Layers.Layer1.Introspector;
using NDatabase.Odb.Impl.Core.Layers.Layer2.Instance;
using NDatabase.Odb.Impl.Core.Layers.Layer3.Buffer;
using NDatabase.Odb.Impl.Core.Layers.Layer3.Engine;
using NDatabase.Odb.Impl.Core.Layers.Layer3.Oid;
using NDatabase.Odb.Impl.Core.Layers.Layer3.Refactor;
using NDatabase.Odb.Impl.Core.Oid;
using NDatabase.Odb.Impl.Core.Transaction;
using NDatabase.Odb.Impl.Core.Trigger;
using NDatabase.Tool.Wrappers.Map;

namespace NDatabase.Odb.Impl
{
    /// <summary>
    ///   The is the default implementation of ODB
    /// </summary>
    public sealed class DefaultCoreProvider : ICoreProvider
    {
        private static readonly IClassPool ClassPool = new OdbClassPool();

        private static readonly IByteArrayConverter ByteArrayConverter = new ByteArrayConverter();

        private static IClassIntrospector _classIntrospector;

        private static readonly IDictionary<IStorageEngine, ITriggerManager> TriggerManagers =
            new OdbHashMap<IStorageEngine, ITriggerManager>();

        #region ICoreProvider Members

        public void Init2()
        {
            ByteArrayConverter.Init2();

            _classIntrospector = new ClassIntrospector();
            _classIntrospector.Init2();
        }

        public void ResetClassDefinitions()
        {
            _classIntrospector.Reset();
            ClassPool.Reset();
        }

        public IByteArrayConverter GetByteArrayConverter()
        {
            return ByteArrayConverter;
        }

        

        public IIdManager GetIdManager(IStorageEngine engine)
        {
            return new IdManager(engine.GetObjectWriter(), engine.GetObjectReader(),
                                        engine.GetCurrentIdBlockInfo());
        }

        public IObjectWriter GetObjectWriter(IStorageEngine engine)
        {
            return new ObjectWriter(engine, ByteArrayConverter, _classIntrospector);
        }

        public IObjectReader GetObjectReader(IStorageEngine engine)
        {
            return new ObjectReader(engine);
        }

        public IStorageEngine GetStorageEngine(IFileIdentification fileIdentification)
        {
            return new StorageEngine(fileIdentification);
        }

        /// <summary>
        ///   Returns the Local Instance Builder
        /// </summary>
        public IInstanceBuilder GetInstanceBuilder(IStorageEngine engine)
        {
            return new InstanceBuilder(engine);
        }

        public IObjectIntrospector GetLocalObjectIntrospector(IStorageEngine engine)
        {
            return new ObjectIntrospector(engine);
        }

        public ITriggerManager GetLocalTriggerManager(IStorageEngine engine)
        {
            // First check if trigger manager has already been built for the engine
            ITriggerManager triggerManager;
            TriggerManagers.TryGetValue(engine, out triggerManager);
            if (triggerManager != null)
                return triggerManager;

            triggerManager = new TriggerManager(engine);
            TriggerManagers[engine] = triggerManager;
            return triggerManager;
        }

        public void RemoveLocalTriggerManager(IStorageEngine engine)
        {
            TriggerManagers.Remove(engine);
        }

        public IClassIntrospector GetClassIntrospector()
        {
            return _classIntrospector;
        }

        public IWriteAction GetWriteAction(long position, byte[] bytes)
        {
            return new WriteAction(position, bytes);
        }

        public ITransaction GetTransaction(ISession session, IFileSystemInterface fsi)
        {
            return new OdbTransaction(session, fsi);
        }

        public ISession GetLocalSession(IStorageEngine engine)
        {
            return new LocalSession(engine);
        }

        public IRefactorManager GetRefactorManager(IStorageEngine engine)
        {
            return new RefactorManager(engine);
        }

        // OIDs
        public OID GetObjectOID(long objectOid, long classOid)
        {
            return new OdbObjectOID(objectOid);
        }

        public OID GetClassOID(long oid)
        {
            return new OdbClassOID(oid);
        }

        public OID GetExternalObjectOID(long objectOid, long classOid)
        {
            return new OdbObjectOID(objectOid);
        }

        public OID GetExternalClassOID(long oid)
        {
            return new OdbClassOID(oid);
        }

        public IClassPool GetClassPool()
        {
            return ClassPool;
        }

        #endregion
    }
}
