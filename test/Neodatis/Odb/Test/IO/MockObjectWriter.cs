using NDatabase.Odb;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Layers.Layer3.Engine;
using NDatabase.Odb.Core.Transaction;
using NDatabase.Odb.Core.Trigger;

namespace IO
{
    public class MockObjectWriter : IObjectWriter
    {
        private readonly IStorageEngine _engine;

        public MockObjectWriter(IStorageEngine engine)
        {
            _engine = engine;
        }

        #region IObjectWriter Members

        public ISession GetSession()
        {
            return _engine.GetSession(true);
        }

        void IObjectWriter.MarkAsDeleted(long currentPosition, OID oid, bool writeInTransaction)
        {
            throw new System.NotImplementedException();
        }

        public ClassInfo AddClass(ClassInfo newClassInfo, bool addDependentClasses)
        {
            return null;
        }

        public void ManageNewObjectPointers(NonNativeObjectInfo objectInfo, ClassInfo classInfo)
        {
            throw new System.NotImplementedException();
        }

        public long InternalStoreObject(NativeObjectInfo noi)
        {
            throw new System.NotImplementedException();
        }

        public ClassInfoList AddClasses(ClassInfoList classInfoList)
        {
            return null;
        }

        public void AfterInit()
        {
        }

        public long AssociateIdToObject(byte idType, byte idStatus, long currentBlockIdPosition, OID oid,
                                        long objectPosition, bool writeInTransaction)
        {
            return 0;
        }

        public void Close()
        {
        }

        public void CreateEmptyDatabaseHeader(long creationDate)
        {
        }

        public OID Delete(ObjectInfoHeader header)
        {
            return null;
        }

        public void WriteLastOdbCloseStatus(bool ok, bool writeInTransaction)
        {
        }

        public void Flush()
        {
        }

        public IFileSystemInterface GetFsi()
        {
            return null;
        }

        public IIdManager GetIdManager()
        {
            return null;
        }

        public int ManageIndexesForDelete(OID oid, NonNativeObjectInfo nnoi)
        {
            return 0;
        }

        public int ManageIndexesForInsert(OID oid, NonNativeObjectInfo nnoi)
        {
            return 0;
        }

        public int ManageIndexesForUpdate(OID oid, NonNativeObjectInfo nnoi, NonNativeObjectInfo oldMetaRepresentation)
        {
            return 0;
        }

        public int MarkAsDeleted(long currentPosition, OID oid, bool writeInTransaction)
        {
            return 0;
        }

        public long MarkIdBlockAsFull(long blockPosition, long nextBlockPosition, bool writeInTransaction)
        {
            return 0;
        }

        public ClassInfo PersistClass(ClassInfo newClassInfo, int lastClassInfoIndex, bool addClass,
                                      bool addDependentClasses)
        {
            return null;
        }

        public void UpdateClassInfo(ClassInfo classInfo, bool writeInTransaction)
        {
        }

        public void UpdateClassPositionForClassOIDWithPosition(long idPosition, long objectPosition,
                                                               bool writeInTransaction)
        {
        }

        public void UpdateInstanceFieldsOfClassInfo(ClassInfo classInfo, bool writeInTransaction)
        {
        }

        public void UpdateNextObjectFieldOfObjectInfo(OID objectOID, OID nextObjectOID, bool writeInTransaction)
        {
        }

        public OID UpdateNonNativeObjectInfo(NonNativeObjectInfo nnoi, bool forceUpdate)
        {
            return null;
        }

        public void UpdateObjectPositionForObjectOIDWithPosition(long idPosition, long objectPosition,
                                                                 bool writeInTransaction)
        {
        }

        public void UpdatePreviousObjectFieldOfObjectInfo(OID objectOID, OID previousObjectOID, bool writeInTransaction)
        {
        }

        public void UpdateStatusForIdWithPosition(long idPosition, byte newStatus, bool writeInTransaction)
        {
        }

        public long WriteAtomicNativeObject(AtomicNativeObjectInfo anoi, bool writeInTransaction, int totalSpaceIfString)
        {
            return 0;
        }

        public void WriteClassInfoHeader(ClassInfo classInfo, long position, bool writeInTransaction)
        {
        }

        public long WriteIdBlock(long position, int idBlockSize, byte blockStatus, int blockNumber,
                                 long previousBlockPosition, bool writeInTransaction)
        {
            return 0;
        }

        public OID WriteNonNativeObjectInfo(OID existingOid, NonNativeObjectInfo objectInfo, long position,
                                            bool writeDataInTransaction, bool isNewObject)
        {
            return null;
        }

        public void Init2()
        {
        }

        public void WriteLastTransactionId(ITransactionId transactionId)
        {
        }

        public void SetTriggerManager(ITriggerManager triggerManager)
        {
        }

        public OID InsertNonNativeObject(OID oid, NonNativeObjectInfo nnoi, bool isNewObject)
        {
            return null;
        }

        #endregion

        public OID StoreObject(OID oid, NonNativeObjectInfo nnoi)
        {
            return null;
        }

        public void Dispose()
        {
            Close();
        }
    }
}