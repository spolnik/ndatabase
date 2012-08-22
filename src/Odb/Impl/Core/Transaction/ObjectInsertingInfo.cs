namespace NDatabase.Odb.Impl.Core.Transaction
{
    public sealed class ObjectInsertingInfo
    {
        public ObjectInsertingInfo(OID oid, int level)
        {
            OID = oid;
            Level = level;
        }

        public int Level { get; set; }
        public OID OID { get; set; }
    }
}
