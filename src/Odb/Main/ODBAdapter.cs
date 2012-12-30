using NDatabase.Odb.Core.Layers.Layer1.Introspector;
using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Query;
using NDatabase.Odb.Core.Query.Criteria;
using NDatabase.Odb.Core.Query.Linq;
using NDatabase.Odb.Core.Query.Values;

namespace NDatabase.Odb.Main
{
    /// <summary>
    ///   A basic adapter for ODB interface
    /// </summary>
    internal abstract class OdbAdapter : IOdb
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

        public IQuery Query<T>()
        {
            var criteriaQuery = new SodaQuery(typeof(T));
            ((IInternalQuery)criteriaQuery).SetStorageEngine(_storageEngine);
            return criteriaQuery;
        }

        public IValuesQuery ValuesQuery<T>() where T : class
        {
            var criteriaQuery = new ValuesCriteriaQuery(typeof(T));
            ((IInternalQuery)criteriaQuery).SetStorageEngine(_storageEngine);
            return criteriaQuery;
        }

        public IValuesQuery ValuesQuery<T>(OID oid) where T : class
        {
            var criteriaQuery = new ValuesCriteriaQuery(typeof(T), oid);
            ((IInternalQuery)criteriaQuery).SetStorageEngine(_storageEngine);
            return criteriaQuery;
        }

        public ILinqQueryable<T> AsQueryable<T>()
        {
            if (typeof(T) == typeof(object)) 
                return new PlaceHolderQuery<T>(this).AsQueryable();

            var linqQuery = new LinqQuery<T>(this);
            return linqQuery.AsQueryable();
        }

        public IValues GetValues(IValuesQuery query)
        {
            return _storageEngine.GetValues(query, -1, -1);
        }

        public virtual void Close()
        {
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
        public void DeleteObjectWithId(OID oid)
        {
            _storageEngine.DeleteObjectWithOid(oid);
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

        public bool IsClosed()
        {
            return _storageEngine.IsClosed();
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
