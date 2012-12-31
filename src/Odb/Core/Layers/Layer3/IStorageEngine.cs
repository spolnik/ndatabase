using System;
using System.Collections.Generic;
using NDatabase.Odb.Core.Layers.Layer1.Introspector;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer3.Engine;
using NDatabase.Odb.Core.Query;
using NDatabase.Odb.Core.Transaction;
using NDatabase.Odb.Core.Trigger;
using NDatabase.Tool.Wrappers.List;

namespace NDatabase.Odb.Core.Layers.Layer3
{
    /// <summary>
    ///   The interface of all that a StorageEngine (Main concept in ODB) must do.
    /// </summary>
    internal interface IStorageEngine
    {
        OID Store<T>(OID oid, T plainObject) where T : class;

        /// <summary>
        ///   Store an object in an database.
        /// </summary>
        /// <remarks>
        ///   Store an object in an database. To detect if object must be updated or insert, we use the cache. To update an object, it must be first selected from the database. When an object is to be stored, if it exist in the cache, then it will be updated, else it will be inserted as a new object. If the object is null, the cache will be used to check if the meta representation is in the cache
        /// </remarks>
        OID Store<T>(T plainObject) where T : class;

        void DeleteObjectWithOid(OID oid);

        OID Delete<T>(T plainObject) where T : class;

        void Close();

        IValues GetValues(IValuesQuery query, int startIndex, int endIndex);

        IInternalObjectSet<T> GetObjects<T>(IQuery query, bool inMemory, int startIndex, int endIndex);

        IObjectReader GetObjectReader();

        IObjectWriter GetObjectWriter();

        IInternalTriggerManager GetTriggerManager();

        ISession GetSession();

        ISession BuildDefaultSession();

        void Commit();

        void Rollback();

        OID GetObjectId<T>(T plainObject, bool throwExceptionIfDoesNotExist) where T : class;

        object GetObjectFromOid(OID oid);

        ObjectInfoHeader GetObjectInfoHeaderFromOid(OID oid);

        void DefragmentTo(string newFileName);

        IList<long> GetAllObjectIds();

        bool IsClosed();

        void AddUpdateTriggerFor(Type type, UpdateTrigger trigger);

        void AddInsertTriggerFor(Type type, InsertTrigger trigger);

        void AddDeleteTriggerFor(Type type, DeleteTrigger trigger);

        void AddSelectTriggerFor(Type type, SelectTrigger trigger);

        void SetDatabaseId(IDatabaseId databaseId);

        void SetCurrentIdBlockInfos(CurrentIdBlockInfo currentIdBlockInfo);

        IFileIdentification GetBaseIdentification();

        void AddSession(ISession session, bool readMetamodel);

        /// <param name="className"> The class name on which the index must be created </param>
        /// <param name="name"> The name of the index </param>
        /// <param name="indexFields"> The list of fields of the index </param>
        /// <param name="acceptMultipleValuesForSameKey"> </param>
        void AddIndexOn(string className, string name, string[] indexFields, bool acceptMultipleValuesForSameKey);

        void AddCommitListener(ICommitListener commitListener);

        IOdbList<ICommitListener> GetCommitListeners();

        /// <summary>
        ///   Returns the object used to refactor the database
        /// </summary>
        IRefactorManager GetRefactorManager();

        void ResetCommitListeners();

        void AddClasses(ClassInfoList classInfoList);

        IDatabaseId GetDatabaseId();

        ITransactionId GetCurrentTransactionId();

        void SetCurrentTransactionId(ITransactionId transactionId);

        /// <summary>
        ///   Used to disconnect the object from the current session.
        /// </summary>
        /// <remarks>
        ///   Used to disconnect the object from the current session. The object is removed from the cache
        /// </remarks>
        void Disconnect<T>(T plainObject) where T : class;

        void RebuildIndex(string className, string indexName);

        void DeleteIndex(string className, string indexName);

        /// <summary>
        ///   Receive the current class info (loaded from current java classes present on classpath and check against the persisted meta model
        /// </summary>
        /// <param name="currentCIs"> </param>
        void CheckMetaModelCompatibility(IDictionary<string, ClassInfo> currentCIs);

        IObjectIntrospector BuildObjectIntrospector();

        IInternalTriggerManager BuildTriggerManager();

        CurrentIdBlockInfo GetCurrentIdBlockInfo();

        IIdManager GetIdManager();

        IInternalTriggerManager GetLocalTriggerManager();
        void RemoveLocalTriggerManager();
    }
}
