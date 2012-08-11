using NDatabase.Odb.Core.Layers.Layer2.Meta;

namespace NDatabase.Odb.Impl.Core.Layers.Layer3.Engine
{
    public class PendingReading
    {
        private readonly OID _attributeOID;
        private readonly ClassInfo _ci;
        private readonly int _id;

        public PendingReading(int id, ClassInfo ci, OID attributeOID)
        {
            _id = id;
            _ci = ci;
            _attributeOID = attributeOID;
        }

        public virtual int GetId()
        {
            return _id;
        }

        public virtual ClassInfo GetCi()
        {
            return _ci;
        }

        public virtual OID GetAttributeOID()
        {
            return _attributeOID;
        }
    }
}
