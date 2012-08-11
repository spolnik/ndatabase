using System;

namespace NDatabase.Odb.Impl.Core.Oid
{
    [Serializable]
    public sealed class ExternalObjectOID : OdbObjectOID, IExternalOID
    {
        private readonly IDatabaseId _databaseId;

        public ExternalObjectOID(OID oid, IDatabaseId databaseId)
            : base(oid.ObjectId)
        {
            _databaseId = databaseId;
        }

        public IDatabaseId GetDatabaseId()
        {
            return _databaseId;
        }
    }
}
