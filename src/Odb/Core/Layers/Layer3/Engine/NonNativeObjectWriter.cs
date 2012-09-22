using System;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer2.Meta.Compare;
using NDatabase.Odb.Core.Layers.Layer3.Block;
using NDatabase.Odb.Core.Transaction;
using NDatabase.Odb.Core.Trigger;
using NDatabase.Tool;
using NDatabase.Tool.Wrappers;

namespace NDatabase.Odb.Core.Layers.Layer3.Engine
{
    public sealed class NonNativeObjectWriter : INonNativeObjectWriter
    {
        private const string LogId = "NonNativeObjectWriter";
        private const string LogIdDebug = "NonNativeObjectWriter.debug";

        private readonly IObjectWriter _objectWriter;
        private readonly ISession _session;
        private readonly IStorageEngine _storageEngine;
        private readonly IObjectInfoComparator _comparator;
        private ITriggerManager _triggerManager;
        private IObjectReader _objectReader;

        public NonNativeObjectWriter(IObjectWriter objectWriter, IStorageEngine storageEngine, IObjectInfoComparator comparator)
        {
            _objectWriter = objectWriter;
            _session = storageEngine.GetSession(true);
            _objectReader = storageEngine.GetObjectReader();
            _storageEngine = storageEngine;
            _comparator = comparator;
        }

        /// <param name="oid"> The Oid of the object to be inserted </param>
        /// <param name="nnoi"> The object meta representation The object to be inserted in the database </param>
        /// <param name="isNewObject"> To indicate if object is new </param>
        /// <returns> The position of the inserted object </returns>
        public OID InsertNonNativeObject(OID oid, NonNativeObjectInfo nnoi, bool isNewObject)
        {
            var ci = nnoi.GetClassInfo();
            var @object = nnoi.GetObject();
            // First check if object is already being inserted
            // This method returns -1 if object is not being inserted
            var cachedOid = _session.GetCache().IdOfInsertingObject(@object);
            if (cachedOid != null)
                return cachedOid;
            // Then checks if the class of this object already exist in the
            // meta model
            ci = _objectWriter.AddClass(ci, true);
            // Resets the ClassInfo in the objectInfo to be sure it contains all
            // updated class info data
            nnoi.SetClassInfo(ci);
            // Mark this object as being inserted. To manage cyclic relations
            // The oid may be equal to -1
            // Later in the process the cache will be updated with the right oid
            _session.GetCache().StartInsertingObjectWithOid(@object, oid, nnoi);
            // false : do not write data in transaction. Data are always written
            // directly to disk. Pointers are written in transaction
            var newOid = WriteNonNativeObjectInfo(oid, nnoi, -1, false, isNewObject);
            if (newOid != StorageEngineConstant.NullObjectId)
                _session.GetCache().AddObject(newOid, @object, nnoi.GetHeader());
            return newOid;
        }

        public OID WriteNonNativeObjectInfo(OID existingOid, NonNativeObjectInfo objectInfo, long position,
                                                    bool writeDataInTransaction, bool isNewObject)
        {
            var lsession = _session;
            var cache = lsession.GetCache();
            var hasObject = objectInfo.GetObject() != null;

            // Checks if object is null,for null objects,there is nothing to do
            if (objectInfo.IsNull())
                return StorageEngineConstant.NullObjectId;

            var metaModel = lsession.GetMetaModel();
            // first checks if the class of this object already exist in the
            // metamodel
            if (!metaModel.ExistClass(objectInfo.GetClassInfo().FullClassName))
                _objectWriter.AddClass(objectInfo.GetClassInfo(), true);

            // if position is -1, gets the position where to write the object
            if (position == -1)
            {
                // Write at the end of the file
                position = _objectWriter.FileSystemProcessor.FileSystemInterface.GetAvailablePosition();
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
                if (_objectWriter.GetIdManager().MustShift())
                {
                    oid = _objectWriter.GetIdManager().GetNextObjectId(position);
                    // The id manager wrote in the file so the position for the
                    // object must be re-computed
                    position = _objectWriter.FileSystemProcessor.FileSystemInterface.GetAvailablePosition();
                    // The oid must be associated to this new position - id
                    // operations are always out of transaction
                    // in this case, the update is done out of the transaction as a
                    // rollback won t need to
                    // undo this. We are just creating the id
                    // => third parameter(write in transaction) = false
                    _objectWriter.GetIdManager().UpdateObjectPositionForOid(oid, position, false);
                }
                else
                    oid = _objectWriter.GetIdManager().GetNextObjectId(position);
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
                _objectWriter.GetIdManager().UpdateObjectPositionForOid(oid, position, true);
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
                        CacheFactory.GetCrossSessionCache(_storageEngine.GetBaseIdentification().Id);
                    crossSessionCache.AddObject(objectInfo.GetObject(), oid);
                }
            }

            objectInfo.SetOid(oid);

            #region Logging

            if (OdbConfiguration.IsDebugEnabled(LogId))
            {
                DLogger.Debug(string.Format("Start Writing non native object of type {0} at {1} , oid = {2} : {3}",
                                            objectInfo.GetClassInfo().FullClassName, position, oid, objectInfo));
            }

            #endregion

            if (objectInfo.GetClassInfo() == null || objectInfo.GetClassInfo().ClassInfoId == null)
            {
                if (objectInfo.GetClassInfo() != null)
                {
                    var clinfo =
                        _storageEngine.GetSession(true).GetMetaModel().GetClassInfo(
                            objectInfo.GetClassInfo().FullClassName, true);
                    objectInfo.SetClassInfo(clinfo);
                }
                else
                    throw new OdbRuntimeException(NDatabaseError.UndefinedClassInfo.AddParameter(objectInfo.ToString()));
            }

            // updates the meta model - If class already exist, it returns the
            // metamodel class, which contains
            // a bit more informations
            var classInfo = _objectWriter.AddClass(objectInfo.GetClassInfo(), true);
            objectInfo.SetClassInfo(classInfo);
            // 
            if (isNewObject)
                _objectWriter.ManageNewObjectPointers(objectInfo, classInfo);

            
            _objectWriter.FileSystemProcessor.FileSystemInterface.SetWritePosition(position, writeDataInTransaction);
            objectInfo.SetPosition(position);
            var nbAttributes = objectInfo.GetClassInfo().Attributes.Count;
            // compute the size of the array of byte needed till the attibute
            // positions
            // BlockSize + Block Type + OID  + ClassOid + PrevOid + NextOid + CreatDate + UpdateDate + objectVersion + NbAttributes
            // Int       + Byte        + Long + Long     + Long    + Long    + Long      + Long       + int          + int
            // 6 Longs + 3Ints + Byte
            var tsize = 6 * OdbType.SizeOfLong + 3 * OdbType.SizeOfInt + 1 * OdbType.SizeOfByte;
            var bytes = new byte[tsize];
            // Block size
            IntToByteArray(0, bytes, 0);
            // Block type
            bytes[4] = BlockTypes.BlockTypeNonNativeObject;
            
            // The object id
            EncodeOid(oid, bytes, 5);
            
            // Class info id
            LongToByteArray(classInfo.ClassInfoId.ObjectId, bytes, 13);
            
            // previous instance
            EncodeOid(objectInfo.GetPreviousObjectOID(), bytes, 21);
            
            // next instance
            EncodeOid(objectInfo.GetNextObjectOID(), bytes, 29);
            
            // creation date, for update operation must be the original one
            LongToByteArray(objectInfo.GetHeader().GetCreationDate(), bytes, 37);
            
            LongToByteArray(OdbTime.GetCurrentTimeInTicks(), bytes, 45);
           
            IntToByteArray(objectInfo.GetHeader().GetObjectVersion(), bytes, 53);
           
            // now write the number of attributes and the position of all
            // attributes, we do not know them yet, so write 00 but at the end of the write operation
            // These positions will be updated The positions that is going to be written are 'int' representing
            // the offset position of the attribute first write the number of attributes
            IntToByteArray(nbAttributes, bytes, 57);

            // Then write the array of bytes
            _objectWriter.FileSystemProcessor.FileSystemInterface.WriteBytes(bytes, writeDataInTransaction, "NonNativeObjectInfoHeader");
            // Store the position
            var attributePositionStart = _objectWriter.FileSystemProcessor.FileSystemInterface.GetPosition();
            var attributeSize = OdbType.SizeOfInt + OdbType.SizeOfLong;
            var abytes = new byte[nbAttributes * (attributeSize)];
            // here, just write an empty (0) array, as real values will be set at
            // the end
            _objectWriter.FileSystemProcessor.FileSystemInterface.WriteBytes(abytes, writeDataInTransaction, "Empty Attributes");
            var attributesIdentification = new long[nbAttributes];
            var attributeIds = new int[nbAttributes];
            // Puts the object info in the cache
            // storageEngine.getSession().getCache().addObject(position,
            // aoi.getObject(), objectInfo.getHeader());

            var maxWritePosition = _objectWriter.FileSystemProcessor.FileSystemInterface.GetPosition();
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
                        aoi2 = new NullNativeObjectInfo(classAttributeInfo.GetAttributeType().Id);
                    else
                        aoi2 = new NonNativeNullObjectInfo(classAttributeInfo.GetClassInfo());
                }
                if (aoi2.IsNative())
                {
                    var nativeAttributePosition = _objectWriter.InternalStoreObject((NativeObjectInfo)aoi2);
                    // For native objects , odb stores their position
                    attributesIdentification[i] = nativeAttributePosition;
                }
                else
                {
                    OID nonNativeAttributeOid;
                    if (aoi2.IsObjectReference())
                    {
                        var or = (ObjectReference)aoi2;
                        nonNativeAttributeOid = or.GetOid();
                    }
                    else
                        nonNativeAttributeOid = _objectWriter.StoreObject(null, (NonNativeObjectInfo)aoi2);
                    // For non native objects , odb stores its oid as a negative
                    // number!!u
                    if (nonNativeAttributeOid != null)
                        attributesIdentification[i] = -nonNativeAttributeOid.ObjectId;
                    else
                        attributesIdentification[i] = StorageEngineConstant.NullObjectIdId;
                }
                var p = _objectWriter.FileSystemProcessor.FileSystemInterface.GetPosition();
                if (p > maxWritePosition)
                    maxWritePosition = p;
            }
            // Updates attributes identification in the object info header
            objectInfo.GetHeader().SetAttributesIdentification(attributesIdentification);
            objectInfo.GetHeader().SetAttributesIds(attributeIds);
            var positionAfterWrite = maxWritePosition;
            // Now writes back the attribute positions
            _objectWriter.FileSystemProcessor.FileSystemInterface.SetWritePosition(attributePositionStart, writeDataInTransaction);
            abytes = new byte[attributesIdentification.Length * (attributeSize)];
            for (var i = 0; i < attributesIdentification.Length; i++)
            {
                IntToByteArray(attributeIds[i], abytes, i * attributeSize);
                LongToByteArray(attributesIdentification[i], abytes,
                                i*(attributeSize) + OdbType.SizeOfInt);
                // fsi.writeInt(attributeIds[i], writeDataInTransaction, "attr id");
                // fsi.writeLong(attributesIdentification[i],
                // writeDataInTransaction, "att real pos",
                // WriteAction.DATA_WRITE_ACTION);
                // if (classInfo.getAttributeInfo(i).isNonNative() &&
                // attributesIdentification[i] > 0) {
                if (objectInfo.GetAttributeValueFromId(attributeIds[i]).IsNonNativeObject() &&
                    attributesIdentification[i] > 0)
                {
                    throw new OdbRuntimeException(
                        NDatabaseError.NonNativeAttributeStoredByPositionInsteadOfOid.AddParameter(
                            classInfo.GetAttributeInfo(i).GetName()).AddParameter(classInfo.FullClassName).
                            AddParameter(attributesIdentification[i]));
                }
            }
            _objectWriter.FileSystemProcessor.FileSystemInterface.WriteBytes(abytes, writeDataInTransaction, "Filled Attributes");
            _objectWriter.FileSystemProcessor.FileSystemInterface.SetWritePosition(positionAfterWrite, writeDataInTransaction);
            var blockSize = (int)(positionAfterWrite - position);
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
                    DLogger.Debug(" - current buffer : " + _objectWriter.FileSystemProcessor.FileSystemInterface.GetIo());
            }
            // Only insert in index for new objects
            if (isNewObject)
            {
                // insert object id in indexes, if exist
                _objectWriter.ManageIndexesForInsert(oid, objectInfo);
                var value = hasObject
                                ? objectInfo.GetObject()
                                : objectInfo;

                _triggerManager.ManageInsertTriggerAfter(objectInfo.GetClassInfo().FullClassName, value, oid);
            }

            return oid;
        }

        public void SetTriggerManager(ITriggerManager triggerManager)
        {
            _triggerManager = triggerManager;
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
        public OID UpdateNonNativeObjectInfo(NonNativeObjectInfo nnoi, bool forceUpdate)
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
            var nbConnectedObjects = nnoi.GetClassInfo().CommitedZoneInfo.GetNbObjects();
            var nbNonConnectedObjects = nnoi.GetClassInfo().UncommittedZoneInfo.GetNbObjects();
            var objectHasChanged = false;
            try
            {
                var lsession = _session;
                var positionBeforeWrite = _objectWriter.FileSystemProcessor.FileSystemInterface.GetPosition();
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
                _storageEngine.GetTriggerManager().ManageUpdateTriggerBefore(nnoi.GetClassInfo().FullClassName,
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
                        _objectWriter.FileSystemProcessor.FileSystemInterface.SetWritePosition(positionBeforeWrite, true);
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
                ObjectWriter.NbNormalUpdates++;
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
                _objectWriter.MarkAsDeleted(currentPosition, oid, objectIsInConnectedZone);
                // Creates the new object
                oid = InsertNonNativeObject(oid, nnoi, false);
                // This position after write must be call just after the insert!!
                var positionAfterWrite = _objectWriter.FileSystemProcessor.FileSystemInterface.GetPosition();
                if (hasObject)
                {
                    // update cache
                    cache.AddObject(oid, @object, nnoi.GetHeader());
                }
                
                _objectWriter.FileSystemProcessor.FileSystemInterface.SetWritePosition(positionAfterWrite, true);
                var nbConnectedObjectsAfter = nnoi.GetClassInfo().CommitedZoneInfo.GetNbObjects();
                var nbNonConnectedObjectsAfter = nnoi.GetClassInfo().UncommittedZoneInfo.GetNbObjects();
                if (nbConnectedObjectsAfter != nbConnectedObjects || nbNonConnectedObjectsAfter != nbNonConnectedObjects)
                {
                    // TODO check this
                    // throw new
                    // ODBRuntimeException(Error.INTERNAL_ERROR.addParameter("Error
                    // in nb connected/unconnected counter"));
                }

                return oid;
            }
            catch (Exception e)
            {
                message = string.Format("Error updating object {0} : {1}", nnoi, e);
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
                    _storageEngine.GetTriggerManager().ManageUpdateTriggerAfter(
                        nnoi.GetClassInfo().FullClassName, oldMetaRepresentation, hasObject ? @object : nnoi, oid);
                }
                if (OdbConfiguration.IsDebugEnabled(LogId))
                {
                    DLogger.Debug("end updating object with oid=" + oid + " at pos " +
                                  nnoi.GetPosition() + " => " + nnoi);
                }
            }
        }

        public void AfterInit()
        {
            _objectReader = _storageEngine.GetObjectReader();
        }

        private static void ManageIndexesForUpdate(OID oid, NonNativeObjectInfo nnoi, NonNativeObjectInfo oldMetaRepresentation)
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
                    if (index.BTree.GetSize() != nnoi.GetClassInfo().NumberOfObjects)
                    {
                        throw new OdbRuntimeException(
                            NDatabaseError.BtreeSizeDiffersFromClassElementNumber.AddParameter(index.BTree.GetSize())
                                .AddParameter(nnoi.GetClassInfo().NumberOfObjects));
                    }
                }
            }
        }

        public void EncodeOid(OID oid, byte[] bytes, int offset)
        {
            if (oid == null)
                LongToByteArray(-1, bytes, offset);
            else
            {
                // fsi.writeLong(-1, writeInTransaction, label, writeAction);
                LongToByteArray(oid.ObjectId, bytes, offset);
            }
        }

        private void WriteBlockSizeAt(long writePosition, int blockSize, bool writeInTransaction, object @object)
        {
            if (blockSize < 0)
            {
                throw new OdbRuntimeException(
                    NDatabaseError.NegativeBlockSize.AddParameter(writePosition).AddParameter(blockSize).AddParameter(
                        @object.ToString()));
            }
            var currentPosition = _objectWriter.FileSystemProcessor.FileSystemInterface.GetPosition();
            _objectWriter.FileSystemProcessor.FileSystemInterface.SetWritePosition(writePosition, writeInTransaction);
            _objectWriter.FileSystemProcessor.FileSystemInterface.WriteInt(blockSize, writeInTransaction, "block size");
            // goes back where we were
            _objectWriter.FileSystemProcessor.FileSystemInterface.SetWritePosition(currentPosition, writeInTransaction);
        }

        private static void BooleanToByteArray(bool b, byte[] arrayWhereToWrite, int offset)
        {
            const byte byteForTrue = 1;
            const byte byteForFalse = 0;

            arrayWhereToWrite[offset] = b
                                            ? byteForTrue
                                            : byteForFalse;
        }

        private static void LongToByteArray(long l, byte[] arrayWhereToWrite, int offset)
        {
            int i;
            var bytes = BitConverter.GetBytes(l);
            for (i = 0; i < 8; i++)
                arrayWhereToWrite[offset + i] = bytes[i];
        }

        private static void IntToByteArray(int l, byte[] arrayWhereToWrite, int offset)
        {
            int i;
            var bytes = BitConverter.GetBytes(l);

            for (i = 0; i < 4; i++)
                arrayWhereToWrite[offset + i] = bytes[i];
        }
    }
}
