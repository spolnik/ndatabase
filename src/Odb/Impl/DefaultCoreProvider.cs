using System.Collections.Generic;
using NDatabase.Odb.Core;
using NDatabase.Odb.Core.Layers.Layer1.Introspector;
using NDatabase.Odb.Core.Layers.Layer2.Instance;
using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Layers.Layer3.Engine;
using NDatabase.Odb.Core.Query;
using NDatabase.Odb.Core.Query.Execution;
using NDatabase.Odb.Core.Transaction;
using NDatabase.Odb.Core.Trigger;
using NDatabase.Odb.Impl.Core.Layers.Layer1.Introspector;
using NDatabase.Odb.Impl.Core.Layers.Layer2.Instance;
using NDatabase.Odb.Impl.Core.Layers.Layer3.Buffer;
using NDatabase.Odb.Impl.Core.Layers.Layer3.Engine;
using NDatabase.Odb.Impl.Core.Layers.Layer3.Oid;
using NDatabase.Odb.Impl.Core.Layers.Layer3.Refactor;
using NDatabase.Odb.Impl.Core.Oid;
using NDatabase.Odb.Impl.Core.Query.Criteria;
using NDatabase.Odb.Impl.Core.Transaction;
using NDatabase.Odb.Impl.Core.Trigger;
using NDatabase.Tool.Wrappers.IO;
using NDatabase.Tool.Wrappers.Map;

namespace NDatabase.Odb.Impl
{
    /// <summary>
    ///   The is the default implementation of ODB
    /// </summary>
    /// <author>olivier</author>
    public class DefaultCoreProvider : ICoreProvider
    {
        private static readonly IClassPool ClassPool = new OdbClassPool();

        private static readonly IByteArrayConverter ByteArrayConverter = new DefaultByteArrayConverter();

        private static IClassIntrospector _classIntrospector;

        private static readonly IDictionary<IStorageEngine, ITriggerManager> TriggerManagers =
            new OdbHashMap<IStorageEngine, ITriggerManager>();

        #region ICoreProvider Members

        public virtual void Init2()
        {
            ByteArrayConverter.Init2();

            _classIntrospector = new DefaultClassIntrospector();
            _classIntrospector.Init2();
        }

        public virtual void ResetClassDefinitions()
        {
            _classIntrospector.Reset();
            ClassPool.Reset();
        }

        public virtual IByteArrayConverter GetByteArrayConverter()
        {
            return ByteArrayConverter;
        }

        /// <summary>
        ///   TODO Return a list of IO to enable replication or other IO mechanism Used by the 
        ///   TODO FileSystemInterface to actual write/read byte to underlying storage
        /// </summary>
        /// <param name="name"> The name of the buffered io </param>
        /// <param name="parameters"> The parameters that define the buffer </param>
        /// <param name="bufferSize"> The size of the buffers </param>
        /// <returns> The buffer implementation @ </returns>
        public virtual IBufferedIO GetIO(string name, IBaseIdentification parameters, int bufferSize)
        {
            var fileParameters = parameters as IOFileParameter;

            if (fileParameters != null)
            {
                // Guarantee that file directory structure exist
                var odbFile = new OdbFile(fileParameters.GetFileName());
                var fparent = odbFile.GetParentFile();

                if (fparent != null && !fparent.Exists())
                    fparent.Mkdirs();

                return new MultiBufferedFileIO(OdbConfiguration.GetNbBuffers(), name, fileParameters.GetFileName(),
                                               fileParameters.CanWrite(), bufferSize);
            }

            throw new OdbRuntimeException(NDatabaseError.UnsupportedIoType.AddParameter(parameters.ToString()));
        }

        public virtual IIdManager GetIdManager(IStorageEngine engine)
        {
            return new DefaultIdManager(engine.GetObjectWriter(), engine.GetObjectReader(),
                                        engine.GetCurrentIdBlockPosition(), engine.GetCurrentIdBlockNumber(),
                                        engine.GetCurrentIdBlockMaxOid());
        }

        public virtual IObjectWriter GetObjectWriter(IStorageEngine engine)
        {
            return new LocalObjectWriter(engine);
        }

        public virtual IObjectReader GetObjectReader(IStorageEngine engine)
        {
            return new ObjectReader(engine);
        }

        public virtual IStorageEngine GetStorageEngine(IBaseIdentification baseIdentification)
        {
            return new LocalStorageEngine(baseIdentification);
        }

        /// <summary>
        ///   Returns the Local Instance Builder
        /// </summary>
        public virtual IInstanceBuilder GetLocalInstanceBuilder(IStorageEngine engine)
        {
            return new LocalInstanceBuilder(engine);
        }

        public virtual IObjectIntrospector GetLocalObjectIntrospector(IStorageEngine engine)
        {
            return new LocalObjectIntrospector(engine);
        }

        public virtual ITriggerManager GetLocalTriggerManager(IStorageEngine engine)
        {
            // First check if trigger manager has already been built for the engine
            ITriggerManager triggerManager;
            TriggerManagers.TryGetValue(engine, out triggerManager);
            if (triggerManager != null)
                return triggerManager;

            triggerManager = new DefaultTriggerManager(engine);
            TriggerManagers[engine] = triggerManager;
            return triggerManager;
        }

        public virtual void RemoveLocalTriggerManager(IStorageEngine engine)
        {
            TriggerManagers.Remove(engine);
        }

        public virtual IClassIntrospector GetClassIntrospector()
        {
            return _classIntrospector;
        }

        public virtual IWriteAction GetWriteAction(long position, byte[] bytes)
        {
            return new DefaultWriteAction(position, bytes);
        }

        public virtual ITransaction GetTransaction(ISession session, IFileSystemInterface fsi)
        {
            return new DefaultTransaction(session, fsi);
        }

        public virtual ISession GetLocalSession(IStorageEngine engine)
        {
            return new LocalSession(engine);
        }

        public virtual IRefactorManager GetRefactorManager(IStorageEngine engine)
        {
            return new DefaultRefactorManager(engine);
        }

        // For query result handler
        public virtual IMatchingObjectAction GetCollectionQueryResultAction(IStorageEngine engine, IQuery query,
                                                                            bool inMemory, bool returnObjects)
        {
            return new CollectionQueryResultAction<object>(query, inMemory, engine, returnObjects,
                                                           engine.GetObjectReader().GetInstanceBuilder());
        }

        // OIDs
        public virtual OID GetObjectOID(long objectOid, long classOid)
        {
            return new OdbObjectOID(objectOid);
        }

        public virtual OID GetClassOID(long oid)
        {
            return new OdbClassOID(oid);
        }

        public virtual OID GetExternalObjectOID(long objectOid, long classOid)
        {
            return new OdbObjectOID(objectOid);
        }

        public virtual OID GetExternalClassOID(long oid)
        {
            return new OdbClassOID(oid);
        }

        public virtual IClassPool GetClassPool()
        {
            return ClassPool;
        }

        #endregion
    }
}
