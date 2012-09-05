namespace NDatabase.Odb.Core.Layers.Layer2.Meta
{
    /// <summary>
    ///   To keep info about a native instance
    /// </summary>
    /// <author>olivier s</author>
    
    public abstract class NativeObjectInfo : AbstractObjectInfo
    {
        protected bool Equals(NativeObjectInfo other)
        {
            return Equals(TheObject, other.TheObject);
        }

        public override int GetHashCode()
        {
            return (TheObject != null
                        ? TheObject.GetHashCode()
                        : 0);
        }

        /// <summary>
        ///   The object being represented
        /// </summary>
        protected object TheObject;

        protected NativeObjectInfo(object @object, int odbTypeId) : base(odbTypeId)
        {
            TheObject = @object;
        }

        protected NativeObjectInfo(object @object, OdbType odbType) : base(odbType)
        {
            TheObject = @object;
        }

        public override string ToString()
        {
            if (TheObject != null)
                return TheObject.ToString();
            return "null";
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var noi = (NativeObjectInfo) obj;

            if (TheObject == noi.GetObject())
                return true;
            return TheObject.Equals(noi.GetObject());
        }

        public virtual bool IsNativeObject()
        {
            return true;
        }

        public override object GetObject()
        {
            return TheObject;
        }

        public override void SetObject(object @object)
        {
            TheObject = @object;
        }
    }
}
