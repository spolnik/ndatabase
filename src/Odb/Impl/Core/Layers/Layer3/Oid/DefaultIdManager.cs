using NDatabase.Odb.Core;
using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Transaction;
using NDatabase.Odb.Impl.Core.Layers.Layer3.Block;
using NDatabase.Odb.Impl.Core.Layers.Layer3.Engine;
using NDatabase.Tool;

namespace NDatabase.Odb.Impl.Core.Layers.Layer3.Oid
{
    /// <summary>
    ///   Class to manage the ids of all the objects of the database.
    /// </summary>
    /// <remarks>
    ///   Class to manage the ids of all the objects of the database.
    /// </remarks>
    /// <author>osmadja</author>
    public class DefaultIdManager : IIdManager
    {
        private const int IdBufferSize = 10;
        private static readonly string LogId = "IdManager";

        private readonly ICoreProvider provider;

        private int currentBlockIdNumber;
        private long currentBlockIdPosition;
        private int lastIdIndex;
        protected long[] lastIdPositions;

        /// <summary>
        ///   Contains the last ids: id value,id position, id value, id position=&gt; the array is created with twice the size
        /// </summary>
        protected OID[] lastIds;

        public OID maxId;
        public OID nextId;

        protected IObjectReader objectReader;
        protected IObjectWriter objectWriter;

        protected ISession session;

        /// <param name="objectWriter"> The object writer </param>
        /// <param name="objectReader"> The object reader </param>
        /// <param name="currentBlockIdPosition"> The position of the current block </param>
        /// <param name="currentBlockIdNumber"> The number of the current block </param>
        /// <param name="currentMaxId"> Maximum Database id </param>
        public DefaultIdManager(IObjectWriter objectWriter
                                , IObjectReader objectReader, long currentBlockIdPosition
                                , int currentBlockIdNumber, OID currentMaxId)
        {
            provider = OdbConfiguration.GetCoreProvider();
            this.objectWriter = objectWriter;
            this.objectReader = objectReader;
            session = objectWriter.GetSession();
            this.currentBlockIdPosition = currentBlockIdPosition;
            this.currentBlockIdNumber = currentBlockIdNumber;
            maxId = provider.GetObjectOID((long) currentBlockIdNumber*OdbConfiguration
                                                                          .GetNbIdsPerBlock(), 0);
            nextId = provider.GetObjectOID(currentMaxId.ObjectId + 1, 0);
            lastIds = new OID[IdBufferSize];
            for (int i = 0; i < IdBufferSize; i++)
            {
                lastIds[i] = StorageEngineConstant.NullObjectId;
            }
            lastIdPositions = new long[IdBufferSize];
            lastIdIndex = 0;
        }

        #region IIdManager Members

        /// <summary>
        ///   To check if the id block must shift: that a new id block must be created
        /// </summary>
        /// <returns> a boolean value to check if block of id is full </returns>
        public virtual bool MustShift()
        {
            lock (this)
            {
                return nextId.CompareTo(maxId) > 0;
            }
        }

        public virtual OID GetNextObjectId(long objectPosition)
        {
            lock (this)
            {
                return GetNextId(objectPosition, IdTypes.Object,
                                 IDStatus.Active, "getNextObjectId");
            }
        }

        public virtual OID GetNextClassId(long objectPosition)
        {
            lock (this)
            {
                return GetNextId(objectPosition, IdTypes.Class, IDStatus
                                                                    .Active, "getNextClassId");
            }
        }

        public virtual void UpdateObjectPositionForOid(OID oid, long objectPosition
                                                       , bool writeInTransaction)
        {
            //TODO Remove comments here
            // Id may be negative to differ from positions
            //if(id<0){
            //	id = -id;
            //}
            long idPosition = GetIdPosition(oid);
            objectWriter.UpdateObjectPositionForObjectOIDWithPosition(idPosition, objectPosition
                                                                      , writeInTransaction);
            if (OdbConfiguration.IsDebugEnabled(LogId))
            {
                DLogger.Debug("IDManager : Updating id " + oid + " with position "
                              + objectPosition);
            }
        }

        public virtual void UpdateClassPositionForId(OID classId, long objectPosition
                                                     , bool writeInTransaction)
        {
            // TODO Remove comments here
            // Id may be negative to differ from positions
            //if(classId<0){
            //    classId = -classId;
            //}
            long idPosition = GetIdPosition(classId);
            objectWriter.UpdateClassPositionForClassOIDWithPosition(idPosition, objectPosition
                                                                    , writeInTransaction);
            if (OdbConfiguration.IsDebugEnabled(LogId))
            {
                DLogger.Debug("Updating id " + classId + " with position " + objectPosition
                    );
            }
        }

        public virtual void UpdateIdStatus(OID id, byte newStatus)
        {
            long idPosition = GetIdPosition(id);
            objectWriter.UpdateStatusForIdWithPosition(idPosition, newStatus, true);
        }

        public virtual OID ConsultNextOid()
        {
            lock (this)
            {
                return nextId;
            }
        }

        public virtual void ReserveIds(long nbIds)
        {
            OID id = null;
            while (nextId.ObjectId < nbIds + 1)
            {
                id = GetNextId(-1, IdTypes.Unknown, IDStatus
                                                        .Unknown, "reserving id");
                if (OdbConfiguration.IsDebugEnabled(LogId))
                {
                    DLogger.Debug("reserving id " + id);
                }
            }
            return;
        }

        public virtual long GetObjectPositionWithOid(OID oid, bool useCache)
        {
            return objectReader.GetObjectPositionFromItsOid(oid, useCache, true);
        }

        public virtual void Clear()
        {
            objectReader = null;
            objectWriter = null;
            session = null;
            lastIdPositions = null;
            lastIds = null;
        }

        #endregion

        /// <summary>
        ///   Gets an id for an object (instance)
        /// </summary>
        /// <param name="objectPosition"> the object position (instance) </param>
        /// <param name="idType"> The type id : object,class, unknown </param>
        /// <param name="label"> A label for debug </param>
        /// <returns> The id </returns>
        internal virtual OID GetNextId(long objectPosition, byte idType, byte
                                                                             idStatus, string label)
        {
            lock (this)
            {
                if (OdbConfiguration.IsDebugEnabled(LogId))
                {
                    DLogger.Debug("  Start of " + label + " for object with position "
                                  + objectPosition);
                }
                if (MustShift())
                {
                    ShiftBlock();
                }
                // Keep the current id
                OID currentNextId = nextId;
                if (idType == IdTypes.Class)
                {
                    // If its a class, build a class OID instead.
                    currentNextId = provider.GetClassOID(currentNextId.ObjectId);
                }
                // Compute the new index to be used to store id and its position in the lastIds and lastIdPositions array
                int currentIndex = (lastIdIndex + 1)%IdBufferSize;
                // Stores the id
                lastIds[currentIndex] = currentNextId;
                // really associate id to the object position
                long idPosition = AssociateIdToObject(idType, idStatus, objectPosition);
                // Store the id position
                lastIdPositions[currentIndex] = idPosition;
                if (OdbConfiguration.IsDebugEnabled(LogId))
                {
                    DLogger.Debug("  End of " + label + " for object with position " +
                                  idPosition + " : returning " + currentNextId);
                }
                // Update the id buffer index
                lastIdIndex = currentIndex;
                return currentNextId;
            }
        }

        private long GetIdPosition(OID oid)
        {
            // first check if it is the last
            if (lastIds[lastIdIndex] != null && lastIds[lastIdIndex].Equals(oid))
            {
                return lastIdPositions[(lastIdIndex)];
            }
            for (int i = 0; i < IdBufferSize; i++)
            {
                if (lastIds[i] != null && lastIds[i].Equals(oid))
                {
                    return lastIdPositions[i];
                }
            }
            // object id is not is cache
            return objectReader.ReadOidPosition(oid);
        }

        private long AssociateIdToObject(byte idType, byte idStatus, long objectPosition)
        {
            long idPosition = objectWriter.AssociateIdToObject(idType, idStatus, currentBlockIdPosition
                                                               , nextId, objectPosition, false);
            nextId = provider.GetObjectOID(nextId.ObjectId + 1, 0);
            return idPosition;
        }

        private void ShiftBlock()
        {
            long currentBlockPosition = currentBlockIdPosition;
            // the block has reached the end, , must create a new id block
            long newBlockPosition = CreateNewBlock();
            // Mark the current block as full
            MarkBlockAsFull(currentBlockPosition, newBlockPosition);
            currentBlockIdNumber++;
            currentBlockIdPosition = newBlockPosition;
            maxId = provider.GetObjectOID((long) currentBlockIdNumber*OdbConfiguration
                                                                          .GetNbIdsPerBlock(), 0);
        }

        private void MarkBlockAsFull(long currentBlockIdPosition, long nextBlockPosition)
        {
            objectWriter.MarkIdBlockAsFull(currentBlockIdPosition, nextBlockPosition, false);
        }

        private long CreateNewBlock()
        {
            long position = objectWriter.WriteIdBlock(-1, OdbConfiguration.GetIdBlockSize
                                                              (), BlockStatus.BlockNotFull, currentBlockIdNumber
                                                                                            + 1, currentBlockIdPosition,
                                                      false);
            return position;
        }

        protected virtual ISession GetSession()
        {
            return session;
        }
    }
}