using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using NDatabase.Btree;
using NDatabase.Odb.Core.BTree;
using NDatabase.Odb.Core.Layers.Layer1.Introspector;
using NDatabase.Odb.Core.Layers.Layer2.Instance;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer3.Oid;
using NDatabase.Odb.Core.Query;
using NDatabase.Odb.Core.Query.Criteria;
using NDatabase.Odb.Core.Transaction;
using NDatabase.Odb.Core.Trigger;
using NDatabase.Tool;
using NDatabase.Tool.Wrappers;
using NDatabase.Tool.Wrappers.List;
using NDatabase.Tool.Wrappers.Map;

namespace NDatabase.Odb.Core.Layers.Layer3.Engine
{
    internal abstract class AbstractStorageEngineReader : IStorageEngine
    {
        private const string LogId = "LocalStorageEngine";

        private static readonly IDictionary<IStorageEngine, ITriggerManager> TriggerManagers =
            new OdbHashMap<IStorageEngine, ITriggerManager>();

        /// <summary>
        ///   The file parameters - if we are accessing a file, it will be a IOFileParameters that contains the file name
        /// </summary>
        protected IFileIdentification FileIdentification;

        /// <summary>
        ///   To check if database has already been closed
        /// </summary>
        protected bool IsDbClosed;

        protected IObjectReader ObjectReader;

        #region IStorageEngine Members

        public ITriggerManager GetLocalTriggerManager()
        {
            // First check if trigger manager has already been built for the engine
            ITriggerManager triggerManager;
            TriggerManagers.TryGetValue(this, out triggerManager);
            if (triggerManager != null)
                return triggerManager;

            triggerManager = new TriggerManager(this);
            TriggerManagers[this] = triggerManager;
            return triggerManager;
        }

        public void RemoveLocalTriggerManager()
        {
            TriggerManagers.Remove(this);
        }

        public virtual IObjects<T> GetObjects<T>(IQuery query, bool inMemory, int startIndex, int endIndex)
        {
            if (IsDbClosed)
                throw new OdbRuntimeException(NDatabaseError.OdbIsClosed.AddParameter(FileIdentification.Id));

            return ObjectReader.GetObjects<T>(query, inMemory, startIndex, endIndex);
        }

        public virtual void DefragmentTo(string newFileName)
        {
            var start = OdbTime.GetCurrentTimeInMs();
            var totalNbObjects = 0L;

            var newStorageEngine = (IStorageEngine)new StorageEngine(new FileIdentification(newFileName));
            IObjects<object> defragObjects;
            var j = 0;
            ClassInfo classInfo;

            // User classes
            IEnumerator iterator = GetMetaModel().GetUserClasses().GetEnumerator();
            while (iterator.MoveNext())
            {
                classInfo = (ClassInfo)iterator.Current;
                Debug.Assert(classInfo != null, "classInfo != null");

                if (OdbConfiguration.IsDebugEnabled(LogId))
                {
                    DLogger.Debug(string.Format("Reading {0} objects of type {1}",
                                                classInfo.CommitedZoneInfo.GetNumberbOfObjects(),
                                                classInfo.FullClassName));
                }

                var type = OdbClassPool.GetClass(classInfo.FullClassName);
                var criteriaQuery = new CriteriaQuery(type);

                defragObjects = GetObjects<object>(criteriaQuery, true, -1, -1);

                while (defragObjects.HasNext())
                {
                    newStorageEngine.Store(defragObjects.Next());
                    totalNbObjects++;

                    if (OdbConfiguration.IsDebugEnabled(LogId))
                    {
                        if (j % 10000 == 0)
                            DLogger.Info("\n" + totalNbObjects + " objects saved.");
                    }

                    j++;
                }
            }

            // System classes
            iterator = GetMetaModel().GetSystemClasses().GetEnumerator();
            while (iterator.MoveNext())
            {
                classInfo = (ClassInfo)iterator.Current;
                Debug.Assert(classInfo != null, "classInfo != null");

                if (OdbConfiguration.IsDebugEnabled(LogId))
                {
                    DLogger.Debug(string.Format("Reading {0} objects of type {1}",
                                                classInfo.CommitedZoneInfo.GetNumberbOfObjects(),
                                                classInfo.FullClassName));
                }

                var type = OdbClassPool.GetClass(classInfo.FullClassName);
                defragObjects = GetObjects<object>(new CriteriaQuery(type), true, -1, -1);

                while (defragObjects.HasNext())
                {
                    newStorageEngine.Store(defragObjects.Next());
                    totalNbObjects++;

                    if (OdbConfiguration.IsDebugEnabled(LogId))
                    {
                        if (j % 10000 == 0)
                            DLogger.Info("\n" + totalNbObjects + " objects saved.");
                    }

                    j++;
                }
            }

            newStorageEngine.Commit();
            newStorageEngine.Close();

            var time = OdbTime.GetCurrentTimeInMs() - start;

            if (OdbConfiguration.IsDebugEnabled(LogId))
                DLogger.Info(string.Format("New storage {0} created with {1} objects in {2} ms.", newFileName,
                                           totalNbObjects, time));
        }

        public abstract ISession GetSession(bool throwExceptionIfDoesNotExist);

        public void DeleteIndex(string className, string indexName, bool verbose)
        {
            var classInfo = GetMetaModel().GetClassInfo(className, true);

            if (!classInfo.HasIndex(indexName))
                throw new OdbRuntimeException(
                    NDatabaseError.IndexDoesNotExist.AddParameter(indexName).AddParameter(className));

            var classInfoIndex = classInfo.GetIndexWithName(indexName);

            if (verbose)
                DLogger.Info(string.Format("Deleting index {0} on class {1}", indexName, className));

            Delete(classInfoIndex);
            classInfo.RemoveIndex(classInfoIndex);

            if (verbose)
                DLogger.Info(string.Format("Index {0} deleted", indexName));
        }

        /// <summary>
        ///   Used to rebuild an index
        /// </summary>
        public virtual void RebuildIndex(string className, string indexName, bool verbose)
        {
            if (verbose)
                DLogger.Info(string.Format("Rebuilding index {0} on class {1}", indexName, className));

            var classInfo = GetMetaModel().GetClassInfo(className, true);

            if (!classInfo.HasIndex(indexName))
                throw new OdbRuntimeException(
                    NDatabaseError.IndexDoesNotExist.AddParameter(indexName).AddParameter(className));

            var classInfoIndex = classInfo.GetIndexWithName(indexName);
            DeleteIndex(className, indexName, verbose);

            AddIndexOn(className, indexName, classInfo.GetAttributeNames(classInfoIndex.AttributeIds), verbose,
                       !classInfoIndex.IsUnique);
        }

        public virtual void AddIndexOn(string className, string indexName, string[] indexFields, bool verbose,
                                       bool acceptMultipleValuesForSameKey)
        {
            var classInfo = GetMetaModel().GetClassInfo(className, true);
            if (classInfo.HasIndex(indexName))
                throw new OdbRuntimeException(
                    NDatabaseError.IndexAlreadyExist.AddParameter(indexName).AddParameter(className));

            var classInfoIndex = classInfo.AddIndexOn(indexName, indexFields, acceptMultipleValuesForSameKey);
            IBTree btree;

            if (acceptMultipleValuesForSameKey)
            {
                btree = new OdbBtreeMultiple(className, OdbConfiguration.GetDefaultIndexBTreeDegree(),
                                             new LazyOdbBtreePersister(this));
            }
            else
                btree = new OdbBtreeSingle(className, OdbConfiguration.GetDefaultIndexBTreeDegree(),
                                           new LazyOdbBtreePersister(this));

            classInfoIndex.BTree = btree;
            Store(classInfoIndex);

            // Now The index must be updated with all existing objects.
            if (classInfo.NumberOfObjects == 0)
            {
                // There are no objects. Nothing to do
                return;
            }
            if (verbose)
            {
                DLogger.Info(
                    string.Format("Creating index {0} on class {1} - Class has already {2} Objects. Updating index",
                                  indexName, className, classInfo.NumberOfObjects));

                DLogger.Info(string.Format("{0} : loading {1} objects from database", indexName,
                                           classInfo.NumberOfObjects));
            }

            // We must load all objects and insert them in the index!
            var type = OdbClassPool.GetClass(className);
            var objects = GetObjectInfos<object>(new CriteriaQuery(type), false, -1, -1, false);

            if (verbose)
                DLogger.Info(string.Format("{0} : {1} objects loaded", indexName, classInfo.NumberOfObjects));

            while (objects.HasNext())
            {
                var nnoi = (NonNativeObjectInfo)objects.Next();

                btree.Insert(classInfoIndex.ComputeKey(nnoi), nnoi.GetOid());
            }

            if (verbose)
                DLogger.Info(string.Format("{0} created!", indexName));
        }

        public virtual IObjects<T> GetObjectInfos<T>(IQuery query, bool inMemory, int startIndex, int endIndex,
                                                     bool returnObjects)
        {
            // Returns the query result handler for normal query result (that return a collection of objects)
            var queryResultAction = new CollectionQueryResultAction<object>(query, inMemory, this, returnObjects,
                                                                            GetObjectReader().GetInstanceBuilder());

            return ObjectReader.GetObjectInfos<T>(query, inMemory, startIndex, endIndex, returnObjects,
                                                  queryResultAction);
        }

        public virtual IObjects<T> GetObjects<T>(Type clazz, bool inMemory, int startIndex, int endIndex)
        {
            if (IsDbClosed)
                throw new OdbRuntimeException(NDatabaseError.OdbIsClosed.AddParameter(FileIdentification.Id));

            return ObjectReader.GetObjects<T>(new CriteriaQuery(clazz), inMemory, startIndex,
                                              endIndex);
        }

        public abstract ClassInfoList AddClasses(ClassInfoList arg1);

        public abstract void AddCommitListener(ICommitListener arg1);

        public abstract void AddDeleteTriggerFor(string arg1, DeleteTrigger arg2);

        public abstract void AddInsertTriggerFor(string arg1, InsertTrigger arg2);

        public abstract void AddSelectTriggerFor(string arg1, SelectTrigger arg2);

        public abstract void AddSession(ISession arg1, bool arg2);

        public abstract void AddUpdateTriggerFor(string arg1, UpdateTrigger arg2);

        public abstract ISession BuildDefaultSession();

        public abstract IObjectIntrospector BuildObjectIntrospector();

        public abstract ITriggerManager BuildTriggerManager();

        public abstract CheckMetaModelResult CheckMetaModelCompatibility(IDictionary<string, ClassInfo> arg1);

        public abstract void Close();

        public abstract void Commit();

        public abstract long Count(CriteriaQuery arg1);

        public abstract CriteriaQuery CriteriaQuery<T>(ICriterion criteria) where T : class;

        public abstract CriteriaQuery CriteriaQuery<T>()  where T : class;

        public abstract OID Delete<T>(T plainObject) where T : class;

        public abstract void DeleteObjectWithOid(OID arg1);

        public abstract void Disconnect<T>(T plainObject) where T : class;

        public abstract IList<FullIDInfo> GetAllObjectIdInfos(string arg1, bool arg2);

        public abstract IList<long> GetAllObjectIds();

        public abstract IFileIdentification GetBaseIdentification();

        public abstract IOdbList<ICommitListener> GetCommitListeners();

        public abstract ITransactionId GetCurrentTransactionId();

        public abstract IDatabaseId GetDatabaseId();

        public abstract NonNativeObjectInfo GetMetaObjectFromOid(OID arg1);

        public abstract object GetObjectFromOid(OID arg1);

        public abstract OID GetObjectId<T>(T plainObject, bool throwExceptionIfDoesNotExist) where T : class;

        public abstract ObjectInfoHeader GetObjectInfoHeaderFromOid(OID arg1);

        public abstract IObjectReader GetObjectReader();

        public abstract IObjectWriter GetObjectWriter();

        public abstract IRefactorManager GetRefactorManager();

        public abstract ITriggerManager GetTriggerManager();

        public abstract IValues GetValues(IValuesQuery arg1, int arg2, int arg3);

        public abstract bool IsClosed();

        public abstract void ResetCommitListeners();

        public abstract void Rollback();

        public abstract void SetCurrentIdBlockInfos(CurrentIdBlockInfo currentIdBlockInfo);

        public abstract void SetCurrentTransactionId(ITransactionId arg1);

        public abstract void SetDatabaseId(IDatabaseId arg1);

        public abstract OID Store<T>(OID oid, T plainObject) where T : class;

        public abstract OID Store<T>(T plainObject) where T : class;

        public abstract OID UpdateObject(NonNativeObjectInfo arg1, bool arg2);

        public abstract OID WriteObjectInfo(OID arg1, NonNativeObjectInfo arg2, long arg3, bool arg4);

        public abstract CurrentIdBlockInfo GetCurrentIdBlockInfo();
        public abstract IIdManager GetIdManager();

        #endregion

        protected virtual MetaModel GetMetaModel()
        {
            return GetSession(true).GetMetaModel();
        }
    }
}
