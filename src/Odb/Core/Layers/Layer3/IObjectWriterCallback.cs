using NDatabase2.Odb.Core.Layers.Layer2.Meta;

namespace NDatabase2.Odb.Core.Layers.Layer3
{
    /// <summary>
    ///   A callback interface - not used
    /// </summary>
    internal interface IObjectWriterCallback
    {
        void MetaObjectHasBeenInserted(long oid, NonNativeObjectInfo nnoi);

        void MetaObjectHasBeenUpdated(long oid, NonNativeObjectInfo nnoi);
    }
}
