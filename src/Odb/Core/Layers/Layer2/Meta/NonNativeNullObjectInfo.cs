using System;

namespace NDatabase.Odb.Core.Layers.Layer2.Meta
{
    /// <summary>
    ///   To keep info about a non native null instance
    /// </summary>
    /// <author>olivier s</author>
    [Serializable]
    public class NonNativeNullObjectInfo : NonNativeObjectInfo
    {
        public NonNativeNullObjectInfo() : base(null)
        {
        }

        public NonNativeNullObjectInfo(ClassInfo classInfo) : base(classInfo)
        {
        }

        public override string ToString()
        {
            return "null non native object ";
        }

        public virtual bool HasChanged(AbstractObjectInfo aoi)
        {
            return aoi.GetType() != typeof (NonNativeNullObjectInfo);
        }

        public virtual bool IsNonNativeNullObject()
        {
            return true;
        }

        public override bool IsNull()
        {
            return true;
        }
    }
}
