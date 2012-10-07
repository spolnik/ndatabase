using System;
using System.Collections.Generic;
using NDatabase.Odb.Core.Layers.Layer1.Introspector;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer3.Engine;
using NDatabase.Odb.Core.Layers.Layer3.Oid;
using NDatabase.Odb.Core.Query;
using NDatabase.Odb.Core.Query.Criteria;
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

        long Count(CriteriaQuery query);

        IValues GetValues(IValuesQuery query, int startIndex, int endIndex);

        IObjects<T> GetObjects<T>(IQuery query, bool inMemory, int startIndex, int endIndex);

        IObjects<T> GetObjects<T>(Type clazz, bool inMemory, int startIndex, int endIndex);

        /// <summary>
        ///   Return Meta representation of objects
        /// </summary>
        /// <param name="query"> The query to select objects </param>
        /// <param name="inMemory"> To indicate if object must be all loaded in memory </param>
        /// <param name="startIndex"> First object index </param>
        /// <param name="endIndex"> Last object index </param>
        /// <param name="returnOjects"> To indicate if object instances must be created </param>
        /// <returns> The list of objects @ </returns>
        IObjects<T> GetObjectInfos<T>(IQuery query, bool inMemory, int startIndex, int endIndex, bool returnOjects);

        IObjectReader GetObjectReader();

        IObjectWriter GetObjectWriter();

        ITriggerManager GetTriggerManager();

        ISession GetSession(bool throwExceptionIfDoesNotExist);

        ISession BuildDefaultSession();

        void Commit();

        void Rollback();

        OID GetObjectId<T>(T plainObject, bool throwExceptionIfDoesNotExist) where T : class;

        object GetObjectFromOid(OID oid);

        NonNativeObjectInfo GetMetaObjectFromOid(OID oid);

        ObjectInfoHeader GetObjectInfoHeaderFromOid(OID oid);

        void DefragmentTo(string newFileName);

        IList<long> GetAllObjectIds();

        IList<FullIDInfo> GetAllObjectIdInfos(string objectType, bool displayObjects);

        bool IsClosed();

        void AddUpdateTriggerFor(string className, UpdateTrigger trigger);

        void AddInsertTriggerFor(string className, InsertTrigger trigger);

        void AddDeleteTriggerFor(string className, DeleteTrigger trigger);

        void AddSelectTriggerFor(string className, SelectTrigger trigger);

        void SetDatabaseId(IDatabaseId databaseId);

        void SetCurrentIdBlockInfos(CurrentIdBlockInfo currentIdBlockInfo);

        IFileIdentification GetBaseIdentification();

        /// <summary>
        ///   Write an object already transformed into meta representation!
        /// </summary>
        /// <param name="oid"> </param>
        /// <param name="nnoi"> </param>
        /// <param name="position"> </param>
        /// <param name="updatePointers"> </param>
        /// <returns> te object position(or id (if &lt;0, it is id)) @ </returns>
        OID WriteObjectInfo(OID oid, NonNativeObjectInfo nnoi, long position, bool updatePointers);

        /// <summary>
        ///   Updates an object already transformed into meta representation!
        /// </summary>
        /// <param name="nnoi"> The Object Meta representation </param>
        /// <param name="forceUpdate"> </param>
        /// <returns> The OID of the update object @ </returns>
        OID UpdateObject(NonNativeObjectInfo nnoi, bool forceUpdate);

        void AddSession(ISession session, bool readMetamodel);

        /// <param name="className"> The class name on which the index must be created </param>
        /// <param name="name"> The name of the index </param>
        /// <param name="indexFields"> The list of fields of the index </param>
        /// <param name="verbose"> A boolean value to indicate of ODB must describe what it is doing @ @ </param>
        /// <param name="acceptMultipleValuesForSameKey"> </param>
        void AddIndexOn(string className, string name, string[] indexFields, bool verbose,
                        bool acceptMultipleValuesForSameKey);

        void AddCommitListener(ICommitListener commitListener);

        IOdbList<ICommitListener> GetCommitListeners();

        /// <summary>
        ///   Returns the object used to refactor the database
        /// </summary>
        IRefactorManager GetRefactorManager();

        void ResetCommitListeners();

        ClassInfoList AddClasses(ClassInfoList classInfoList);

        IDatabaseId GetDatabaseId();

        ITransactionId GetCurrentTransactionId();

        void SetCurrentTransactionId(ITransactionId transactionId);

        /// <summary>
        ///   Used to disconnect the object from the current session.
        /// </summary>
        /// <remarks>
        ///   Used to disconnect the object from the current session. The object is removed from the cache
        /// </remarks>
        void Disconnect(object @object);

        void RebuildIndex(string className, string indexName, bool verbose);

        void DeleteIndex(string className, string indexName, bool verbose);

        /// <summary>
        ///   Receive the current class info (loaded from current java classes present on classpath and check against the persisted meta model
        /// </summary>
        /// <param name="currentCIs"> </param>
        CheckMetaModelResult CheckMetaModelCompatibility(IDictionary<string, ClassInfo> currentCIs);

        IObjectIntrospector BuildObjectIntrospector();

        ITriggerManager BuildTriggerManager();

        /// <param name="clazz"> </param>
        /// <param name="criterion"> </param>
        /// <returns> </returns>
        CriteriaQuery CriteriaQuery(Type clazz, ICriterion criterion);

        /// <param name="clazz"> </param>
        /// <returns> </returns>
        CriteriaQuery CriteriaQuery(Type clazz);

        CurrentIdBlockInfo GetCurrentIdBlockInfo();

        IIdManager GetIdManager();

        ITriggerManager GetLocalTriggerManager();
        void RemoveLocalTriggerManager();
    }
}
