using System.Collections.Generic;

namespace NDatabase.Odb.Core.Layers.Layer2.Meta
{
    /// <summary>
    ///   A super class for CollectionObjectInfo, MapObjectInfo and ArrayObjectInfo.
    /// </summary>
    /// <remarks>
    ///   A super class for CollectionObjectInfo, MapObjectInfo and ArrayObjectInfo. 
    ///   It keeps a list of reference to non native objects contained in theses structures
    /// </remarks>
    internal abstract class GroupObjectInfo : NativeObjectInfo
    {
        private ICollection<NonNativeObjectInfo> _nonNativeObjects;

        protected GroupObjectInfo(object @object, int odbTypeId) : base(@object, odbTypeId)
        {
            _nonNativeObjects = new List<NonNativeObjectInfo>();
        }

        protected GroupObjectInfo(object @object, OdbType odbType) : base(@object, odbType)
        {
            _nonNativeObjects = new List<NonNativeObjectInfo>();
        }

        public virtual ICollection<NonNativeObjectInfo> GetNonNativeObjects()
        {
            return _nonNativeObjects;
        }

        protected void SetNonNativeObjects(ICollection<NonNativeObjectInfo> nonNativeObjects)
        {
            _nonNativeObjects = nonNativeObjects;
        }

        public void AddNonNativeObjectInfo(NonNativeObjectInfo nnoi)
        {
            _nonNativeObjects.Add(nnoi);
        }
    }
}
