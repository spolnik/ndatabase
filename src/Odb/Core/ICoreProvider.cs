using NDatabase.Odb.Core.Layers.Layer1.Introspector;
using NDatabase.Odb.Core.Layers.Layer2.Instance;
using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Layers.Layer3.Engine;
using NDatabase.Odb.Core.Query;
using NDatabase.Odb.Core.Query.Execution;
using NDatabase.Odb.Core.Transaction;
using NDatabase.Odb.Core.Trigger;

namespace NDatabase.Odb.Core
{
    /// <summary>
    ///   This is the default Core Object Provider.
    /// </summary>
    /// <remarks>
    ///   This is the default Core Object Provider.
    /// </remarks>
    /// <author>olivier</author>
    public interface ICoreProvider : ITwoPhaseInit
    {
        IByteArrayConverter GetByteArrayConverter();

        /// <summary>
        ///   TODO Return a list of IO to enable replication or other IO mechanism Used by the FileSystemInterface to actual write/read byte to underlying storage
        /// </summary>
        /// <param name="name"> The name of the buffered io </param>
        /// <param name="parameters"> The parameters that define the buffer </param>
        /// <param name="bufferSize"> The size of the buffers </param>
        /// <returns> The buffer implementation </returns>
        IBufferedIO GetIO(string name, IFileIdentification parameters, int bufferSize);

        /// <summary>
        ///   Returns the Local Instance Builder
        /// </summary>
        IInstanceBuilder GetInstanceBuilder(IStorageEngine engine);

        IObjectIntrospector GetLocalObjectIntrospector(IStorageEngine engine);

        ITriggerManager GetLocalTriggerManager(IStorageEngine engine);

        IClassIntrospector GetClassIntrospector();

        ITransaction GetTransaction(ISession session, IFileSystemInterface fsi);

        IWriteAction GetWriteAction(long position, byte[] bytes);

        ISession GetLocalSession(IStorageEngine engine);

        IRefactorManager GetRefactorManager(IStorageEngine engine);

        // OIDs
        OID GetObjectOID(long objectOid, long classOid);

        OID GetClassOID(long oid);

        OID GetExternalObjectOID(long objectOid, long classOid);

        OID GetExternalClassOID(long oid);

        IClassPool GetClassPool();

        void ResetClassDefinitions();

        void RemoveLocalTriggerManager(IStorageEngine engine);

        IIdManager GetIdManager(IStorageEngine engine);

        IObjectWriter GetObjectWriter(IStorageEngine engine);

        IObjectReader GetObjectReader(IStorageEngine engine);

        IStorageEngine GetStorageEngine(IFileIdentification fileIdentification);
    }
}
