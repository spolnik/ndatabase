using System;
using System.Collections;
using NDatabase.Btree.Exception;
using NDatabase.Odb.Core;
using NDatabase.Odb.Core.Layers.Layer1.Introspector;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer2.Meta.Compare;
using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Layers.Layer3.Engine;
using NDatabase.Odb.Core.Oid;
using NDatabase.Odb.Core.Transaction;
using NDatabase.Odb.Core.Trigger;
using NDatabase.Odb.Impl.Core.Layers.Layer2.Meta.Compare;
using NDatabase.Odb.Impl.Core.Layers.Layer2.Meta.History;
using NDatabase.Odb.Impl.Core.Layers.Layer3.Block;
using NDatabase.Odb.Impl.Core.Layers.Layer3.Oid;
using NDatabase.Odb.Impl.Core.Oid;
using NDatabase.Odb.Impl.Core.Transaction;
using NDatabase.Odb.Impl.Tool;
using NDatabase.Tool;
using NDatabase.Tool.Wrappers;

namespace NDatabase.Odb.Impl.Core.Layers.Layer3.Engine
{
    /// <summary>
    ///   Manage all IO writing
    /// </summary>
    /// <author>olivier s</author>
    public abstract class AbstractObjectWriter : IObjectWriter
    {
        private const string LogId = "ObjectWriter";
        private const string LogIdDebug = "ObjectWriter.debug";

        private static readonly int NonNativeHeaderBlockSize = OdbType.Integer.GetSize() + OdbType.Byte.GetSize() +
                                                               OdbType.Long.GetSize();

        private static readonly int NativeHeaderBlockSize = OdbType.Integer.GetSize() + OdbType.Byte.GetSize() +
                                                            OdbType.Integer.GetSize() + OdbType.Boolean.GetSize();

        private static byte[] _nativeHeaderBlockSizeByte;
        private static int _nbInPlaceUpdates;
        private static int _nbNormalUpdates;

        private readonly IByteArrayConverter _byteArrayConverter;
        private readonly IClassIntrospector _classIntrospector;
        private readonly IObjectInfoComparator _comparator;

        private IFileSystemInterface _fsi;

        private IIdManager _idManager;
        private IObjectReader _objectReader;

        /// <summary>
        ///   To manage triggers
        /// </summary>
        private ITriggerManager _triggerManager;

        protected AbstractObjectWriter(IStorageEngine engine)
        {
            // public ISession session;
            // Just for display matters
            StorageEngine = engine;
            _objectReader = StorageEngine.GetObjectReader();

            var provider = OdbConfiguration.GetCoreProvider();
            _byteArrayConverter = provider.GetByteArrayConverter();
            _classIntrospector = provider.GetClassIntrospector();
            _nativeHeaderBlockSizeByte = _byteArrayConverter.IntToByteArray(NativeHeaderBlockSize);
            _comparator = new ObjectInfoComparator();
        }

        public IStorageEngine StorageEngine { get; set; }

        #region IObjectWriter Members

        public abstract ISession GetSession();

        /// <summary>
        ///   The init2 method is the two phase init implementation The FileSystemInterface depends on the session creation which is done by subclasses after the ObjectWriter constructor So we can not execute the buildFSI in the constructor as it would result in a non initialized object reference (the session)
        /// </summary>
        public virtual void Init2()
        {
            _fsi = BuildFsi();
        }

        public virtual void AfterInit()
        {
            _objectReader = StorageEngine.GetObjectReader();
            _idManager = OdbConfiguration.GetCoreProvider().GetIdManager(StorageEngine);
        }

        /// <summary>
        ///   Creates the header of the file
        /// </summary>
        /// <param name="creationDate"> The creation date </param>
        public virtual void CreateEmptyDatabaseHeader(long creationDate)
        {
            WriteVersion(false);
            var databaseId = WriteDatabaseId(creationDate, false);
            
            // Create the first Transaction Id
            ITransactionId tid = new TransactionIdImpl(databaseId, 0, 1);

            StorageEngine.SetCurrentTransactionId(tid);

            WriteLastTransactionId(tid);
            WriteNumberOfClasses(0, false);
            WriteFirstClassInfoOID(StorageEngineConstant.NullObjectId, false);
            WriteLastOdbCloseStatus(false, false);
            WriteDatabaseCharacterEncoding(false);

            // This is the position of the first block id. But it will always
            // contain the position of the current id block
            _fsi.WriteLong(StorageEngineConstant.DatabaseHeaderFirstIdBlockPosition, false, "current id block position",
                           DefaultWriteAction.DirectWriteAction);

            // Write an empty id block
            WriteIdBlock(-1, OdbConfiguration.GetIdBlockSize(), BlockStatus.BlockNotFull, 1, -1, false);
            Flush();
            StorageEngine.SetCurrentIdBlockInfos(StorageEngineConstant.DatabaseHeaderFirstIdBlockPosition, 1,
                                                 OIDFactory.BuildObjectOID(0));
        }

        /// <summary>
        ///   Write the current transaction Id, out of transaction
        /// </summary>
        public virtual void WriteLastTransactionId(ITransactionId transactionId)
        {
            _fsi.SetWritePosition(StorageEngineConstant.DatabaseHeaderLastTransactionId, false);
            // FIXME This should always be written directly without transaction
            _fsi.WriteLong(transactionId.GetId1(), false, "last transaction id 1/2",
                           DefaultWriteAction.DirectWriteAction);

            _fsi.WriteLong(transactionId.GetId2(), false, "last transaction id 2/2",
                           DefaultWriteAction.DirectWriteAction);
        }

        /// <summary>
        ///   Write the status of the last odb close
        /// </summary>
        public virtual void WriteLastOdbCloseStatus(bool ok, bool writeInTransaction)
        {
            _fsi.SetWritePosition(StorageEngineConstant.DatabaseHeaderLastCloseStatusPosition, writeInTransaction);
            _fsi.WriteBoolean(ok, writeInTransaction, "odb last close status");
        }

        /// <summary>
        ///   Writes the header of a block of type ID - a block that contains ids of objects and classes
        /// </summary>
        /// <param name="position"> Position at which the block must be written, if -1, take the next available position </param>
        /// <param name="idBlockSize"> The block size in byte </param>
        /// <param name="blockStatus"> The block status </param>
        /// <param name="blockNumber"> The number of the block </param>
        /// <param name="previousBlockPosition"> The position of the previous block of the same type </param>
        /// <param name="writeInTransaction"> To indicate if write must be done in transaction </param>
        /// <returns> The position of the id @ </returns>
        public virtual long WriteIdBlock(long position, int idBlockSize, byte blockStatus, int blockNumber,
                                         long previousBlockPosition, bool writeInTransaction)
        {
            if (position == -1)
                position = _fsi.GetAvailablePosition();

            // LogUtil.fileSystemOn(true);
            // Updates the database header with the current id block position
            _fsi.SetWritePosition(StorageEngineConstant.DatabaseHeaderCurrentIdBlockPosition, writeInTransaction);
            _fsi.WriteLong(position, false, "current id block position", DefaultWriteAction.DirectWriteAction);
            _fsi.SetWritePosition(position, writeInTransaction);
            _fsi.WriteInt(idBlockSize, writeInTransaction, "block size");

            // LogUtil.fileSystemOn(false);
            _fsi.WriteByte(BlockTypes.BlockTypeIds, writeInTransaction);
            _fsi.WriteByte(blockStatus, writeInTransaction);

            // prev position
            _fsi.WriteLong(previousBlockPosition, writeInTransaction, "prev block pos",
                           DefaultWriteAction.DirectWriteAction);

            // next position
            _fsi.WriteLong(-1, writeInTransaction, "next block pos", DefaultWriteAction.DirectWriteAction);
            _fsi.WriteInt(blockNumber, writeInTransaction, "id block number");
            _fsi.WriteLong(0, writeInTransaction, "id block max id", DefaultWriteAction.DirectWriteAction);
            _fsi.SetWritePosition(position + OdbConfiguration.GetIdBlockSize() - 1, writeInTransaction);
            _fsi.WriteByte(0, writeInTransaction);

            if (OdbConfiguration.IsDebugEnabled(LogIdDebug))
                DLogger.Debug(string.Format("After create block, available position is {0}", _fsi.GetAvailablePosition()));

            return position;
        }

        /// <summary>
        ///   Marks a block of type id as full, changes the status and the next block position
        /// </summary>
        /// <param name="blockPosition"> </param>
        /// <param name="nextBlockPosition"> </param>
        /// <param name="writeInTransaction"> </param>
        /// <returns> The block position @ </returns>
        public virtual long MarkIdBlockAsFull(long blockPosition, long nextBlockPosition, bool writeInTransaction)
        {
            _fsi.SetWritePosition(blockPosition + StorageEngineConstant.BlockIdOffsetForBlockStatus, writeInTransaction);
            _fsi.WriteByte(BlockStatus.BlockFull, writeInTransaction);
            _fsi.SetWritePosition(blockPosition + StorageEngineConstant.BlockIdOffsetForNextBlock, writeInTransaction);

            _fsi.WriteLong(nextBlockPosition, writeInTransaction, "next id block pos",
                           DefaultWriteAction.DirectWriteAction);

            return blockPosition;
        }

        /// <summary>
        ///   Associate an object OID to its position
        /// </summary>
        /// <param name="idType"> The type : can be object or class </param>
        /// <param name="idStatus"> The status of the OID </param>
        /// <param name="currentBlockIdPosition"> The current OID block position </param>
        /// <param name="oid"> The OID </param>
        /// <param name="objectPosition"> The position </param>
        /// <param name="writeInTransaction"> To indicate if write must be executed in transaction </param>
        /// <returns> @ </returns>
        public virtual long AssociateIdToObject(byte idType, byte idStatus, long currentBlockIdPosition, OID oid,
                                                long objectPosition, bool writeInTransaction)
        {
            // Update the max id of the current block
            _fsi.SetWritePosition(currentBlockIdPosition + StorageEngineConstant.BlockIdOffsetForMaxId,
                                  writeInTransaction);

            _fsi.WriteLong(oid.ObjectId, writeInTransaction, "id block max id update",
                           DefaultWriteAction.PointerWriteAction);

            var l1 = (oid.ObjectId - 1)%OdbConfiguration.GetNbIdsPerBlock();

            var idPosition = currentBlockIdPosition + StorageEngineConstant.BlockIdOffsetForStartOfRepetition +
                             (l1)*OdbConfiguration.GetIdBlockRepetitionSize();

            // go to the next id position
            _fsi.SetWritePosition(idPosition, writeInTransaction);

            // id type
            _fsi.WriteByte(idType, writeInTransaction, "id type");

            // id
            _fsi.WriteLong(oid.ObjectId, writeInTransaction, "oid", DefaultWriteAction.PointerWriteAction);

            // id status
            _fsi.WriteByte(idStatus, writeInTransaction, "id status");

            // object position
            _fsi.WriteLong(objectPosition, writeInTransaction, "obj pos", DefaultWriteAction.PointerWriteAction);

            return idPosition;
        }

        /// <summary>
        ///   Updates the real object position of the object OID
        /// </summary>
        /// <param name="idPosition"> The OID position </param>
        /// <param name="objectPosition"> The real object position </param>
        /// <param name="writeInTransaction"> indicate if write must be done in transaction @ </param>
        public virtual void UpdateObjectPositionForObjectOIDWithPosition(long idPosition, long objectPosition,
                                                                         bool writeInTransaction)
        {
            _fsi.SetWritePosition(idPosition, writeInTransaction);
            _fsi.WriteByte(IdTypes.Object, writeInTransaction, "id type");
            _fsi.SetWritePosition(idPosition + StorageEngineConstant.BlockIdRepetitionIdStatus, writeInTransaction);
            _fsi.WriteByte(IDStatus.Active, writeInTransaction);
            _fsi.WriteLong(objectPosition, writeInTransaction, "Updating object position of id",
                           DefaultWriteAction.PointerWriteAction);
        }

        /// <summary>
        ///   Udates the real class positon of the class OID
        /// </summary>
        public virtual void UpdateClassPositionForClassOIDWithPosition(long idPosition, long objectPosition,
                                                                       bool writeInTransaction)
        {
            _fsi.SetWritePosition(idPosition, writeInTransaction);
            _fsi.WriteByte(IdTypes.Class, writeInTransaction, "id type");
            _fsi.SetWritePosition(idPosition + StorageEngineConstant.BlockIdRepetitionIdStatus, writeInTransaction);
            _fsi.WriteByte(IDStatus.Active, writeInTransaction);
            _fsi.WriteLong(objectPosition, writeInTransaction, "Updating class position of id",
                           DefaultWriteAction.PointerWriteAction);
        }

        public virtual void UpdateStatusForIdWithPosition(long idPosition, byte newStatus, bool writeInTransaction)
        {
            _fsi.SetWritePosition(idPosition + StorageEngineConstant.BlockIdRepetitionIdStatus, writeInTransaction);
            _fsi.WriteByte(newStatus, writeInTransaction, "Updating id status");
        }

        /// <summary>
        ///   Persist a single class info - This method is used by the XML Importer.
        /// </summary>
        /// <remarks>
        ///   Persist a single class info - This method is used by the XML Importer.
        /// </remarks>
        public virtual ClassInfo PersistClass(ClassInfo newClassInfo, int lastClassInfoIndex, bool addClass,
                                              bool addDependentClasses)
        {
            var metaModel = GetSession().GetMetaModel();
            var classInfoId = newClassInfo.GetId();
            if (classInfoId == null)
            {
                classInfoId = GetIdManager().GetNextClassId(-1);
                newClassInfo.SetId(classInfoId);
            }
            var writePosition = _fsi.GetAvailablePosition();
            newClassInfo.SetPosition(writePosition);
            GetIdManager().UpdateClassPositionForId(classInfoId, writePosition, true);
            if (OdbConfiguration.IsDebugEnabled(LogId))
            {
                DLogger.Debug(string.Format("Persisting class into database : {0} with oid {1} at pos {2}",
                                            newClassInfo.GetFullClassName(), classInfoId, writePosition));

                DLogger.Debug(string.Format("class {0} has {1} attributes : {2}", newClassInfo.GetFullClassName(),
                                            newClassInfo.GetNumberOfAttributes(), newClassInfo.GetAttributes()));
            }

            // The class info oid is created in ObjectWriter.writeClassInfoHeader
            if (metaModel.GetNumberOfClasses() > 0 && lastClassInfoIndex != -2)
            {
                var lastClassinfo = lastClassInfoIndex == -1
                                        ? metaModel.GetLastClassInfo()
                                        : metaModel.GetClassInfo(lastClassInfoIndex);

                lastClassinfo.SetNextClassOID(newClassInfo.GetId());
                if (OdbConfiguration.IsDebugEnabled(LogId))
                {
                    DLogger.Debug(
                        string.Format("changing next class oid. of class info {0} @ {1} + offset {2} to {3}({4})",
                                      lastClassinfo.GetFullClassName(), lastClassinfo.GetPosition(),
                                      StorageEngineConstant.ClassOffsetNextClassPosition, newClassInfo.GetId(),
                                      newClassInfo.GetFullClassName()));
                }

                _fsi.SetWritePosition(lastClassinfo.GetPosition() + StorageEngineConstant.ClassOffsetNextClassPosition,
                                      true);

                _fsi.WriteLong(newClassInfo.GetId().ObjectId, true, "next class oid",
                               DefaultWriteAction.PointerWriteAction);

                newClassInfo.SetPreviousClassOID(lastClassinfo.GetId());
            }

            if (addClass)
                metaModel.AddClass(newClassInfo);

            // updates number of classes
            WriteNumberOfClasses(metaModel.GetNumberOfClasses(), true);
            // If it is the first class , updates the first class OID
            if (newClassInfo.GetPreviousClassOID() == null)
                WriteFirstClassInfoOID(newClassInfo.GetId(), true);

            // Writes the header of the class - out of transaction (FIXME why out of
            // transaction)
            WriteClassInfoHeader(newClassInfo, writePosition, false);

            if (addDependentClasses)
            {
                var dependingAttributes = newClassInfo.GetAllNonNativeAttributes();

                foreach (var classAttributeInfo in dependingAttributes)
                {
                    try
                    {
                        var existingClassInfo = metaModel.GetClassInfo(classAttributeInfo.GetFullClassname(), false);
                        if (existingClassInfo == null)
                        {
                            // TODO check if this getClassInfo is ok. Maybe, should
                            // use
                            // a buffered one
                            AddClasses(_classIntrospector.Introspect(classAttributeInfo.GetFullClassname(), true));
                        }
                        else
                        {
                            // Even,if it exist,take the one from metamodel
                            classAttributeInfo.SetClassInfo(existingClassInfo);
                        }
                    }
                    catch (Exception e)
                    {
                        throw new OdbRuntimeException(
                            NDatabaseError.ClassIntrospectionError.AddParameter(classAttributeInfo.GetFullClassname()), e);
                    }
                }
            }

            WriteClassInfoBody(newClassInfo, _fsi.GetAvailablePosition(), true);
            return newClassInfo;
        }

        public virtual ClassInfo AddClass(ClassInfo newClassInfo, bool addDependentClasses)
        {
            var classInfo = GetSession().GetMetaModel().GetClassInfo(newClassInfo.GetFullClassName(), false);
            if (classInfo != null && classInfo.GetPosition() != -1)
                return classInfo;

            return PersistClass(newClassInfo, -1, true, addDependentClasses);
        }

        public virtual ClassInfoList AddClasses(ClassInfoList classInfoList)
        {
            IEnumerator iterator = classInfoList.GetClassInfos().GetEnumerator();
            while (iterator.MoveNext())
                AddClass((ClassInfo) iterator.Current, true);

            return classInfoList;
        }

        public virtual void WriteClassInfoHeader(ClassInfo classInfo, long position, bool writeInTransaction)
        {
            var classId = classInfo.GetId();
            if (classId == null)
            {
                classId = _idManager.GetNextClassId(position);
                classInfo.SetId(classId);
            }
            else
                _idManager.UpdateClassPositionForId(classId, position, true);

            _fsi.SetWritePosition(position, writeInTransaction);
            if (OdbConfiguration.IsDebugEnabled(LogId))
                DLogger.Debug(string.Format("Writing new Class info header at {0} : {1}", position, classInfo));

            // Real value of block size is only known at the end of the writing
            _fsi.WriteInt(0, writeInTransaction, "block size");
            _fsi.WriteByte(BlockTypes.BlockTypeClassHeader, writeInTransaction, "class header block type");
            _fsi.WriteByte(classInfo.GetClassCategory(), writeInTransaction, "Class info category");
            _fsi.WriteLong(classId.ObjectId, writeInTransaction, "class id", DefaultWriteAction.DataWriteAction);

            WriteOid(classInfo.GetPreviousClassOID(), writeInTransaction, "prev class oid",
                     DefaultWriteAction.DataWriteAction);

            WriteOid(classInfo.GetNextClassOID(), writeInTransaction, "next class oid",
                     DefaultWriteAction.DataWriteAction);

            _fsi.WriteLong(classInfo.GetCommitedZoneInfo().GetNbObjects(), writeInTransaction, "class nb objects",
                           DefaultWriteAction.DataWriteAction);

            WriteOid(classInfo.GetCommitedZoneInfo().First, writeInTransaction, "class first obj pos",
                     DefaultWriteAction.DataWriteAction);

            WriteOid(classInfo.GetCommitedZoneInfo().Last, writeInTransaction, "class last obj pos",
                     DefaultWriteAction.DataWriteAction);

            // FIXME : append extra info if not empty (.net compatibility)
            _fsi.WriteString(classInfo.GetFullClassName(), false, writeInTransaction);
            _fsi.WriteInt(classInfo.GetMaxAttributeId(), writeInTransaction, "Max attribute id");

            if (classInfo.GetAttributesDefinitionPosition() != -1)
            {
                _fsi.WriteLong(classInfo.GetAttributesDefinitionPosition(), writeInTransaction, "class att def pos",
                               DefaultWriteAction.DataWriteAction);
            }
            else
            {
                // @todo check this
                _fsi.WriteLong(-1, writeInTransaction, "class att def pos", DefaultWriteAction.DataWriteAction);
            }

            var blockSize = (int) (_fsi.GetPosition() - position);
            WriteBlockSizeAt(position, blockSize, writeInTransaction, classInfo);
        }

        public virtual void UpdateClassInfo(ClassInfo classInfo, bool writeInTransaction)
        {
            // first check dependent classes
            var dependingAttributes = classInfo.GetAllNonNativeAttributes();
            var metaModel = GetSession().GetMetaModel();

            foreach (var classAttributeInfo in dependingAttributes)
            {
                try
                {
                    var existingClassInfo = metaModel.GetClassInfo(classAttributeInfo.GetFullClassname(), false);
                    if (existingClassInfo == null)
                    {
                        // TODO check if this getClassInfo is ok. Maybe, should
                        // use
                        // a buffered one
                        AddClasses(_classIntrospector.Introspect(classAttributeInfo.GetFullClassname(), true));
                    }
                    else
                    {
                        // FIXME should we update class info?
                        classAttributeInfo.SetClassInfo(existingClassInfo);
                    }
                }
                catch (Exception e)
                {
                    throw new OdbRuntimeException(
                        NDatabaseError.ClassIntrospectionError.AddParameter(classAttributeInfo.GetFullClassname()), e);
                }
            }
            // To force the rewrite of class info body
            classInfo.SetAttributesDefinitionPosition(-1);
            var newCiPosition = _fsi.GetAvailablePosition();
            classInfo.SetPosition(newCiPosition);
            WriteClassInfoHeader(classInfo, newCiPosition, writeInTransaction);
            WriteClassInfoBody(classInfo, _fsi.GetAvailablePosition(), writeInTransaction);
        }

        public virtual OID WriteNonNativeObjectInfo(OID existingOid, NonNativeObjectInfo objectInfo, long position,
                                                    bool writeDataInTransaction, bool isNewObject)
        {
            var lsession = GetSession();
            var cache = lsession.GetCache();
            var hasObject = objectInfo.GetObject() != null;

            // Checks if object is null,for null objects,there is nothing to do
            if (objectInfo.IsNull())
                return StorageEngineConstant.NullObjectId;

            var metaModel = lsession.GetMetaModel();
            // first checks if the class of this object already exist in the
            // metamodel
            if (!metaModel.ExistClass(objectInfo.GetClassInfo().GetFullClassName()))
                AddClass(objectInfo.GetClassInfo(), true);

            // if position is -1, gets the position where to write the object
            if (position == -1)
            {
                // Write at the end of the file
                position = _fsi.GetAvailablePosition();
                // Updates the meta object position
                objectInfo.SetPosition(position);
            }

            // Gets the object id
            var oid = existingOid;
            if (oid == null)
            {
                // If, to get the next id, a new id block must be created, then
                // there is an extra work
                // to update the current object position
                if (_idManager.MustShift())
                {
                    oid = _idManager.GetNextObjectId(position);
                    // The id manager wrote in the file so the position for the
                    // object must be re-computed
                    position = _fsi.GetAvailablePosition();
                    // The oid must be associated to this new position - id
                    // operations are always out of transaction
                    // in this case, the update is done out of the transaction as a
                    // rollback won t need to
                    // undo this. We are just creating the id
                    // => third parameter(write in transaction) = false
                    _idManager.UpdateObjectPositionForOid(oid, position, false);
                }
                else
                    oid = _idManager.GetNextObjectId(position);
            }
            else
            {
                // If an oid was passed, it is because object already exist and
                // is being updated. So we
                // must update the object position
                // Here the update of the position of the id must be done in
                // transaction as the object
                // position of the id is being updated, and a rollback should undo
                // this
                // => third parameter(write in transaction) = true
                _idManager.UpdateObjectPositionForOid(oid, position, true);
                // Keep the relation of id and position in the cache until the
                // commit
                cache.SavePositionOfObjectWithOid(oid, position);
            }

            // Sets the oid of the object in the inserting cache
            cache.UpdateIdOfInsertingObject(objectInfo.GetObject(), oid);

            // Only add the oid to unconnected zone if it is a new object
            if (isNewObject)
            {
                cache.AddOIDToUnconnectedZone(oid);
                if (OdbConfiguration.ReconnectObjectsToSession())
                {
                    var crossSessionCache =
                        CacheFactory.GetCrossSessionCache(StorageEngine.GetBaseIdentification().GetIdentification());
                    crossSessionCache.AddObject(objectInfo.GetObject(), oid);
                }
            }

            objectInfo.SetOid(oid);
            if (OdbConfiguration.IsDebugEnabled(LogId))
            {
                DLogger.Debug(string.Format("Start Writing non native object of type {0} at {1} , oid = {2} : {3}",
                                            objectInfo.GetClassInfo().GetFullClassName(), position, oid,
                                            objectInfo));
            }

            if (objectInfo.GetClassInfo() == null || objectInfo.GetClassInfo().GetId() == null)
            {
                if (objectInfo.GetClassInfo() != null)
                {
                    var clinfo =
                        StorageEngine.GetSession(true).GetMetaModel().GetClassInfo(
                            objectInfo.GetClassInfo().GetFullClassName(), true);
                    objectInfo.SetClassInfo(clinfo);
                }
                else
                    throw new OdbRuntimeException(NDatabaseError.UndefinedClassInfo.AddParameter(objectInfo.ToString()));
            }

            // updates the meta model - If class already exist, it returns the
            // metamodel class, which contains
            // a bit more informations
            var classInfo = AddClass(objectInfo.GetClassInfo(), true);
            objectInfo.SetClassInfo(classInfo);
            // 
            if (isNewObject)
                ManageNewObjectPointers(objectInfo, classInfo);

            if (OdbConfiguration.SaveHistory())
            {
                classInfo.AddHistory(new InsertHistoryInfo("insert", oid, position, objectInfo.GetPreviousObjectOID(),
                                                           objectInfo.GetNextObjectOID()));
            }

            _fsi.SetWritePosition(position, writeDataInTransaction);
            objectInfo.SetPosition(position);
            var nbAttributes = objectInfo.GetClassInfo().GetAttributes().Count;
            // compute the size of the array of byte needed till the attibute
            // positions
            // BlockSize + Block Type + ObjectId + ClassInfoId + Previous + Next +
            // CreatDate + UpdateDate + VersionNumber + ObjectRef + isSync + NbAttri
            // + Attributes
            // Int + Int + Long + Long + Long + Long + Long + Long + int + Long +
            // Bool + int + variable
            // 7 Longs + 4Ints + 1Bool + variable
            var tsize = 7*OdbType.SizeOfLong + 3*OdbType.SizeOfInt + 2*OdbType.SizeOfByte;
            var bytes = new byte[tsize];
            // Block size
            _byteArrayConverter.IntToByteArray(0, bytes, 0);
            // Block type
            bytes[4] = BlockTypes.BlockTypeNonNativeObject;
            // fsi.writeInt(BlockTypes.BLOCK_TYPE_NON_NATIVE_OBJECT,
            // writeDataInTransaction, "block size");
            // The object id
            EncodeOid(oid, bytes, 5);
            // fsi.writeLong(oid.getObjectId(), writeDataInTransaction, "oid",
            // DefaultWriteAction.DATA_WRITE_ACTION);
            // Class info id
            _byteArrayConverter.LongToByteArray(classInfo.GetId().ObjectId, bytes, 13);
            // fsi.writeLong(classInfo.getId().getObjectId(),
            // writeDataInTransaction, "class info id",
            // DefaultWriteAction.DATA_WRITE_ACTION);
            // previous instance
            EncodeOid(objectInfo.GetPreviousObjectOID(), bytes, 21);
            // writeOid(objectInfo.getPreviousObjectOID(), writeDataInTransaction,
            // "prev instance", DefaultWriteAction.DATA_WRITE_ACTION);
            // next instance
            EncodeOid(objectInfo.GetNextObjectOID(), bytes, 29);
            // writeOid(objectInfo.getNextObjectOID(), writeDataInTransaction,
            // "next instance", DefaultWriteAction.DATA_WRITE_ACTION);
            // creation date, for update operation must be the original one
            _byteArrayConverter.LongToByteArray(objectInfo.GetHeader().GetCreationDate(), bytes, 37);
            // fsi.writeLong(objectInfo.getHeader().getCreationDate(),
            // writeDataInTransaction, "creation date",
            // DefaultWriteAction.DATA_WRITE_ACTION);
            _byteArrayConverter.LongToByteArray(OdbTime.GetCurrentTimeInTicks(), bytes, 45);
            // fsi.writeLong(OdbTime.getCurrentTimeInMs(), writeDataInTransaction,
            // "update date", DefaultWriteAction.DATA_WRITE_ACTION);
            // TODO check next version number
            _byteArrayConverter.IntToByteArray(objectInfo.GetHeader().GetObjectVersion(), bytes, 53);
            // fsi.writeInt(objectInfo.getHeader().getObjectVersion(),
            // writeDataInTransaction, "object version number");
            // not used yet. But it will point to an internal object of type
            // ObjectReference that will have details on the references:
            // All the objects that point to it: to enable object integrity
            _byteArrayConverter.LongToByteArray(-1, bytes, 57);
            // fsi.writeLong(-1, writeDataInTransaction, "object reference pointer",
            // DefaultWriteAction.DATA_WRITE_ACTION);
            // True if this object have been synchronized with main database, else
            // false
            _byteArrayConverter.BooleanToByteArray(false, bytes, 65);
            // fsi.writeBoolean(false, writeDataInTransaction,
            // "is syncronized with external db");
            // now write the number of attributes and the position of all
            // attributes, we do not know them yet, so write 00 but at the end
            // of the write operation
            // These positions will be updated
            // The positions that is going to be written are 'int' representing
            // the offset position of the attribute
            // first write the number of attributes
            // fsi.writeInt(nbAttributes, writeDataInTransaction, "nb attr");
            _byteArrayConverter.IntToByteArray(nbAttributes, bytes, 66);
            // Then write the array of bytes
            _fsi.WriteBytes(bytes, writeDataInTransaction, "NonNativeObjectInfoHeader");
            // Store the position
            var attributePositionStart = _fsi.GetPosition();
            var attributeSize = OdbType.SizeOfInt + OdbType.SizeOfLong;
            var abytes = new byte[nbAttributes*(attributeSize)];
            // here, just write an empty (0) array, as real values will be set at
            // the end
            _fsi.WriteBytes(abytes, writeDataInTransaction, "Empty Attributes");
            var attributesIdentification = new long[nbAttributes];
            var attributeIds = new int[nbAttributes];
            // Puts the object info in the cache
            // storageEngine.getSession().getCache().addObject(position,
            // aoi.getObject(), objectInfo.getHeader());

            var maxWritePosition = _fsi.GetPosition();
            // Loop on all attributes
            for (var i = 0; i < nbAttributes; i++)
            {
                // Gets the attribute meta description
                var classAttributeInfo = classInfo.GetAttributeInfo(i);
                // Gets the id of the attribute
                attributeIds[i] = classAttributeInfo.GetId();
                // Gets the attribute data
                var aoi2 = objectInfo.GetAttributeValueFromId(classAttributeInfo.GetId());
                if (aoi2 == null)
                {
                    // This only happens in 1 case : when a class has a field with
                    // the same name of one of is superclass. In this, the deeper
                    // attribute is null
                    if (classAttributeInfo.IsNative())
                        aoi2 = new NullNativeObjectInfo(classAttributeInfo.GetAttributeType().GetId());
                    else
                        aoi2 = new NonNativeNullObjectInfo(classAttributeInfo.GetClassInfo());
                }
                if (aoi2.IsNative())
                {
                    var nativeAttributePosition = InternalStoreObject((NativeObjectInfo) aoi2);
                    // For native objects , odb stores their position
                    attributesIdentification[i] = nativeAttributePosition;
                }
                else
                {
                    OID nonNativeAttributeOid;
                    if (aoi2.IsObjectReference())
                    {
                        var or = (ObjectReference) aoi2;
                        nonNativeAttributeOid = or.GetOid();
                    }
                    else
                        nonNativeAttributeOid = StoreObject(null, (NonNativeObjectInfo) aoi2);
                    // For non native objects , odb stores its oid as a negative
                    // number!!u
                    if (nonNativeAttributeOid != null)
                        attributesIdentification[i] = -nonNativeAttributeOid.ObjectId;
                    else
                        attributesIdentification[i] = StorageEngineConstant.NullObjectIdId;
                }
                var p = _fsi.GetPosition();
                if (p > maxWritePosition)
                    maxWritePosition = p;
            }
            // Updates attributes identification in the object info header
            objectInfo.GetHeader().SetAttributesIdentification(attributesIdentification);
            objectInfo.GetHeader().SetAttributesIds(attributeIds);
            var positionAfterWrite = maxWritePosition;
            // Now writes back the attribute positions
            _fsi.SetWritePosition(attributePositionStart, writeDataInTransaction);
            abytes = new byte[attributesIdentification.Length*(attributeSize)];
            for (var i = 0; i < attributesIdentification.Length; i++)
            {
                _byteArrayConverter.IntToByteArray(attributeIds[i], abytes, i*attributeSize);
                _byteArrayConverter.LongToByteArray(attributesIdentification[i], abytes,
                                                    i*(attributeSize) + OdbType.SizeOfInt);
                // fsi.writeInt(attributeIds[i], writeDataInTransaction, "attr id");
                // fsi.writeLong(attributesIdentification[i],
                // writeDataInTransaction, "att real pos",
                // DefaultWriteAction.DATA_WRITE_ACTION);
                // if (classInfo.getAttributeInfo(i).isNonNative() &&
                // attributesIdentification[i] > 0) {
                if (objectInfo.GetAttributeValueFromId(attributeIds[i]).IsNonNativeObject() &&
                    attributesIdentification[i] > 0)
                {
                    throw new OdbRuntimeException(
                        NDatabaseError.NonNativeAttributeStoredByPositionInsteadOfOid.AddParameter(
                            classInfo.GetAttributeInfo(i).GetName()).AddParameter(classInfo.GetFullClassName()).
                            AddParameter(attributesIdentification[i]));
                }
            }
            _fsi.WriteBytes(abytes, writeDataInTransaction, "Filled Attributes");
            _fsi.SetWritePosition(positionAfterWrite, writeDataInTransaction);
            var blockSize = (int) (positionAfterWrite - position);
            try
            {
                WriteBlockSizeAt(position, blockSize, writeDataInTransaction, objectInfo);
            }
            catch (OdbRuntimeException)
            {
                DLogger.Debug("Error while writing block size. pos after write " + positionAfterWrite +
                              " / start pos = " + position);
                // throw new ODBRuntimeException(storageEngine,"Error while writing
                // block size. pos after write " + positionAfterWrite + " / start
                // pos = " + position,e);
                throw;
            }
            if (OdbConfiguration.IsDebugEnabled(LogId))
            {
                DLogger.Debug("  Attributes positions of object with oid " + oid + " are " +
                              DisplayUtility.LongArrayToString(attributesIdentification));
                DLogger.Debug("End Writing non native object at " + position + " with oid " + oid +
                              " - prev oid=" + objectInfo.GetPreviousObjectOID() + " / next oid=" +
                              objectInfo.GetNextObjectOID());
                if (OdbConfiguration.IsDebugEnabled(LogIdDebug))
                    DLogger.Debug(" - current buffer : " + _fsi.GetIo());
            }
            // Only insert in index for new objects
            if (isNewObject)
            {
                // insert object id in indexes, if exist
                ManageIndexesForInsert(oid, objectInfo);
                var value = hasObject
                                ? objectInfo.GetObject()
                                : objectInfo;

                _triggerManager.ManageInsertTriggerAfter(objectInfo.GetClassInfo().GetFullClassName(), value, oid);
            }

            return oid;
        }

        /// <summary>
        ///   Insert the object in the index
        /// </summary>
        /// <param name="oid"> The object id </param>
        /// <param name="nnoi"> The object meta represenation </param>
        /// <returns> The number of indexes </returns>
        public virtual int ManageIndexesForInsert(OID oid, NonNativeObjectInfo nnoi)
        {
            var indexes = nnoi.GetClassInfo().GetIndexes();

            foreach (var index in indexes)
            {
                try
                {
                    index.BTree.Insert(index.ComputeKey(nnoi), oid);
                }
                catch (DuplicatedKeyException)
                {
                    // rollback what has been done
                    // bug #2510966
                    GetSession().Rollback();
                    throw;
                }
                // Check consistency : index should have size equal to the class
                // info element number
                if (index.BTree.GetSize() != nnoi.GetClassInfo().GetNumberOfObjects())
                {
                    throw new OdbRuntimeException(
                        NDatabaseError.BtreeSizeDiffersFromClassElementNumber.AddParameter(index.BTree.GetSize()).
                            AddParameter(nnoi.GetClassInfo().GetNumberOfObjects()));
                }
            }
            return indexes.Count;
        }

        /// <summary>
        ///   Insert the object in the index
        /// </summary>
        /// <param name="oid"> The object id </param>
        /// <param name="nnoi"> The object meta represenation </param>
        /// <returns> The number of indexes </returns>
        /// <exception cref="System.Exception">System.Exception</exception>
        public virtual int ManageIndexesForDelete(OID oid, NonNativeObjectInfo nnoi)
        {
            var indexes = nnoi.GetClassInfo().GetIndexes();

            foreach (var index in indexes)
            {
                // TODO manage collision!
                index.BTree.Delete(index.ComputeKey(nnoi), oid);
                // Check consistency : index should have size equal to the class
                // info element number
                if (index.BTree.GetSize() != nnoi.GetClassInfo().GetNumberOfObjects())
                {
                    throw new OdbRuntimeException(
                        NDatabaseError.BtreeSizeDiffersFromClassElementNumber.AddParameter(index.BTree.GetSize()).
                            AddParameter(nnoi.GetClassInfo().GetNumberOfObjects()));
                }
            }
            return indexes.Count;
        }

        public virtual int ManageIndexesForUpdate(OID oid, NonNativeObjectInfo nnoi,
                                                  NonNativeObjectInfo oldMetaRepresentation)
        {
            // takes the indexes from the oldMetaRepresentation because noi comes
            // from the client and is not always
            // in sync with the server meta model (In Client Server mode)
            var indexes = oldMetaRepresentation.GetClassInfo().GetIndexes();

            foreach (var index in indexes)
            {
                var oldKey = index.ComputeKey(oldMetaRepresentation);
                var newKey = index.ComputeKey(nnoi);

                // Only update index if key has changed!
                if (oldKey.CompareTo(newKey) != 0)
                {
                    var btree = index.BTree;
                    // TODO manage collision!
                    var old = btree.Delete(oldKey, oid);
                    // TODO check if old is equal to oldKey
                    btree.Insert(newKey, oid);
                    // Check consistency : index should have size equal to the class
                    // info element number
                    if (index.BTree.GetSize() != nnoi.GetClassInfo().GetNumberOfObjects())
                    {
                        throw new OdbRuntimeException(
                            NDatabaseError.BtreeSizeDiffersFromClassElementNumber.AddParameter(index.BTree.GetSize())
                                .AddParameter(nnoi.GetClassInfo().GetNumberOfObjects()));
                    }
                }
            }
            return indexes.Count;
        }

        /// <param name="oid"> The Oid of the object to be inserted </param>
        /// <param name="nnoi"> The object meta representation The object to be inserted in the database </param>
        /// <param name="isNewObject"> To indicate if object is new </param>
        /// <returns> The position of the inserted object </returns>
        public virtual OID InsertNonNativeObject(OID oid, NonNativeObjectInfo nnoi, bool isNewObject)
        {
            var ci = nnoi.GetClassInfo();
            var @object = nnoi.GetObject();
            // First check if object is already being inserted
            // This method returns -1 if object is not being inserted
            var cachedOid = GetSession().GetCache().IdOfInsertingObject(@object);
            if (cachedOid != null)
                return cachedOid;
            // Then checks if the class of this object already exist in the
            // meta model
            ci = AddClass(ci, true);
            // Resets the ClassInfo in the objectInfo to be sure it contains all
            // updated class info data
            nnoi.SetClassInfo(ci);
            // Mark this object as being inserted. To manage cyclic relations
            // The oid may be equal to -1
            // Later in the process the cache will be updated with the right oid
            GetSession().GetCache().StartInsertingObjectWithOid(@object, oid, nnoi);
            // false : do not write data in transaction. Data are always written
            // directly to disk. Pointers are written in transaction
            var newOid = WriteNonNativeObjectInfo(oid, nnoi, -1, false, isNewObject);
            if (newOid != StorageEngineConstant.NullObjectId)
                GetSession().GetCache().AddObject(newOid, @object, nnoi.GetHeader());
            return newOid;
        }

        /// <summary>
        ///   Updates an object.
        /// </summary>
        /// <remarks>
        ///   Updates an object. <pre>Try to update in place. Only change what has changed. This is restricted to particular types (fixed size types). If in place update is
        ///                        not possible, then deletes the current object and creates a new at the end of the database file and updates
        ///                        OID object position.
        ///                        &#064;param object The object to be updated
        ///                        &#064;param forceUpdate when true, no verification is done to check if update must be done.
        ///                        &#064;return The oid of the object, as a negative number
        ///                        &#064;</pre>
        /// </remarks>
        public virtual OID UpdateNonNativeObjectInfo(NonNativeObjectInfo nnoi, bool forceUpdate)
        {
            var hasObject = true;
            string message;
            var @object = nnoi.GetObject();
            var oid = nnoi.GetOid();
            if (@object == null)
                hasObject = false;
            // When there is index,we must *always* load the old meta representation
            // to compute index keys
            var withIndex = !nnoi.GetClassInfo().GetIndexes().IsEmpty();
            NonNativeObjectInfo oldMetaRepresentation = null;
            // Used to check consistency, at the end, the number of
            // nbConnectedObjects must and nbUnconnected must remain unchanged
            var nbConnectedObjects = nnoi.GetClassInfo().GetCommitedZoneInfo().GetNbObjects();
            var nbNonConnectedObjects = nnoi.GetClassInfo().GetUncommittedZoneInfo().GetNbObjects();
            var objectHasChanged = false;
            try
            {
                var lsession = GetSession();
                var positionBeforeWrite = _fsi.GetPosition();
                var tmpCache = lsession.GetTmpCache();
                var cache = lsession.GetCache();
                // Get header of the object (position, previous object position,
                // next
                // object position and class info position)
                // The header must be in the cache.
                var lastHeader = cache.GetObjectInfoHeaderFromOid(oid, true);
                if (lastHeader == null)
                    throw new OdbRuntimeException(
                        NDatabaseError.UnexpectedSituation.AddParameter("Header is null in update"));
                if (lastHeader.GetOid() == null)
                    throw new OdbRuntimeException(
                        NDatabaseError.InternalError.AddParameter("Header oid is null for oid " + oid));
                var objectIsInConnectedZone = cache.ObjectWithIdIsInCommitedZone(oid);
                var currentPosition = lastHeader.GetPosition();

                if (currentPosition == -1)
                {
                    throw new OdbRuntimeException(
                        NDatabaseError.InstancePositionIsNegative.AddParameter(currentPosition).AddParameter(oid).
                            AddParameter("In Object Info Header"));
                }
                if (OdbConfiguration.IsDebugEnabled(LogId))
                {
                    message = string.Format("start updating object at {0}, oid={1} : {2}",
                                            currentPosition, oid, nnoi);
                    DLogger.Debug(message);
                }
                // triggers,FIXME passing null to old object representation
                StorageEngine.GetTriggerManager().ManageUpdateTriggerBefore(nnoi.GetClassInfo().GetFullClassName(),
                                                                            null, hasObject ? @object : nnoi, oid);
                // Use to control if the in place update is ok. The
                // ObjectInstrospector stores the number of changes
                // that were detected and here we try to apply them using in place
                // update.If at the end
                // of the in place update the number of applied changes is smaller
                // then the number
                // of detected changes, then in place update was not successfully,
                // we
                // must do a real update,
                // creating an object elsewhere :-(
                if (!forceUpdate)
                {
                    var cachedOid = cache.IdOfInsertingObject(@object);
                    if (cachedOid != null)
                    {
                        // The object is being inserted (must be a cyclic
                        // reference), simply returns id id
                        return cachedOid;
                    }
                    // the nnoi (NonNativeObjectInfo is the meta representation of
                    // the object to update
                    // To know what must be upated we must get the meta
                    // representation of this object before
                    // The modification. Taking this 'old' meta representation from
                    // the
                    // cache does not resolve
                    // : because cache is a reference to the real object and object
                    // has been changed,
                    // so the cache is pointing to the reference, that has changed!
                    // This old meta representation must be re-read from the last
                    // committed database
                    // false, = returnInstance (java object) = false
                    try
                    {
                        var useCache = !objectIsInConnectedZone;
                        oldMetaRepresentation = _objectReader.ReadNonNativeObjectInfoFromPosition(null, oid,
                                                                                                  currentPosition,
                                                                                                  useCache, false);
                        tmpCache.ClearObjectInfos();
                    }
                    catch (OdbRuntimeException e)
                    {
                        throw new OdbRuntimeException(
                            NDatabaseError.InternalError.AddParameter("Error while reading old Object Info of oid " + oid +
                                                                     " at pos " + currentPosition), e);
                    }
                    // Make sure we work with the last version of the object
                    var onDiskVersion = oldMetaRepresentation.GetHeader().GetObjectVersion();
                    var onDiskUpdateDate = oldMetaRepresentation.GetHeader().GetUpdateDate();
                    var inCacheVersion = lastHeader.GetObjectVersion();
                    var inCacheUpdateDate = lastHeader.GetUpdateDate();
                    if (onDiskUpdateDate > inCacheUpdateDate || onDiskVersion > inCacheVersion)
                        lastHeader = oldMetaRepresentation.GetHeader();
                    nnoi.SetHeader(lastHeader);
                    // increase the object version number from the old meta
                    // representation
                    nnoi.GetHeader().IncrementVersionAndUpdateDate();
                    // Keep the creation date
                    nnoi.GetHeader().SetCreationDate(oldMetaRepresentation.GetHeader().GetCreationDate());
                    // Set the object of the old meta to make the object comparator
                    // understand, they are 2
                    // meta representation of the same object
                    // TODO , check if if is the best way to do
                    oldMetaRepresentation.SetObject(nnoi.GetObject());
                    // Reset the comparator
                    _comparator.Clear();
                    objectHasChanged = _comparator.HasChanged(oldMetaRepresentation, nnoi);
                    if (!objectHasChanged)
                    {
                        _fsi.SetWritePosition(positionBeforeWrite, true);
                        if (OdbConfiguration.IsDebugEnabled(LogId))
                            DLogger.Debug("updateObject : Object is unchanged - doing nothing");
                        return oid;
                    }
                    if (OdbConfiguration.IsDebugEnabled(LogId))
                    {
                        DLogger.Debug("\tmax recursion level is " +
                                      _comparator.GetMaxObjectRecursionLevel());
                        DLogger.Debug("\tattribute actions are : " +
                                      _comparator.GetChangedAttributeActions());
                        DLogger.Debug("\tnew objects are : " + _comparator.GetNewObjects());
                    }
                    if (OdbConfiguration.InPlaceUpdate() && _comparator.SupportInPlaceUpdate())
                    {
                        var nbAppliedChanges = ManageInPlaceUpdate(_comparator, @object, oid, lastHeader, cache,
                                                                   objectIsInConnectedZone);
                        // if number of applied changes is equal to the number of
                        // detected change
                        if (nbAppliedChanges == _comparator.GetNbChanges())
                        {
                            _nbInPlaceUpdates++;
                            UpdateUpdateTimeAndObjectVersionNumber(lastHeader, true);
                            cache.AddObject(oid, @object, lastHeader);
                            return oid;
                        }
                    }
                }
                // If we reach this update, In Place Update was not possible. Do a
                // normal update. Deletes the
                // current object and creates a new one
                if (oldMetaRepresentation == null && withIndex)
                {
                    // We must load old meta representation to be able to compute
                    // old index key to update index
                    oldMetaRepresentation = _objectReader.ReadNonNativeObjectInfoFromPosition(null, oid, currentPosition,
                                                                                              false, false);
                }
                _nbNormalUpdates++;
                if (hasObject)
                    cache.StartInsertingObjectWithOid(@object, oid, nnoi);
                // gets class info from in memory meta model
                var ci = lsession.GetMetaModel().GetClassInfoFromId(lastHeader.GetClassInfoId());
                if (hasObject)
                {
                    // removes the object from the cache
                    // cache.removeObjectWithOid(oid, object);
                    cache.EndInsertingObject(@object);
                }
                var previousObjectOID = lastHeader.GetPreviousObjectOID();
                var nextObjectOid = lastHeader.GetNextObjectOID();
                if (OdbConfiguration.IsDebugEnabled(LogId))
                {
                    DLogger.Debug("Updating object " + nnoi);
                    DLogger.Debug("position =  " + currentPosition + " | prev instance = " +
                                  previousObjectOID + " | next instance = " + nextObjectOid);
                }
                nnoi.SetPreviousInstanceOID(previousObjectOID);
                nnoi.SetNextObjectOID(nextObjectOid);
                // Mark the block of current object as deleted
                MarkAsDeleted(currentPosition, oid, objectIsInConnectedZone);
                // Creates the new object
                oid = InsertNonNativeObject(oid, nnoi, false);
                // This position after write must be call just after the insert!!
                var positionAfterWrite = _fsi.GetPosition();
                if (hasObject)
                {
                    // update cache
                    cache.AddObject(oid, @object, nnoi.GetHeader());
                }
                //TODO check if we must update cross session cache
                _fsi.SetWritePosition(positionAfterWrite, true);
                var nbConnectedObjectsAfter = nnoi.GetClassInfo().GetCommitedZoneInfo().GetNbObjects();
                var nbNonConnectedObjectsAfter = nnoi.GetClassInfo().GetUncommittedZoneInfo().GetNbObjects();
                if (nbConnectedObjectsAfter != nbConnectedObjects || nbNonConnectedObjectsAfter != nbNonConnectedObjects)
                {
                }
                // TODO check this
                // throw new
                // ODBRuntimeException(Error.INTERNAL_ERROR.addParameter("Error
                // in nb connected/unconnected counter"));
                return oid;
            }
            catch (Exception e)
            {
                message = "Error updating object " + nnoi + " : " +
                          e;
                DLogger.Error(message);
                throw new OdbRuntimeException(e, message);
            }
            finally
            {
                if (objectHasChanged)
                {
                    if (withIndex)
                        ManageIndexesForUpdate(oid, nnoi, oldMetaRepresentation);
                    // triggers,FIXME passing null to old object representation
                    // (oldMetaRepresentation may be null)
                    StorageEngine.GetTriggerManager().ManageUpdateTriggerAfter(
                        nnoi.GetClassInfo().GetFullClassName(), oldMetaRepresentation, hasObject ? @object : nnoi, oid);
                }
                if (OdbConfiguration.IsDebugEnabled(LogId))
                {
                    DLogger.Debug("end updating object with oid=" + oid + " at pos " +
                                  nnoi.GetPosition() + " => " + nnoi);
                }
            }
        }

        public virtual long WriteAtomicNativeObject(AtomicNativeObjectInfo anoi, bool writeInTransaction,
                                                    int totalSpaceIfString)
        {
            var startPosition = _fsi.GetPosition();
            var odbTypeId = anoi.GetOdbTypeId();
            WriteNativeObjectHeader(odbTypeId, anoi.IsNull(), BlockTypes.BlockTypeNativeObject, writeInTransaction);
            if (anoi.IsNull())
            {
                // Even if object is null, reserve space for to simplify/enable in
                // place update
                _fsi.EnsureSpaceFor(anoi.GetOdbType());
                return startPosition;
            }
            var @object = anoi.GetObject();
            switch (odbTypeId)
            {
                case OdbType.ByteId:
                    {
                        _fsi.WriteByte(((byte) @object), writeInTransaction);
                        break;
                    }

                case OdbType.BooleanId:
                    {
                        _fsi.WriteBoolean(((bool) @object), writeInTransaction);
                        break;
                    }

                case OdbType.CharacterId:
                    {
                        _fsi.WriteChar(((char) @object), writeInTransaction);
                        break;
                    }

                case OdbType.FloatId:
                    {
                        _fsi.WriteFloat(((float) @object), writeInTransaction);
                        break;
                    }

                case OdbType.DoubleId:
                    {
                        _fsi.WriteDouble(((double) @object), writeInTransaction);
                        break;
                    }

                case OdbType.IntegerId:
                    {
                        _fsi.WriteInt(((int) @object), writeInTransaction, "native attr");
                        break;
                    }

                case OdbType.LongId:
                    {
                        _fsi.WriteLong(((long) @object), writeInTransaction, "native attr",
                                       DefaultWriteAction.DataWriteAction);
                        break;
                    }

                case OdbType.ShortId:
                    {
                        _fsi.WriteShort(((short) @object), writeInTransaction);
                        break;
                    }

                case OdbType.BigDecimalId:
                    {
                        _fsi.WriteBigDecimal((Decimal) @object, writeInTransaction);
                        break;
                    }

                case OdbType.DateId:
                case OdbType.DateSqlId:
                case OdbType.DateTimestampId:
                    {
                        _fsi.WriteDate((DateTime) @object, writeInTransaction);
                        break;
                    }

                case OdbType.StringId:
                    {
                        _fsi.WriteString((string) @object, writeInTransaction, true, totalSpaceIfString);
                        break;
                    }

                case OdbType.OidId:
                    {
                        var oid = ((OdbObjectOID) @object).ObjectId;
                        _fsi.WriteLong(oid, writeInTransaction, "ODB OID", DefaultWriteAction.DataWriteAction);
                        break;
                    }

                case OdbType.ObjectOidId:
                    {
                        var ooid = ((OdbObjectOID) @object).ObjectId;
                        _fsi.WriteLong(ooid, writeInTransaction, "ODB OID", DefaultWriteAction.DataWriteAction);
                        break;
                    }

                case OdbType.ClassOidId:
                    {
                        var coid = ((OdbClassOID) @object).ObjectId;
                        _fsi.WriteLong(coid, writeInTransaction, "ODB OID", DefaultWriteAction.DataWriteAction);
                        break;
                    }

                default:
                    {
                        // FIXME replace RuntimeException by a
                        throw new Exception("native type with odb type id " + odbTypeId + " (" +
                                            OdbType.GetNameFromId(odbTypeId) + ") for attribute ? is not suported");
                    }
            }
            return startPosition;
        }

        /// <summary>
        ///   Updates the previous object position field of the object at objectPosition
        /// </summary>
        /// <param name="objectOID"> </param>
        /// <param name="previousObjectOID"> </param>
        /// <param name="writeInTransaction"> </param>
        public virtual void UpdatePreviousObjectFieldOfObjectInfo(OID objectOID, OID previousObjectOID,
                                                                  bool writeInTransaction)
        {
            var objectPosition = _idManager.GetObjectPositionWithOid(objectOID, true);
            _fsi.SetWritePosition(objectPosition + StorageEngineConstant.ObjectOffsetPreviousObjectOid,
                                  writeInTransaction);
            WriteOid(previousObjectOID, writeInTransaction, "prev object position",
                     DefaultWriteAction.PointerWriteAction);
        }

        /// <summary>
        ///   Update next object oid field of the object at the specific position
        /// </summary>
        public virtual void UpdateNextObjectFieldOfObjectInfo(OID objectOID, OID nextObjectOID, bool writeInTransaction)
        {
            var objectPosition = _idManager.GetObjectPositionWithOid(objectOID, true);
            _fsi.SetWritePosition(objectPosition + StorageEngineConstant.ObjectOffsetNextObjectOid, writeInTransaction);
            WriteOid(nextObjectOID, writeInTransaction, "next object oid of object info",
                     DefaultWriteAction.PointerWriteAction);
        }

        /// <summary>
        ///   Mark a block as deleted
        /// </summary>
        /// <returns> The block size </returns>
        public virtual int MarkAsDeleted(long currentPosition, OID oid, bool writeInTransaction)
        {
            _fsi.SetReadPosition(currentPosition);
            var blockSize = _fsi.ReadInt();
            _fsi.SetWritePosition(currentPosition + StorageEngineConstant.NativeObjectOffsetBlockType,
                                  writeInTransaction);
            // Do not write block size, leave it as it is, to know the available
            // space for future use
            _fsi.WriteByte(BlockTypes.BlockTypeDeleted, writeInTransaction);
            StoreFreeSpace(currentPosition, blockSize);
            return blockSize;
        }

        /// <summary>
        ///   Updates the instance related field of the class info into the database file Updates the number of objects, the first object oid and the next class oid
        /// </summary>
        /// <param name="classInfo"> The class info to be updated </param>
        /// <param name="writeInTransaction"> To specify if it must be part of a transaction @ </param>
        public virtual void UpdateInstanceFieldsOfClassInfo(ClassInfo classInfo, bool writeInTransaction)
        {
            var currentPosition = _fsi.GetPosition();
            if (OdbConfiguration.IsDebugEnabled(LogIdDebug))
                DLogger.Debug("Start of updateInstanceFieldsOfClassInfo for " +
                              classInfo.GetFullClassName());
            var position = classInfo.GetPosition() + StorageEngineConstant.ClassOffsetClassNbObjects;
            _fsi.SetWritePosition(position, writeInTransaction);
            var nbObjects = classInfo.GetNumberOfObjects();
            _fsi.WriteLong(nbObjects, writeInTransaction, "class info update nb objects",
                           DefaultWriteAction.PointerWriteAction);
            WriteOid(classInfo.GetCommitedZoneInfo().First, writeInTransaction, "class info update first obj oid",
                     DefaultWriteAction.PointerWriteAction);
            WriteOid(classInfo.GetCommitedZoneInfo().Last, writeInTransaction, "class info update last obj oid",
                     DefaultWriteAction.PointerWriteAction);
            if (OdbConfiguration.IsDebugEnabled(LogIdDebug))
                DLogger.Debug("End of updateInstanceFieldsOfClassInfo for " +
                              classInfo.GetFullClassName());
            _fsi.SetWritePosition(currentPosition, writeInTransaction);
        }

        public virtual void Flush()
        {
            _fsi.Flush();
        }

        public virtual IIdManager GetIdManager()
        {
            return _idManager;
        }

        public virtual void Close()
        {
            _objectReader = null;
            if (_idManager != null)
            {
                _idManager.Clear();
                _idManager = null;
            }
            StorageEngine = null;
            _fsi.Close();
            _fsi = null;
        }

        public virtual IFileSystemInterface GetFsi()
        {
            return _fsi;
        }

        public virtual OID Delete(ObjectInfoHeader header)
        {
            var lsession = GetSession();
            var cache = lsession.GetCache();
            var objectPosition = header.GetPosition();
            var classInfoId = header.GetClassInfoId();
            var oid = header.GetOid();
            // gets class info from in memory meta model
            var ci = GetSession().GetMetaModel().GetClassInfoFromId(classInfoId);
            var withIndex = !ci.GetIndexes().IsEmpty();
            NonNativeObjectInfo nnoi = null;
            // When there is index,we must *always* load the old meta representation
            // to compute index keys
            if (withIndex)
                nnoi = _objectReader.ReadNonNativeObjectInfoFromPosition(ci, header.GetOid(), objectPosition, true,
                                                                         false);
            // a boolean value to indicate if object is in connected zone or not
            // This will be used to know if work can be done out of transaction
            // for unconnected object,changes can be written directly, else we must
            // use Transaction (using WriteAction)
            var objectIsInConnectedZone = cache.ObjectWithIdIsInCommitedZone(header.GetOid());
            // triggers
            // FIXME
            _triggerManager.ManageDeleteTriggerBefore(ci.GetFullClassName(), null, header.GetOid());
            var nbObjects = ci.GetNumberOfObjects();
            var previousObjectOID = header.GetPreviousObjectOID();
            var nextObjectOID = header.GetNextObjectOID();
            if (OdbConfiguration.IsDebugEnabled(LogId))
            {
                DLogger.Debug("Deleting object with id " + header.GetOid() + " - In connected zone =" +
                              objectIsInConnectedZone + " -  with index =" + withIndex);
                DLogger.Debug("position =  " + objectPosition + " | prev oid = " + previousObjectOID + " | next oid = " +
                              nextObjectOID);
            }
            var isFirstObject = previousObjectOID == null;
            var isLastObject = nextObjectOID == null;
            var mustUpdatePreviousObjectPointers = false;
            var mustUpdateNextObjectPointers = false;
            var mustUpdateLastObjectOfClassInfo = false;

            if (isFirstObject || isLastObject)
            {
                if (isFirstObject)
                {
                    // The deleted object is the first, must update first instance
                    // OID field of the class
                    if (objectIsInConnectedZone)
                    {
                        // update first object oid of the class info in memory
                        ci.GetCommitedZoneInfo().First = nextObjectOID;
                    }
                    else
                    {
                        // update first object oid of the class info in memory
                        ci.GetUncommittedZoneInfo().First = nextObjectOID;
                    }
                    if (nextObjectOID != null)
                    {
                        // Update next object 'previous object oid' to null
                        UpdatePreviousObjectFieldOfObjectInfo(nextObjectOID, null, objectIsInConnectedZone);
                        mustUpdateNextObjectPointers = true;
                    }
                }
                // It can be first and last
                if (isLastObject)
                {
                    // The deleted object is the last, must update last instance
                    // OID field of the class
                    // update last object position of the class info in memory
                    if (objectIsInConnectedZone)
                    {
                        // the object is a committed object
                        ci.GetCommitedZoneInfo().Last = previousObjectOID;
                    }
                    else
                    {
                        // The object is not committed and it is the last and is
                        // being deleted
                        ci.GetUncommittedZoneInfo().Last = previousObjectOID;
                    }
                    if (previousObjectOID != null)
                    {
                        // Update 'next object oid' of previous object to null
                        // if we are in unconnected zone, change can be done
                        // directly,else it must be done in transaction
                        UpdateNextObjectFieldOfObjectInfo(previousObjectOID, null, objectIsInConnectedZone);
                        // Now update data of the cache
                        mustUpdatePreviousObjectPointers = true;
                        mustUpdateLastObjectOfClassInfo = true;
                    }
                }
            }
            else
            {
                // Normal case, the deleted object has previous and next object
                // pull the deleted object
                // Mark the 'next object oid field' of the previous object
                // pointing the next object
                UpdateNextObjectFieldOfObjectInfo(previousObjectOID, nextObjectOID, objectIsInConnectedZone);
                // Mark the 'previous object position field' of the next object
                // pointing the previous object
                UpdatePreviousObjectFieldOfObjectInfo(nextObjectOID, previousObjectOID, objectIsInConnectedZone);
                mustUpdateNextObjectPointers = true;
                mustUpdatePreviousObjectPointers = true;
            }
            if (mustUpdateNextObjectPointers)
                UpdateNextObjectPreviousPointersInCache(nextObjectOID, previousObjectOID, cache);
            if (mustUpdatePreviousObjectPointers)
            {
                var oih = UpdatePreviousObjectNextPointersInCache(nextObjectOID, previousObjectOID, cache);
                if (mustUpdateLastObjectOfClassInfo)
                    ci.SetLastObjectInfoHeader(oih);
            }
            var metaModel = lsession.GetMetaModel();
            // Saves the fact that something has changed in the class (number of
            // objects and/or last object oid)
            metaModel.AddChangedClass(ci);
            if (objectIsInConnectedZone)
                ci.GetCommitedZoneInfo().DecreaseNbObjects();
            else
                ci.GetUncommittedZoneInfo().DecreaseNbObjects();
            // Manage deleting the last object of the committed zone
            CIZoneInfo commitedZoneInfo = ci.GetCommitedZoneInfo();

            var isLastObjectOfCommitedZone = oid.Equals(commitedZoneInfo.Last);
            if (isLastObjectOfCommitedZone)
            {
                // Load the object info header of the last committed object
                var oih = _objectReader.ReadObjectInfoHeaderFromOid(oid, true);
                // Updates last committed object id of the committed zone.
                // Here, it can be null, but there is no problem
                commitedZoneInfo.Last = oih.GetPreviousObjectOID();
                // A simple check, if commitedZI.last is null, nbObject must be 0
                if (commitedZoneInfo.Last == null && commitedZoneInfo.HasObjects())
                {
                    throw new OdbRuntimeException(
                        NDatabaseError.InternalError.AddParameter(
                            "The last object of the commited zone has been deleted but the Zone still have objects : nbobjects=" +
                            commitedZoneInfo.GetNbObjects()));
                }
            }
            // Manage deleting the first object of the uncommitted zone
            var uncommittedZoneInfo = ci.GetUncommittedZoneInfo();

            var isFirstObjectOfUncommitedZone = oid.Equals(uncommittedZoneInfo.First);
            if (isFirstObjectOfUncommitedZone)
            {
                if (uncommittedZoneInfo.HasObjects())
                {
                    // Load the object info header of the first uncommitted object
                    var oih = _objectReader.ReadObjectInfoHeaderFromOid(oid, true);
                    // Updates first uncommitted oid with the second uncommitted oid
                    // Here, it can be null, but there is no problem
                    uncommittedZoneInfo.First = oih.GetNextObjectOID();
                }
                else
                    uncommittedZoneInfo.First = null;
            }
            if (isFirstObject && isLastObject)
            {
                // The object was the first and the last object => it was the only
                // object
                // There is no more objects of this type => must set to null the
                // ClassInfo LastObjectOID
                ci.SetLastObjectInfoHeader(null);
            }
            GetIdManager().UpdateIdStatus(header.GetOid(), IDStatus.Deleted);
            // The update of the place must be done in transaction if object is in
            // committed zone, else it can be done directly in the file
            MarkAsDeleted(objectPosition, header.GetOid(), objectIsInConnectedZone);
            cache.MarkIdAsDeleted(header.GetOid());
            if (withIndex)
                ManageIndexesForDelete(header.GetOid(), nnoi);
            // triggers
            _triggerManager.ManageDeleteTriggerAfter(ci.GetFullClassName(), null, header.GetOid());
            return header.GetOid();
        }

        public virtual void SetTriggerManager(ITriggerManager triggerManager)
        {
            _triggerManager = triggerManager;
        }

        #endregion

        public abstract IFileSystemInterface BuildFsi();

//        public virtual void WriteUserAndPassword(string user, string password, bool writeInTransaction)
//        {
//            if (user != null && password != null)
//            {
//                var encryptedPassword = password;
//                _fsi.WriteBoolean(true, writeInTransaction, "has user and password");
//                if (user.Length > 20)
//                    throw new OdbRuntimeException(NDatabaseError.UserNameTooLong.AddParameter(user).AddParameter(20));
//                if (password.Length > 20)
//                    throw new OdbRuntimeException(NDatabaseError.PasswordTooLong.AddParameter(20));
//                _fsi.WriteString(user, writeInTransaction, true, 50);
//                _fsi.SetWritePosition(StorageEngineConstant.DatabaseHeaderDatabasePassword, writeInTransaction);
//                _fsi.WriteString(encryptedPassword, writeInTransaction, true, 50);
//            }
//            else
//            {
//                _fsi.WriteBoolean(false, writeInTransaction, "database without user and password");
//                _fsi.WriteString("no-user", writeInTransaction, true, 50);
//                _fsi.WriteString("no-password", writeInTransaction, true, 50);
//            }
//        }

        /// <summary>
        ///   Write the version in the database file
        /// </summary>
        public virtual void WriteVersion(bool writeInTransaction)
        {
            _fsi.SetWritePosition(StorageEngineConstant.DatabaseHeaderVersionPosition, writeInTransaction);
            _fsi.WriteInt(StorageEngineConstant.CurrentFileFormatVersion, writeInTransaction,
                          "database file format version");
        }

        public virtual IDatabaseId WriteDatabaseId(long creationDate, bool writeInTransaction)
        {
            var databaseId = UUID.GetDatabaseId(creationDate);

            _fsi.WriteLong(databaseId.GetIds()[0], writeInTransaction, "database id 1/4",
                           DefaultWriteAction.DirectWriteAction);
            _fsi.WriteLong(databaseId.GetIds()[1], writeInTransaction, "database id 2/4",
                           DefaultWriteAction.DirectWriteAction);
            _fsi.WriteLong(databaseId.GetIds()[2], writeInTransaction, "database id 3/4",
                           DefaultWriteAction.DirectWriteAction);
            _fsi.WriteLong(databaseId.GetIds()[3], writeInTransaction, "database id 4/4",
                           DefaultWriteAction.DirectWriteAction);

            StorageEngine.SetDatabaseId(databaseId);
            return databaseId;
        }

        /// <summary>
        ///   Write the number of classes in meta-model
        /// </summary>
        public virtual void WriteNumberOfClasses(long number, bool writeInTransaction)
        {
            _fsi.SetWritePosition(StorageEngineConstant.DatabaseHeaderNumberOfClassesPosition, writeInTransaction);
            _fsi.WriteLong(number, writeInTransaction, "nb classes", DefaultWriteAction.DirectWriteAction);
        }

        /// <summary>
        ///   Write the database characterEncoding
        /// </summary>
        public virtual void WriteDatabaseCharacterEncoding(bool writeInTransaction)
        {
            _fsi.SetWritePosition(StorageEngineConstant.DatabaseHeaderDatabaseCharacterEncodingPosition,
                                  writeInTransaction);
            if (OdbConfiguration.HasEncoding())
                _fsi.WriteString(OdbConfiguration.GetDatabaseCharacterEncoding(), writeInTransaction, true, 50);
            else
                _fsi.WriteString(StorageEngineConstant.NoEncoding, writeInTransaction, false, 50);
        }

        public virtual void EncodeOid(OID oid, byte[] bytes, int offset)
        {
            if (oid == null)
                _byteArrayConverter.LongToByteArray(-1, bytes, offset);
            else
            {
                // fsi.writeLong(-1, writeInTransaction, label, writeAction);
                _byteArrayConverter.LongToByteArray(oid.ObjectId, bytes, offset);
            }
        }

        // fsi.writeLong(oid.getObjectId(), writeInTransaction, label,
        // writeAction);
        public virtual void WriteOid(OID oid, bool writeInTransaction, string label, int writeAction)
        {
            if (oid == null)
                _fsi.WriteLong(-1, writeInTransaction, label, writeAction);
            else
                _fsi.WriteLong(oid.ObjectId, writeInTransaction, label, writeAction);
        }

        /// <summary>
        ///   Write the class info body to the database file.
        /// </summary>
        /// <remarks>
        ///   Write the class info body to the database file. TODO Check if we really must recall the writeClassInfoHeader
        /// </remarks>
        public virtual void WriteClassInfoBody(ClassInfo classInfo, long position, bool writeInTransaction)
        {
            if (OdbConfiguration.IsDebugEnabled(LogId))
                DLogger.Debug("Writing new Class info body at " + position + " : " + classInfo);
            // updates class info
            classInfo.SetAttributesDefinitionPosition(position);
            // FIXME : change this to write only the position and not the whole
            // header
            WriteClassInfoHeader(classInfo, classInfo.GetPosition(), writeInTransaction);
            _fsi.SetWritePosition(position, writeInTransaction);
            // block definition
            _fsi.WriteInt(0, writeInTransaction, "block size");
            _fsi.WriteByte(BlockTypes.BlockTypeClassBody, writeInTransaction);
            // number of attributes
            _fsi.WriteLong(classInfo.GetAttributes().Count, writeInTransaction, "class nb attributes",
                           DefaultWriteAction.DataWriteAction);
            for (var i = 0; i < classInfo.GetAttributes().Count; i++)
            {
                var classAttributeInfo = classInfo.GetAttributes()[i];
                WriteClassAttributeInfo(classAttributeInfo, writeInTransaction);
            }
            var blockSize = (int) (_fsi.GetPosition() - position);
            WriteBlockSizeAt(position, blockSize, writeInTransaction, classInfo);
        }

        public virtual long WriteClassInfoIndexes(ClassInfo classInfo)
        {
            var position = _fsi.GetAvailablePosition();
            _fsi.SetWritePosition(position, true);

            long previousIndexPosition = -1;
            for (var i = 0; i < classInfo.GetNumberOfIndexes(); i++)
            {
                var currentIndexPosition = _fsi.GetPosition();
                var classInfoIndex = classInfo.GetIndex(i);
                _fsi.WriteInt(0, true, "block size");
                _fsi.WriteByte(BlockTypes.BlockTypeIndex, true, "Index block type");
                _fsi.WriteLong(previousIndexPosition, true, "prev index pos",
                               DefaultWriteAction.PointerWriteAction);
                // The next position is only know at the end of the write
                _fsi.WriteLong(-1, true, "next index pos", DefaultWriteAction.PointerWriteAction);
                _fsi.WriteString(classInfoIndex.Name, false, true);
                _fsi.WriteBoolean(classInfoIndex.IsUnique, true, "index is unique");
                _fsi.WriteByte(classInfoIndex.Status, true, "index status");
                _fsi.WriteLong(classInfoIndex.CreationDate, true, "creation date",
                               DefaultWriteAction.DataWriteAction);
                _fsi.WriteLong(classInfoIndex.LastRebuild, true, "last rebuild",
                               DefaultWriteAction.DataWriteAction);
                _fsi.WriteInt(classInfoIndex.AttributeIds.Length, true, "number of fields");
                for (var j = 0; j < classInfoIndex.AttributeIds.Length; j++)
                    _fsi.WriteInt(classInfoIndex.AttributeIds[j], true, "attr id");
                var currentPosition = _fsi.GetPosition();
                // Write the block size
                var blockSize = (int) (_fsi.GetPosition() - currentIndexPosition);
                WriteBlockSizeAt(currentIndexPosition, blockSize, true, classInfo);
                // Write the next index position
                long nextIndexPosition;
                if (i + 1 < classInfo.GetNumberOfIndexes())
                    nextIndexPosition = currentPosition;
                else
                    nextIndexPosition = -1;
                // reset cursor to write the next position
                _fsi.SetWritePosition(
                    currentIndexPosition + OdbType.Integer.GetSize() + OdbType.Byte.GetSize() + OdbType.Long.GetSize(),
                    true);
                _fsi.WriteLong(nextIndexPosition, true, "next index pos",
                               DefaultWriteAction.PointerWriteAction);
                previousIndexPosition = currentIndexPosition;
                // reset the write cursor
                _fsi.SetWritePosition(currentPosition, true);
            }
            return position;
        }

        /// <summary>
        ///   Resets the position of the first class of the metamodel.
        /// </summary>
        /// <remarks>
        ///   Resets the position of the first class of the metamodel. It Happens when database is being refactored
        /// </remarks>
        public virtual void WriteFirstClassInfoOID(OID classInfoId, bool inTransaction)
        {
            long positionToWrite = StorageEngineConstant.DatabaseHeaderFirstClassOid;
            _fsi.SetWritePosition(positionToWrite, inTransaction);
            WriteOid(classInfoId, inTransaction, "first class info oid", DefaultWriteAction.DataWriteAction);
            if (OdbConfiguration.IsDebugEnabled(LogId))
                DLogger.Debug("Updating first class info oid at " + positionToWrite + " with oid " +
                              classInfoId);
        }

        /// <summary>
        ///   Writes a class attribute info, an attribute of a class
        /// </summary>
        private void WriteClassAttributeInfo(ClassAttributeInfo cai, bool writeInTransaction)
        {
            _fsi.WriteInt(cai.GetId(), writeInTransaction, "attribute id");
            _fsi.WriteBoolean(cai.IsNative(), writeInTransaction);
            if (cai.IsNative())
            {
                _fsi.WriteInt(cai.GetAttributeType().GetId(), writeInTransaction, "att odb type id");
                if (cai.GetAttributeType().IsArray())
                {
                    _fsi.WriteInt(cai.GetAttributeType().GetSubType().GetId(), writeInTransaction, "att array sub type");
                    // when the attribute is not native, then write its class info
                    // position
                    if (cai.GetAttributeType().GetSubType().IsNonNative())
                    {
                        _fsi.WriteLong(
                            StorageEngine.GetSession(true).GetMetaModel().GetClassInfo(
                                cai.GetAttributeType().GetSubType().GetName(), true).GetId().ObjectId,
                            writeInTransaction, "class info id of array subtype", DefaultWriteAction.DataWriteAction);
                    }
                }
                // For enum, we write the class info id of the enum class
                if (cai.GetAttributeType().IsEnum())
                {
                    _fsi.WriteLong(
                        StorageEngine.GetSession(true).GetMetaModel().GetClassInfo(cai.GetFullClassname(), true).GetId()
                            .ObjectId, writeInTransaction, "class info id", DefaultWriteAction.DataWriteAction);
                }
            }
            else
            {
                _fsi.WriteLong(
                    StorageEngine.GetSession(true).GetMetaModel().GetClassInfo(cai.GetFullClassname(), true).GetId().
                        ObjectId, writeInTransaction, "class info id", DefaultWriteAction.DataWriteAction);
            }
            _fsi.WriteString(cai.GetName(), false, writeInTransaction);
            _fsi.WriteBoolean(cai.IsIndex(), writeInTransaction);
        }

        /// <summary>
        ///   Actually write the object data to the database file
        /// </summary>
        /// <param name="noi"> The object meta infor The object info to be written </param>
        /// <param name="position"> if -1, it is a new instance, if not, it is an update </param>
        /// <param name="writeInTransaction"> </param>
        /// <returns> The object posiiton or id(if &lt; 0) </returns>
        private long WriteNativeObjectInfo(NativeObjectInfo noi, long position, bool writeInTransaction)
        {
            if (OdbConfiguration.IsDebugEnabled(LogIdDebug))
            {
                DLogger.Debug(string.Format("Writing native object at {0} : Type={1} | Value={2}",
                                            position, OdbType.GetNameFromId(noi.GetOdbTypeId()), noi));
            }
            if (noi.IsAtomicNativeObject())
                return WriteAtomicNativeObject((AtomicNativeObjectInfo) noi, writeInTransaction);
            if (noi.IsNull())
            {
                WriteNullNativeObjectHeader(noi.GetOdbTypeId(), writeInTransaction);
                return position;
            }
            if (noi.IsCollectionObject())
                return WriteCollection((CollectionObjectInfo) noi, writeInTransaction);
            if (noi.IsMapObject())
                return WriteMap((MapObjectInfo) noi, writeInTransaction);
            if (noi.IsArrayObject())
                return WriteArray((ArrayObjectInfo) noi, writeInTransaction);
            if (noi.IsEnumObject())
                return WriteEnumNativeObject((EnumNativeObjectInfo) noi, writeInTransaction);
            throw new OdbRuntimeException(NDatabaseError.NativeTypeNotSupported.AddParameter(noi.GetOdbTypeId()));
        }

        public virtual OID WriteNonNativeObjectInfoOld(OID existingOid, NonNativeObjectInfo objectInfo, long position,
                                                       bool writeDataInTransaction, bool isNewObject)
        {
            var lsession = GetSession();
            var cache = lsession.GetCache();
            var hasObject = objectInfo.GetObject() != null;
            if (isNewObject)
            {
                _triggerManager.ManageInsertTriggerBefore(objectInfo.GetClassInfo().GetFullClassName(),
                                                          hasObject ? objectInfo.GetObject() : objectInfo);
            }
            // Checks if object is null,for null objects,there is nothing to do
            if (objectInfo.IsNull())
                return StorageEngineConstant.NullObjectId;
            var metaModel = lsession.GetMetaModel();
            // first checks if the class of this object already exist in the
            // metamodel
            if (!metaModel.ExistClass(objectInfo.GetClassInfo().GetFullClassName()))
                AddClass(objectInfo.GetClassInfo(), true);
            // if position is -1, gets the position where to write the object
            if (position == -1)
            {
                // Write at the end of the file
                position = _fsi.GetAvailablePosition();
                // Updates the meta object position
                objectInfo.SetPosition(position);
            }
            // Gets the object id
            var oid = existingOid;
            if (oid == null)
            {
                // If, to get the next id, a new id block must be created, then
                // there is an extra work
                // to update the current object position
                if (_idManager.MustShift())
                {
                    oid = _idManager.GetNextObjectId(position);
                    // The id manager wrote in the file so the position for the
                    // object must be re-computed
                    position = _fsi.GetAvailablePosition();
                    // The oid must be associated to this new position - id
                    // operations are always out of transaction
                    // in this case, the update is done out of the transaction as a
                    // rollback won t need to
                    // undo this. We are just creating the id
                    // => third parameter(write in transaction) = false
                    _idManager.UpdateObjectPositionForOid(oid, position, false);
                }
                else
                    oid = _idManager.GetNextObjectId(position);
            }
            else
            {
                // If an oid was passed, it is because object already exist and
                // is being updated. So we
                // must update the object position
                // Here the update of the position of the id must be done in
                // transaction as the object
                // position of the id is being updated, and a rollback should undo
                // this
                // => third parameter(write in transaction) = true
                _idManager.UpdateObjectPositionForOid(oid, position, true);
                // Keep the relation of id and position in the cache until the
                // commit
                cache.SavePositionOfObjectWithOid(oid, position);
            }
            // Sets the oid of the object in the inserting cache
            cache.UpdateIdOfInsertingObject(objectInfo.GetObject(), oid);
            // Only add the oid to unconnected zone if it is a new object
            if (isNewObject)
            {
                cache.AddOIDToUnconnectedZone(oid);
                if (OdbConfiguration.ReconnectObjectsToSession())
                {
                    var crossSessionCache =
                        CacheFactory.GetCrossSessionCache(StorageEngine.GetBaseIdentification().GetIdentification());
                    crossSessionCache.AddObject(objectInfo.GetObject(), oid);
                }
            }
            objectInfo.SetOid(oid);
            if (OdbConfiguration.IsDebugEnabled(LogId))
            {
                DLogger.Debug("Start Writing non native object of type " +
                              objectInfo.GetClassInfo().GetFullClassName() + " at " + position + " , oid = " + oid +
                              " : " + objectInfo);
            }
            if (objectInfo.GetClassInfo() == null || objectInfo.GetClassInfo().GetId() == null)
            {
                if (objectInfo.GetClassInfo() != null)
                {
                    var clinfo =
                        StorageEngine.GetSession(true).GetMetaModel().GetClassInfo(
                            objectInfo.GetClassInfo().GetFullClassName(), true);
                    objectInfo.SetClassInfo(clinfo);
                }
                else
                    throw new OdbRuntimeException(NDatabaseError.UndefinedClassInfo.AddParameter(objectInfo.ToString()));
            }
            // updates the meta model - If class already exist, it returns the
            // metamodel class, which contains
            // a bit more informations
            var classInfo = AddClass(objectInfo.GetClassInfo(), true);
            objectInfo.SetClassInfo(classInfo);
            // 
            if (isNewObject)
                ManageNewObjectPointers(objectInfo, classInfo);
            if (OdbConfiguration.SaveHistory())
            {
                classInfo.AddHistory(new InsertHistoryInfo("insert", oid, position, objectInfo.GetPreviousObjectOID(),
                                                           objectInfo.GetNextObjectOID()));
            }
            _fsi.SetWritePosition(position, writeDataInTransaction);
            objectInfo.SetPosition(position);
            // Block size
            _fsi.WriteInt(0, writeDataInTransaction, "block size");
            // Block type
            _fsi.WriteByte(BlockTypes.BlockTypeNonNativeObject, writeDataInTransaction, "object block type");
            // The object id
            _fsi.WriteLong(oid.ObjectId, writeDataInTransaction, "oid", DefaultWriteAction.DataWriteAction);
            // Class info id
            _fsi.WriteLong(classInfo.GetId().ObjectId, writeDataInTransaction, "class info id",
                           DefaultWriteAction.DataWriteAction);
            // previous instance
            WriteOid(objectInfo.GetPreviousObjectOID(), writeDataInTransaction, "prev instance",
                     DefaultWriteAction.DataWriteAction);
            // next instance
            WriteOid(objectInfo.GetNextObjectOID(), writeDataInTransaction, "next instance",
                     DefaultWriteAction.DataWriteAction);
            // creation date, for update operation must be the original one
            _fsi.WriteLong(objectInfo.GetHeader().GetCreationDate(), writeDataInTransaction, "creation date",
                           DefaultWriteAction.DataWriteAction);
            _fsi.WriteLong(OdbTime.GetCurrentTimeInTicks(), writeDataInTransaction, "update date",
                           DefaultWriteAction.DataWriteAction);
            // TODO check next version number
            _fsi.WriteInt(objectInfo.GetHeader().GetObjectVersion(), writeDataInTransaction, "object version number");
            // not used yet. But it will point to an internal object of type
            // ObjectReference that will have details on the references:
            // All the objects that point to it: to enable object integrity
            _fsi.WriteLong(-1, writeDataInTransaction, "object reference pointer", DefaultWriteAction.DataWriteAction);
            // True if this object have been synchronized with main database, else
            // false
            _fsi.WriteBoolean(false, writeDataInTransaction, "is syncronized with external db");
            var nbAttributes = objectInfo.GetClassInfo().GetAttributes().Count;
            // now write the number of attributes and the position of all
            // attributes, we do not know them yet, so write 00 but at the end
            // of the write operation
            // These positions will be updated
            // The positions that is going to be written are 'int' representing
            // the offset position of the attribute
            // first write the number of attributes
            _fsi.WriteInt(nbAttributes, writeDataInTransaction, "nb attr");
            // Store the position
            var attributePositionStart = _fsi.GetPosition();
            // TODO Could remove this, and pull to the right position
            for (var i = 0; i < nbAttributes; i++)
            {
                _fsi.WriteInt(0, writeDataInTransaction, "attr id -1");
                _fsi.WriteLong(0, writeDataInTransaction, "att pos", DefaultWriteAction.DataWriteAction);
            }
            var attributesIdentification = new long[nbAttributes];
            var attributeIds = new int[nbAttributes];
            // Puts the object info in the cache
            // storageEngine.getSession().getCache().addObject(position,
            // aoi.getObject(), objectInfo.getHeader());
            var maxWritePosition = _fsi.GetPosition();
            // Loop on all attributes
            for (var i = 0; i < nbAttributes; i++)
            {
                // Gets the attribute meta description
                var classAttributeInfo = classInfo.GetAttributeInfo(i);
                // Gets the id of the attribute
                attributeIds[i] = classAttributeInfo.GetId();
                // Gets the attribute data
                var aoi2 = objectInfo.GetAttributeValueFromId(classAttributeInfo.GetId());
                if (aoi2 == null)
                {
                    // This only happens in 1 case : when a class has a field with
                    // the same name of one of is superclass. In this, the deeper
                    // attribute is null
                    if (classAttributeInfo.IsNative())
                        aoi2 = new NullNativeObjectInfo(classAttributeInfo.GetAttributeType().GetId());
                    else
                        aoi2 = new NonNativeNullObjectInfo(classAttributeInfo.GetClassInfo());
                }
                if (aoi2.IsNative())
                {
                    var nativeAttributePosition = InternalStoreObject((NativeObjectInfo) aoi2);
                    // For native objects , odb stores their position
                    attributesIdentification[i] = nativeAttributePosition;
                }
                else
                {
                    OID nonNativeAttributeOid;
                    if (aoi2.IsObjectReference())
                    {
                        var or = (ObjectReference) aoi2;
                        nonNativeAttributeOid = or.GetOid();
                    }
                    else
                        nonNativeAttributeOid = StoreObject(null, (NonNativeObjectInfo) aoi2);
                    // For non native objects , odb stores its oid as a negative
                    // number!!u
                    if (nonNativeAttributeOid != null)
                        attributesIdentification[i] = -nonNativeAttributeOid.ObjectId;
                    else
                        attributesIdentification[i] = StorageEngineConstant.NullObjectIdId;
                }
                var p = _fsi.GetPosition();
                if (p > maxWritePosition)
                    maxWritePosition = p;
            }
            // Updates attributes identification in the object info header
            objectInfo.GetHeader().SetAttributesIdentification(attributesIdentification);
            objectInfo.GetHeader().SetAttributesIds(attributeIds);
            var positionAfterWrite = maxWritePosition;
            // Now writes back the attribute positions
            _fsi.SetWritePosition(attributePositionStart, writeDataInTransaction);
            for (var i = 0; i < attributesIdentification.Length; i++)
            {
                _fsi.WriteInt(attributeIds[i], writeDataInTransaction, "attr id");
                _fsi.WriteLong(attributesIdentification[i], writeDataInTransaction, "att real pos",
                               DefaultWriteAction.DataWriteAction);
                // if (classInfo.getAttributeInfo(i).isNonNative() &&
                // attributesIdentification[i] > 0) {
                if (objectInfo.GetAttributeValueFromId(attributeIds[i]).IsNonNativeObject() &&
                    attributesIdentification[i] > 0)
                {
                    throw new OdbRuntimeException(
                        NDatabaseError.NonNativeAttributeStoredByPositionInsteadOfOid.AddParameter(
                            classInfo.GetAttributeInfo(i).GetName()).AddParameter(classInfo.GetFullClassName()).
                            AddParameter(attributesIdentification[i]));
                }
            }
            _fsi.SetWritePosition(positionAfterWrite, writeDataInTransaction);
            var blockSize = (int) (positionAfterWrite - position);
            try
            {
                WriteBlockSizeAt(position, blockSize, writeDataInTransaction, objectInfo);
            }
            catch (OdbRuntimeException)
            {
                DLogger.Debug("Error while writing block size. pos after write " + positionAfterWrite +
                              " / start pos = " + position);
                // throw new ODBRuntimeException(storageEngine,"Error while writing
                // block size. pos after write " + positionAfterWrite + " / start
                // pos = " + position,e);
                throw;
            }
            if (OdbConfiguration.IsDebugEnabled(LogId))
            {
                DLogger.Debug("  Attributes positions of object with oid " + oid + " are " +
                              DisplayUtility.LongArrayToString(attributesIdentification));
                DLogger.Debug("End Writing non native object at " + position + " with oid " + oid +
                              " - prev oid=" + objectInfo.GetPreviousObjectOID() + " / next oid=" +
                              objectInfo.GetNextObjectOID());
                if (OdbConfiguration.IsDebugEnabled(LogIdDebug))
                    DLogger.Debug(" - current buffer : " + _fsi.GetIo());
            }
            // Only insert in index for new objects
            if (isNewObject)
            {
                // insert object id in indexes, if exist
                ManageIndexesForInsert(oid, objectInfo);
                _triggerManager.ManageInsertTriggerAfter(objectInfo.GetClassInfo().GetFullClassName(),
                                                         hasObject ? objectInfo.GetObject() : objectInfo, oid);
            }
            return oid;
        }

        /// <summary>
        ///   Updates pointers of objects, Only changes uncommitted info pointers
        /// </summary>
        /// <param name="objectInfo"> The meta representation of the object being inserted </param>
        /// <param name="classInfo"> The class of the object being inserted </param>
        private void ManageNewObjectPointers(NonNativeObjectInfo objectInfo, ClassInfo classInfo)
        {
            var cache = StorageEngine.GetSession(true).GetCache();
            var isFirstUncommitedObject = !classInfo.GetUncommittedZoneInfo().HasObjects();
            // if it is the first uncommitted object
            if (isFirstUncommitedObject)
            {
                classInfo.GetUncommittedZoneInfo().First = objectInfo.GetOid();
                var lastCommittedObjectOid = classInfo.GetCommitedZoneInfo().Last;
                if (lastCommittedObjectOid != null)
                {
                    // Also updates the last committed object next object oid in
                    // memory to connect the committed
                    // zone with unconnected for THIS transaction (only in memory)
                    var oih = cache.GetObjectInfoHeaderFromOid(lastCommittedObjectOid, true);
                    oih.SetNextObjectOID(objectInfo.GetOid());
                    // And sets the previous oid of the current object with the last
                    // committed oid
                    objectInfo.SetPreviousInstanceOID(lastCommittedObjectOid);
                }
            }
            else
            {
                // Gets the last object, updates its (next object)
                // pointer to the new object and updates the class info 'last
                // uncommitted object
                // oid' field
                var oip = classInfo.GetLastObjectInfoHeader();
                if (oip == null)
                {
                    throw new OdbRuntimeException(
                        NDatabaseError.InternalError.AddParameter("last OIP is null in manageNewObjectPointers oid=" +
                                                                 objectInfo.GetOid()));
                }
                if (oip.GetNextObjectOID() != objectInfo.GetOid())
                {
                    oip.SetNextObjectOID(objectInfo.GetOid());
                    // Here we are working in unconnected zone, so this
                    // can be done without transaction: actually
                    // write in database file
                    UpdateNextObjectFieldOfObjectInfo(oip.GetOid(), oip.GetNextObjectOID(), false);
                    objectInfo.SetPreviousInstanceOID(oip.GetOid());
                    // Resets the class info oid: In some case,
                    // (client // server) it may be -1.
                    oip.SetClassInfoId(classInfo.GetId());
                    // object info oip has been changed, we must put it
                    // in the cache to turn this change available for current
                    // transaction until the commit
                    StorageEngine.GetSession(true).GetCache().AddObjectInfo(oip);
                }
            }
            // always set the new last object oid and the number of objects
            classInfo.GetUncommittedZoneInfo().Last = objectInfo.GetOid();
            classInfo.GetUncommittedZoneInfo().IncreaseNbObjects();
            // Then updates the last info pointers of the class info
            // with this new created object
            // At this moment, the objectInfo.getHeader() do not have the
            // attribute ids.
            // but later in this code, the attributes will be set, so the class
            // info also will have them
            classInfo.SetLastObjectInfoHeader(objectInfo.GetHeader());
            // // Saves the fact that something has changed in the class (number of
            // objects and/or last object oid)
            StorageEngine.GetSession(true).GetMetaModel().AddChangedClass(classInfo);
        }

        // This will be done by the mainStoreObject method
        // Context.getCache().endInsertingObject(object);
        /// <param name="noi"> The native object meta representation The object to be inserted in the database </param>
        /// <returns> The position of the inserted object </returns>
        private long InsertNativeObject(NativeObjectInfo noi)
        {
            var writePosition = _fsi.GetAvailablePosition();
            _fsi.SetWritePosition(writePosition, true);
            // true,false = update pointers,do not write in transaction, writes
            // directly to hard disk
            var position = WriteNativeObjectInfo(noi, writePosition, false);
            return position;
        }

        /// <summary>
        ///   Store a meta representation of an object(already as meta representation)in ODBFactory database.
        /// </summary>
        /// <remarks>
        ///   Store a meta representation of an object(already as meta representation)in ODBFactory database. To detect if object must be updated or insert, we use the cache. To update an object, it must be first selected from the database. When an object is to be stored, if it exist in the cache, then it will be updated, else it will be inserted as a new object. If the object is null, the cache will be used to check if the meta representation is in the cache
        /// </remarks>
        /// <param name="oid"> The oid of the object to be inserted/updates </param>
        /// <param name="nnoi"> The meta representation of an object </param>
        /// <returns> The object position </returns>
        public virtual OID StoreObject(OID oid, NonNativeObjectInfo nnoi)
        {
            // first detects if we must perform an insert or an update
            // If object is in the cache, we must perform an update, else an insert
            var @object = nnoi.GetObject();
            var mustUpdate = false;
            var cache = GetSession().GetCache();
            if (@object != null)
            {
                var cacheOid = cache.IdOfInsertingObject(@object);
                if (cacheOid != null)
                    return cacheOid;
                // throw new ODBRuntimeException("Inserting meta representation of
                // an object without the object itself is not yet supported");
                mustUpdate = cache.ExistObject(@object);
            }
            if (!mustUpdate)
                mustUpdate = !Equals(nnoi.GetOid(), StorageEngineConstant.NullObjectId);
            // To enable auto - reconnect object loaded from previous sessions
            // auto reconnect is on
            if (!mustUpdate && OdbConfiguration.ReconnectObjectsToSession())
            {
                var crossSessionCache =
                    CacheFactory.GetCrossSessionCache(StorageEngine.GetBaseIdentification().GetIdentification());
                if (crossSessionCache.ExistObject(@object))
                {
                    StorageEngine.Reconnect(@object);
                    mustUpdate = true;
                }
            }
            if (mustUpdate)
                return UpdateNonNativeObjectInfo(nnoi, false);
            return InsertNonNativeObject(oid, nnoi, true);
        }

        /// <summary>
        ///   Store a meta representation of a native object(already as meta representation)in ODBFactory database.
        /// </summary>
        /// <remarks>
        ///   Store a meta representation of a native object(already as meta representation)in ODBFactory database. A Native object is an object that use native language type, String for example To detect if object must be updated or insert, we use the cache. To update an object, it must be first selected from the database. When an object is to be stored, if it exist in the cache, then it will be updated, else it will be inserted as a new object. If the object is null, the cache will be used to check if the meta representation is in the cache
        /// </remarks>
        /// <param name="noi"> The meta representation of an object </param>
        /// <returns> The object position @ </returns>
        internal virtual long InternalStoreObject(NativeObjectInfo noi)
        {
            return InsertNativeObject(noi);
        }

        public virtual OID UpdateObject(AbstractObjectInfo aoi, bool forceUpdate)
        {
            if (aoi.IsNonNativeObject())
                return UpdateNonNativeObjectInfo((NonNativeObjectInfo) aoi, forceUpdate);
            if (aoi.IsNative())
                return UpdateObject(aoi, forceUpdate);
            // TODO : here should use if then else
            throw new OdbRuntimeException(
                NDatabaseError.AbstractObjectInfoTypeNotSupported.AddParameter(aoi.GetType().FullName));
        }

        /// <summary>
        ///   Upate the version number of the object
        /// </summary>
        /// <param name="header"> </param>
        /// <param name="writeInTransaction"> </param>
        private void UpdateUpdateTimeAndObjectVersionNumber(ObjectInfoHeader header, bool writeInTransaction)
        {
            var objectPosition = header.GetPosition();
            _fsi.SetWritePosition(objectPosition + StorageEngineConstant.ObjectOffsetUpdateDate, writeInTransaction);
            _fsi.WriteLong(header.GetUpdateDate(), writeInTransaction, "update date time",
                           DefaultWriteAction.DataWriteAction);
            _fsi.WriteInt(header.GetObjectVersion(), writeInTransaction, "object version");
        }

        protected virtual ObjectInfoHeader GetObjectInfoHeader(OID oid, ICache cache)
        {
            var oih = cache.GetObjectInfoHeaderFromOid(oid, false) ??
                      _objectReader.ReadObjectInfoHeaderFromOid(oid, false);
            // If object is not in the cache, then read the header from the file
            return oih;
        }

        public virtual ObjectInfoHeader UpdateNextObjectPreviousPointersInCache(OID nextObjectOID, OID previousObjectOID,
                                                                                ICache cache)
        {
            var oip = cache.GetObjectInfoHeaderFromOid(nextObjectOID, false);
            // If object is not in the cache, then read the header from the file
            if (oip == null)
            {
                oip = _objectReader.ReadObjectInfoHeaderFromOid(nextObjectOID, false);
                cache.AddObjectInfo(oip);
            }
            oip.SetPreviousObjectOID(previousObjectOID);
            return oip;
        }

        public virtual ObjectInfoHeader UpdatePreviousObjectNextPointersInCache(OID nextObjectOID, OID previousObjectOID,
                                                                                ICache cache)
        {
            var oip = cache.GetObjectInfoHeaderFromOid(previousObjectOID, false);
            // If object is not in the cache, then read the header from the file
            if (oip == null)
            {
                oip = _objectReader.ReadObjectInfoHeaderFromOid(previousObjectOID, false);
                cache.AddObjectInfo(oip);
            }
            oip.SetNextObjectOID(nextObjectOID);
            return oip;
        }

        /// <summary>
        ///   Manage in place update.
        /// </summary>
        /// <remarks>
        ///   Manage in place update. Just write the value at the exact position if possible.
        /// </remarks>
        /// <param name="objectComparator"> Contains all infos about differences between all version objects and new version </param>
        /// <param name="object"> The object being modified (new version) </param>
        /// <param name="oid"> The oid of the object being modified </param>
        /// <param name="header"> The header of the object meta representation (Comes from the cache) </param>
        /// <param name="cache"> The cache it self </param>
        /// <param name="objectIsInConnectedZone"> A boolean value to indicate if object is in connected zone. I true, change must be made in transaction. If false, changes can be made in the database file directly. </param>
        /// <returns> The number of in place update successfully executed </returns>
        /// <exception cref="System.Exception">System.Exception</exception>
        private int ManageInPlaceUpdate(IObjectInfoComparator objectComparator, object @object, OID oid,
                                        ObjectInfoHeader header, ICache cache, bool objectIsInConnectedZone)
        {
            var canUpdateInPlace = true;
            // If object is is connected zone, changes must be done in transaction,
            // if not in connected zone, changes can be made out of
            // transaction, directly to the database
            var writeInTransaction = objectIsInConnectedZone;
            var nbAppliedChanges = 0;
            // if 0, only direct attribute have been changed
            // if (objectComparator.getMaxObjectRecursionLevel() == 0) {
            // if some direct native attribute have changed
            if (objectComparator.GetChangedAttributeActions().Count > 0)
            {
                // Check if in place update is possible
                var actions = objectComparator.GetChangedAttributeActions();
                foreach (var attribute in actions)
                {
                    if (attribute is ChangedNativeAttributeAction)
                    {
                        var changedNativeAttributeAction = (ChangedNativeAttributeAction) attribute;
                        if (changedNativeAttributeAction.ReallyCantDoInPlaceUpdate())
                        {
                            canUpdateInPlace = false;
                            break;
                        }
                        if (!changedNativeAttributeAction.InPlaceUpdateIsGuaranteed())
                        {
                            if (changedNativeAttributeAction.IsString() && changedNativeAttributeAction.GetUpdatePosition() != StorageEngineConstant.NullObjectPosition)
                            {
                                var position = SafeOverWriteAtomicNativeObject(changedNativeAttributeAction.GetUpdatePosition(),
                                                                               (AtomicNativeObjectInfo)
                                                                               changedNativeAttributeAction.GetNoiWithNewValue(),
                                                                               writeInTransaction);
                                canUpdateInPlace = position != -1;
                                if (!canUpdateInPlace)
                                    break;
                            }
                            else
                            {
                                canUpdateInPlace = false;
                                break;
                            }
                        }
                        else
                        {
                            _fsi.SetWritePosition(changedNativeAttributeAction.GetUpdatePosition(), true);
                            WriteAtomicNativeObject((AtomicNativeObjectInfo) changedNativeAttributeAction.GetNoiWithNewValue(),
                                                    writeInTransaction);
                        }
                    }
                    else
                    {
                        var changedObjectReferenceAttributeAction = attribute as ChangedObjectReferenceAttributeAction;
                        if (changedObjectReferenceAttributeAction != null)
                        {
                            UpdateObjectReference(changedObjectReferenceAttributeAction.GetUpdatePosition(), changedObjectReferenceAttributeAction.GetNewId(), writeInTransaction);
                        }
                    }
                    nbAppliedChanges++;
                }
                if (canUpdateInPlace)
                {
                    if (OdbConfiguration.IsDebugEnabled(LogId))
                        DLogger.Debug("Sucessfull in place updating");
                }
            }
            // if canUpdateInplace is false, a full update (writing
            // object elsewhere) is necessary so
            // there is no need to try to update object references.
            if (canUpdateInPlace)
            {
                // For non native attribute that have been replaced!
                for (var i = 0; i < objectComparator.GetNewObjectMetaRepresentations().Count; i++)
                {
                    // to avoid stackOverFlow, check if the object is
                    // already beeing inserted
                    var newNonNativeObjectAction = objectComparator.GetNewObjectMetaRepresentation(i);
                    if (cache.IdOfInsertingObject(newNonNativeObjectAction) == null)
                    {
                        // If Meta representation have an id == null, then
                        // this is a new object
                        // it must be inserted, else just update
                        // reference
                        var ooid = newNonNativeObjectAction.GetNnoi().GetOid() ??
                                   InsertNonNativeObject(null, newNonNativeObjectAction.GetNnoi(), true);
                        UpdateObjectReference(newNonNativeObjectAction.GetUpdatePosition(), ooid, writeInTransaction);
                        nbAppliedChanges++;
                    }
                }
                // For attribute that have been set to null
                for (var i = 0; i < objectComparator.GetAttributeToSetToNull().Count; i++)
                {
                    var setAttributeToNullAction = objectComparator.GetAttributeToSetToNull()[i];
                    UpdateObjectReference(setAttributeToNullAction.GetUpdatePosition(),
                                          StorageEngineConstant.NullObjectId,
                                          writeInTransaction);
                    nbAppliedChanges++;
                }
                // For attribute that have been set to null
                for (var i = 0; i < objectComparator.GetArrayChanges().Count; i++)
                {
                    var arrayModifyElement = objectComparator.GetArrayChanges()[i];
                    if (!arrayModifyElement.SupportInPlaceUpdate())
                        break;
                    _fsi.SetReadPosition(arrayModifyElement.GetArrayPositionDefinition());
                    var arrayPosition = _fsi.ReadLong();
                    // If we reach this line,the ArrayModifyElement
                    // suuports In Place Update so it must be a Native
                    // Object Info!
                    // The cast is safe :-)
                    UpdateArrayElement(arrayPosition, arrayModifyElement.GetArrayElementIndexToChange(),
                                       (NativeObjectInfo) arrayModifyElement.GetNewValue(), writeInTransaction);
                    nbAppliedChanges++;
                }
            }
            // }// only direct attribute have been changed
            return nbAppliedChanges;
        }

        private void WriteBlockSizeAt(long writePosition, int blockSize, bool writeInTransaction, object @object)
        {
            if (blockSize < 0)
            {
                throw new OdbRuntimeException(
                    NDatabaseError.NegativeBlockSize.AddParameter(writePosition).AddParameter(blockSize).AddParameter(
                        @object.ToString()));
            }
            var currentPosition = _fsi.GetPosition();
            _fsi.SetWritePosition(writePosition, writeInTransaction);
            _fsi.WriteInt(blockSize, writeInTransaction, "block size");
            // goes back where we were
            _fsi.SetWritePosition(currentPosition, writeInTransaction);
        }

        /// <summary>
        ///   TODO check if we should pass the position instead of requesting if to fsi <pre>Write a collection to the database
        ///                                                                               This is done by writing the number of element s and then the position of all elements.</pre>
        /// </summary>
        /// <remarks>
        ///   TODO check if we should pass the position instead of requesting if to fsi <pre>Write a collection to the database
        ///                                                                               This is done by writing the number of element s and then the position of all elements.
        ///                                                                               Example : a list with two string element : 'ola' and 'chico'
        ///                                                                               write 2 (as an int) : the number of elements
        ///                                                                               write two times 0 (as long) to reserve the space for the elements positions
        ///                                                                               then write the string 'ola', and keeps its position in the 'positions' array of long
        ///                                                                               then write the string 'chico' and keeps its position in the 'positions' array of long
        ///                                                                               Then write back all the positions (in this case , 2 positions) after the size of the collection
        ///                                                                               &lt;pre&gt;
        ///                                                                               &#064;param coi
        ///                                                                               &#064;param writeInTransaction
        ///                                                                               &#064;</pre>
        /// </remarks>
        private long WriteCollection(CollectionObjectInfo coi, bool writeInTransaction)
        {
            var startPosition = _fsi.GetPosition();
            WriteNativeObjectHeader(coi.GetOdbTypeId(), coi.IsNull(), BlockTypes.BlockTypeCollectionObject,
                                    writeInTransaction);
            if (coi.IsNull())
                return startPosition;
            var collection = coi.GetCollection();
            var collectionSize = collection.Count;
            IEnumerator iterator = collection.GetEnumerator();
            // write the real type of the collection
            _fsi.WriteString(coi.GetRealCollectionClassName(), false, writeInTransaction);
            // write the size of the collection
            _fsi.WriteInt(collectionSize, writeInTransaction, "collection size");
            // build a n array to store all element positions
            var attributeIdentifications = new long[collectionSize];
            // Gets the current position, to know later where to put the
            // references
            var firstObjectPosition = _fsi.GetPosition();
            // reserve space for object positions : write 'collectionSize' long
            // with zero to store each object position
            for (var i = 0; i < collectionSize; i++)
                _fsi.WriteLong(0, writeInTransaction, "collection element pos ", DefaultWriteAction.DataWriteAction);
            var currentElement = 0;
            while (iterator.MoveNext())
            {
                var element = (AbstractObjectInfo) iterator.Current;
                attributeIdentifications[currentElement] = InternalStoreObjectWrapper(element);
                currentElement++;
            }
            var positionAfterWrite = _fsi.GetPosition();
            // now that all objects have been stored, sets their position in the
            // space that have been reserved
            _fsi.SetWritePosition(firstObjectPosition, writeInTransaction);
            for (var i = 0; i < collectionSize; i++)
            {
                _fsi.WriteLong(attributeIdentifications[i], writeInTransaction, "collection element real pos ",
                               DefaultWriteAction.DataWriteAction);
            }
            // Goes back to the end of the array
            _fsi.SetWritePosition(positionAfterWrite, writeInTransaction);
            return startPosition;
        }

        /// <summary>
        ///   <pre>Write an array to the database
        ///     This is done by writing :
        ///     - the array type : array
        ///     - the array element type (String if it os a String [])
        ///     - the position of the non native type, if element are non java / C# native
        ///     - the number of element s and then the position of all elements.
        ///     </pre>
        /// </summary>
        /// <remarks>
        ///   <pre>Write an array to the database
        ///     This is done by writing :
        ///     - the array type : array
        ///     - the array element type (String if it os a String [])
        ///     - the position of the non native type, if element are non java / C# native
        ///     - the number of element s and then the position of all elements.
        ///     Example : an array with two string element : 'ola' and 'chico'
        ///     write 22 : array
        ///     write  20 : array of STRING
        ///     write 0 : it is a java native object
        ///     write 2 (as an int) : the number of elements
        ///     write two times 0 (as long) to reserve the space for the elements positions
        ///     then write the string 'ola', and keeps its position in the 'positions' array of long
        ///     then write the string 'chico' and keeps its position in the 'positions' array of long
        ///     Then write back all the positions (in this case , 2 positions) after the size of the array
        ///     Example : an array with two User element : user1 and user2
        ///     write 22 : array
        ///     write  23 : array of NON NATIVE Objects
        ///     write 251 : if 250 is the position of the user class info in database
        ///     write 2 (as an int) : the number of elements
        ///     write two times 0 (as long) to reserve the space for the elements positions
        ///     then write the user user1, and keeps its position in the 'positions' array of long
        ///     then write the user user2 and keeps its position in the 'positions' array of long
        ///     &lt;pre&gt;
        ///     &#064;param object
        ///     &#064;param odbType
        ///     &#064;param position
        ///     &#064;param writeInTransaction
        ///     &#064;</pre>
        /// </remarks>
        private long WriteArray(ArrayObjectInfo aoi, bool writeInTransaction)
        {
            var startPosition = _fsi.GetPosition();
            WriteNativeObjectHeader(aoi.GetOdbTypeId(), aoi.IsNull(), BlockTypes.BlockTypeArrayObject,
                                    writeInTransaction);
            if (aoi.IsNull())
                return startPosition;
            var array = aoi.GetArray();
            var arraySize = array.Length;
            // Writes the fact that it is an array
            _fsi.WriteString(aoi.GetRealArrayComponentClassName(), false, writeInTransaction);
            // write the size of the array
            _fsi.WriteInt(arraySize, writeInTransaction, "array size");
            // build a n array to store all element positions
            var attributeIdentifications = new long[arraySize];
            // Gets the current position, to know later where to put the
            // references
            var firstObjectPosition = _fsi.GetPosition();
            // reserve space for object positions : write 'arraySize' long
            // with zero to store each object position
            for (var i = 0; i < arraySize; i++)
                _fsi.WriteLong(0, writeInTransaction, "array element pos ", DefaultWriteAction.DataWriteAction);
            for (var i = 0; i < arraySize; i++)
            {
                var element = (AbstractObjectInfo) array[i];
                if (element == null || element.IsNull())
                {
                    // TODO Check this
                    attributeIdentifications[i] = StorageEngineConstant.NullObjectIdId;
                    continue;
                }
                attributeIdentifications[i] = InternalStoreObjectWrapper(element);
            }
            var positionAfterWrite = _fsi.GetPosition();
            // now that all objects have been stored, sets their position in the
            // space that have been reserved
            _fsi.SetWritePosition(firstObjectPosition, writeInTransaction);
            for (var i = 0; i < arraySize; i++)
            {
                _fsi.WriteLong(attributeIdentifications[i], writeInTransaction, "array real element pos",
                               DefaultWriteAction.DataWriteAction);
            }
            // Gos back to the end of the array
            _fsi.SetWritePosition(positionAfterWrite, writeInTransaction);
            return startPosition;
        }

        /// <summary>
        ///   <pre>Write a map to the database
        ///     This is done by writing the number of element s and then the key and value pair of all elements.</pre>
        /// </summary>
        /// <remarks>
        ///   <pre>Write a map to the database
        ///     This is done by writing the number of element s and then the key and value pair of all elements.
        ///     Example : a map with two string element : '1/olivier' and '2/chico'
        ///     write 2 (as an int) : the number of elements
        ///     write 4 times 0 (as long) to reserve the space for the elements positions
        ///     then write the object '1' and 'olivier', and keeps the two posiitons in the 'positions' array of long
        ///     then write the object '2' and the string chico' and keep the two position in the 'positions' array of long
        ///     Then write back all the positions (in this case , 4 positions) after the size of the map
        ///     &#064;param object
        ///     &#064;param writeInTransaction To specify if these writes must be done in or out of a transaction
        ///     &#064;</pre>
        /// </remarks>
        private long WriteMap(MapObjectInfo moi, bool writeInTransaction)
        {
            var startPosition = _fsi.GetPosition();
            WriteNativeObjectHeader(moi.GetOdbTypeId(), moi.IsNull(), BlockTypes.BlockTypeMapObject, writeInTransaction);
            if (moi.IsNull())
                return startPosition;
            var map = moi.GetMap();
            var mapSize = map.Count;
            var keys = map.Keys.GetEnumerator();
            // write the map class
            _fsi.WriteString(moi.GetRealMapClassName(), false, writeInTransaction);
            // write the size of the map
            _fsi.WriteInt(mapSize, writeInTransaction, "map size");
            // build a n array to store all element positions
            var positions = new long[mapSize*2];
            // Gets the current position, to know later where to put the
            // references
            var firstObjectPosition = _fsi.GetPosition();
            // reserve space for object positions : write 'mapSize*2' long
            // with zero to store each object position
            for (var i = 0; i < mapSize*2; i++)
                _fsi.WriteLong(0, writeInTransaction, "map element pos", DefaultWriteAction.DataWriteAction);
            var currentElement = 0;
            while (keys.MoveNext())
            {
                var key = keys.Current;
                var value = map[key];
                
                positions[currentElement++] = InternalStoreObjectWrapper(key);
                positions[currentElement++] = InternalStoreObjectWrapper(value);
            }
            var positionAfterWrite = _fsi.GetPosition();
            // now that all objects have been stored, sets their position in the
            // space that have been reserved
            _fsi.SetWritePosition(firstObjectPosition, writeInTransaction);
            for (var i = 0; i < mapSize*2; i++)
                _fsi.WriteLong(positions[i], writeInTransaction, "map real element pos",
                               DefaultWriteAction.DataWriteAction);
            // Gos back to the end of the array
            _fsi.SetWritePosition(positionAfterWrite, writeInTransaction);
            return startPosition;
        }

        /// <summary>
        ///   This method is used to store the object : natibe or non native and return a number : - The position of the object if it is a native object - The oid (as a negative number) if it is a non native object
        /// </summary>
        /// <param name="aoi"> </param>
        /// <returns> </returns>
        /// <exception cref="System.Exception">System.Exception</exception>
        private long InternalStoreObjectWrapper(AbstractObjectInfo aoi)
        {
            if (aoi.IsNative())
                return InternalStoreObject((NativeObjectInfo) aoi);
            if (aoi.IsNonNativeObject())
            {
                var oid = StoreObject(null, (NonNativeObjectInfo) aoi);
                return -oid.ObjectId;
            }
            // Object references are references to object already stored.
            // But in the case of map, the reference can appear before the real
            // object (as order may change)
            // If objectReference.getOid() is null, it is the case. In this case,
            // We take the object being referenced and stores it directly.
            var objectReference = (ObjectReference) aoi;
            if (objectReference.GetOid() == null)
            {
                var oid = StoreObject(null, objectReference.GetNnoi());
                return -oid.ObjectId;
            }
            return -objectReference.GetOid().ObjectId;
        }

        protected virtual void WriteNullNativeObjectHeader(int odbTypeId, bool writeInTransaction)
        {
            WriteNativeObjectHeader(odbTypeId, true, BlockTypes.BlockTypeNativeNullObject, writeInTransaction);
        }

        protected virtual void WriteNonNativeNullObjectHeader(OID classInfoId, bool writeInTransaction)
        {
            // Block size
            _fsi.WriteInt(NonNativeHeaderBlockSize, writeInTransaction, "block size");
            // Block type
            _fsi.WriteByte(BlockTypes.BlockTypeNonNativeNullObject, writeInTransaction);
            // class info id
            _fsi.WriteLong(classInfoId.ObjectId, writeInTransaction, "null non native obj class info position",
                           DefaultWriteAction.DataWriteAction);
        }

        /// <summary>
        ///   Write the header of a native attribute
        /// </summary>
        protected virtual void WriteNativeObjectHeader(int odbTypeId, bool isNull, byte blockType,
                                                       bool writeDataInTransaction)
        {
            var bytes = new byte[10];
            bytes[0] = _nativeHeaderBlockSizeByte[0];
            bytes[1] = _nativeHeaderBlockSizeByte[1];
            bytes[2] = _nativeHeaderBlockSizeByte[2];
            bytes[3] = _nativeHeaderBlockSizeByte[3];
            bytes[4] = blockType;
            var bytesTypeId = _byteArrayConverter.IntToByteArray(odbTypeId);
            bytes[5] = bytesTypeId[0];
            bytes[6] = bytesTypeId[1];
            bytes[7] = bytesTypeId[2];
            bytes[8] = bytesTypeId[3];
            bytes[9] = _byteArrayConverter.BooleanToByteArray(isNull)[0];
            _fsi.WriteBytes(bytes, writeDataInTransaction, "NativeObjectHeader");
        }

        public virtual long SafeOverWriteAtomicNativeObject(long position, AtomicNativeObjectInfo newAnoi,
                                                            bool writeInTransaction)
        {
            // If the attribute an a non fix ize, check if this write is safe
            if (OdbType.HasFixSize(newAnoi.GetOdbTypeId()))
            {
                _fsi.SetWritePosition(position, writeInTransaction);
                return WriteAtomicNativeObject(newAnoi, writeInTransaction);
            }
            if (OdbType.IsStringOrBigDecimal(newAnoi.GetOdbTypeId()))
            {
                _fsi.SetReadPosition(position + StorageEngineConstant.NativeObjectOffsetDataArea);
                var totalSize = _fsi.ReadInt("String total size");
                var stringNumberOfBytes = _byteArrayConverter.GetNumberOfBytesOfAString(newAnoi.GetObject().ToString(),
                                                                                        true);
                // Checks if there is enough space to store this new string in place
                var canUpdate = totalSize >= stringNumberOfBytes;
                if (canUpdate)
                {
                    _fsi.SetWritePosition(position, writeInTransaction);
                    return WriteAtomicNativeObject(newAnoi, writeInTransaction, totalSize);
                }
            }
            return -1;
        }

        public virtual long WriteEnumNativeObject(EnumNativeObjectInfo anoi, bool writeInTransaction)
        {
            var startPosition = _fsi.GetPosition();
            var odbTypeId = anoi.GetOdbTypeId();
            WriteNativeObjectHeader(odbTypeId, anoi.IsNull(), BlockTypes.BlockTypeNativeObject, writeInTransaction);
            // Writes the Enum ClassName
            _fsi.WriteLong(anoi.GetEnumClassInfo().GetId().ObjectId, writeInTransaction, "enum class info id",
                           DefaultWriteAction.DataWriteAction);
            // Write the Enum String value
            _fsi.WriteString(anoi.GetObject().ToString(), writeInTransaction, true, -1);
            return startPosition;
        }

        /// <summary>
        ///   Writes a natibve attribute
        /// </summary>
        /// <param name="anoi"> </param>
        /// <param name="writeInTransaction"> To specify if data must be written in the transaction or directly to database file </param>
        /// <returns> The object position </returns>
        public virtual long WriteAtomicNativeObject(AtomicNativeObjectInfo anoi, bool writeInTransaction)
        {
            return WriteAtomicNativeObject(anoi, writeInTransaction, -1);
        }

        public virtual void StoreFreeSpace(long currentPosition, int blockSize)
        {
            if (OdbConfiguration.IsDebugEnabled(LogId))
                DLogger.Debug("Storing free space at position " + currentPosition + " | block size = " + blockSize);
        }

        /// <summary>
        ///   Writes a pointer block : A pointer block is like a goto.
        /// </summary>
        /// <remarks>
        ///   Writes a pointer block : A pointer block is like a goto. It can be used for example when an instance has been updated. To enable all the references to it to be updated, we just create o pointer at the place of the updated instance. When searching for the instance, if the block type is POINTER, then the position will be set to the pointer position
        /// </remarks>
        protected virtual void MarkAsAPointerTo(OID oid, long currentPosition, long newObjectPosition)
        {
            throw new OdbRuntimeException(
                NDatabaseError.FoundPointer.AddParameter(oid.ObjectId).AddParameter(newObjectPosition));
        }

        /// <summary>
        ///   Updates the last instance field of the class info into the database file
        /// </summary>
        /// <param name="classInfoId"> The class info to be updated </param>
        /// <param name="lastInstancePosition"> The last instance position @ </param>
        protected virtual void UpdateLastInstanceFieldOfClassInfoWithId(OID classInfoId, long lastInstancePosition)
        {
            var currentPosition = _fsi.GetPosition();
            // TODO CHECK LOGIC of getting position of class using this method for
            // object)
            var classInfoPosition = _idManager.GetObjectPositionWithOid(classInfoId, true);
            _fsi.SetWritePosition(classInfoPosition + StorageEngineConstant.ClassOffsetClassLastObjectPosition, true);
            _fsi.WriteLong(lastInstancePosition, true, "class info update last instance field",
                           DefaultWriteAction.PointerWriteAction);
            // TODO check if we need this
            _fsi.SetWritePosition(currentPosition, true);
        }

        /// <summary>
        ///   Updates the first instance field of the class info into the database file
        /// </summary>
        /// <param name="classInfoId"> The class info to be updated </param>
        /// <param name="firstInstancePosition"> The first instance position @ </param>
        protected virtual void UpdateFirstInstanceFieldOfClassInfoWithId(OID classInfoId, long firstInstancePosition)
        {
            var currentPosition = _fsi.GetPosition();
            // TODO CHECK LOGIC of getting position of class using this method for
            // object)
            var classInfoPosition = _idManager.GetObjectPositionWithOid(classInfoId, true);
            _fsi.SetWritePosition(classInfoPosition + StorageEngineConstant.ClassOffsetClassFirstObjectPosition, true);
            _fsi.WriteLong(firstInstancePosition, true, "class info update first instance field",
                           DefaultWriteAction.PointerWriteAction);
            // TODO check if we need this
            _fsi.SetWritePosition(currentPosition, true);
        }

        /// <summary>
        ///   Updates the number of objects of the class info into the database file
        /// </summary>
        /// <param name="classInfoId"> The class info to be updated </param>
        /// <param name="nbObjects"> The number of object @ </param>
        protected virtual void UpdateNbObjectsFieldOfClassInfo(OID classInfoId, long nbObjects)
        {
            var currentPosition = _fsi.GetPosition();
            var classInfoPosition = GetSession().GetMetaModel().GetClassInfoFromId(classInfoId).GetPosition();
            _fsi.SetWritePosition(classInfoPosition + StorageEngineConstant.ClassOffsetClassNbObjects, true);
            _fsi.WriteLong(nbObjects, true, "class info update nb objects", DefaultWriteAction.PointerWriteAction);
            // TODO check if we need this
            _fsi.SetWritePosition(currentPosition, true);
        }

        /// <summary>
        ///   <pre>Class User{
        ///     private String name;
        ///     private Function function;
        ///     }
        ///     When an object of type User is stored, it stores a reference to its function object.
        /// </pre>
        /// </summary>
        /// <remarks>
        ///   <pre>Class User{
        ///     private String name;
        ///     private Function function;
        ///     }
        ///     When an object of type User is stored, it stores a reference to its function object.
        ///     If the function is set to another, the pointer to the function object must be changed.
        ///     for example, it was pointing to a function at the position 1407, the 1407 value is stored while
        ///     writing the USer object, let's say at the position 528. To make the user point to another function object (which exist at the position 1890)
        ///     The position 528 must be updated to 1890.</pre>
        /// </remarks>
        public virtual void UpdateObjectReference(long positionWhereTheReferenceIsStored, OID newOid,
                                                  bool writeInTransaction)
        {
            var position = positionWhereTheReferenceIsStored;
            if (position < 0)
                throw new OdbRuntimeException(NDatabaseError.NegativePosition.AddParameter(position));
            _fsi.SetWritePosition(position, writeInTransaction);
            // Ids are always stored as negative value to differ from a position!
            var oid = StorageEngineConstant.NullObjectIdId;
            if (newOid != null)
                oid = -newOid.ObjectId;
            _fsi.WriteLong(oid, writeInTransaction, "object reference", DefaultWriteAction.PointerWriteAction);
        }

        /// <summary>
        ///   In place update for array element, only do in place update for atomic native fixed size elements
        /// </summary>
        /// <returns> true if in place update has been done,false if not </returns>
        private bool UpdateArrayElement(long arrayPosition, int arrayElementIndexToChange, NativeObjectInfo newValue,
                                        bool writeInTransaction)
        {
            // block size, block type, odb typeid,is null?
            long offset = OdbType.Integer.GetSize() + OdbType.Byte.GetSize() + OdbType.Integer.GetSize() +
                          OdbType.Boolean.GetSize();
            _fsi.SetReadPosition(arrayPosition + offset);
            // read class name of array elements
            var arrayElementClassName = _fsi.ReadString(false);
            // TODO try to get array element type from the ArrayObjectInfo
            // Check if the class has fixed size : array support in place update
            // only for fixed size class like int, long, date,...
            // String array,for example do not support in place update
            var arrayElementType = OdbType.GetFromName(arrayElementClassName);
            if (!arrayElementType.IsAtomicNative() || !arrayElementType.HasFixSize())
                return false;

            // reads the size of the array
            var arraySize = _fsi.ReadInt();
            if (arrayElementIndexToChange >= arraySize)
            {
                throw new OdbRuntimeException(
                    NDatabaseError.InplaceUpdateNotPossibleForArray.AddParameter(arraySize).AddParameter(
                        arrayElementIndexToChange));
            }
            // Gets the position where to write the object
            // Skip the positions where we have the pointers to each array element
            // then
            // jump to the right position
            long skip = arrayElementIndexToChange*OdbType.Long.GetSize();
            _fsi.SetReadPosition(_fsi.GetPosition() + skip);
            var elementArrayPosition = _fsi.ReadLong();
            _fsi.SetWritePosition(elementArrayPosition, writeInTransaction);
            // Actually update the array element
            WriteNativeObjectInfo(newValue, elementArrayPosition, writeInTransaction);
            return true;
        }

        public static int GetNbInPlaceUpdates()
        {
            return _nbInPlaceUpdates;
        }

        public static void SetNbInPlaceUpdates(int nbInPlaceUpdates)
        {
            _nbInPlaceUpdates = nbInPlaceUpdates;
        }

        public static int GetNbNormalUpdates()
        {
            return _nbNormalUpdates;
        }

        public static void SetNbNormalUpdates(int nbNormalUpdates)
        {
            _nbNormalUpdates = nbNormalUpdates;
        }

        public static void ResetNbUpdates()
        {
            _nbInPlaceUpdates = 0;
            _nbNormalUpdates = 0;
        }
    }
}