using NDatabase.Odb.Core.Layers.Layer2.Meta;

namespace NDatabase.Odb.Core.Transaction
{
    /// <summary>
    ///   An interface for temporary cache
    /// </summary>
    public interface ITmpCache
    {
        NonNativeObjectInfo GetReadingObjectInfoFromOid(OID oid);

        bool IsReadingObjectInfoWithOid(OID oid);

        void StartReadingObjectInfoWithOid(OID oid, NonNativeObjectInfo objectInfo);

        void ClearObjectInfos();

        int Size();
    }
}
