using System;
using System.Collections.Generic;
using System.Reflection;
using NDatabase2.Btree;
using NDatabase2.Odb.Core.BTree;
using NDatabase2.Odb.Core.Layers.Layer1.Introspector;
using NDatabase2.Odb.Core.Layers.Layer2.Meta;
using NDatabase2.Odb.Core.Layers.Layer3.Oid;
using NDatabase2.Odb.Core.Query;
using NDatabase2.Odb.Core.Query.Criteria;
using NDatabase2.Odb.Core.Transaction;
using NDatabase2.Odb.Core.Trigger;
using NDatabase2.Tool;
using NDatabase2.Tool.Wrappers;
using NDatabase2.Tool.Wrappers.List;
using NDatabase2.Tool.Wrappers.Map;

namespace NDatabase2.Odb.Core.Layers.Layer3.Engine
{
    internal abstract class AbstractStorageEngineReader : IStorageEngine
    {
        private static readonly Type UnclosedCriteriaQueryType = typeof (CriteriaQuery<>);

        private static readonly MethodInfo GenericGetObjectInfos =
            typeof(AbstractStorageEngineReader).GetMethod("GetObjectInfos", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly IDictionary<Type, Type> CriteriaQueryTypeCache =
            new Dictionary<Type, Type>();

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

        public virtual IObjects<T> GetObjects<T>(IQuery query, bool inMemory, int startIndex, int endIndex) where T : class
        {
            if (IsDbClosed)
                throw new OdbRuntimeException(NDatabaseError.OdbIsClosed.AddParameter(FileIdentification.Id));

            return ObjectReader.GetObjects<T>(query, inMemory, startIndex, endIndex);
        }

        public virtual void DefragmentTo(string newFileName)
        {
            var start = OdbTime.GetCurrentTimeInMs();
            var totalNbObjects = 0L;

            var newStorageEngine = new StorageEngine(new FileIdentification(newFileName));
            var j = 0;

            var criteriaQuery = new CriteriaQuery<object>();
            var defragObjects = GetObjects<object>(criteriaQuery, true, -1, -1);

            foreach (var defragObject in defragObjects)
            {
                newStorageEngine.Store(defragObject);
                totalNbObjects++;

                if (OdbConfiguration.IsLoggingEnabled())
                {
                    if (j % 10000 == 0)
                        DLogger.Info(string.Concat("\n", totalNbObjects.ToString(), " objects saved."));
                }

                j++;
            }

            newStorageEngine.Commit();
            newStorageEngine.Close();

            var time = OdbTime.GetCurrentTimeInMs() - start;

            if (OdbConfiguration.IsLoggingEnabled())
            {
                var nbObjectsAsString = totalNbObjects.ToString();
                var timeAsString = time.ToString();

                DLogger.Info(string.Format("New storage {0} created with {1} objects in {2} ms.", newFileName,
                                           nbObjectsAsString, timeAsString));
            }
        }

        private static IQuery PrepareCriteriaQuery(Type type)
        {
            Type criteriaQueryType;
            var success = CriteriaQueryTypeCache.TryGetValue(type, out criteriaQueryType);

            if (!success)
            {
                criteriaQueryType = UnclosedCriteriaQueryType.MakeGenericType(type);
                CriteriaQueryTypeCache.Add(type, criteriaQueryType);
            }

            return (IQuery) Activator.CreateInstance(criteriaQueryType);
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
                var numberOfObjectsAsString = classInfo.NumberOfObjects.ToString();
                DLogger.Info(
                    string.Format("Creating index {0} on class {1} - Class has already {2} Objects. Updating index",
                                  indexName, className, numberOfObjectsAsString));

                DLogger.Info(string.Format("{0} : loading {1} objects from database", indexName,
                                           numberOfObjectsAsString));
            }

            // We must load all objects and insert them in the index!
            var criteriaQuery = PrepareCriteriaQuery(classInfo.UnderlyingType);

            var methodInfo = GenericGetObjectInfos.MakeGenericMethod(classInfo.UnderlyingType);
            var objects = (IObjects<object>) methodInfo.Invoke(this, new object[] {criteriaQuery});

            if (verbose)
            {
                var numberOfObjectsAsString = classInfo.NumberOfObjects.ToString();
                DLogger.Info(string.Format("{0} : {1} objects loaded", indexName, numberOfObjectsAsString));
            }

            while (objects.HasNext())
            {
                var nnoi = (NonNativeObjectInfo)objects.Next();

                btree.Insert(classInfoIndex.ComputeKey(nnoi), nnoi.GetOid());
            }

            if (verbose)
                DLogger.Info(string.Format("{0} created!", indexName));
        }

        /// <summary>
        ///   Invoked by reflection!
        ///   //TODO: analyse what should be returned instead of object if  possible
        /// </summary>
        internal IObjects<object> GetObjectInfos<T>(IQuery query) where T : class
        {
            // Returns the query result handler for normal query result (that return a collection of objects)
            var queryResultAction = new QueryResultAction<object>(query, false, this, false,
                                                                            GetObjectReader().GetInstanceBuilder());

            return ObjectReader.GetObjectInfos<object,T>(query, false, -1, -1, false,
                                                  queryResultAction);
        }

        public virtual IObjects<T> GetObjects<T>(bool inMemory, int startIndex, int endIndex) where T : class
        {
            if (IsDbClosed)
                throw new OdbRuntimeException(NDatabaseError.OdbIsClosed.AddParameter(FileIdentification.Id));

            var criteriaQuery = PrepareCriteriaQuery(typeof(T));

            return ObjectReader.GetObjects<T>(criteriaQuery, inMemory, startIndex, endIndex);
        }

        public abstract ClassInfoList AddClasses(ClassInfoList arg1);

        public abstract void AddCommitListener(ICommitListener arg1);

        public abstract void AddDeleteTriggerFor(Type type, DeleteTrigger arg2);

        public abstract void AddInsertTriggerFor(Type type, InsertTrigger arg2);

        public abstract void AddSelectTriggerFor(Type type, SelectTrigger arg2);

        public abstract void AddUpdateTriggerFor(Type type, UpdateTrigger arg2);

        public abstract void AddSession(ISession arg1, bool arg2);

        public abstract ISession BuildDefaultSession();

        public abstract IObjectIntrospector BuildObjectIntrospector();

        public abstract ITriggerManager BuildTriggerManager();

        public abstract CheckMetaModelResult CheckMetaModelCompatibility(IDictionary<string, ClassInfo> arg1);

        public abstract void Close();

        public abstract void Commit();

        public abstract long Count<T>(CriteriaQuery<T> arg1) where T : class;

        public abstract CriteriaQuery<T> CriteriaQuery<T>(IConstraint criteria) where T : class;

        public abstract CriteriaQuery<T> CriteriaQuery<T>()  where T : class;

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

        public abstract IValues GetValues<T>(IValuesQuery arg1, int arg2, int arg3) where T : class;

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
