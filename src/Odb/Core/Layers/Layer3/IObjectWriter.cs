using System;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Trigger;

namespace NDatabase.Odb.Core.Layers.Layer3
{
    public interface IObjectWriter : ITwoPhaseInit, IDisposable
    {
        ClassInfoList AddClasses(ClassInfoList classInfoList);

        void UpdateClassInfo(ClassInfo classInfo, bool writeInTransaction);

        /// <param name="oid"> The Oid of the object to be inserted </param>
        /// <param name="nnoi"> The object meta representation The object to be inserted in the database </param>
        /// <param name="isNewObject"> To indicate if object is new </param>
        /// <returns> The position of the inserted object </returns>
        OID InsertNonNativeObject(OID oid, NonNativeObjectInfo nnoi, bool isNewObject);

        /// <summary>
        ///   Updates an object.
        /// </summary>
        /// <remarks>
        ///   Updates an object. Deletes the current object and creates a new at the end of the database file and updates
        ///                        OID object position.
        /// </remarks>
        /// <param name="nnoi"> The meta representation of the object to be updated </param>
        /// <param name="forceUpdate"> when true, no verification is done to check if update must be done. </param>
        /// <returns> The oid of the object, as a negative number </returns>
        OID UpdateNonNativeObjectInfo(NonNativeObjectInfo nnoi, bool forceUpdate);

        /// <summary>
        ///   Write an object representation to database file
        /// </summary>
        /// <param name="existingOid"> The oid of the object, can be null </param>
        /// <param name="objectInfo"> The Object meta representation </param>
        /// <param name="position"> The position where the object must be written, can be -1 </param>
        /// <param name="writeDataInTransaction"> To indicate if the write must be done in or out of transaction </param>
        /// <param name="isNewObject"> </param>
        /// <returns> The oid of the object </returns>
        /// <exception cref="System.Exception">System.Exception</exception>
        OID WriteNonNativeObjectInfo(OID existingOid, NonNativeObjectInfo objectInfo, long position,
                                     bool writeDataInTransaction, bool isNewObject);

        IIdManager GetIdManager();

        void Close();

        IFileSystemInterface GetFsi();

        /// <summary>
        ///   Creates the header of the file
        /// </summary>
        /// <param name="creationDate"> The creation date </param>
        void CreateEmptyDatabaseHeader(long creationDate);

        /// <summary>
        ///   Write the status of the last odb close
        /// </summary>
        void WriteLastOdbCloseStatus(bool ok, bool writeInTransaction);

        void Flush();

        OID Delete(ObjectInfoHeader header);

        void UpdateStatusForIdWithPosition(long idPosition, byte newStatus, bool writeInTransaction);

        /// <summary>
        ///   Updates the real object position of the object OID
        /// </summary>
        /// <param name="idPosition"> The OID position </param>
        /// <param name="objectPosition"> The real object position </param>
        /// <param name="writeInTransaction"> To indicate if write must be done in transaction </param>
        void UpdateObjectPositionForObjectOIDWithPosition(long idPosition, long objectPosition, bool writeInTransaction);

        /// <summary>
        ///   Udates the real class positon of the class OID
        /// </summary>
        /// <param name="idPosition"> </param>
        /// <param name="objectPosition"> </param>
        /// <param name="writeInTransaction"> </param>
        void UpdateClassPositionForClassOIDWithPosition(long idPosition, long objectPosition, bool writeInTransaction);

        /// <summary>
        ///   Associate an object OID to its position
        /// </summary>
        /// <param name="idType"> The type : can be object or class </param>
        /// <param name="idStatus"> The status of the OID </param>
        /// <param name="currentBlockIdPosition"> The current OID block position </param>
        /// <param name="oid"> The OID </param>
        /// <param name="objectPosition"> The position </param>
        /// <param name="writeInTransaction"> To indicate if write must be executed in transaction </param>
        /// <returns> </returns>
        long AssociateIdToObject(byte idType, byte idStatus, long currentBlockIdPosition, OID oid, long objectPosition,
                                 bool writeInTransaction);

        /// <summary>
        ///   Marks a block of type id as full, changes the status and the next block position
        /// </summary>
        /// <param name="blockPosition"> </param>
        /// <param name="nextBlockPosition"> </param>
        /// <param name="writeInTransaction"> </param>
        /// <returns> The block position </returns>
        long MarkIdBlockAsFull(long blockPosition, long nextBlockPosition, bool writeInTransaction);

        /// <summary>
        ///   Writes the header of a block of type ID - a block that contains ids of objects and classes
        /// </summary>
        /// <param name="position"> Position at which the block must be written, if -1, take the next available position </param>
        /// <param name="idBlockSize"> The block size in byte </param>
        /// <param name="blockStatus"> The block status </param>
        /// <param name="blockNumber"> The number of the block </param>
        /// <param name="previousBlockPosition"> The position of the previous block of the same type </param>
        /// <param name="writeInTransaction"> To indicate if write must be done in transaction </param>
        /// <returns> The position of the id </returns>
        long WriteIdBlock(long position, int idBlockSize, byte blockStatus, int blockNumber, long previousBlockPosition,
                          bool writeInTransaction);

        /// <summary>
        ///   Updates the previous object position field of the object at objectPosition
        /// </summary>
        /// <param name="objectOID"> </param>
        /// <param name="previousObjectOID"> </param>
        /// <param name="writeInTransaction"> </param>
        void UpdatePreviousObjectFieldOfObjectInfo(OID objectOID, OID previousObjectOID, bool writeInTransaction);

        /// <summary>
        ///   Update next object oid field of the object at the specific position
        /// </summary>
        /// <param name="objectOID"> </param>
        /// <param name="nextObjectOID"> </param>
        /// <param name="writeInTransaction"> </param>
        void UpdateNextObjectFieldOfObjectInfo(OID objectOID, OID nextObjectOID, bool writeInTransaction);

        /// <summary>
        ///   Updates the instance related field of the class info into the database file Updates the number of objects, the first object oid and the next class oid
        /// </summary>
        /// <param name="classInfo"> The class info to be updated </param>
        /// <param name="writeInTransaction"> To specify if it must be part of a transaction </param>
        void UpdateInstanceFieldsOfClassInfo(ClassInfo classInfo, bool writeInTransaction);

        void AfterInit();

        void WriteLastTransactionId(ITransactionId transactionId);

        void SetTriggerManager(ITriggerManager triggerManager);

        /// <summary>
        ///   Mark a block as deleted
        /// </summary>
        /// <returns> The block size </returns>
        void MarkAsDeleted(long currentPosition, OID oid, bool writeInTransaction);

        ClassInfo AddClass(ClassInfo newClassInfo, bool addDependentClasses);

        /// <summary>
        ///   Updates pointers of objects, Only changes uncommitted info pointers
        /// </summary>
        /// <param name="objectInfo"> The meta representation of the object being inserted </param>
        /// <param name="classInfo"> The class of the object being inserted </param>
        void ManageNewObjectPointers(NonNativeObjectInfo objectInfo, ClassInfo classInfo);

        /// <summary>
        ///   Store a meta representation of a native object(already as meta representation)in ODBFactory database.
        /// </summary>
        /// <remarks>
        ///   Store a meta representation of a native object(already as meta representation)in ODBFactory database. A Native object is an object that use native language type, String for example To detect if object must be updated or insert, we use the cache. To update an object, it must be first selected from the database. When an object is to be stored, if it exist in the cache, then it will be updated, else it will be inserted as a new object. If the object is null, the cache will be used to check if the meta representation is in the cache
        /// </remarks>
        /// <param name="noi"> The meta representation of an object </param>
        /// <returns> The object position @ </returns>
        long InternalStoreObject(NativeObjectInfo noi);

        /// <summary>
        ///   Insert the object in the index
        /// </summary>
        /// <param name="oid"> The object id </param>
        /// <param name="nnoi"> The object meta represenation </param>
        /// <returns> The number of indexes </returns>
        int ManageIndexesForInsert(OID oid, NonNativeObjectInfo nnoi);

        /// <summary>
        ///   Store a meta representation of an object(already as meta representation)in ODBFactory database.
        /// </summary>
        /// <remarks>
        ///   Store a meta representation of an object(already as meta representation)in ODBFactory database. To detect if object must be updated or insert, we use the cache. To update an object, it must be first selected from the database. When an object is to be stored, if it exist in the cache, then it will be updated, else it will be inserted as a new object. If the object is null, the cache will be used to check if the meta representation is in the cache
        /// </remarks>
        /// <param name="oid"> The oid of the object to be inserted/updates </param>
        /// <param name="nnoi"> The meta representation of an object </param>
        /// <returns> The object position </returns>
        OID StoreObject(OID oid, NonNativeObjectInfo nnoi);
    }
}
