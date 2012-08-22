namespace NDatabase.Odb.Core.Oid
{
    public static class OIDFactory
    {
        public static OID BuildObjectOID(long oid)
        {
            return OdbConfiguration.GetCoreProvider().GetObjectOID(oid, 0);
        }

        public static OID BuildClassOID(long oid)
        {
            return OdbConfiguration.GetCoreProvider().GetClassOID(oid);
        }
    }
}
