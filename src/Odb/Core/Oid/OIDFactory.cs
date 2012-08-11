using NDatabase.Odb.Impl.Core.Oid;

namespace NDatabase.Odb.Core.Oid
{
    public class OIDFactory
    {
        public static OID BuildObjectOID(long oid)
        {
            return OdbConfiguration.GetCoreProvider().GetObjectOID(oid, 0);
        }

        public static OID BuildClassOID(long oid)
        {
            return OdbConfiguration.GetCoreProvider().GetClassOID(oid);
        }

        public static OID OidFromString(string oidString)
        {
            var tokens = oidString.Split(':');

            if (tokens[0].Equals(OIDTypes.TypeNameObjectOid))
                return OdbObjectOID.OidFromString(oidString);

            if (tokens[0].Equals(OIDTypes.TypeNameClassOid))
                return OdbClassOID.OidFromString(oidString);

            if (tokens[0].Equals(OIDTypes.TypeNameExternalObjectOid))
                return ExternalObjectOID.OidFromString(oidString);

            if (tokens[0].Equals(OIDTypes.TypeNameExternalClassOid))
                return ExternalClassOID.OidFromString(oidString);

            throw new OdbRuntimeException(NDatabaseError.InvalidOidRepresentation.AddParameter(oidString));
        }
    }
}
