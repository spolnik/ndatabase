namespace NDatabase.Odb.Core.Layers.Layer2.Meta
{
    /// <summary>
    ///   To keep info about a non native null instance
    /// </summary>
    internal sealed class NonNativeNullObjectInfo : NonNativeObjectInfo
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

        public override bool IsNull()
        {
            return true;
        }
    }
}
