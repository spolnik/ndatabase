using System;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer3;

namespace NDatabase.Odb.Core.Transaction
{
    internal interface ISession : IComparable
    {
        IOdbCache GetCache();

        IReadObjectsCache GetTmpCache();

        void Rollback();

        void Close();

        bool IsRollbacked();

        IStorageEngine GetStorageEngine();

        bool TransactionIsPending();

        void Commit();

        ITransaction GetTransaction();

        void SetFileSystemInterfaceToApplyTransaction(IFileSystemInterface fsi);

        IMetaModel GetMetaModel();

        void SetMetaModel(IMetaModel metaModel2);

        string GetId();

        void RemoveObjectFromCache(object @object);

        IObjectWriter GetObjectWriter();
    }
}
