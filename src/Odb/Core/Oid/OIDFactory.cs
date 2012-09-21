namespace NDatabase.Odb.Core.Oid
{
    public static class OIDFactory
    {
        public static OID BuildObjectOID(long oid)
        {
            return new OdbObjectOID(oid);
        }

        public static OID BuildClassOID(long oid)
        {
            return new OdbClassOID(oid);
        }
    }
}
