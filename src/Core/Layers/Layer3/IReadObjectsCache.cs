using NDatabase.Api;
using NDatabase.Core.Layers.Layer2.Meta;

namespace NDatabase.Core.Layers.Layer3
{
    /// <summary>
    ///   An interface for temporary cache
    /// </summary>
    internal interface IReadObjectsCache
    {
        NonNativeObjectInfo GetObjectInfoByOid(OID oid);

        bool IsReadingObjectInfoWithOid(OID oid);

        void StartReadingObjectInfoWithOid(OID oid, NonNativeObjectInfo objectInfo);

        void ClearObjectInfos();
    }
}
