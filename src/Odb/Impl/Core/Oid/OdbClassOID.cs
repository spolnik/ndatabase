using System;
using NDatabase.Odb.Core;
using NDatabase.Odb.Core.Layers.Layer2.Meta;

namespace NDatabase.Odb.Impl.Core.Oid
{
    [Serializable]
    public class OdbClassOID : OdbAbstractOID
    {
        public OdbClassOID(long oid) : base(oid)
        {
        }

        public override string GetTypeName()
        {
            return OdbType.TypeNameClassOid;
        }

        public override int CompareTo(object @object)
        {
            if (@object == null || !(@object is OdbClassOID))
                return -1000;

            var otherOid = (OID)@object;
            return (int)(ObjectId - otherOid.ObjectId);
        }

        public override bool Equals(object @object)
        {
            return this == @object || CompareTo(@object) == 0;
        }

        public override int GetHashCode()
        {
            // Copy of the Long hashcode algorithm
            return (int)(ObjectId ^ (UrShift(ObjectId, 32)));
        }

        public static OdbClassOID OidFromString(string oidString)
        {
            var tokens = oidString.Split(':');

            if (tokens.Length != 2 || !(tokens[0].Equals(OdbType.TypeNameClassOid)))
                throw new OdbRuntimeException(NDatabaseError.InvalidOidRepresentation.AddParameter(oidString));

            var oid = long.Parse(tokens[1]);
            return new OdbClassOID(oid);
        }
    }
}
