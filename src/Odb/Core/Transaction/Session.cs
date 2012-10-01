using System;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer3;

namespace NDatabase.Odb.Core.Transaction
{
    /// <summary>
    ///   An ODB Session.
    /// </summary>
    /// <remarks>
    ///   An ODB Session. Keeps track of all the session operations. 
    ///   Caches objects and manage the transaction. The meta model of the database is stored in the session.
    /// </remarks>
    internal abstract class Session : ISession
    {
        public override int GetHashCode()
        {
            return (_id != null
                        ? _id.GetHashCode()
                        : 0);
        }

        private readonly IOdbCache _cache;

        /// <summary>
        ///   A temporary cache used for object info read
        /// </summary>
        private readonly ITmpCache _tmpCache;

        private string _baseIdentification;

        private readonly string _id;

        protected MetaModel MetaModel;
        private bool _rollbacked;

        protected Session(string id, string baseIdentification)
        {
            _cache = BuildInMemoryStorage();
            _tmpCache = BuildTmpCache();
            _id = id;
            _baseIdentification = baseIdentification;
        }

        #region IComparable Members

        public virtual int CompareTo(object o)
        {
            if (o == null || !(o is Session))
                return -100;

            var session = (ISession) o;
            return String.Compare(GetId(), session.GetId(), StringComparison.Ordinal);
        }

        #endregion

        #region ISession Members

        public virtual IOdbCache GetInMemoryStorage()
        {
            return _cache;
        }

        public virtual ITmpCache GetTmpCache()
        {
            return _tmpCache;
        }

        public virtual void Rollback()
        {
            ClearCache();
            _rollbacked = true;
        }

        public virtual void Close()
        {
            Clear();
        }

        public virtual void ClearCache()
        {
            _cache.Clear(false);
        }

        public virtual bool IsRollbacked()
        {
            return _rollbacked;
        }

        public virtual void Clear()
        {
            _cache.Clear(true);
            if (MetaModel != null)
                MetaModel.Clear();
        }

        public virtual string GetId()
        {
            return _id;
        }

        public abstract IStorageEngine GetStorageEngine();

        public abstract bool TransactionIsPending();

        public abstract void Commit();

        public abstract ITransaction GetTransaction();

        public abstract void SetFileSystemInterfaceToApplyTransaction(IFileSystemInterface fsi);

        public virtual string GetBaseIdentification()
        {
            return _baseIdentification;
        }

        public virtual MetaModel GetMetaModel()
        {
            if (MetaModel == null)
            {
                // MetaModel can be null (this happens at the end of the
                // Transaction.commitMetaModel() method)when the user commited the
                // database
                // And continue using it. In this case, after the commit, the
                // metamodel is set to null
                // and lazy-reloaded when the user use the odb again.
                MetaModel = new MetaModel();
                try
                {
                    GetStorageEngine().GetObjectReader().LoadMetaModel(MetaModel, true);
                }
                catch (Exception e)
                {
                    throw new OdbRuntimeException(NDatabaseError.InternalError.AddParameter("Session.getMetaModel"), e);
                }
            }
            return MetaModel;
        }

        public virtual void SetMetaModel(MetaModel metaModel2)
        {
            MetaModel = metaModel2;
        }

        public virtual void RemoveObjectFromCache(object @object)
        {
            _cache.RemoveObject(@object);
        }

        public virtual void AddObjectToCache(OID oid, object @object, ObjectInfoHeader oih)
        {
            if (@object == null)
                throw new OdbRuntimeException(NDatabaseError.CacheNullObject.AddParameter("@object"));

            if (oid == null)
                throw new OdbRuntimeException(NDatabaseError.CacheNullOid.AddParameter("oid"));

            if (oih == null)
                throw new OdbRuntimeException(NDatabaseError.CacheNullObject.AddParameter("oih"));

            _cache.AddObject(oid, @object, oih);
        }

        #endregion

        public IOdbCache BuildInMemoryStorage()
        {
            return new OdbCache();
        }

        public ITmpCache BuildTmpCache()
        {
            return new TmpCache();
        }

        public override string ToString()
        {
            var transaction = GetTransaction();
            if (transaction == null)
                return string.Format("name={0} sid={1} - no transaction", _baseIdentification, _id);

            var n = transaction.GetNumberOfWriteActions();
            return string.Format("name={0} - sid={1} - Nb Actions = {2}", _baseIdentification, _id, n);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Session))
                return false;
            var session = (ISession) obj;
            return GetId().Equals(session.GetId());
        }

        public virtual void SetBaseIdentification(string baseIdentification)
        {
            _baseIdentification = baseIdentification;
        }
    }
}
