using NDatabase.Odb.Core.Layers.Layer1.Introspector;
using NDatabase.Odb.Core.Layers.Layer2.Instance;
using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Transaction;
using NDatabase.Odb.Core.Trigger;

namespace NDatabase.Odb.Core
{
    /// <summary>
    ///   This is the default Core Object Provider.
    /// </summary>
    public interface ICoreProvider : ITwoPhaseInit
    {
        /// <summary>
        ///   Returns the Local Instance Builder
        /// </summary>
        IInstanceBuilder GetInstanceBuilder(IStorageEngine engine);

        IObjectIntrospector GetLocalObjectIntrospector(IStorageEngine engine);

        ITriggerManager GetLocalTriggerManager(IStorageEngine engine);

        IClassIntrospector GetClassIntrospector();

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
