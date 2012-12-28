using NDatabase.Odb.Core;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Oid;

namespace NDatabase.Odb.Main
{
    internal sealed class OdbExt : IOdbExt
    {
        private readonly IStorageEngine _storageEngine;

        internal OdbExt(IStorageEngine storageEngine)
        {
            _storageEngine = storageEngine;
        }

        #region IOdbExt Members

        public IExternalOID ConvertToExternalOID(OID oid)
        {
            return new ExternalObjectOID(oid, _storageEngine.GetDatabaseId());
        }

        public ITransactionId GetCurrentTransactionId()
        {
            return _storageEngine.GetCurrentTransactionId();
        }

        public IDatabaseId GetDatabaseId()
        {
            return _storageEngine.GetDatabaseId();
        }

        public IExternalOID GetObjectExternalOID<T>(T plainObject) where T : class 
        {
            return ConvertToExternalOID(_storageEngine.GetObjectId(plainObject, true));
        }

        public int GetObjectVersion(OID oid)
        {
            var objectInfoHeader = _storageEngine.GetObjectInfoHeaderFromOid(oid);
            if (objectInfoHeader == null)
                throw new OdbRuntimeException(NDatabaseError.ObjectWithOidDoesNotExistInCache.AddParameter(oid));

            return objectInfoHeader.GetObjectVersion();
        }

        public long GetObjectCreationDate(OID oid)
        {
            var objectInfoHeader = _storageEngine.GetObjectInfoHeaderFromOid(oid);
            if (objectInfoHeader == null)
                throw new OdbRuntimeException(NDatabaseError.ObjectWithOidDoesNotExistInCache.AddParameter(oid));

            return objectInfoHeader.GetCreationDate();
        }

        public long GetObjectUpdateDate(OID oid)
        {
            var objectInfoHeader = _storageEngine.GetObjectInfoHeaderFromOid(oid);
            if (objectInfoHeader == null)
                throw new OdbRuntimeException(NDatabaseError.ObjectWithOidDoesNotExistInCache.AddParameter(oid));

            return objectInfoHeader.GetUpdateDate();
        }

        public string GetDbId()
        {
            return _storageEngine.GetBaseIdentification().Id;
        }

        public IMetaModel GetSchema()
        {
            return _storageEngine.GetSession(true).GetMetaModel();
        }

        #endregion
    }
}
