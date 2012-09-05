using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Layers.Layer3.Engine;

namespace NDatabase.Odb.Core.Transaction
{
    public interface ISession
    {
        ICache GetCache();

        ITmpCache GetTmpCache();

        void Rollback();

        void Close();

        void ClearCache();

        bool IsRollbacked();

        void Clear();

        IStorageEngine GetStorageEngine();

        bool TransactionIsPending();

        void Commit();

        ITransaction GetTransaction();

        void SetFileSystemInterfaceToApplyTransaction(IFileSystemInterface fsi);

        string GetBaseIdentification();

        MetaModel GetMetaModel();

        void SetMetaModel(MetaModel metaModel2);

        string GetId();

        void RemoveObjectFromCache(object @object);

        /// <summary>
        ///   Add these information on a session cache.
        /// </summary>
        /// <remarks>
        ///   Add these information on a session cache.
        /// </remarks>
        void AddObjectToCache(OID oid, object @object, ObjectInfoHeader oih);
    }
}
