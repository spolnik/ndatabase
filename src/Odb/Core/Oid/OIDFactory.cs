namespace NDatabase2.Odb.Core.Oid
{
    public static class OIDFactory
    {
        public static OID BuildObjectOID(long oid)
        {
            return new ObjectOID(oid);
        }

        public static OID BuildClassOID(long oid)
        {
            return new ClassOID(oid);
        }
    }
}
