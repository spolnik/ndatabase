using NDatabase.Odb.Core.Layers.Layer2.Meta;

namespace NDatabase.Odb.Core.Layers.Layer3
{
    public interface IOdbCache
    {
        void AddObject(OID oid, object @object, ObjectInfoHeader objectInfoHeader);

        void StartInsertingObjectWithOid(object @object, OID oid, NonNativeObjectInfo nnoi);

        void UpdateIdOfInsertingObject(object @object, OID oid);

        void AddObjectInfoOfNonCommitedObject(ObjectInfoHeader objectInfoHeader);

        void RemoveObjectWithOid(OID oid);

        void RemoveObject(object @object);

        bool Contains(object @object);

        object GetObject(OID oid);

        ObjectInfoHeader GetObjectInfoHeaderFromObject(object @object);

        ObjectInfoHeader GetObjectInfoHeaderFromOid(OID oid, bool throwExceptionIfNotFound);

        OID GetOid(object @object);

        /// <summary>
        ///   To resolve uncommitted updates where the oid change and is not committed yet
        /// </summary>
        void SavePositionOfObjectWithOid(OID oid, long objectPosition);

        void MarkIdAsDeleted(OID oid);

        bool IsDeleted(OID oid);

        long GetObjectPositionByOid(OID oid);

        void ClearOnCommit();

        void Clear(bool setToNull);

        void ClearInsertingObjects();

        OID IdOfInsertingObject(object @object);

        bool ObjectWithIdIsInCommitedZone(OID oid);

        void AddOIDToUnconnectedZone(OID oid);
    }
}
