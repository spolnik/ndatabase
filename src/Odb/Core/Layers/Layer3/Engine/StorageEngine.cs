using System;
using System.Collections.Generic;
using System.Text;
using NDatabase.Odb.Core.Layers.Layer1.Introspector;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer3.Oid;
using NDatabase.Odb.Core.Layers.Layer3.Refactor;
using NDatabase.Odb.Core.Query;
using NDatabase.Odb.Core.Query.Criteria;
using NDatabase.Odb.Core.Query.Values;
using NDatabase.Odb.Core.Transaction;
using NDatabase.Odb.Core.Trigger;
using NDatabase.Tool;
using NDatabase.Tool.Wrappers;
using NDatabase.Tool.Wrappers.List;

namespace NDatabase.Odb.Core.Layers.Layer3.Engine
{
    /// <summary>
    ///   The storage Engine. The Local Storage Engine class in the most important class in ODB.
    /// </summary>
    /// <remarks>
    ///   The storage Engine <pre>The Local Storage Engine class in the most important class in ODB. It manages reading, writing and querying objects.
    ///                        All write operations are delegated to the ObjectWriter class.
    ///                        All read operations are delegated to the ObjectReader class.
    ///                        All Id operations are delegated to the IdManager class.
    ///                        All Introspecting operations are delegated to the ObjectIntrospector class.
    ///                        All Trigger operations are delegated to the TriggerManager class.
    ///                        All session related operations are executed by The Session class. Session Class using the Transaction
    ///                        class are responsible for ACID behavior.</pre>
    /// </remarks>
    internal sealed class StorageEngine : AbstractStorageEngineReader
    {
        private readonly IOdbList<ICommitListener> _commitListeners;
        private ISession _session;

        private CurrentIdBlockInfo _currentIdBlockInfo;

        /// <summary>
        ///   To keep track of current transaction Id
        /// </summary>
        private ITransactionId _currentTransactionId;

        private IDatabaseId _databaseId;

        /// <summary>
        ///   This is a visitor used to execute some specific action(like calling 'Before Insert Trigger') when introspecting an object
        /// </summary>
        private readonly IIntrospectionCallback _introspectionCallbackForInsert;

        /// <summary>
        ///   This is a visitor used to execute some specific action when introspecting an object
        /// </summary>
        private readonly IIntrospectionCallback _introspectionCallbackForUpdate;

        private IObjectIntrospector _objectIntrospector;
        private readonly IObjectWriter _objectWriter;
        private readonly ITriggerManager _triggerManager;

        /// <summary>
        ///   The database file name
        /// </summary>
        public StorageEngine(IFileIdentification parameters)
        {
            _currentIdBlockInfo = new CurrentIdBlockInfo();

            FileIdentification = parameters;
            
            IsDbClosed = false;

            // The check if it is a new Database must be executed before object
            // writer initialization. Because Object Writer Init
            // Creates the file so the check (which is based on the file existence
            // would always return false*/
            var isNewDatabase = IsNewDatabase();

            _commitListeners = new OdbList<ICommitListener>();
            var session = BuildDefaultSession();

            // Object Writer must be created before object Reader
            _objectWriter = new ObjectWriter(this);

            ObjectReader = new ObjectReader(this);
            AddSession(session, false);

            // If the file does not exist, then a default header must be created
            if (isNewDatabase)
            {
                _objectWriter.CreateEmptyDatabaseHeader(OdbTime.GetCurrentTimeInTicks());
            }
            else
            {
                GetObjectReader().ReadDatabaseHeader();
            }
            _objectWriter.AfterInit();
            _objectIntrospector = BuildObjectIntrospector();
            _triggerManager = BuildTriggerManager();
            // This forces the initialization of the meta model
            var metaModel = GetMetaModel();

            CheckMetaModelCompatibility(ClassIntrospector.Instrospect(metaModel.GetAllClasses()));

            // logically locks access to the file (only for this machine)
            FileMutex.GetInstance().OpenFile(GetStorageDeviceName());
            // Updates the Transaction Id in the file
            _objectWriter.FileSystemProcessor.WriteLastTransactionId(GetCurrentTransactionId());
            _objectWriter.SetTriggerManager(_triggerManager);
            _introspectionCallbackForInsert = new InstrumentationCallbackForStore(_triggerManager, false);
            _introspectionCallbackForUpdate = new InstrumentationCallbackForStore(_triggerManager, true);
        }

        public override void AddSession(ISession session, bool readMetamodel)
        {
            // Associate current session to the fsi -> all transaction writes
            // will be applied to this FileSystemInterface
            session.SetFileSystemInterfaceToApplyTransaction(_objectWriter.FileSystemProcessor.FileSystemInterface);

            if (!readMetamodel)
                return;

            ObjectReader.ReadDatabaseHeader();

            var metaModel = new MetaModel();
            session.SetMetaModel(metaModel);
            ObjectReader.LoadMetaModel(metaModel, true);

            // Updates the Transaction Id in the file
            _objectWriter.FileSystemProcessor.WriteLastTransactionId(GetCurrentTransactionId());
        }

        /// <summary>
        ///   Receive the current class info (loaded from current classes present on runtime and check against the persisted meta model
        /// </summary>
        /// <param name="currentCIs"> </param>
        public override CheckMetaModelResult CheckMetaModelCompatibility(IDictionary<string, ClassInfo> currentCIs)
        {
            ClassInfo currentCI;
            ClassInfoCompareResult result;
            var checkMetaModelResult = new CheckMetaModelResult();
            
            // User classes
            foreach (var persistedCI in GetMetaModel().GetUserClasses())
            {
                currentCI = currentCIs[persistedCI.FullClassName];
                result = persistedCI.ExtractDifferences(currentCI, true);

                if (!result.IsCompatible())
                    throw new OdbRuntimeException(NDatabaseError.IncompatibleMetamodel.AddParameter(result.ToString()));

                if (result.HasCompatibleChanges())
                    checkMetaModelResult.Add(result);
            }
           
            foreach (var persistedCI in GetMetaModel().GetSystemClasses())
            {
                currentCI = currentCIs[persistedCI.FullClassName];
                result = persistedCI.ExtractDifferences(currentCI, true);

                if (!result.IsCompatible())
                    throw new OdbRuntimeException(NDatabaseError.IncompatibleMetamodel.AddParameter(result.ToString()));

                if (result.HasCompatibleChanges())
                    checkMetaModelResult.Add(result);
            }

            for (var i = 0; i < checkMetaModelResult.Size(); i++)
            {
                result = checkMetaModelResult.GetResults()[i];
                DLogger.Info(string.Format("Class {0} has changed :", result.GetFullClassName()));
                DLogger.Info(result.ToString());
            }

            if (!checkMetaModelResult.GetResults().IsEmpty())
            {
                UpdateMetaModel();
                checkMetaModelResult.SetModelHasBeenUpdated(true);
            }

            return checkMetaModelResult;
        }

        public override OID Store<T>(T plainObject)
        {
            if (IsDbClosed)
            {
                throw new OdbRuntimeException(
                    NDatabaseError.OdbIsClosed.AddParameter(FileIdentification.Id));
            }

            var newOid = InternalStore(null, plainObject);
            GetSession(true).GetCache().ClearInsertingObjects();

            return newOid;
        }

        public override OID Store<T>(OID oid, T plainObject)
        {
            if (IsDbClosed)
            {
                throw new OdbRuntimeException(
                    NDatabaseError.OdbIsClosed.AddParameter(FileIdentification.Id));
            }

            var newOid = InternalStore(oid, plainObject);
            GetSession(true).GetCache().ClearInsertingObjects();

            return newOid;
        }

        /// <summary>
        ///   Warning,
        /// </summary>
        public override void DeleteObjectWithOid(OID oid)
        {
            var lsession = GetSession(true);
            var cache = lsession.GetCache();
            // Check if oih is in the cache
            var objectInfoHeader = cache.GetObjectInfoHeaderByOid(oid, false) ??
                                   ObjectReader.ReadObjectInfoHeaderFromOid(oid, true);

            _objectWriter.Delete(objectInfoHeader);
            // removes the object from the cache
            cache.RemoveObjectByOid(objectInfoHeader.GetOid());
        }

        /// <summary>
        ///   Actually deletes an object database
        /// </summary>
        public override OID Delete<T>(T plainObject)
        {
            var lsession = GetSession(true);
            if (lsession.IsRollbacked())
            {
                throw new OdbRuntimeException(
                    NDatabaseError.OdbHasBeenRollbacked.AddParameter(FileIdentification.ToString()));
            }

            var cache = lsession.GetCache();

            // Get header of the object (position, previous object position, next
            // object position and class info position)
            // Header must come from cache because it may have been updated before.
            var header = cache.GetObjectInfoHeaderFromObject(plainObject);
            if (header == null)
            {
                var cachedOid = cache.GetOid(plainObject);
                
                if (cachedOid == null)
                {
                    throw new OdbRuntimeException(
                        NDatabaseError.ObjectDoesNotExistInCacheForDelete.AddParameter(plainObject.GetType().FullName).
                            AddParameter(plainObject.ToString()));
                }

                header = ObjectReader.ReadObjectInfoHeaderFromOid(cachedOid, false);
            }

            _triggerManager.ManageDeleteTriggerBefore(plainObject.GetType(), plainObject, header.GetOid());
            var oid = _objectWriter.Delete(header);
            _triggerManager.ManageDeleteTriggerAfter(plainObject.GetType(), plainObject, oid);
            // removes the object from the cache
            cache.RemoveObjectByOid(header.GetOid());

            return oid;
        }

        public override IIdManager GetIdManager()
        {
            return new IdManager(GetObjectWriter(), GetObjectReader(), GetCurrentIdBlockInfo());
        }

        public override void Close()
        {
            if (IsDbClosed)
            {
                throw new OdbRuntimeException(
                    NDatabaseError.OdbIsClosed.AddParameter(FileIdentification.Id));
            }

            // When not local (client server) session can be null
            var lsession = GetSession(true);
            _objectWriter.FileSystemProcessor.WriteLastOdbCloseStatus(true, false);

            _objectWriter.FileSystemProcessor.Flush();
            if (lsession.TransactionIsPending())
                throw new OdbRuntimeException(NDatabaseError.TransactionIsPending.AddParameter(lsession.GetId()));

            IsDbClosed = true;
            ObjectReader.Close();
            _objectWriter.Close();
            // Logically release this file (only for this machine)
            FileMutex.GetInstance().ReleaseFile(GetStorageDeviceName());
            lsession.Close();

            if (_objectIntrospector != null)
            {
                _objectIntrospector.Clear();
                _objectIntrospector = null;
            }

            // remove trigger manager
            RemoveLocalTriggerManager();
        }

        public override long Count<T>(CriteriaQuery<T> query)
        {
            if (IsDbClosed)
            {
                throw new OdbRuntimeException(
                    NDatabaseError.OdbIsClosed.AddParameter(FileIdentification.Id));
            }

            var valuesQuery = new ValuesCriteriaQuery<T>(query).Count("count");
            var values = GetValues<T>(valuesQuery, -1, -1);
            var count = (long) values.NextValues().GetByIndex(0);

            return count;
        }

        public override IObjectReader GetObjectReader()
        {
            return ObjectReader;
        }

        public override IObjectWriter GetObjectWriter()
        {
            return _objectWriter;
        }

        public override void Commit()
        {
            if (IsClosed())
            {
                throw new OdbRuntimeException(
                    NDatabaseError.OdbIsClosed.AddParameter(FileIdentification.Id));
            }

            GetSession(true).Commit();
            _objectWriter.FileSystemProcessor.Flush();
        }

        public override void Rollback()
        {
            GetSession(true).Rollback();
        }

        public override OID GetObjectId<T>(T plainObject, bool throwExceptionIfDoesNotExist)
        {
            if (plainObject != null)
            {
                var oid = GetSession(true).GetCache().GetOid(plainObject);
                
                if (oid == null && throwExceptionIfDoesNotExist)
                    throw new OdbRuntimeException(NDatabaseError.UnknownObjectToGetOid.AddParameter(plainObject.ToString()));

                return oid;
            }

            throw new OdbRuntimeException(NDatabaseError.OdbCanNotReturnOidOfNullObject);
        }

        public override object GetObjectFromOid(OID oid)
        {
            if (oid == null)
                throw new OdbRuntimeException(NDatabaseError.CanNotGetObjectFromNullOid);

            var nnoi = GetObjectReader().ReadNonNativeObjectInfoFromOid(null, oid, true, true);

            if (nnoi.IsDeletedObject())
                throw new OdbRuntimeException(NDatabaseError.ObjectIsMarkedAsDeletedForOid.AddParameter(oid));

            var objectFromOid = nnoi.GetObject() ??
                                GetObjectReader().GetInstanceBuilder().BuildOneInstance(nnoi,
                                                                                        GetSession(true).
                                                                                            GetCache());

            var lsession = GetSession(true);
            // Here oid can be different from nnoi.getOid(). This is the case when
            // the oid is an external oid. That`s why we use
            // nnoi.getOid() to put in the cache
            lsession.GetCache().AddObject(nnoi.GetOid(), objectFromOid, nnoi.GetHeader());
            lsession.GetTmpCache().ClearObjectInfos();

            return objectFromOid;
        }

        public override NonNativeObjectInfo GetMetaObjectFromOid(OID oid)
        {
            var nnoi = GetObjectReader().ReadNonNativeObjectInfoFromOid(null, oid, true, false);
            GetSession(true).GetTmpCache().ClearObjectInfos();
            return nnoi;
        }

        public override ObjectInfoHeader GetObjectInfoHeaderFromOid(OID oid)
        {
            return GetObjectReader().ReadObjectInfoHeaderFromOid(oid, true);
        }

        public override IList<long> GetAllObjectIds()
        {
            return ObjectReader.GetAllIds(IdTypes.Object);
        }

        public override IList<FullIDInfo> GetAllObjectIdInfos(string objectType, bool displayObjects)
        {
            return ObjectReader.GetAllIdInfos(objectType, IdTypes.Object, displayObjects);
        }

        public override void SetDatabaseId(IDatabaseId databaseId)
        {
            _databaseId = databaseId;
        }

        public override void SetCurrentIdBlockInfos(CurrentIdBlockInfo currentIdBlockInfo)
        {
            _currentIdBlockInfo = currentIdBlockInfo;
        }

        public override IDatabaseId GetDatabaseId()
        {
            return _databaseId;
        }

        public override CurrentIdBlockInfo GetCurrentIdBlockInfo()
        {
            return _currentIdBlockInfo;
        }

        public override bool IsClosed()
        {
            return IsDbClosed;
        }

        public override IFileIdentification GetBaseIdentification()
        {
            return FileIdentification;
        }

        public override OID WriteObjectInfo(OID oid, NonNativeObjectInfo aoi, long position, bool updatePointers)
        {
            // TODO check if it must be written in transaction
            return _objectWriter.WriteNonNativeObjectInfo(oid, aoi, position, updatePointers, true);
        }

        public override OID UpdateObject(NonNativeObjectInfo nnoi, bool forceUpdate)
        {
            return _objectWriter.UpdateNonNativeObjectInfo(nnoi, forceUpdate);
        }

        public override IValues GetValues<T>(IValuesQuery query, int startIndex, int endIndex)
        {
            if (IsDbClosed)
            {
                throw new OdbRuntimeException(
                    NDatabaseError.OdbIsClosed.AddParameter(FileIdentification.Id));
            }

            return ObjectReader.GetValues<T>(query, startIndex, endIndex);
        }

        public override void AddCommitListener(ICommitListener commitListener)
        {
            _commitListeners.Add(commitListener);
        }

        public override IOdbList<ICommitListener> GetCommitListeners()
        {
            return _commitListeners;
        }

        public override IRefactorManager GetRefactorManager()
        {
            return new RefactorManager(this);
        }

        public override void ResetCommitListeners()
        {
            _commitListeners.Clear();
        }

        public override ITransactionId GetCurrentTransactionId()
        {
            return _currentTransactionId;
        }

        public override void SetCurrentTransactionId(ITransactionId transactionId)
        {
            _currentTransactionId = transactionId;
        }

        public override void Disconnect<T>(T plainObject)
        {
            GetSession(true).RemoveObjectFromCache(plainObject);
        }

        public override ITriggerManager GetTriggerManager()
        {
            return _triggerManager;
        }

        public override void AddDeleteTriggerFor(Type type, DeleteTrigger trigger)
        {
            _triggerManager.AddDeleteTriggerFor(type, trigger);
        }

        public override void AddInsertTriggerFor(Type type, InsertTrigger trigger)
        {
            _triggerManager.AddInsertTriggerFor(type, trigger);
        }

        public override void AddSelectTriggerFor(Type type, SelectTrigger trigger)
        {
            _triggerManager.AddSelectTriggerFor(type, trigger);
        }

        public override void AddUpdateTriggerFor(Type type, UpdateTrigger trigger)
        {
            _triggerManager.AddUpdateTriggerFor(type, trigger);
        }

        public override CriteriaQuery<T> CriteriaQuery<T>(ICriterion criterion)
        {
            var criteriaQuery = new CriteriaQuery<T>(criterion);
            criteriaQuery.SetStorageEngine(this);

            if (criterion != null)
                criterion.Ready();

            return criteriaQuery;
        }

        public override CriteriaQuery<T> CriteriaQuery<T>()
        {
            var criteriaQuery = new CriteriaQuery<T>();
            criteriaQuery.SetStorageEngine(this);
            return criteriaQuery;
        }

        public override ClassInfoList AddClasses(ClassInfoList classInfoList)
        {
            return GetObjectWriter().AddClasses(classInfoList);
        }

        public override ISession BuildDefaultSession()
        {
            _session = new LocalSession(this);
            return _session;
        }

        public override ISession GetSession(bool throwExceptionIfDoesNotExist)
        {
            return _session;
        }

        public override IObjectIntrospector BuildObjectIntrospector()
        {
            return new ObjectIntrospector(this);
        }

        public override ITriggerManager BuildTriggerManager()
        {
            return GetLocalTriggerManager();
        }

        public void UpdateMetaModel()
        {
            if (OdbConfiguration.IsDebugEnabled())
                DLogger.Info("Automatic refactoring : updating meta model");
            
            var metaModel = GetMetaModel();

            foreach (var userClass in metaModel.GetUserClasses())
                _objectWriter.UpdateClassInfo(userClass, true);

            foreach (var systemClass in metaModel.GetSystemClasses())
                _objectWriter.UpdateClassInfo(systemClass, true);
        }

        private string GetStorageDeviceName()
        {
            return FileIdentification.Id;
        }

        private bool IsNewDatabase()
        {
            return FileIdentification.IsNew();
        }

        /// <summary>
        ///   Store an object with the specific id
        /// </summary>
        /// <param name="oid"> </param>
        /// <param name="plainObject"> </param>
        private OID InternalStore<T>(OID oid, T plainObject) where T : class
        {
            if (GetSession(true).IsRollbacked())
            {
                throw new OdbRuntimeException(
                    NDatabaseError.OdbHasBeenRollbacked.AddParameter(GetBaseIdentification().ToString()));
            }

            if (plainObject == null)
                throw new OdbRuntimeException(NDatabaseError.OdbCanNotStoreNullObject);

            var type = typeof(T);
            if (OdbType.IsNative(type))
            {
                throw new OdbRuntimeException(
                    NDatabaseError.OdbCanNotStoreNativeObjectDirectly.AddParameter(type.FullName).AddParameter(
                        OdbType.GetFromClass(type).Name).AddParameter(type.FullName));
            }

            // The object must be transformed into meta representation
            ClassInfo classInfo;

            // first checks if the class of this object already exist in the
            // metamodel
            if (GetMetaModel().ExistClass(type))
            {
                classInfo = GetMetaModel().GetClassInfo(type, true);
            }
            else
            {
                var classInfoList = ClassIntrospector.Introspect(plainObject.GetType(), true);

                // All new classes found
                _objectWriter.AddClasses(classInfoList);
                classInfo = classInfoList.GetMainClassInfo();
            }

            // first detects if we must perform an insert or an update
            // If object is in the cache, we must perform an update, else an insert
            var cache = GetSession(true).GetCache();

            var cacheOid = cache.IdOfInsertingObject(plainObject);
            if (cacheOid != null)
                return cacheOid;

            // throw new ODBRuntimeException("Inserting meta representation of
            // an object without the object itself is not yet supported");
            var mustUpdate = cache.Contains(plainObject);

            // The introspection callback is used to execute some specific task (like calling trigger, for example) while introspecting the object
            var callback = _introspectionCallbackForInsert;
            if (mustUpdate)
                callback = _introspectionCallbackForUpdate;

            // Transform the object into an ObjectInfo
            var nnoi =
                (NonNativeObjectInfo)
                _objectIntrospector.GetMetaRepresentation(plainObject, classInfo, true, null, callback);

            // During the introspection process, if object is to be updated, then the oid has been set
            mustUpdate = nnoi.GetOid() != null;

            if (mustUpdate)
                return _objectWriter.UpdateNonNativeObjectInfo(nnoi, false);
            return _objectWriter.InsertNonNativeObject(oid, nnoi, true);
        }

        /// <summary>
        ///   Returns a string of the meta-model
        /// </summary>
        /// <returns> The engine description </returns>
        public override string ToString()
        {
            var buffer = new StringBuilder();
            buffer.Append(GetMetaModel());
            return buffer.ToString();
        }
    }
}
