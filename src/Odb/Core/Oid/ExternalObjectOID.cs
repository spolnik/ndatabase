namespace NDatabase.Odb.Core.Oid
{
    
    public sealed class ExternalObjectOID : ObjectOID, IExternalOID
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
