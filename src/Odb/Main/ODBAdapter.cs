using NDatabase2.Odb.Core.Layers.Layer1.Introspector;
using NDatabase2.Odb.Core.Layers.Layer3;
using NDatabase2.Odb.Core.Query;
using NDatabase2.Odb.Core.Query.Criteria;
using NDatabase2.Tool;

namespace NDatabase2.Odb.Main
{
    /// <summary>
    ///   A basic adapter for ODB interface
    /// </summary>
    public abstract class OdbAdapter : IOdb
    {
        private readonly IStorageEngine _storageEngine;
        private IOdbExt _ext;

        internal OdbAdapter(IStorageEngine storageEngine)
        {
            _storageEngine = storageEngine;
        }

        #region IOdb Members

        public virtual void Commit()
        {
            _storageEngine.Commit();
        }

        public virtual void Rollback()
        {
            _storageEngine.Rollback();
        }

        public OID Store<T>(T plainObject) where T : class
        {
            return _storageEngine.Store(plainObject);
        }

        public virtual IObjectSet<T> Query<T>() where T : class
        {
            return _storageEngine.GetObjects<T>(new CriteriaQuery<T>(), true, -1, -1);
        }

        public virtual IObjectSet<T> Query<T>(bool inMemory) where T : class
        {
            return _storageEngine.GetObjects<T>(inMemory, -1, -1);
        }

        public virtual IObjectSet<T> Query<T>(bool inMemory, int startIndex, int endIndex) where T : class
        {
            return _storageEngine.GetObjects<T>(inMemory, startIndex, endIndex);
        }

        public virtual void Close()
        {
            _storageEngine.Commit();
            _storageEngine.Close();
        }

        public OID Delete<T>(T plainObject) where T : class
        {
            return _storageEngine.Delete(plainObject);
        }

        /// <summary>
        ///   Delete an object from the database with the id
        /// </summary>
        /// <param name="oid"> The object id to be deleted </param>
        public virtual void DeleteObjectWithId(OID oid)
        {
            _storageEngine.DeleteObjectWithOid(oid);
        }

        public virtual IValues GetValues<T>(IValuesQuery query) where T : class
        {
            return _storageEngine.GetValues<T>(query, -1, -1);
        }

        public virtual IObjectSet<T> Query<T>(IQuery query, bool inMemory, int startIndex, int endIndex) where T : class
        {
            try
            {
                return _storageEngine.GetObjects<T>(query, inMemory, startIndex, endIndex);
            }
            catch (OdbRuntimeException e)
            {
                DLogger.Info(e);
                throw;
            }
        }

        public OID GetObjectId<T>(T plainObject) where T : class
        {
            return _storageEngine.GetObjectId(plainObject, true);
        }

        public virtual object GetObjectFromId(OID id)
        {
            return _storageEngine.GetObjectFromOid(id);
        }

        public virtual void DefragmentTo(string newFileName)
        {
            _storageEngine.DefragmentTo(newFileName);
        }

        public virtual IIndexManager IndexManagerFor<T>() where T : class
        {
            var clazz = typeof (T);
            var classInfo = _storageEngine.GetSession(true).GetMetaModel().GetClassInfo(clazz, false);

            if (classInfo == null)
            {
                var classInfoList = ClassIntrospector.Introspect(clazz, true);
                _storageEngine.AddClasses(classInfoList);
                classInfo = classInfoList.GetMainClassInfo();
            }

            return new IndexManager(_storageEngine, classInfo);
        }

        public virtual ITriggerManager TriggerManagerFor<T>() where T : class
        {
            return new TriggerManager<T>(_storageEngine);
        }

        public virtual IRefactorManager GetRefactorManager()
        {
            return _storageEngine.GetRefactorManager();
        }

        public virtual IOdbExt Ext()
        {
            return _ext ?? (_ext = new OdbExt(_storageEngine));
        }

        public virtual void Disconnect<T>(T plainObject) where T : class
        {
            _storageEngine.Disconnect(plainObject);
        }

        public virtual bool IsClosed()
        {
            return _storageEngine.IsClosed();
        }

        public virtual IQuery CreateCriteriaQuery<T>() where T : class
        {
            var criteriaQuery = new CriteriaQuery<T>();
            ((IInternalQuery)criteriaQuery).SetStorageEngine(_storageEngine);
            return criteriaQuery;
        }

        public virtual string GetDbId()
        {
            return _storageEngine.GetBaseIdentification().Id;
        }

        public void Dispose()
        {
            Close();
        }

        #endregion

        internal IStorageEngine GetStorageEngine()
        {
            return _storageEngine.GetSession(true).GetStorageEngine();
        }
    }
}
