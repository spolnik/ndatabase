namespace NDatabase.Odb.Core.Oid
{
    
    public sealed class ExternalClassOID : OdbClassOID, IExternalOID
    {
        private readonly IDatabaseId _databaseId;

        public ExternalClassOID(OID oid, IDatabaseId databaseId) : base(oid.ObjectId)
        {
            _databaseId = databaseId;
        }

        #region IExternalOID Members

        public IDatabaseId GetDatabaseId()
        {
            return _databaseId;
        }

        #endregion
    }
}
