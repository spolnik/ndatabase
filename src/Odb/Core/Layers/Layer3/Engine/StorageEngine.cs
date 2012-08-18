using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NDatabase.Odb.Core.Layers.Layer1.Introspector;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Query;
using NDatabase.Odb.Core.Query.Criteria;
using NDatabase.Odb.Core.Transaction;
using NDatabase.Odb.Core.Trigger;
using NDatabase.Odb.Impl.Core.Layers.Layer1.Introspector;
using NDatabase.Odb.Impl.Core.Layers.Layer3.Engine;
using NDatabase.Odb.Impl.Core.Layers.Layer3.Oid;
using NDatabase.Odb.Impl.Core.Query.Criteria;
using NDatabase.Odb.Impl.Core.Query.Values;
using NDatabase.Odb.Impl.Core.Transaction;
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
    public sealed class StorageEngine : AbstractStorageEngineReader
    {
        private IOdbList<ICommitListener> _commitListeners;
        private ISession _session;

        /// <summary>
        ///   The max id already allocated in the current id block
        /// </summary>
        private OID _currentIdBlockMaxOid;

        /// <summary>
        ///   The current id block number
        /// </summary>
        private int _currentIdBlockNumber;

        /// <summary>
        ///   The position of the current block where IDs are stored
        /// </summary>
        private long _currentIdBlockPosition;

        /// <summary>
        ///   To keep track of current transaction Id
        /// </summary>
        private ITransactionId _currentTransactionId;

        private IDatabaseId _databaseId;

        /// <summary>
        ///   This is a visitor used to execute some specific action(like calling 'Before Insert Trigger') when introspecting an object
        /// </summary>
        private IIntrospectionCallback _introspectionCallbackForInsert;

        /// <summary>
        ///   This is a visitor used to execute some specific action when introspecting an object
        /// </summary>
        private IIntrospectionCallback _introspectionCallbackForUpdate;

        private IObjectIntrospector _objectIntrospector;
        private IObjectWriter _objectWriter;
        private ITriggerManager _triggerManager;

        /// <summary>
        ///   The database file name
        /// </summary>
        public StorageEngine(IBaseIdentification parameters)
        {
            CoreProvider = OdbConfiguration.GetCoreProvider();
            BaseIdentification = parameters;
            Init();
        }

        public override void AddSession(ISession session, bool readMetamodel)
        {
            // Associate current session to the fsi -> all transaction writes
            // will be applied to this FileSystemInterface
            session.SetFileSystemInterfaceToApplyTransaction(_objectWriter.GetFsi());

            if (!readMetamodel)
                return;

            ObjectReader.ReadDatabaseHeader();

            MetaModel metaModel = new SessionMetaModel();
            session.SetMetaModel(metaModel);
            metaModel = ObjectReader.ReadMetaModel(metaModel, true);
            // Updates the Transaction Id in the file
            _objectWriter.WriteLastTransactionId(GetCurrentTransactionId());
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
                currentCI = currentCIs[persistedCI.GetFullClassName()];
                result = persistedCI.ExtractDifferences(currentCI, true);

                if (!result.IsCompatible())
                    throw new OdbRuntimeException(NDatabaseError.IncompatibleMetamodel.AddParameter(result.ToString()));

                if (result.HasCompatibleChanges())
                    checkMetaModelResult.Add(result);
            }
           
            foreach (var persistedCI in GetMetaModel().GetSystemClasses())
            {
                currentCI = currentCIs[persistedCI.GetFullClassName()];
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

        public override OID Store(object @object)
        {
            return Store(null, @object);
        }

        public override OID Store(OID oid, object @object)
        {
            if (IsDbClosed)
            {
                throw new OdbRuntimeException(
                    NDatabaseError.OdbIsClosed.AddParameter(BaseIdentification.GetIdentification()));
            }

            // triggers before
            // triggerManager.manageInsertTriggerBefore(object.getClass().getName(),
            // object);
            var newOid = InternalStore(oid, @object);
            // triggers after - fixme
            // triggerManager.manageInsertTriggerAfter(object.getClass().getName(),
            // object, newOid);
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
            var objectInfoHeader = cache.GetObjectInfoHeaderFromOid(oid, false) ??
                                   ObjectReader.ReadObjectInfoHeaderFromOid(oid, true);

            if (OdbConfiguration.ReconnectObjectsToSession())
                CacheFactory.GetCrossSessionCache(GetBaseIdentification().GetIdentification()).RemoveOid(oid);

            _objectWriter.Delete(objectInfoHeader);
            // removes the object from the cache
            cache.RemoveObjectWithOid(objectInfoHeader.GetOid());
        }

        /// <summary>
        ///   Actually deletes an object database
        /// </summary>
        public override OID Delete(object @object)
        {
            var lsession = GetSession(true);
            if (lsession.IsRollbacked())
            {
                throw new OdbRuntimeException(
                    NDatabaseError.OdbHasBeenRollbacked.AddParameter(BaseIdentification.ToString()));
            }

            if (@object == null)
                throw new OdbRuntimeException(NDatabaseError.OdbCanNotDeleteNullObject);

            var cache = lsession.GetCache();
            const bool throwExceptionIfNotInCache = false;

            // Get header of the object (position, previous object position, next
            // object position and class info position)
            // Header must come from cache because it may have been updated before.
            var header = cache.GetObjectInfoHeaderFromObject(@object, throwExceptionIfNotInCache);
            if (header == null)
            {
                var cachedOid = cache.GetOid(@object, false);
                //reconnect object is turn on tries to get object from cross session
                if (cachedOid == null && OdbConfiguration.ReconnectObjectsToSession())
                {
                    var crossSessionCache =
                        CacheFactory.GetCrossSessionCache(GetBaseIdentification().GetIdentification());
                    cachedOid = crossSessionCache.GetOid(@object);
                }

                if (cachedOid == null)
                {
                    throw new OdbRuntimeException(
                        NDatabaseError.ObjectDoesNotExistInCacheForDelete.AddParameter(@object.GetType().FullName).
                            AddParameter(@object.ToString()));
                }

                header = ObjectReader.ReadObjectInfoHeaderFromOid(cachedOid, false);
            }

            _triggerManager.ManageDeleteTriggerBefore(@object.GetType().FullName, @object, header.GetOid());
            var oid = _objectWriter.Delete(header);
            _triggerManager.ManageDeleteTriggerAfter(@object.GetType().FullName, @object, oid);
            // removes the object from the cache
            cache.RemoveObjectWithOid(header.GetOid());

            if (OdbConfiguration.ReconnectObjectsToSession())
                CacheFactory.GetCrossSessionCache(GetBaseIdentification().GetIdentification()).RemoveObject(@object);

            return oid;
        }

        public override void Close()
        {
            if (IsDbClosed)
            {
                throw new OdbRuntimeException(
                    NDatabaseError.OdbIsClosed.AddParameter(BaseIdentification.GetIdentification()));
            }

            // When not local (client server) session can be null
            var lsession = GetSession(true);
            if (BaseIdentification.CanWrite())
                _objectWriter.WriteLastOdbCloseStatus(true, false);

            _objectWriter.Flush();
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
            CoreProvider.RemoveLocalTriggerManager(this);
        }

        public override long Count(CriteriaQuery query)
        {
            if (IsDbClosed)
            {
                throw new OdbRuntimeException(
                    NDatabaseError.OdbIsClosed.AddParameter(BaseIdentification.GetIdentification()));
            }

            var valuesQuery = new ValuesCriteriaQuery(query).Count("count");
            var values = GetValues(valuesQuery, -1, -1);
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
                    NDatabaseError.OdbIsClosed.AddParameter(BaseIdentification.GetIdentification()));
            }

            GetSession(true).Commit();
            _objectWriter.Flush();
        }

        public override void Rollback()
        {
            GetSession(true).Rollback();
        }

        public override OID GetObjectId(object @object, bool throwExceptionIfDoesNotExist)
        {
            if (@object != null)
            {
                var oid = GetSession(true).GetCache().GetOid(@object, false);
                // If cross cache session is on, just check if current object has the OID on the cache
                if (oid == null && OdbConfiguration.ReconnectObjectsToSession())
                {
                    var cache =
                        CacheFactory.GetCrossSessionCache(GetBaseIdentification().GetIdentification());

                    oid = cache.GetOid(@object);
                    if (oid != null)
                        return oid;
                }

                oid = GetSession(true).GetCache().GetOid(@object, false);
                
                if (oid == null && throwExceptionIfDoesNotExist)
                    throw new OdbRuntimeException(NDatabaseError.UnknownObjectToGetOid.AddParameter(@object.ToString()));

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

            var objectFromOid = nnoi.GetObject() ?? GetObjectReader().GetInstanceBuilder().BuildOneInstance(nnoi);

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

        public override void SetCurrentIdBlockInfos(long currentBlockPosition, int currentBlockNumber, OID maxId)
        {
            _currentIdBlockPosition = currentBlockPosition;
            _currentIdBlockNumber = currentBlockNumber;
            _currentIdBlockMaxOid = maxId;
        }

        public override int GetCurrentIdBlockNumber()
        {
            return _currentIdBlockNumber;
        }

        public override long GetCurrentIdBlockPosition()
        {
            return _currentIdBlockPosition;
        }

        public override IDatabaseId GetDatabaseId()
        {
            return _databaseId;
        }

        public override OID GetCurrentIdBlockMaxOid()
        {
            return _currentIdBlockMaxOid;
        }

        public override bool IsClosed()
        {
            return IsDbClosed;
        }

        public override IBaseIdentification GetBaseIdentification()
        {
            return BaseIdentification;
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

        public override IValues GetValues(IValuesQuery query, int startIndex, int endIndex)
        {
            if (IsDbClosed)
            {
                throw new OdbRuntimeException(
                    NDatabaseError.OdbIsClosed.AddParameter(BaseIdentification.GetIdentification()));
            }

            return ObjectReader.GetValues(query, startIndex, endIndex);
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
            return CoreProvider.GetRefactorManager(this);
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

        public override void Disconnect(object @object)
        {
            GetSession(true).RemoveObjectFromCache(@object);

            //remove from cross session cache
            if (OdbConfiguration.ReconnectObjectsToSession())
                CacheFactory.GetCrossSessionCache(GetBaseIdentification().GetIdentification()).RemoveObject(@object);
        }

        /// <summary>
        ///   Reconnect an object to the current session.
        /// </summary>
        /// <remarks>
        ///   Reconnect an object to the current session. It connects the object and all the dependent objects (Objects accessible from the object graph of the root object
        /// </remarks>
        public override void Reconnect(object @object)
        {
            if (@object == null)
                throw new OdbRuntimeException(NDatabaseError.ReconnectCanReconnectNullObject);

            var crossSessionCache = CacheFactory.GetCrossSessionCache(GetBaseIdentification().GetIdentification());

            var oid = crossSessionCache.GetOid(@object);
            //in some situation the user can control the disconnect and reconnect
            //so before throws an exception test if in the current session 
            //there is the object on the cache
            if (oid == null)
                throw new OdbRuntimeException(NDatabaseError.CrossSessionCacheNullOidForObject.AddParameter(@object));

            var objectInfoHeader = ObjectReader.ReadObjectInfoHeaderFromOid(oid, false);
            GetSession(true).AddObjectToCache(oid, @object, objectInfoHeader);

            // Retrieve Dependent Objects
            var getObjectsCallback = new GetDependentObjectIntrospectingCallback();
            var classInfo = GetSession(true).GetMetaModel().GetClassInfoFromId(objectInfoHeader.GetClassInfoId());

            _objectIntrospector.GetMetaRepresentation(@object, classInfo, true, null, getObjectsCallback);

            var dependentObjects = getObjectsCallback.GetObjects();
            var iterator = dependentObjects.GetEnumerator();

            while (iterator.MoveNext())
            {
                var current = iterator.Current;

                if (current == null)
                    continue;

                oid = crossSessionCache.GetOid(current);
                if (oid == null)
                    throw new OdbRuntimeException(NDatabaseError.CrossSessionCacheNullOidForObject.AddParameter(current));

                objectInfoHeader = ObjectReader.ReadObjectInfoHeaderFromOid(oid, false);
                GetSession(true).AddObjectToCache(oid, current, objectInfoHeader);
            }
        }

        public override ITriggerManager GetTriggerManager()
        {
            return _triggerManager;
        }

        public override void AddDeleteTriggerFor(string className, DeleteTrigger trigger)
        {
            _triggerManager.AddDeleteTriggerFor(className, trigger);
        }

        public override void AddInsertTriggerFor(string className, InsertTrigger trigger)
        {
            _triggerManager.AddInsertTriggerFor(className, trigger);
        }

        public override void AddSelectTriggerFor(string className, SelectTrigger trigger)
        {
            _triggerManager.AddSelectTriggerFor(className, trigger);
        }

        public override void AddUpdateTriggerFor(string className, UpdateTrigger trigger)
        {
            _triggerManager.AddUpdateTriggerFor(className, trigger);
        }

        public override CriteriaQuery CriteriaQuery(Type clazz, ICriterion criterion)
        {
            var criteriaQuery = new CriteriaQuery(clazz, criterion);
            criteriaQuery.SetStorageEngine(this);

            if (criterion != null)
                criterion.Ready();

            return criteriaQuery;
        }

        public override CriteriaQuery CriteriaQuery(Type clazz)
        {
            var criteriaQuery = new CriteriaQuery(clazz);
            criteriaQuery.SetStorageEngine(this);
            return criteriaQuery;
        }

        public override ClassInfoList AddClasses(ClassInfoList classInfoList)
        {
            return GetObjectWriter().AddClasses(classInfoList);
        }

        public override ISession BuildDefaultSession()
        {
            _session = CoreProvider.GetLocalSession(this);
            return _session;
        }

        public override ISession GetSession(bool throwExceptionIfDoesNotExist)
        {
            return _session;
        }

        public override IObjectIntrospector BuildObjectIntrospector()
        {
            return CoreProvider.GetLocalObjectIntrospector(this);
        }

        public override IObjectReader BuildObjectReader()
        {
            return CoreProvider.GetObjectReader(this);
        }

        public override IObjectWriter BuildObjectWriter()
        {
            return CoreProvider.GetObjectWriter(this);
        }

        public override ITriggerManager BuildTriggerManager()
        {
            return CoreProvider.GetLocalTriggerManager(this);
        }

        private void Init()
        {
            IsDbClosed = false;

            // The check if it is a new Database must be executed before object
            // writer initialization. Because Object Writer Init
            // Creates the file so the check (which is based on the file existence
            // would always return false*/
            var isNewDatabase = IsNewDatabase();

            _commitListeners = new OdbArrayList<ICommitListener>();
            var session = BuildDefaultSession();

            // Object Writer must be created before object Reader
            _objectWriter = BuildObjectWriter();

            // Object writer is a two Phase init object
            _objectWriter.Init2();

            ObjectReader = BuildObjectReader();
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

            if (OdbConfiguration.CheckModelCompatibility())
                CheckMetaModelCompatibility(CoreProvider.GetClassIntrospector().Instrospect(metaModel.GetAllClasses()));

            // logically locks access to the file (only for this machine)
            FileMutex.GetInstance().OpenFile(GetStorageDeviceName());
            // Updates the Transaction Id in the file
            _objectWriter.WriteLastTransactionId(GetCurrentTransactionId());
            _objectWriter.SetTriggerManager(_triggerManager);
            _introspectionCallbackForInsert = new DefaultInstrumentationCallbackForStore(this, _triggerManager, false);
            _introspectionCallbackForUpdate = new DefaultInstrumentationCallbackForStore(this, _triggerManager, true);
        }

        public void UpdateMetaModel()
        {
            var metaModel = GetMetaModel();
            DLogger.Info("Automatic refactoring : updating meta model");
            
            // User classes
            IEnumerator iterator = metaModel.GetUserClasses().GetEnumerator();
            while (iterator.MoveNext())
                _objectWriter.UpdateClassInfo((ClassInfo) iterator.Current, true);

            // System classes
            iterator = metaModel.GetSystemClasses().GetEnumerator();
            while (iterator.MoveNext())
                _objectWriter.UpdateClassInfo((ClassInfo) iterator.Current, true);
        }

        private string GetStorageDeviceName()
        {
            return BaseIdentification.GetIdentification();
        }

        private bool IsNewDatabase()
        {
            return BaseIdentification.IsNew();
        }

        /// <summary>
        ///   Store an object with the specific id
        /// </summary>
        /// <param name="oid"> </param>
        /// <param name="object"> </param>
        private OID InternalStore(OID oid, object @object)
        {
            if (GetSession(true).IsRollbacked())
            {
                throw new OdbRuntimeException(
                    NDatabaseError.OdbHasBeenRollbacked.AddParameter(GetBaseIdentification().ToString()));
            }

            if (@object == null)
                throw new OdbRuntimeException(NDatabaseError.OdbCanNotStoreNullObject);

            var type = @object.GetType();
            if (OdbType.IsNative(type))
            {
                throw new OdbRuntimeException(
                    NDatabaseError.OdbCanNotStoreNativeObjectDirectly.AddParameter(type.FullName).AddParameter(
                        OdbType.GetFromClass(type).GetName()).AddParameter(type.FullName));
            }

            // The object must be transformed into meta representation
            ClassInfo classInfo;
            var className = OdbClassUtil.GetFullName(type);

            // first checks if the class of this object already exist in the
            // metamodel
            if (GetMetaModel().ExistClass(className))
            {
                classInfo = GetMetaModel().GetClassInfo(className, true);
            }
            else
            {
                var classInfoList = CoreProvider.GetClassIntrospector().Introspect(@object.GetType(), true);

                // All new classes found
                _objectWriter.AddClasses(classInfoList);
                classInfo = classInfoList.GetMainClassInfo();
            }

            // first detects if we must perform an insert or an update
            // If object is in the cache, we must perform an update, else an insert
            var cache = GetSession(true).GetCache();
            
            var cacheOid = cache.IdOfInsertingObject(@object);
            if (cacheOid != null)
                return cacheOid;

            // throw new ODBRuntimeException("Inserting meta representation of
            // an object without the object itself is not yet supported");
            var mustUpdate = cache.ExistObject(@object);

            // The introspection callback is used to execute some specific task (like calling trigger, for example) while introspecting the object
            var callback = _introspectionCallbackForInsert;
            if (mustUpdate)
                callback = _introspectionCallbackForUpdate;

            // Transform the object into an ObjectInfo
            var nnoi =
                (NonNativeObjectInfo)
                _objectIntrospector.GetMetaRepresentation(@object, classInfo, true, null, callback);

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
