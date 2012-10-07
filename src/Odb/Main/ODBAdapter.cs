using System;
using NDatabase.Odb.Core.Layers.Layer1.Introspector;
using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Query;
using NDatabase.Odb.Core.Query.Criteria;
using NDatabase.Odb.Core.Query.Values;
using NDatabase.Odb.Core.Trigger;
using NDatabase.Tool;
using NDatabase.Tool.Wrappers;

namespace NDatabase.Odb.Main
{
    /// <summary>
    ///   A basic adapter for ODB interface
    /// </summary>
    /// <author>osmadja</author>
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

        public virtual IObjects<T> GetObjects<T>()
        {
            return _storageEngine.GetObjects<T>(new CriteriaQuery(typeof (T)), true, -1, -1);
        }

        public virtual IObjects<T> GetObjects<T>(bool inMemory)
        {
            return _storageEngine.GetObjects<T>(typeof (T), inMemory, -1, -1);
        }

        public virtual IObjects<T> GetObjects<T>(bool inMemory, int startIndex, int endIndex)
        {
            return _storageEngine.GetObjects<T>(typeof (T), inMemory, startIndex, endIndex);
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

        public virtual IObjects<T> GetObjects<T>(IQuery query)
        {
            ((AbstractQuery)query).SetStorageEngine(_storageEngine);
            return _storageEngine.GetObjects<T>(query, true, -1, -1);
        }

        public virtual IValues GetValues(IValuesQuery query)
        {
            return _storageEngine.GetValues(query, -1, -1);
        }

        public virtual long Count(CriteriaQuery query)
        {
            var valuesQuery = new ValuesCriteriaQuery(query).Count("count");
            
            var values = _storageEngine.GetValues(valuesQuery, -1, -1);

            var count = (Decimal) values.NextValues().GetByIndex(0);
            return Decimal.ToInt64(count);
        }

        public virtual IObjects<T> GetObjects<T>(IQuery query, bool inMemory)
        {
            return _storageEngine.GetObjects<T>(query, inMemory, -1, -1);
        }

        public virtual IObjects<T> GetObjects<T>(IQuery query, bool inMemory, int startIndex, int endIndex)
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

        public virtual IClassRepresentation GetClassRepresentation(Type clazz)
        {
            var classInfo = _storageEngine.GetSession(true).GetMetaModel().GetClassInfo(clazz, false);

            if (classInfo == null)
            {
                var classInfoList = ClassIntrospector.Introspect(clazz, true);
                _storageEngine.AddClasses(classInfoList);
                classInfo = classInfoList.GetMainClassInfo();
            }

            return new ClassRepresentation(_storageEngine, classInfo);
        }

        public virtual void AddUpdateTrigger(Type clazz, UpdateTrigger trigger)
        {
            var classFullName = OdbClassUtil.GetFullName(clazz);
            _storageEngine.AddUpdateTriggerFor(classFullName, trigger);
        }

        public virtual void AddInsertTrigger(Type clazz, InsertTrigger trigger)
        {
            var classFullName = OdbClassUtil.GetFullName(clazz);
            _storageEngine.AddInsertTriggerFor(classFullName, trigger);
        }

        public virtual void AddDeleteTrigger(Type clazz, DeleteTrigger trigger)
        {
            var classFullName = OdbClassUtil.GetFullName(clazz);
            _storageEngine.AddDeleteTriggerFor(classFullName, trigger);
        }

        public virtual void AddSelectTrigger(Type clazz, SelectTrigger trigger)
        {
            var classFullName = OdbClassUtil.GetFullName(clazz);
            _storageEngine.AddSelectTriggerFor(classFullName, trigger);
        }

        public virtual IRefactorManager GetRefactorManager()
        {
            return _storageEngine.GetRefactorManager();
        }

        public virtual IOdbExt Ext()
        {
            return _ext ?? (_ext = new OdbExt(_storageEngine));
        }

        public virtual void Disconnect(object @object)
        {
            _storageEngine.Disconnect(@object);
        }

        public virtual bool IsClosed()
        {
            return _storageEngine.IsClosed();
        }

        public virtual CriteriaQuery CriteriaQuery(Type clazz, ICriterion criterion)
        {
            return _storageEngine.CriteriaQuery(clazz, criterion);
        }

        public virtual CriteriaQuery CriteriaQuery(Type clazz)
        {
            return _storageEngine.CriteriaQuery(clazz);
        }

        public virtual string GetName()
        {
            return _storageEngine.GetBaseIdentification().Id;
        }

        internal IStorageEngine GetStorageEngine()
        {
            return _storageEngine.GetSession(true).GetStorageEngine();
        }

        public void Dispose()
        {
            Commit();
            Close();
        }

        #endregion
    }
}
