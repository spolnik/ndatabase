using NDatabase.Odb.Core;
using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Impl.Core.Oid;

namespace NDatabase.Odb.Impl.Main
{
    public class OdbExtImpl : IOdbExt
    {
        private readonly IStorageEngine _storageEngine;

        public OdbExtImpl(IStorageEngine storageEngine)
        {
            _storageEngine = storageEngine;
        }

        #region IOdbExt Members

        public virtual IExternalOID ConvertToExternalOID(OID oid)
        {
            return new ExternalObjectOID(oid, _storageEngine.GetDatabaseId());
        }

        public virtual ITransactionId GetCurrentTransactionId()
        {
            return _storageEngine.GetCurrentTransactionId();
        }

        public virtual IDatabaseId GetDatabaseId()
        {
            return _storageEngine.GetDatabaseId();
        }

        public virtual IExternalOID GetObjectExternalOID(object @object)
        {
            return ConvertToExternalOID(_storageEngine.GetObjectId(@object, true));
        }

        public virtual int GetObjectVersion(OID oid)
        {
            var objectInfoHeader = _storageEngine.GetObjectInfoHeaderFromOid(oid);
            if (objectInfoHeader == null)
                throw new OdbRuntimeException(NDatabaseError.ObjectWithOidDoesNotExistInCache.AddParameter(oid));

            return objectInfoHeader.GetObjectVersion();
        }

        public virtual long GetObjectCreationDate(OID oid)
        {
            var objectInfoHeader = _storageEngine.GetObjectInfoHeaderFromOid(oid);
            if (objectInfoHeader == null)
                throw new OdbRuntimeException(NDatabaseError.ObjectWithOidDoesNotExistInCache.AddParameter(oid));

            return objectInfoHeader.GetCreationDate();
        }

        public virtual long GetObjectUpdateDate(OID oid)
        {
            var objectInfoHeader = _storageEngine.GetObjectInfoHeaderFromOid(oid);
            if (objectInfoHeader == null)
                throw new OdbRuntimeException(NDatabaseError.ObjectWithOidDoesNotExistInCache.AddParameter(oid));

            return objectInfoHeader.GetUpdateDate();
        }

        #endregion
    }
}
