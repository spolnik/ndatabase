using System;
using System.Collections.Generic;
using System.Text;
using NDatabase2.Btree;
using NDatabase2.Odb.Core.BTree;
using NDatabase2.Odb.Core.Layers.Layer2.Instance;
using NDatabase2.Odb.Core.Layers.Layer2.Meta;
using NDatabase2.Odb.Core.Layers.Layer3.Block;
using NDatabase2.Odb.Core.Layers.Layer3.Oid;
using NDatabase2.Odb.Core.Oid;
using NDatabase2.Odb.Core.Query;
using NDatabase2.Odb.Core.Query.Criteria;
using NDatabase2.Odb.Core.Query.Execution;
using NDatabase2.Odb.Core.Query.Values;
using NDatabase2.Tool;
using NDatabase2.Tool.Wrappers.List;
using NDatabase2.Tool.Wrappers.Map;

namespace NDatabase2.Odb.Core.Layers.Layer3.Engine
{
    /// <summary>
    ///   Manage all IO Reading
    /// </summary>
    /// <author>olivier smadja</author>
    internal sealed class ObjectReader : IObjectReader
    {
        /// <summary>
        ///   The fsi is the object that knows how to write and read native types
        /// </summary>
        private readonly IFileSystemInterface _fsi;

        /// <summary>
        ///   to build instances
        /// </summary>
        private readonly IInstanceBuilder _instanceBuilder;

        /// <summary>
        ///   To hold block number.
        /// </summary>
        /// <remarks>
        ///   To hold block number. ODB compute the block number from the oid (as one block has 1000 oids), then it has to search the position of the block number! This cache is used to keep track of the positions of the block positions The key is the block number(Long) and the value the position (Long)
        /// </remarks>
        private IDictionary<long, long> _blockPositions;

        /// <summary>
        ///   A local variable to monitor object recursion
        /// </summary>
        private int _currentDepth;

        /// <summary>
        ///   The storage engine
        /// </summary>
        private IStorageEngine _storageEngine;

        /// <summary>
        ///   The constructor
        /// </summary>
        public ObjectReader(IStorageEngine engine)
        {
            _storageEngine = engine;
            _fsi = engine.GetObjectWriter().FileSystemProcessor.FileSystemInterface;
            _blockPositions = new OdbHashMap<long, long>();
            _instanceBuilder = BuildInstanceBuilder();
        }

        #region IObjectReader Members

        public void ReadDatabaseHeader()
        {
            // Reads the version of the database file
            var version = ReadVersion();
            var versionIsCompatible = version == StorageEngineConstant.CurrentFileFormatVersion;
            if (!versionIsCompatible)
            {
                throw new OdbRuntimeException(
                    NDatabaseError.RuntimeIncompatibleVersion.AddParameter(version).AddParameter(
                        StorageEngineConstant.CurrentFileFormatVersion));
            }
            var databaseIdsArray = new long[4];
            databaseIdsArray[0] = _fsi.ReadLong();
            databaseIdsArray[1] = _fsi.ReadLong();
            databaseIdsArray[2] = _fsi.ReadLong();
            databaseIdsArray[3] = _fsi.ReadLong();
            IDatabaseId databaseId = new DatabaseIdImpl(databaseIdsArray);

            var lastTransactionId = ReadLastTransactionId(databaseId);
            // Increment transaction id
            lastTransactionId = lastTransactionId.Next();
            var nbClasses = ReadNumberOfClasses();
            var firstClassPosition = ReadFirstClassOid();
            if (nbClasses < 0)
            {
                throw new CorruptedDatabaseException(
                    NDatabaseError.NegativeClassNumberInHeader.AddParameter(nbClasses).AddParameter(firstClassPosition));
            }
            ReadLastOdbCloseStatus();
            ReadDatabaseCharacterEncoding();

            var currentBlockPosition = _fsi.ReadLong("current block position");
            // Gets the current id block number
            _fsi.SetReadPosition(currentBlockPosition + StorageEngineConstant.BlockIdOffsetForBlockNumber);
            var currentBlockNumber = _fsi.ReadInt("current block id number");
            var maxId = OIDFactory.BuildObjectOID(_fsi.ReadLong("Block max id"));
            _storageEngine.SetDatabaseId(databaseId);

            var currentBlockInfo = new CurrentIdBlockInfo
                {
                    CurrentIdBlockPosition = currentBlockPosition,
                    CurrentIdBlockNumber = currentBlockNumber,
                    CurrentIdBlockMaxOid = maxId
                };

            _storageEngine.SetCurrentIdBlockInfos(currentBlockInfo);
            _storageEngine.SetCurrentTransactionId(lastTransactionId);
        }

        public void LoadMetaModel(MetaModel metaModel, bool full)
        {
            ClassInfo classInfo;
            var nbClasses = ReadNumberOfClasses();
            if (nbClasses == 0)
                return;

            // Set the cursor Where We Can Find The First Class info OID
            _fsi.SetReadPosition(StorageEngineConstant.DatabaseHeaderFirstClassOid);
            var classOID = OIDFactory.BuildClassOID(ReadFirstClassOid());
            // read headers
            for (var i = 0; i < nbClasses; i++)
            {
                classInfo = ReadClassInfoHeader(classOID);
                if (OdbConfiguration.IsLoggingEnabled())
                {
                    DLogger.Debug(string.Format(
                        "{0}Reading class header for {1} - oid = {2} prevOid={3} - nextOid={4}", DepthToSpaces(),
                        classInfo.FullClassName, classOID, classInfo.PreviousClassOID,
                        classInfo.NextClassOID));
                }
                metaModel.AddClass(classInfo);
                classOID = classInfo.NextClassOID;
            }

            if (!full)
                return;

            var allClasses = metaModel.GetAllClasses();

            // Read class info bodies
            foreach (var currentClassInfo in allClasses)
            {
                classInfo = ReadClassInfoBody(currentClassInfo);

                if (OdbConfiguration.IsLoggingEnabled())
                    DLogger.Debug(DepthToSpaces() + "Reading class body for " + classInfo.FullClassName);
            }

            // No need to add it to metamodel, it is already in it.
            // metaModel.addClass(classInfo);
            // Read last object of each class
            foreach (var actualClassInfo in allClasses)
            {
                if (OdbConfiguration.IsLoggingEnabled())
                {
                    DLogger.Debug(string.Format("{0}Reading class info last instance {1}", DepthToSpaces(),
                                                actualClassInfo.FullClassName));
                }
                if (actualClassInfo.CommitedZoneInfo.HasObjects())
                {
                    // TODO Check if must use true or false in return object
                    // parameter
                    try
                    {
                        // Retrieve the object by oid instead of position
                        var oid = actualClassInfo.CommitedZoneInfo.Last;
                        actualClassInfo.LastObjectInfoHeader = ReadObjectInfoHeaderFromOid(oid, true);
                    }
                    catch (OdbRuntimeException e)
                    {
                        throw new OdbRuntimeException(
                            NDatabaseError.MetamodelReadingLastObject.AddParameter(actualClassInfo.FullClassName).
                                AddParameter(actualClassInfo.CommitedZoneInfo.Last), e);
                    }
                }
            }

            _storageEngine.ResetCommitListeners();

            // Read class info indexes
            foreach (var actualClassInfo in allClasses)
            {
                IOdbList<ClassInfoIndex> indexes = new OdbList<ClassInfoIndex>();
                IQuery queryClassInfo = new CriteriaQuery(typeof(ClassInfoIndex));
                queryClassInfo.Descend("ClassInfoId").Equal(actualClassInfo.ClassInfoId);
                var classIndexes = GetObjects<ClassInfoIndex>(queryClassInfo, true, -1, -1);
                indexes.AddAll(classIndexes);
                // Sets the btree persister
                foreach (var classInfoIndex in indexes)
                {
                    IBTreePersister persister = new LazyOdbBtreePersister(_storageEngine);
                    var btree = classInfoIndex.BTree;
                    btree.SetPersister(persister);
                    btree.GetRoot().SetBTree(btree);
                }

                if (OdbConfiguration.IsLoggingEnabled())
                {
                    var count = indexes.Count.ToString();
                    DLogger.Debug(
                        string.Format("{0}Reading indexes for {1} : ", DepthToSpaces(), actualClassInfo.FullClassName) +
                        count + " indexes");
                }

                actualClassInfo.SetIndexes(indexes);
            }

            if (OdbConfiguration.IsLoggingEnabled())
                DLogger.Debug(DepthToSpaces() + "Current Meta Model is :" + metaModel);
        }

        public IOdbList<ClassInfoIndex> ReadClassInfoIndexesAt(long position, ClassInfo classInfo)
        {
            IOdbList<ClassInfoIndex> indexes = new OdbList<ClassInfoIndex>();
            _fsi.SetReadPosition(position);
            var nextIndexPosition = position;

            do
            {
                var classInfoIndex = new ClassInfoIndex();
                _fsi.SetReadPosition(nextIndexPosition);
                _fsi.ReadInt("block size");//blockSize
                var blockType = _fsi.ReadByte("block type");
                if (!BlockTypes.IsIndex(blockType))
                {
                    throw new OdbRuntimeException(
                        NDatabaseError.WrongTypeForBlockType.AddParameter(BlockTypes.BlockTypeIndex).AddParameter(
                            blockType).AddParameter(position).AddParameter("while reading indexes for " +
                                                                           classInfo.FullClassName));
                }
                _fsi.ReadLong("prev index pos"); //previousIndexPosition
                nextIndexPosition = _fsi.ReadLong("next index pos");
                classInfoIndex.Name = _fsi.ReadString("Index name");
                classInfoIndex.IsUnique = _fsi.ReadBoolean("index is unique");
                classInfoIndex.Status = _fsi.ReadByte("index status");
                classInfoIndex.CreationDate = _fsi.ReadLong("creation date");
                classInfoIndex.LastRebuild = _fsi.ReadLong("last rebuild");
                var nbAttributes = _fsi.ReadInt("number of fields");
                var attributeIds = new int[nbAttributes];
                for (var j = 0; j < nbAttributes; j++)
                    attributeIds[j] = _fsi.ReadInt("attr id");
                classInfoIndex.AttributeIds = attributeIds;
                indexes.Add(classInfoIndex);
            } while (nextIndexPosition != -1);
            return indexes;
        }

        /// <summary>
        ///   Reads the pointers(ids or positions) of an object that has the specific oid
        /// </summary>
        /// <param name="oid"> The oid of the object we want to read the pointers </param>
        /// <param name="useCache"> </param>
        /// <returns> The ObjectInfoHeader @ </returns>
        public ObjectInfoHeader ReadObjectInfoHeaderFromOid(OID oid, bool useCache)
        {
            if (useCache)
            {
                var objectInfoHeader = _storageEngine.GetSession(true).GetCache().GetObjectInfoHeaderByOid(oid, false);
                if (objectInfoHeader != null)
                    return objectInfoHeader;
            }

            var position = GetObjectPositionFromItsOid(oid, useCache, true);
            return ReadObjectInfoHeaderFromPosition(oid, position, useCache);
        }

        public NonNativeObjectInfo ReadNonNativeObjectInfoFromOid(ClassInfo classInfo, OID oid, bool useCache,
                                                                  bool returnObjects)
        {
            // FIXME if useCache, why not directly search the cache?
            var position = GetObjectPositionFromItsOid(oid, useCache, false);
            if (position == StorageEngineConstant.DeletedObjectPosition)
                return new NonNativeDeletedObjectInfo(position);
            if (position == StorageEngineConstant.ObjectDoesNotExist)
                throw new OdbRuntimeException(NDatabaseError.ObjectWithOidDoesNotExist.AddParameter(oid));
            var nnoi = ReadNonNativeObjectInfoFromPosition(classInfo, oid, position, useCache, returnObjects);

            return nnoi;
        }

        /// <summary>
        ///   Reads a non non native Object Info (Layer2) from its position
        /// </summary>
        /// <param name="classInfo"> </param>
        /// <param name="oid"> can be null </param>
        /// <param name="position"> </param>
        /// <param name="useCache"> </param>
        /// <param name="returnInstance"> </param>
        /// <returns> The meta representation of the object @ </returns>
        public NonNativeObjectInfo ReadNonNativeObjectInfoFromPosition(ClassInfo classInfo, OID oid, long position,
                                                                       bool useCache, bool returnInstance)
        {
            var lsession = _storageEngine.GetSession(true);
            // Get a temporary cache just to cache NonNativeObjectInfo being read to
            // avoid duplicated reads
            var cache = lsession.GetCache();
            var tmpCache = lsession.GetTmpCache();
            // ICache tmpCache =cache;
            // We are dealing with a non native object
            if (OdbConfiguration.IsLoggingEnabled())
                DLogger.Debug(DepthToSpaces() + "Reading Non Native Object info with oid " + oid);
            // If the object is already being read, then return from the cache
            if (tmpCache.IsReadingObjectInfoWithOid(oid))
                return tmpCache.GetObjectInfoByOid(oid);
            var objectInfoHeader = GetObjectInfoHeader(oid, position, useCache, cache);
            if (classInfo == null)
                classInfo =
                    _storageEngine.GetSession(true).GetMetaModel().GetClassInfoFromId(objectInfoHeader.GetClassInfoId());
            oid = objectInfoHeader.GetOid();
            // if class info do not match, reload class info
            if (!classInfo.ClassInfoId.Equals(objectInfoHeader.GetClassInfoId()))
                classInfo =
                    _storageEngine.GetSession(true).GetMetaModel().GetClassInfoFromId(objectInfoHeader.GetClassInfoId());
            
            if (OdbConfiguration.IsLoggingEnabled())
            {
                var positionAsString = objectInfoHeader.GetPosition().ToString();
                DLogger.Debug(DepthToSpaces() + "Reading Non Native Object info of " + (classInfo == null
                                                                                            ? "?"
                                                                                            : classInfo.FullClassName) + " at " +
                              positionAsString + " with id " + oid);
                DLogger.Debug(DepthToSpaces() + "  Object Header is " + objectInfoHeader);
            }

            var objectInfo = new NonNativeObjectInfo(objectInfoHeader, classInfo);
            objectInfo.SetOid(oid);
            objectInfo.SetClassInfo(classInfo);
            objectInfo.SetPosition(objectInfoHeader.GetPosition());
            // Adds the Object Info in cache. The remove (cache clearing) is done by
            // the Query Executor. This tmp cache is used to resolve cyclic reference problem.
            // When an object has cyclic reference, if we don t cache the object info, we will read the reference for ever!
            // With the cache , we detect the cyclic reference and return what has been read already
            tmpCache.StartReadingObjectInfoWithOid(objectInfo.GetOid(), objectInfo);
            AbstractObjectInfo aoi;
            IOdbList<PendingReading> pendingReadings = new OdbList<PendingReading>();
            for (var id = 1; id <= classInfo.MaxAttributeId; id++)
            {
                var cai = objectInfo.GetClassInfo().GetAttributeInfoFromId(id);
                if (cai == null)
                {
                    // the attribute does not exist anymore
                    continue;
                }
                var attributeIdentification = objectInfoHeader.GetAttributeIdentificationFromId(id);
                if (attributeIdentification == StorageEngineConstant.NullObjectPosition ||
                    attributeIdentification == StorageEngineConstant.NullObjectIdId)
                {
                    if (cai.IsNative())
                        aoi = NullNativeObjectInfo.GetInstance();
                    else
                        aoi = new NonNativeNullObjectInfo();
                    objectInfo.SetAttributeValue(id, aoi);
                }
                else
                {
                    // Here we can not use cai.isNonNative because of interfaces :
                    // because an interface will always be considered as non native
                    // (Object for example) but
                    // could contain a String for example. So we assume that if
                    // attributeIdentification is negative
                    // the object is non native,if positive the object is native.
                    if (attributeIdentification < 0)
                    {
                        // ClassInfo ci =
                        // storageEngine.getSession(true).getMetaModel().getClassInfo(cai.getFullClassname(),
                        // true);
                        // For non native objects. attribute identification is the
                        // oid (*-1)
                        var attributeOid = OIDFactory.BuildObjectOID(-
                                                                     attributeIdentification);
                        // We do not read now, store the reading as pending and
                        // reads it later
                        pendingReadings.Add(new PendingReading(id, null, attributeOid));
                    }
                    else
                    {
                        aoi = ReadObjectInfo(attributeIdentification, useCache, returnInstance);
                        objectInfo.SetAttributeValue(id, aoi);
                    }
                }
            }
            foreach (var pendingReading in pendingReadings)
            {
                // If object is not in connected zone , the cache must be used
                var useCacheForAttribute = useCache ||
                                           !cache.IsInCommitedZone(pendingReading.GetAttributeOID());
                aoi = ReadNonNativeObjectInfoFromOid(pendingReading.GetCi(), pendingReading.GetAttributeOID(),
                                                     useCacheForAttribute, returnInstance);
                objectInfo.SetAttributeValue(pendingReading.GetId(), aoi);
            }

            return objectInfo;
        }

        public AttributeValuesMap ReadObjectInfoValuesFromOID(ClassInfo classInfo, OID oid, bool useCache,
                                                              IOdbList<string> attributeNames,
                                                              IOdbList<string> relationAttributeNames,
                                                              int recursionLevel, string[] orderByFields)
        {
            var position = GetObjectPositionFromItsOid(oid, useCache, true);
            return ReadObjectInfoValuesFromPosition(classInfo, oid, position, useCache, attributeNames,
                                                    relationAttributeNames, recursionLevel);
        }

        public object ReadAtomicNativeObjectInfoAsObject(long position, int odbTypeId)
        {
            object o = null;
            switch (odbTypeId)
            {
                case OdbType.ByteId:
                {
                    o = _fsi.ReadByte("atomic");
                    break;
                }

                case OdbType.SByteId:
                {
                    o = _fsi.ReadSByte("atomic");
                    break;
                }

                case OdbType.BooleanId:
                {
                    o = _fsi.ReadBoolean("atomic");
                    break;
                }

                case OdbType.CharacterId:
                {
                    o = _fsi.ReadChar("atomic");
                    break;
                }

                case OdbType.FloatId:
                {
                    o = _fsi.ReadFloat("atomic");
                    break;
                }

                case OdbType.DoubleId:
                {
                    o = _fsi.ReadDouble("atomic");
                    break;
                }

                case OdbType.IntegerId:
                {
                    o = _fsi.ReadInt("atomic");
                    break;
                }

                case OdbType.UIntegerId:
                {
                    o = _fsi.ReadUInt("atomic");
                    break;
                }

                case OdbType.LongId:
                {
                    o = _fsi.ReadLong("atomic");
                    break;
                }

                case OdbType.ULongId:
                {
                    o = _fsi.ReadULong("atomic");
                    break;
                }

                case OdbType.ShortId:
                {
                    o = _fsi.ReadShort("atomic");
                    break;
                }

                case OdbType.UShortId:
                {
                    o = _fsi.ReadUShort("atomic");
                    break;
                }

                case OdbType.DecimalId:
                {
                    o = _fsi.ReadBigDecimal("atomic");
                    break;
                }

                case OdbType.DateId:
                {
                    o = _fsi.ReadDate("atomic");
                    break;
                }

                case OdbType.ObjectOidId:
                {
                    var oid = _fsi.ReadLong("oid");
                    o = OIDFactory.BuildObjectOID(oid);
                    break;
                }

                case OdbType.ClassOidId:
                {
                    var cid = _fsi.ReadLong("oid");
                    o = OIDFactory.BuildClassOID(cid);
                    break;
                }

                case OdbType.StringId:
                {
                    o = _fsi.ReadString();
                    break;
                }

                case OdbType.EnumId:
                {
                    o = _fsi.ReadString();
                    break;
                }
            }
            if (o == null)
            {
                throw new OdbRuntimeException(
                    NDatabaseError.NativeTypeNotSupported.AddParameter(odbTypeId).AddParameter(
                        OdbType.GetNameFromId(odbTypeId)));
            }
            return o;
        }

        /// <summary>
        ///   Reads an atomic object
        /// </summary>
        public AtomicNativeObjectInfo ReadAtomicNativeObjectInfo(long position, int odbTypeId)
        {
            var @object = ReadAtomicNativeObjectInfoAsObject(position, odbTypeId);
            return new AtomicNativeObjectInfo(@object, odbTypeId);
        }

        /// <summary>
        ///   Gets the next object oid of the object with the specific oid
        /// </summary>
        /// <returns> The position of the next object. If there is no next object, return -1 @ </returns>
        public OID GetNextObjectOID(OID oid)
        {
            var position = _storageEngine.GetObjectWriter().GetIdManager().GetObjectPositionWithOid(oid, true);
            _fsi.SetReadPosition(position + StorageEngineConstant.ObjectOffsetNextObjectOid);
            return OIDFactory.BuildObjectOID(_fsi.ReadLong());
        }

        public long ReadOidPosition(OID oid)
        {
            if (OdbConfiguration.IsLoggingEnabled())
                DLogger.Debug("  Start of readOidPosition for oid " + oid);

            var blockNumber = GetIdBlockNumberOfOid(oid);
            var blockPosition = GetIdBlockPositionFromNumber(blockNumber);

            if (OdbConfiguration.IsLoggingEnabled())
            {
                var blockNumberAsString = blockNumber.ToString();
                var blockPositionAsString = blockPosition.ToString();
                DLogger.Debug(string.Format("  Block number of oid {0} is ", oid) + blockNumberAsString +
                              " / block position = " + blockPositionAsString);
            }

            var position = blockPosition + StorageEngineConstant.BlockIdOffsetForStartOfRepetition +
                           ((oid.ObjectId - 1) % OdbConfiguration.GetNbIdsPerBlock()) *
                           OdbConfiguration.GetIdBlockRepetitionSize();

            if (OdbConfiguration.IsLoggingEnabled())
            {
                var positionAsString = position.ToString();
                DLogger.Debug(string.Format("  End of readOidPosition for oid {0} returning position ", oid) + positionAsString);
            }

            return position;
        }

        public object GetObjectFromOid(OID oid, bool returnInstance, bool useCache)
        {
            var position = GetObjectPositionFromItsOid(oid, useCache, true);
            var o = ReadNonNativeObjectAtPosition(position, useCache, returnInstance);
            // Clear the tmp cache. This cache is use to resolve cyclic references
            _storageEngine.GetSession(true).GetTmpCache().ClearObjectInfos();
            return o;
        }

        /// <summary>
        ///   Gets the real object position from its OID
        /// </summary>
        /// <param name="oid"> The oid of the object to get the position </param>
        /// <param name="useCache"> </param>
        /// <param name="throwException"> To indicate if an exception must be thrown if object is not found </param>
        /// <returns> The object position, if object has been marked as deleted then return StorageEngineConstant.DELETED_OBJECT_POSITION @ </returns>
        public long GetObjectPositionFromItsOid(OID oid, bool useCache, bool throwException)
        {
            if (OdbConfiguration.IsLoggingEnabled())
                DLogger.Debug("  getObjectPositionFromItsId for oid " + oid);
            // Check if oid is in cache
            var position = StorageEngineConstant.ObjectIsNotInCache;
            if (useCache)
            {
                // This return -1 if not in the cache
                position = _storageEngine.GetSession(true).GetCache().GetObjectPositionByOid(oid);
            }
            // FIXME Check if we need this. Removing it causes the TestDelete.test6 to fail 
            if (position == StorageEngineConstant.DeletedObjectPosition)
            {
                if (throwException)
                    throw new CorruptedDatabaseException(NDatabaseError.ObjectIsMarkedAsDeletedForOid.AddParameter(oid));
                return StorageEngineConstant.DeletedObjectPosition;
            }
            if (position != StorageEngineConstant.ObjectIsNotInCache &&
                position != StorageEngineConstant.DeletedObjectPosition)
                return position;
            // The position was not found is the cache
            position = ReadOidPosition(oid);
            position += StorageEngineConstant.BlockIdRepetitionIdStatus;
            _fsi.SetReadPosition(position);
            var idStatus = _fsi.ReadByte();
            var objectPosition = _fsi.ReadLong();
            if (!IDStatus.IsActive(idStatus))
            {
                // if object position == 0, The object dos not exist
                if (throwException)
                {
                    if (objectPosition == 0)
                        throw new CorruptedDatabaseException(NDatabaseError.ObjectWithOidDoesNotExist.AddParameter(oid));
                    throw new CorruptedDatabaseException(NDatabaseError.ObjectIsMarkedAsDeletedForOid.AddParameter(oid));
                }
                if (objectPosition == 0)
                    return StorageEngineConstant.ObjectDoesNotExist;
                return StorageEngineConstant.DeletedObjectPosition;
            }
            if (OdbConfiguration.IsLoggingEnabled())
            {
                var positionAsString = objectPosition.ToString();
                DLogger.Debug("  object position of object with oid " + oid + " is " + positionAsString);
            }
            return objectPosition;
        }

        /// <summary>
        ///   Returns information about all OIDs of the database
        /// </summary>
        /// <param name="idType"> </param>
        /// <returns> @ </returns>
        public IList<long> GetAllIds(byte idType)
        {
            IList<long> ids = new List<long>(5000);
            long currentBlockPosition = StorageEngineConstant.DatabaseHeaderFirstIdBlockPosition;
            while (currentBlockPosition != -1)
            {
                // Gets the next block position
                _fsi.SetReadPosition(currentBlockPosition + StorageEngineConstant.BlockIdOffsetForNextBlock);
                var nextBlockPosition = _fsi.ReadLong();
                // Gets the block max id
                _fsi.SetReadPosition(currentBlockPosition + StorageEngineConstant.BlockIdOffsetForMaxId);
                var blockMaxId = _fsi.ReadLong();
                long currentId;
                do
                {
                    var nextRepetitionPosition = _fsi.GetPosition() + OdbConfiguration.GetIdBlockRepetitionSize();
                    var idTypeRead = _fsi.ReadByte();
                    currentId = _fsi.ReadLong();
                    var idStatus = _fsi.ReadByte();
                    if (idType == idTypeRead && IDStatus.IsActive(idStatus))
                        ids.Add(currentId);
                    _fsi.SetReadPosition(nextRepetitionPosition);
                } while (currentId != blockMaxId);
                currentBlockPosition = nextBlockPosition;
            }
            return ids;
        }

        public IList<FullIDInfo> GetAllIdInfos(string objectTypeToDisplay, byte idType, bool displayObject)
        {
            IList<FullIDInfo> idInfos = new List<FullIDInfo>(5000);
            OID prevObjectOID = null;
            OID nextObjectOID = null;

            long currentBlockPosition = StorageEngineConstant.DatabaseHeaderFirstIdBlockPosition;
            var objectToString = "empty";

            while (currentBlockPosition != -1)
            {
                var positionAsString = currentBlockPosition.ToString();
                DLogger.Debug("Current block position = " + positionAsString);

                _fsi.SetReadPosition(currentBlockPosition + StorageEngineConstant.BlockIdOffsetForBlockNumber);
                _fsi.SetReadPosition(currentBlockPosition + StorageEngineConstant.BlockIdOffsetForNextBlock);
                var nextBlockPosition = _fsi.ReadLong();
                // Gets block number
                long blockId = _fsi.ReadInt();
                var blockMaxId = _fsi.ReadLong();
                long currentId;
                do
                {
                    var nextRepetitionPosition = _fsi.GetPosition() + OdbConfiguration.GetIdBlockRepetitionSize();
                    var idTypeRead = _fsi.ReadByte();
                    currentId = _fsi.ReadLong();
                    var idStatus = _fsi.ReadByte();
                    var objectPosition = _fsi.ReadLong();
                    FullIDInfo info;
                    string objectType;
                    if (idType == idTypeRead)
                    {
                        // && IDStatus.isActive(idStatus)) {
                        var currentPosition = _fsi.GetPosition();
                        if (displayObject)
                        {
                            try
                            {
                                AbstractObjectInfo aoi = ReadNonNativeObjectInfoFromPosition(null, null, objectPosition,
                                                                                             false, false);
                                if (!(aoi is NonNativeDeletedObjectInfo))
                                {
                                    objectToString = aoi.ToString();
                                    var nnoi = (NonNativeObjectInfo) aoi;
                                    prevObjectOID = nnoi.GetPreviousObjectOID();
                                    nextObjectOID = nnoi.GetNextObjectOID();
                                }
                                else
                                {
                                    objectToString = " deleted";
                                    prevObjectOID = null;
                                    nextObjectOID = null;
                                }
                            }
                            catch (Exception)
                            {
                                // info = new IDInfo(currentId, objectPosition,
                                // idStatus, blockId, "unknow", "Error", -1, -1);
                                // idInfos.add(info);
                                objectToString = "?";
                                prevObjectOID = null;
                                nextObjectOID = null;
                            }
                        }
                        try
                        {
                            objectType = GetObjectTypeFromPosition(objectPosition);
                        }
                        catch (Exception)
                        {
                            objectType = "(error?)";
                        }
                        if (objectTypeToDisplay == null || objectTypeToDisplay.Equals(objectType))
                        {
                            _fsi.SetReadPosition(currentPosition);
                            info = new FullIDInfo(currentId, objectPosition, idStatus, blockId, objectType,
                                                  objectToString, prevObjectOID, nextObjectOID);
                            idInfos.Add(info);
                        }
                    }
                    else
                    {
                        try
                        {
                            var ci = ReadClassInfoHeader(OIDFactory.BuildClassOID(currentId));
                            objectType = "Class def. of " + ci.FullClassName;
                            objectToString = ci.ToString();
                            prevObjectOID = ci.PreviousClassOID;
                            nextObjectOID = ci.NextClassOID;
                            info = new FullIDInfo(currentId, objectPosition, idStatus, blockId, objectType,
                                                  objectToString, prevObjectOID, nextObjectOID);
                            idInfos.Add(info);
                        }
                        catch (Exception)
                        {
                            info = new FullIDInfo(currentId, objectPosition, idStatus, blockId, "unknow", "Error", null,
                                                  null);
                            idInfos.Add(info);
                        }
                    }
                    _fsi.SetReadPosition(nextRepetitionPosition);
                } while (currentId != blockMaxId);
                currentBlockPosition = nextBlockPosition;
            }
            return idInfos;
        }

        public OID GetIdOfObjectAt(long position, bool includeDeleted)
        {
            _fsi.SetReadPosition(position + OdbType.Integer.Size);
            var blockType = _fsi.ReadByte("object block type");
            if (BlockTypes.IsPointer(blockType))
                return GetIdOfObjectAt(_fsi.ReadLong("new position"), includeDeleted);
            if (BlockTypes.IsNonNative(blockType))
                return OIDFactory.BuildObjectOID(_fsi.ReadLong("oid"));
            if (includeDeleted && BlockTypes.IsDeletedObject(blockType))
                return OIDFactory.BuildObjectOID(_fsi.ReadLong("oid"));
            throw new CorruptedDatabaseException(
                NDatabaseError.WrongTypeForBlockType.AddParameter(BlockTypes.BlockTypeNonNativeObject).AddParameter(
                    blockType).AddParameter(position));
        }

        public void Close()
        {
            _storageEngine = null;
            _blockPositions.Clear();
            _blockPositions = null;
        }

        public object BuildOneInstance(NonNativeObjectInfo objectInfo)
        {
            return _instanceBuilder.BuildOneInstance(objectInfo, _storageEngine.GetSession(true).GetCache());
        }

        public IInternalObjectSet<T> GetObjects<T>(IQuery query, bool inMemory, int startIndex, int endIndex) where T : class
        {
            IMatchingObjectAction queryResultAction = new QueryResultAction<T>(query, inMemory, _storageEngine,
                                                                                         true, _instanceBuilder);
            
            var queryExecutor = QueryManager.GetQueryExecutor(query, _storageEngine);

            return queryExecutor.Execute<T>(inMemory, startIndex, endIndex, true, queryResultAction);
        }

        public IValues GetValues(IValuesQuery valuesQuery, int startIndex, int endIndex)
        {
            IMatchingObjectAction queryResultAction;
            if (valuesQuery.HasGroupBy())
                queryResultAction = new GroupByValuesQueryResultAction(valuesQuery, _instanceBuilder);
            else
                queryResultAction = new ValuesQueryResultAction(valuesQuery, _storageEngine, _instanceBuilder);
            var objects = GetObjectInfos<IObjectValues>(valuesQuery, true, startIndex, endIndex, false,
                                                        queryResultAction);
            return (IValues) objects;
        }

        public IObjectSet<TResult> GetObjectInfos<TResult>(IQuery query, bool inMemory, int startIndex, int endIndex,
                                             bool returnObjects, IMatchingObjectAction queryResultAction)
        {
            var executor = QueryManager.GetQueryExecutor(query, _storageEngine);
            return executor.Execute<TResult>(inMemory, startIndex, endIndex, returnObjects, queryResultAction);
        }

        public string GetBaseIdentification()
        {
            return _storageEngine.GetBaseIdentification().Id;
        }

        public IInstanceBuilder GetInstanceBuilder()
        {
            return _instanceBuilder;
        }

        #endregion

        /// <summary>
        ///   A small method for indentation
        /// </summary>
        public string DepthToSpaces()
        {
            var buffer = new StringBuilder();
            for (var i = 0; i < _currentDepth; i++)
                buffer.Append("  ");
            return buffer.ToString();
        }

        private IInstanceBuilder BuildInstanceBuilder()
        {
            return new InstanceBuilder(_storageEngine);
        }

        /// <summary>
        ///   Read the version of the database file
        /// </summary>
        private int ReadVersion()
        {
            _fsi.SetReadPosition(StorageEngineConstant.DatabaseHeaderVersionPosition);
            return _fsi.ReadInt();
        }

        /// <summary>
        ///   Read the last transaction id
        /// </summary>
        private ITransactionId ReadLastTransactionId(IDatabaseId databaseId)
        {
            _fsi.SetReadPosition(StorageEngineConstant.DatabaseHeaderLastTransactionId);
            var id = new long[2];
            id[0] = _fsi.ReadLong();
            id[1] = _fsi.ReadLong();
            return new TransactionIdImpl(databaseId, id[0], id[1]);
        }

        /// <summary>
        ///   Reads the number of classes in database file
        /// </summary>
        private long ReadNumberOfClasses()
        {
            _fsi.SetReadPosition(StorageEngineConstant.DatabaseHeaderNumberOfClassesPosition);
            return _fsi.ReadLong();
        }

        /// <summary>
        ///   Reads the first class OID
        /// </summary>
        private long ReadFirstClassOid()
        {
            _fsi.SetReadPosition(StorageEngineConstant.DatabaseHeaderFirstClassOid);
            return _fsi.ReadLong();
        }

        /// <summary>
        ///   Reads the status of the last odb close
        /// </summary>
        private bool ReadLastOdbCloseStatus()
        {
            _fsi.SetReadPosition(StorageEngineConstant.DatabaseHeaderLastCloseStatusPosition);
            return _fsi.ReadBoolean("last odb status");
        }

        /// <summary>
        ///   Reads the database character encoding
        /// </summary>
        private string ReadDatabaseCharacterEncoding()
        {
            _fsi.SetReadPosition(StorageEngineConstant.DatabaseHeaderDatabaseCharacterEncodingPosition);
            return _fsi.ReadString();
        }

        /// <summary>
        ///   Read the class info header with the specific oid
        /// </summary>
        /// <returns> The read class info object @ </returns>
        private ClassInfo ReadClassInfoHeader(OID classInfoOid)
        {
            if (OdbConfiguration.IsLoggingEnabled())
                DLogger.Debug(DepthToSpaces() + "Reading new Class info Header with oid " + classInfoOid);
            var classInfoPosition = GetObjectPositionFromItsOid(classInfoOid, true, true);
            _fsi.SetReadPosition(classInfoPosition);
            var blockSize = _fsi.ReadInt("class info block size");
            var blockType = _fsi.ReadByte("class info block type");
            if (!BlockTypes.IsClassHeader(blockType))
            {
                throw new OdbRuntimeException(
                    NDatabaseError.WrongTypeForBlockType.AddParameter("Class Header").AddParameter(blockType).
                        AddParameter(classInfoPosition));
            }
            var classInfoCategory = _fsi.ReadByte("class info category");
            
            var classInfoId = OIDFactory.BuildClassOID(_fsi.ReadLong());
            var previousClassOID = ReadOid("prev class oid");
            var nextClassOID = ReadOid("next class oid");
            var nbObjects = _fsi.ReadLong();
            var originalZoneInfoFirst = ReadOid("ci first object oid");
            var originalZoneInfoLast = ReadOid("ci last object oid");
            var fullClassName = _fsi.ReadString();
            var maxAttributeId = _fsi.ReadInt();
            var attributesDefinitionPosition = _fsi.ReadLong();

            var classInfo = new ClassInfo(fullClassName)
                {
                    ClassCategory = classInfoCategory,
                    Position = classInfoPosition,
                    ClassInfoId = classInfoId,
                    PreviousClassOID = previousClassOID,
                    NextClassOID = nextClassOID,
                    MaxAttributeId = maxAttributeId,
                    AttributesDefinitionPosition = attributesDefinitionPosition
                };

            classInfo.OriginalZoneInfo.SetNbObjects(nbObjects);
            classInfo.OriginalZoneInfo.First = originalZoneInfoFirst;
            classInfo.OriginalZoneInfo.Last = originalZoneInfoLast;
            classInfo.CommitedZoneInfo.SetBasedOn(classInfo.OriginalZoneInfo);

            // FIXME Convert block size to long ??
            var realBlockSize = (int) (_fsi.GetPosition() - classInfoPosition);
            if (blockSize != realBlockSize)
            {
                throw new OdbRuntimeException(
                    NDatabaseError.WrongBlockSize.AddParameter(blockSize).AddParameter(realBlockSize).AddParameter(
                        classInfoPosition));
            }
            return classInfo;
        }

        private static OID DecodeOid(byte[] bytes, int offset)
        {
            var oid = ByteArrayConverter.ByteArrayToLong(bytes, offset);
            if (oid == -1)
                return null;
            return OIDFactory.BuildObjectOID(oid);
        }

        private OID ReadOid(string label)
        {
            var oid = _fsi.ReadLong(label);
            if (oid == -1)
                return null;
            return OIDFactory.BuildObjectOID(oid);
        }

        /// <summary>
        ///   Reads the body of a class info
        /// </summary>
        /// <param name="classInfo"> The class info to be read with already read header </param>
        /// <returns> The read class info @ </returns>
        private ClassInfo ReadClassInfoBody(ClassInfo classInfo)
        {
            if (OdbConfiguration.IsLoggingEnabled())
            {
                var attributesDefinitionPositionAsString = classInfo.AttributesDefinitionPosition.ToString();
                DLogger.Debug(DepthToSpaces() + "Reading new Class info Body at " + attributesDefinitionPositionAsString);
            }
            _fsi.SetReadPosition(classInfo.AttributesDefinitionPosition);
            var blockSize = _fsi.ReadInt();
            var blockType = _fsi.ReadByte();
            if (!BlockTypes.IsClassBody(blockType))
            {
                throw new OdbRuntimeException(
                    NDatabaseError.WrongTypeForBlockType.AddParameter("Class Body").AddParameter(blockType).AddParameter(
                        classInfo.AttributesDefinitionPosition));
            }
            // TODO This should be a short instead of long
            var nbAttributes = _fsi.ReadLong();
            IOdbList<ClassAttributeInfo> attributes = new OdbList<ClassAttributeInfo>((int) nbAttributes);
            for (var i = 0; i < nbAttributes; i++)
                attributes.Add(ReadClassAttributeInfo());
            classInfo.Attributes = attributes;
            // FIXME Convert blocksize to long ??
            var realBlockSize = (int) (_fsi.GetPosition() - classInfo.AttributesDefinitionPosition);
            if (blockSize != realBlockSize)
            {
                throw new OdbRuntimeException(
                    NDatabaseError.WrongBlockSize.AddParameter(blockSize).AddParameter(realBlockSize).AddParameter(
                        classInfo.AttributesDefinitionPosition));
            }
            return classInfo;
        }

        /// <summary>
        ///   Read an attribute of a class at the current position
        /// </summary>
        /// <returns> The ClassAttributeInfo description of the class attribute @ </returns>
        private ClassAttributeInfo ReadClassAttributeInfo()
        {
            var cai = new ClassAttributeInfo();
            var attributeId = _fsi.ReadInt();
            var isNative = _fsi.ReadBoolean();
            if (isNative)
            {
                var attributeTypeId = _fsi.ReadInt();
                var type = OdbType.GetFromId(attributeTypeId);
                // if it is an array, read also the subtype
                if (type.IsArray())
                {
                    type = type.Copy();
                    var subTypeId = _fsi.ReadInt();
                    var subType = OdbType.GetFromId(subTypeId);
                    if (subType.IsNonNative())
                    {
                        var fullClassName =
                            _storageEngine.GetSession(true).GetMetaModel().GetClassInfoFromId(
                                OIDFactory.BuildClassOID(_fsi.ReadLong())).FullClassName;

                        subType = subType.Copy(fullClassName);
                    }
                    type.SubType = subType;
                }
                cai.SetAttributeType(type);
                // For enum, we get the class info id of the enum class
                if (type.IsEnum())
                {
                    var classInfoId = _fsi.ReadLong();
                    var metaModel = _storageEngine.GetSession(true).GetMetaModel();
                    cai.SetFullClassName(
                        metaModel.GetClassInfoFromId(OIDFactory.BuildClassOID(classInfoId)).FullClassName);
                    // For enum, we need to create a new type just to set the real enum class name
                    type = type.Copy(cai.GetFullClassname());
                    cai.SetAttributeType(type);
                }
                else
                    cai.SetFullClassName(cai.GetAttributeType().Name);
            }
            else
            {
                // This is a non native, gets the id of the type and gets it from
                // meta-model
                var metaModel = _storageEngine.GetSession(true).GetMetaModel();
                var typeId = _fsi.ReadLong();
                cai.SetFullClassName(metaModel.GetClassInfoFromId(OIDFactory.BuildClassOID(typeId)).FullClassName);
                cai.SetClassInfo(metaModel.GetClassInfo(cai.GetFullClassname(), true));
                cai.SetAttributeType(OdbType.GetFromName(cai.GetFullClassname()));
            }
            cai.SetName(_fsi.ReadString());
            cai.SetIndex(_fsi.ReadBoolean());
            cai.SetId(attributeId);
            return cai;
        }

        /// <summary>
        ///   Reads an object at the specific position
        /// </summary>
        /// <param name="position"> The position to read </param>
        /// <param name="useCache"> To indicate if cache must be used </param>
        /// <param name="returnInstance"> indicate if an instance must be return of just the meta info </param>
        /// <returns> The object with position @ </returns>
        private object ReadNonNativeObjectAtPosition(long position, bool useCache, bool returnInstance)
        {
            // First reads the object info - which is a meta representation of the
            // object
            var nnoi = ReadNonNativeObjectInfoFromPosition(null, null, position, useCache, returnInstance);
            if (nnoi.IsDeletedObject())
                throw new OdbRuntimeException(NDatabaseError.ObjectIsMarkedAsDeletedForPosition.AddParameter(position));
            if (!returnInstance)
                return nnoi;
            // Then converts it to the real object
            var o = _instanceBuilder.BuildOneInstance(nnoi, _storageEngine.GetSession(true).GetCache());
            return o;
        }

        private AbstractObjectInfo ReadObjectInfo(long objectIdentification, bool useCache, bool returnObjects)
        {
            // If object identification is negative, it is an oid.
            if (objectIdentification < 0)
            {
                var oid = OIDFactory.BuildObjectOID(-objectIdentification);
                return ReadNonNativeObjectInfoFromOid(null, oid, useCache, returnObjects);
            }
            return ReadObjectInfoFromPosition(null, objectIdentification, useCache, returnObjects);
        }

        private ObjectInfoHeader ReadObjectInfoHeaderFromPosition(OID oid, long position, bool useCache)
        {
            if (position > _fsi.GetLength())
            {
                throw new CorruptedDatabaseException(
                    NDatabaseError.InstancePositionOutOfFile.AddParameter(position).AddParameter(_fsi.GetLength()));
            }
            if (position < 0)
            {
                throw new CorruptedDatabaseException(
                    NDatabaseError.InstancePositionIsNegative.AddParameter(position).AddParameter(oid.ToString()));
            }
            // adds an integer because, we pull the block size
            _fsi.SetReadPosition(position + OdbType.Integer.Size);
            var blockType = _fsi.ReadByte("object block type");
            if (BlockTypes.IsNonNative(blockType))
            {
                // compute the number of bytes to read
                // OID + ClassOid + PrevOid + NextOid + createDate + update Date + objectVersion + nbAttributes + RefCoutner + IsRoot
                // Long + Long +    Long    +  Long    + Long       + Long       +   int         + Int          + long       + byte
                var tsize = 7 * OdbType.SizeOfLong + 2 * OdbType.SizeOfInt + OdbType.SizeOfByte;
                var abytes = _fsi.ReadBytes(tsize);
                var readOid = DecodeOid(abytes, 0);
                // oid can be -1 (if was not set),in this case there is no way to
                // check
                if (oid != null && readOid.CompareTo(oid) != 0)
                {
                    throw new CorruptedDatabaseException(
                        NDatabaseError.WrongOidAtPosition.AddParameter(oid).AddParameter(position).AddParameter(readOid));
                }
                // If oid is not defined, uses the one that has been read
                if (oid == null)
                    oid = readOid;
                // It is a non native object
                var classInfoId = OIDFactory.BuildClassOID(ByteArrayConverter.ByteArrayToLong(abytes, 8));
                var prevObjectOID = DecodeOid(abytes, 16);
                var nextObjectOID = DecodeOid(abytes, 24);
                var creationDate = ByteArrayConverter.ByteArrayToLong(abytes, 32);
                var updateDate = ByteArrayConverter.ByteArrayToLong(abytes, 40);
                var objectVersion = ByteArrayConverter.ByteArrayToInt(abytes, 48);
                var refCounter = ByteArrayConverter.ByteArrayToLong(abytes, 52);
                var isRoot = ByteArrayConverter.ByteArrayToBoolean(abytes, 60);

                // Now gets info about attributes
                var nbAttributesRead = ByteArrayConverter.ByteArrayToInt(abytes, 61);
                // Now gets an array with the identification all attributes (can be
                // positions(for native objects) or ids(for non native objects))
                var attributesIdentification = new long[nbAttributesRead];
                var attributeIds = new int[nbAttributesRead];
                var atsize = OdbType.SizeOfInt + OdbType.SizeOfLong;
                // Reads the bytes and then convert to values
                var bytes = _fsi.ReadBytes(nbAttributesRead * atsize);
                for (var i = 0; i < nbAttributesRead; i++)
                {
                    attributeIds[i] = ByteArrayConverter.ByteArrayToInt(bytes, i * atsize);
                    attributesIdentification[i] = ByteArrayConverter.ByteArrayToLong(bytes,
                                                                                      i * atsize + OdbType.SizeOfInt);
                }
                var oip = new ObjectInfoHeader(position, prevObjectOID, nextObjectOID, classInfoId,
                                               attributesIdentification, attributeIds);
                oip.SetObjectVersion(objectVersion);
                oip.SetCreationDate(creationDate);
                oip.SetUpdateDate(updateDate);
                oip.SetOid(oid);
                oip.SetClassInfoId(classInfoId);
                oip.IsRoot = isRoot;
                oip.RefCounter = refCounter;

                if (useCache)
                {
                    // the object info does not exist in the cache
                    _storageEngine.GetSession(true).GetCache().AddObjectInfoOfNonCommitedObject(oip);
                }
                return oip;
            }
            if (BlockTypes.IsPointer(blockType))
                throw new CorruptedDatabaseException(NDatabaseError.FoundPointer.AddParameter(oid).AddParameter(position));
            
            var positionAsString = position.ToString();
            throw new CorruptedDatabaseException(
                NDatabaseError.WrongTypeForBlockType.AddParameter(BlockTypes.BlockTypeNonNativeObject).AddParameter(
                    blockType).AddParameter(positionAsString + "/oid=" + oid));
        }

        /// <summary>
        ///   Reads an object info(Object meta information like its type and its values) from the database file <p /> <pre>reads its type and then read all its attributes.</pre>
        /// </summary>
        /// <remarks>
        ///   Reads an object info(Object meta information like its type and its values) from the database file <p /> <pre>reads its type and then read all its attributes.
        ///                                                                                                             If one attribute is a non native object, it will be read (recursivly).
        ///                                                                                                             &lt;p/&gt;</pre>
        /// </remarks>
        /// <param name="classInfo"> If null, we are probably reading a native instance : String for example </param>
        /// <param name="objectPosition"> </param>
        /// <param name="useCache"> To indicate if cache must be used. If not, the old version of the object will read </param>
        /// <param name="returnObjects"> </param>
        /// <returns> The object abstract meta representation @ </returns>
        private AbstractObjectInfo ReadObjectInfoFromPosition(ClassInfo classInfo, long objectPosition, bool useCache,
                                                             bool returnObjects)
        {
            _currentDepth++;
            try
            {
                // Protection against bad parameter value
                if (objectPosition > _fsi.GetLength())
                {
                    throw new OdbRuntimeException(
                        NDatabaseError.InstancePositionOutOfFile.AddParameter(objectPosition).AddParameter(
                            _fsi.GetLength()));
                }
                if (objectPosition == StorageEngineConstant.DeletedObjectPosition ||
                    objectPosition == StorageEngineConstant.NullObjectPosition)
                {
                    // TODO Is this correct ?
                    return new NonNativeDeletedObjectInfo(objectPosition);
                }
                
                // Read block size and block type
                // block type is used to decide what to do
                _fsi.SetReadPosition(objectPosition);
                // Reads the block size
                var blockSize = _fsi.ReadInt("object block size");
                // And the block type
                var blockType = _fsi.ReadByte("object block type");
                // Null objects
                if (BlockTypes.IsNullNonNativeObject(blockType))
                    return new NonNativeNullObjectInfo(classInfo);
                if (BlockTypes.IsNullNativeObject(blockType))
                    return NullNativeObjectInfo.GetInstance();
                // Deleted objects
                if (BlockTypes.IsDeletedObject(blockType))
                    return new NonNativeDeletedObjectInfo(objectPosition);
                // Checks if what we are reading is only a pointer to the real
                // block, if
                // it is the case, just recall this method with the right position
                if (BlockTypes.IsPointer(blockType))
                    throw new CorruptedDatabaseException(NDatabaseError.FoundPointer.AddParameter(objectPosition));
                // Native of non native object ?
                if (BlockTypes.IsNative(blockType))
                {
                    // Reads the odb type id of the native objects
                    var odbTypeId = _fsi.ReadInt();
                    // Reads a boolean to know if object is null
                    var isNull = _fsi.ReadBoolean("Native object is null ?");
                    if (isNull)
                        return new NullNativeObjectInfo(odbTypeId);
                    // last parameter is false=> no need to read native object
                    // header, it has been done
                    return ReadNativeObjectInfo(odbTypeId, objectPosition, useCache, returnObjects, false);
                }
                if (BlockTypes.IsNonNative(blockType))
                    throw new OdbRuntimeException(NDatabaseError.ObjectReaderDirectCall);
                throw new OdbRuntimeException(
                    NDatabaseError.UnknownBlockType.AddParameter(blockType).AddParameter(_fsi.GetPosition() - 1));
            }
            finally
            {
                _currentDepth--;
            }
        }

        /// <param name="classInfo"> The class info of the objects to be returned </param>
        /// <param name="oid"> The Object id of the object to return data </param>
        /// <param name="position"> The position of the object to read </param>
        /// <param name="useCache"> To indicate if cache must be used </param>
        /// <param name="attributeNames"> The list of the attribute name for which we need to return a value, an attributename can contain relation like profile.name </param>
        /// <param name="relationAttributeNames"> The original names of attributes to read the values, an attributename can contain relation like profile.name </param>
        /// <param name="recursionLevel"> The recursion level of this call </param>
        /// <returns> A Map where keys are attributes names and values are the values of there attributes @ </returns>
        private AttributeValuesMap ReadObjectInfoValuesFromPosition(ClassInfo classInfo, OID oid, long position,
                                                                    bool useCache, IOdbList<string> attributeNames,
                                                                    IOdbList<string> relationAttributeNames,
                                                                    int recursionLevel)
        {
            _currentDepth++;
            // The resulting map
            var map = new AttributeValuesMap();
            // Protection against bad parameter value
            if (position > _fsi.GetLength())
            {
                throw new OdbRuntimeException(
                    NDatabaseError.InstancePositionOutOfFile.AddParameter(position).AddParameter(_fsi.GetLength()));
            }
            var cache = _storageEngine.GetSession(true).GetCache();
            // If object is already being read, simply return its cache - to avoid
            // stackOverflow for cyclic references
            // FIXME check this : should we use cache?
            // Go to the object position
            _fsi.SetReadPosition(position);
            // Read the block size of the object
            var blockSize = _fsi.ReadInt();
            // Read the block type of the object
            var blockType = _fsi.ReadByte();
            if (BlockTypes.IsNull(blockType) || BlockTypes.IsDeletedObject(blockType))
                return map;
            // Checks if what we are reading is only a pointer to the real block, if
            // it is the case, Throw an exception. Pointer are not used anymore
            if (BlockTypes.IsPointer(blockType))
                throw new CorruptedDatabaseException(NDatabaseError.FoundPointer.AddParameter(oid).AddParameter(position));
            try
            {
                // Read the header of the object, no need to cache when reading
                // object infos
                // For local mode, we need to use cache to get unconnected objects.
                // TestDelete.test14
                var objectInfoHeader = GetObjectInfoHeader(oid, position, true, cache);
                // Get the object id
                oid = objectInfoHeader.GetOid();
                // If class info is not defined, define it
                if (classInfo == null)
                    classInfo =
                        _storageEngine.GetSession(true).GetMetaModel().GetClassInfoFromId(
                            objectInfoHeader.GetClassInfoId());
                if (recursionLevel == 0)
                    map.SetObjectInfoHeader(objectInfoHeader);
                // If object is native, it can have attributes, just return the
                // empty
                // map
                if (BlockTypes.IsNative(blockType))
                    return map;
                var nbAttributes = attributeNames.Count;
                // The query contains a list of attribute to search
                // Loop on attribute to search
                for (var attributeIndex = 0; attributeIndex < nbAttributes; attributeIndex++)
                {
                    var attributeNameToSearch = attributeNames[attributeIndex];
                    var relationNameToSearch = relationAttributeNames[attributeIndex];
                    // If an attribute name has a ., it is a relation
                    var mustNavigate = attributeNameToSearch.IndexOf(".", StringComparison.Ordinal) != -1;
                    long attributeIdentification;
                    long attributePosition;
                    OID attributeOid = null;
                    ClassAttributeInfo cai;
                    if (mustNavigate)
                    {
                        // Get the relation name and the relation attribute name
                        // profile.name => profile = singleAttributeName, name =
                        // relationAttributeName
                        var firstDotIndex = attributeNameToSearch.IndexOf(".", StringComparison.Ordinal);
                        var relationAttributeName = attributeNameToSearch.Substring(firstDotIndex + 1);
                        var singleAttributeName = attributeNameToSearch.Substring(0, firstDotIndex);
                        var attributeId = classInfo.GetAttributeId(singleAttributeName);
                        if (attributeId == -1)
                        {
                            throw new OdbRuntimeException(
                                NDatabaseError.CriteriaQueryUnknownAttribute.AddParameter(attributeNameToSearch).
                                    AddParameter(classInfo.FullClassName));
                        }
                        cai = classInfo.GetAttributeInfoFromId(attributeId);
                        // Gets the identification (id or position from the object
                        // info) for the attribute with the id of the class
                        // attribute info
                        attributeIdentification = objectInfoHeader.GetAttributeIdentificationFromId(cai.GetId());
                        // When object is non native, then attribute identification
                        // is the oid of the object. It is stored as negative, so we
                        // must do *-1
                        if (!cai.IsNative())
                        {
                            // Relations can be null
                            if (attributeIdentification == StorageEngineConstant.NullObjectIdId)
                            {
                                map.Add(attributeNameToSearch, null);
                                continue;
                            }
                            attributeOid = OIDFactory.BuildObjectOID(-attributeIdentification);
                            attributePosition = GetObjectPositionFromItsOid(attributeOid, useCache, false);
                            IOdbList<string> list1 = new OdbList<string>(1);
                            list1.Add(relationAttributeName);
                            IOdbList<string> list2 = new OdbList<string>(1);
                            list2.Add(attributeNameToSearch);
                            map.PutAll(ReadObjectInfoValuesFromPosition(cai.GetClassInfo(), attributeOid,
                                                                        attributePosition, useCache, list1, list2,
                                                                        recursionLevel + 1));
                        }
                        else
                        {
                            throw new OdbRuntimeException(
                                NDatabaseError.CriteriaQueryUnknownAttribute.AddParameter(attributeNameToSearch).
                                    AddParameter(classInfo.FullClassName));
                        }
                    }
                    else
                    {
                        var attributeId = classInfo.GetAttributeId(attributeNameToSearch);
                        if (attributeId == -1)
                        {
                            throw new OdbRuntimeException(
                                NDatabaseError.CriteriaQueryUnknownAttribute.AddParameter(attributeNameToSearch).
                                    AddParameter(classInfo.FullClassName));
                        }
                        cai = classInfo.GetAttributeInfoFromId(attributeId);
                        // Gets the identification (id or position from the object
                        // info) for the attribute with the id of the class
                        // attribute info
                        attributeIdentification = objectInfoHeader.GetAttributeIdentificationFromId(cai.GetId());
                        // When object is non native, then attribute identification
                        // is the oid of the object. It is stored as negative, so we
                        // must do *-1
                        if (cai.IsNonNative())
                            attributeOid = OIDFactory.BuildObjectOID(-attributeIdentification);
                        // For non native object, the identification is the oid,
                        // which is stored as negative long
                        // TODO The attributeIdentification <0 clause should not be necessary
                        // But there is a case (found by Jeremias) where even for non
                        // native the attribute is a position and not an id! identification
                        if (cai.IsNonNative() && attributeIdentification < 0)
                            attributePosition = GetObjectPositionFromItsOid(attributeOid, useCache, false);
                        else
                            attributePosition = attributeIdentification;
                        if (attributePosition == StorageEngineConstant.DeletedObjectPosition ||
                            attributePosition == StorageEngineConstant.NullObjectPosition ||
                            attributePosition == StorageEngineConstant.FieldDoesNotExist)
                        {
                            // TODO is this correct?
                            continue;
                        }
                        _fsi.SetReadPosition(attributePosition);
                        object @object;
                        if (cai.IsNative())
                        {
                            var aoi = ReadNativeObjectInfo(cai.GetAttributeType().Id, attributePosition, useCache,
                                                           true, true);
                            @object = aoi.GetObject();
                            map.Add(relationNameToSearch, @object);
                        }
                        else
                        {
                            var nnoi = ReadNonNativeObjectInfoFromOid(cai.GetClassInfo(), attributeOid, true, false);
                            @object = nnoi.GetObject();
                            if (@object == null)
                            {
                            }
                            //object = instanceBuilder.buildOneInstance(nnoi);
                            map.Add(relationNameToSearch, nnoi.GetOid());
                        }
                    }
                }
                return map;
            }
            finally
            {
                _currentDepth--;
            }
        }

        private ObjectInfoHeader GetObjectInfoHeader(OID oid, long position, bool useCache, IOdbCache cache)
        {
            // first check if the object info pointers exist in the cache
            ObjectInfoHeader objectInfoHeader = null;
            if (useCache && oid != null)
                objectInfoHeader = cache.GetObjectInfoHeaderByOid(oid, false);
            if (objectInfoHeader == null)
            {
                // Here we read by position because it is possible to have the
                // oid == null. And it is faster by position than by oid
                objectInfoHeader = ReadObjectInfoHeaderFromPosition(oid, position, false);
                var oidWasNull = oid == null;
                oid = objectInfoHeader.GetOid();
                if (useCache)
                {
                    var needToUpdateCache = true;
                    if (oidWasNull)
                    {
                        // The oid was null, now we have it, check the cache again !
                        var cachedOih = cache.GetObjectInfoHeaderByOid(oid, false);
                        if (cachedOih != null)
                        {
                            // Then use the one from the cache
                            objectInfoHeader = cachedOih;
                            // In this case the cache is up to date , no need to
                            // update
                            needToUpdateCache = false;
                        }
                    }
                    if (needToUpdateCache)
                        cache.AddObjectInfoOfNonCommitedObject(objectInfoHeader);
                }
            }
            return objectInfoHeader;
        }

        /// <summary>
        ///   Read the header of a native attribute <pre>The header contains
        ///                                           - The block size = int
        ///                                           - The block type = byte
        ///                                           - The OdbType ID = int
        ///                                           - A boolean to indicate if object is nulls.</pre>
        /// </summary>
        /// <remarks>
        ///   Read the header of a native attribute <pre>The header contains
        ///                                           - The block size = int
        ///                                           - The block type = byte
        ///                                           - The OdbType ID = int
        ///                                           - A boolean to indicate if object is nulls.
        ///                                           This method reads all the bytes and then convert the byte array to the values</pre>
        /// </remarks>
        private NativeAttributeHeader ReadNativeAttributeHeader()
        {
            var nah = new NativeAttributeHeader();
            var size = OdbType.Integer.Size + OdbType.Byte.Size + OdbType.Integer.Size +
                       OdbType.Boolean.Size;
            var bytes = _fsi.ReadBytes(size);
            var blockSize = ByteArrayConverter.ByteArrayToInt(bytes);
            var blockType = bytes[4];
            var odbTypeId = ByteArrayConverter.ByteArrayToInt(bytes, 5);
            var isNull = ByteArrayConverter.ByteArrayToBoolean(bytes, 9);
            nah.SetBlockSize(blockSize);
            nah.SetBlockType(blockType);
            nah.SetOdbTypeId(odbTypeId);
            nah.SetNull(isNull);
            return nah;
        }

        /// <summary>
        ///   Reads a meta representation of a native object
        /// </summary>
        /// <param name="odbDeclaredTypeId"> The type of attribute declared in the ClassInfo. May be different from actual attribute type in caso of OID and OdbObjectId </param>
        /// <param name="position"> </param>
        /// <param name="useCache"> </param>
        /// <param name="returnObject"> </param>
        /// <param name="readHeader"> </param>
        /// <returns> The native object representation @ </returns>
        private AbstractObjectInfo ReadNativeObjectInfo(int odbDeclaredTypeId, long position, bool useCache,
                                                        bool returnObject, bool readHeader)
        {
            if (OdbConfiguration.IsLoggingEnabled())
            {
                var positionAsString = position.ToString();
                DLogger.Debug(DepthToSpaces() + "Reading native object of type " +
                              OdbType.GetNameFromId(odbDeclaredTypeId) + " at position " + positionAsString);
            }
            // The realType is initialized with the declared type
            var realTypeId = odbDeclaredTypeId;
            if (readHeader)
            {
                var nah = ReadNativeAttributeHeader();
                // since version 3 of ODB File Format, the native object header has
                // an info to indicate
                // if object is null!
                if (nah.IsNull())
                    return new NullNativeObjectInfo(odbDeclaredTypeId);
                realTypeId = nah.GetOdbTypeId();
            }
            if (OdbType.IsAtomicNative(realTypeId))
                return ReadAtomicNativeObjectInfo(position, realTypeId);
            if (OdbType.IsNull(realTypeId))
                return new NullNativeObjectInfo(realTypeId);
            if (OdbType.IsCollection(realTypeId))
                return ReadCollection(position, useCache, returnObject);
            if (OdbType.IsArray(realTypeId))
                return ReadArray(position, useCache, returnObject);
            if (OdbType.IsMap(realTypeId))
                return ReadMap(position, useCache, returnObject);
            if (OdbType.IsEnum(realTypeId))
                return ReadEnumObjectInfo(position, realTypeId);
            throw new OdbRuntimeException(NDatabaseError.NativeTypeNotSupported.AddParameter(realTypeId));
        }

        /// <summary>
        ///   Reads an enum object
        /// </summary>
        public EnumNativeObjectInfo ReadEnumObjectInfo(long position, int odbTypeId)
        {
            var enumClassInfoId = _fsi.ReadLong("EnumClassInfoId");
            var enumValue = _fsi.ReadString();
            var enumCi = _storageEngine.GetSession(true).GetMetaModel().GetClassInfoFromId(OIDFactory.BuildClassOID(enumClassInfoId));
            return new EnumNativeObjectInfo(enumCi, enumValue);
        }

        /// <summary>
        ///   Reads a collection from the database file <p /> <pre>This method do not returns the object but a collection of representation of the objects using AsbtractObjectInfo
        ///                                                     &lt;p/&gt;
        ///                                                     The conversion to a real Map object will be done by the buildInstance method</pre>
        /// </summary>
        private CollectionObjectInfo ReadCollection(long position, bool useCache, bool returnObjects)
        {
            var realCollectionClassName = _fsi.ReadString("Real collection class name");
            // read the size of the collection
            var collectionSize = _fsi.ReadInt("Collection size");

            ICollection<AbstractObjectInfo> c = new List<AbstractObjectInfo>(collectionSize);
            // build a n array to store all element positions
            var objectIdentifications = new long[collectionSize];
            for (var i = 0; i < collectionSize; i++)
            {
                var index = (i + 1).ToString();
                objectIdentifications[i] = _fsi.ReadLong("position of element " + index);
            }
            for (var i = 0; i < collectionSize; i++)
            {
                try
                {
                    var aoi = ReadObjectInfo(objectIdentifications[i], useCache, returnObjects);
                    if (!(aoi is NonNativeDeletedObjectInfo))
                        c.Add(aoi);
                }
                catch (Exception e)
                {
                    var positionAsString = position.ToString();
                    throw new OdbRuntimeException(
                        NDatabaseError.InternalError.AddParameter("in ObjectReader.readCollection - at position " +
                                                                 positionAsString), e);
                }
            }
            var coi = new CollectionObjectInfo(c);
            coi.SetRealCollectionClassName(realCollectionClassName);
            return coi;
        }

        /// <summary>
        ///   Reads an array from the database file
        /// </summary>
        /// <returns> The Collection or the array @ </returns>
        private ArrayObjectInfo ReadArray(long position, bool useCache, bool returnObjects)
        {
            var realArrayComponentClassName = _fsi.ReadString("real array class name");
            var subTypeId = OdbType.GetFromName(realArrayComponentClassName);
            var componentIsNative = subTypeId.IsNative();
            // read the size of the array
            var arraySize = _fsi.ReadInt();
            if (OdbConfiguration.IsLoggingEnabled())
            {
                var size = arraySize.ToString();
                DLogger.Debug(DepthToSpaces() + "reading an array of " + realArrayComponentClassName + " with " +
                              size + " elements");
            }
            // Class clazz = ODBClassPool.getClass(realArrayClassName);
            // Object array = Array.newInstance(clazz, arraySize);
            var array = new object[arraySize];
            // build a n array to store all element positions
            var objectIdentifications = new long[arraySize];
            for (var i = 0; i < arraySize; i++)
                objectIdentifications[i] = _fsi.ReadLong();
            for (var i = 0; i < arraySize; i++)
            {
                try
                {
                    if (objectIdentifications[i] != StorageEngineConstant.NullObjectIdId)
                    {
                        object o = ReadObjectInfo(objectIdentifications[i], useCache, returnObjects);
                        if (!(o is NonNativeDeletedObjectInfo))
                        {
                            array.SetValue(o, i);
                        }
                    }
                    else
                    {
                        if (componentIsNative)
                        {
                            array.SetValue(NullNativeObjectInfo.GetInstance(), i);
                        }
                        else
                        {
                            array.SetValue(new NonNativeNullObjectInfo(), i);
                        }
                    }
                }
                catch (Exception e)
                {
                    var positionAsString = position.ToString();
                    throw new OdbRuntimeException(
                        NDatabaseError.InternalError.AddParameter("in ObjectReader.readArray - at position " + positionAsString),e);
                }
            }
            var aoi = new ArrayObjectInfo(array);
            aoi.SetRealArrayComponentClassName(realArrayComponentClassName);
            aoi.SetComponentTypeId(subTypeId.Id);
            return aoi;
        }

        /// <summary>
        ///   Reads a map from the database file <p /> <pre>WARNING : this method returns a collection representation of the map
        ///                                              &lt;p/&gt;
        ///                                              Firts it does not return the objects but its meta information using AbstractObjectInfo
        ///                                              &lt;p/&gt;
        ///                                              So for example, the map [1=olivier,2=chico]
        ///                                              will be returns as a collection : [1,olivier,2,chico]
        ///                                              and each element of the collection is an abstractObjectInfo (NativeObjectInfo or NonNativeObjectInfo)
        ///                                              &lt;p/&gt;
        ///                                              The conversion to a real Map object will be done by the buildInstance method</pre>
        /// </summary>
        private MapObjectInfo ReadMap(long position, bool useCache, bool returnObjects)
        {
            // Reads the real map class
            var realMapClassName = _fsi.ReadString();
            // read the size of the map
            var mapSize = _fsi.ReadInt();
            IDictionary<AbstractObjectInfo, AbstractObjectInfo> map =
                new OdbHashMap<AbstractObjectInfo, AbstractObjectInfo>();

            // build a n array to store all element positions
            var objectIdentifications = new long[mapSize * 2];
            for (var i = 0; i < mapSize * 2; i++)
                objectIdentifications[i] = _fsi.ReadLong();
            for (var i = 0; i < mapSize; i++)
            {
                try
                {
                    var aoiKey = ReadObjectInfo(objectIdentifications[2 * i], useCache, returnObjects);
                    var aoiValue = ReadObjectInfo(objectIdentifications[2 * i + 1], useCache, returnObjects);
                    if (!aoiKey.IsDeletedObject() && !aoiValue.IsDeletedObject())
                        map.Add(aoiKey, aoiValue);
                }
                catch (Exception e)
                {
                    var positionAsString = position.ToString();
                    throw new OdbRuntimeException(
                        NDatabaseError.InternalError.AddParameter("in ObjectReader.readMap - at position " + positionAsString), e);
                }
            }
            return new MapObjectInfo(map, realMapClassName);
        }

        /// <summary>
        ///   Returns the name of the class of an object from its position
        /// </summary>
        /// <param name="objectPosition"> </param>
        /// <returns> The object class name @ </returns>
        public string GetObjectTypeFromPosition(long objectPosition)
        {
            var blockPosition = objectPosition + StorageEngineConstant.ObjectOffsetBlockType;
            _fsi.SetReadPosition(blockPosition);
            var blockType = _fsi.ReadByte();
            if (BlockTypes.IsNull(blockType))
            {
                var classIdForNullObject = OIDFactory.BuildClassOID(_fsi.ReadLong("class id of object"));
                return "null " +
                       _storageEngine.GetSession(true).GetMetaModel().GetClassInfoFromId(classIdForNullObject).FullClassName;
            }
            var classIdPosition = objectPosition + StorageEngineConstant.ObjectOffsetClassInfoId;
            _fsi.SetReadPosition(classIdPosition);
            var classId = OIDFactory.BuildClassOID(_fsi.ReadLong("class id of object"));
            return _storageEngine.GetSession(true).GetMetaModel().GetClassInfoFromId(classId).FullClassName;
        }

        /// <param name="blockNumberToFind"> </param>
        /// <returns> The block position @ </returns>
        private long GetIdBlockPositionFromNumber(long blockNumberToFind)
        {
            // first check if it exist in cache
            long lposition;

            _blockPositions.TryGetValue(blockNumberToFind, out lposition);
            if (lposition != 0)
                return lposition;
            long currentBlockPosition = StorageEngineConstant.DatabaseHeaderFirstIdBlockPosition;
            while (currentBlockPosition != -1)
            {
                // Gets the next block position
                _fsi.SetReadPosition(currentBlockPosition + StorageEngineConstant.BlockIdOffsetForNextBlock);
                var nextBlockPosition = _fsi.ReadLong();
                // Reads the block number
                var blockNumber = _fsi.ReadInt();
                if (blockNumber == blockNumberToFind)
                {
                    // Put result in map
                    _blockPositions.Add(blockNumberToFind, currentBlockPosition);
                    return currentBlockPosition;
                }
                currentBlockPosition = nextBlockPosition;
            }
            throw new CorruptedDatabaseException(NDatabaseError.BlockNumberDoesExist.AddParameter(blockNumberToFind));
        }

        private static long GetIdBlockNumberOfOid(OID oid)
        {
            long number;
            var objectId = oid.ObjectId;
            if (objectId % OdbConfiguration.GetNbIdsPerBlock() == 0)
                number = objectId / OdbConfiguration.GetNbIdsPerBlock();
            else
                number = objectId / OdbConfiguration.GetNbIdsPerBlock() + 1;
            return number;
        }
    }
}
