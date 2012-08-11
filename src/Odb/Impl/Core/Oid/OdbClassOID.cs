using System;

namespace NDatabase.Odb.Impl.Core.Oid
{
    [Serializable]
    public class OdbClassOID : OdbAbstractOID
    {
        public OdbClassOID(long oid) : base(oid)
        {
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
    }
}
