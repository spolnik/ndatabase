namespace NDatabase.Odb.Core.Transaction
{
    public sealed class IdInfo
    {
        public IdInfo(OID oid, long position, byte status)
        {
            OID = oid;
            Position = position;
            Status = status;
        }

        public OID OID { get; set; }
        public long Position { get; set; }
        public byte Status { get; set; }
    }
}
