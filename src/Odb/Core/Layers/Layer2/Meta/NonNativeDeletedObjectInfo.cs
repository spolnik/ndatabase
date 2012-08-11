using System;

namespace NDatabase.Odb.Core.Layers.Layer2.Meta
{
    /// <summary>
    ///   To specify that an object has been mark as deleted
    /// </summary>
    /// <author>olivier s</author>
    [Serializable]
    public class NonNativeDeletedObjectInfo : NonNativeObjectInfo
    {
        public NonNativeDeletedObjectInfo(long position, OID oid) : base(null, null)
        {
            Position = position;
        }

        public override string ToString()
        {
            return "deleted";
        }

        public virtual bool HasChanged(AbstractObjectInfo aoi)
        {
            return aoi.GetType() != typeof (NonNativeDeletedObjectInfo);
        }

        public override object GetObject()
        {
            return null;
        }

        public override bool IsDeletedObject()
        {
            return true;
        }

        /// <summary>
        ///   A deleted non native object is considered to be null!
        /// </summary>
        public override bool IsNull()
        {
            return true;
        }
    }
}
