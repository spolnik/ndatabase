using System.Collections.Generic;
using NDatabase.Odb.Core.Layers.Layer2.Meta;

namespace NDatabase.Odb.Core.Transaction
{
    public interface ICache
    {
        void AddObject(OID oid, object @object, ObjectInfoHeader objectInfoHeader);

        void StartInsertingObjectWithOid(object @object, OID oid, NonNativeObjectInfo nnoi);

        void UpdateIdOfInsertingObject(object @object, OID oid);

        void EndInsertingObject(object @object);

        void AddObjectInfo(ObjectInfoHeader objectInfoHeader);

        void RemoveObjectWithOid(OID oid);

        void RemoveObject(object @object);

        bool ExistObject(object @object);

        object GetObjectWithOid(OID oid);

        ObjectInfoHeader GetObjectInfoHeaderFromObject(object @object, bool throwExceptionIfNotFound);

        ObjectInfoHeader GetObjectInfoHeaderFromOid(OID oid, bool throwExceptionIfNotFound);

        OID GetOid(object @object, bool throwExceptionIfNotFound);

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

        string ToString();

        string ToCompleteString();

        int GetNumberOfObjects();

        int GetNumberOfObjectHeader();

        OID IdOfInsertingObject(object @object);

        int InsertingLevelOf(object @object);

        bool IsReadingObjectInfoWithOid(OID oid);

        NonNativeObjectInfo GetReadingObjectInfoFromOid(OID oid);

        /// <summary>
        ///   To resolve cyclic reference, keep track of objects being read The read count is used to store how many times the object has been recursively read
        /// </summary>
        /// <param name="oid"> The Object OID </param>
        /// <param name="objectInfo"> The object info (not fully set) that is being read </param>
        void StartReadingObjectInfoWithOid(OID oid, NonNativeObjectInfo objectInfo);

        void EndReadingObjectInfo(OID oid);

        IDictionary<OID, object> GetOids();

        IDictionary<OID, ObjectInfoHeader> GetObjectInfoPointersCacheFromOid();

        IDictionary<object, OID> GetObjects();

        bool ObjectWithIdIsInCommitedZone(OID oid);

        void AddOIDToUnconnectedZone(OID oid);
    }
}
