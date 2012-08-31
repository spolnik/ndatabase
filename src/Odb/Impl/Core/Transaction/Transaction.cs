using System.Collections;
using System.Collections.Generic;
using System.Text;
using NDatabase.Odb.Core;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Layers.Layer3.Engine;
using NDatabase.Odb.Core.Layers.Layer3.IO;
using NDatabase.Odb.Core.Transaction;
using NDatabase.Tool;
using NDatabase.Tool.Wrappers;
using NDatabase.Tool.Wrappers.List;
using NDatabase.Tool.Wrappers.Map;

namespace NDatabase.Odb.Impl.Core.Transaction
{
    /// <summary>
    ///   <pre>The transaction class is used to guarantee ACID behavior.</pre>
    /// </summary>
    /// <remarks>
    ///   <pre>The transaction class is used to guarantee ACID behavior. It keep tracks of all session
    ///     operations. It uses the WriteAction class to store all changes that can not be written to the file
    ///     before the commit.
    ///     The transaction is held by The Session class and manage commits and rollbacks.
    ///     All WriteActions are written in a transaction file to be sure to be able to commit and in case
    ///     of very big transaction where all WriteActions can not be stored in memory.</pre>
    /// </remarks>
    /// <author>osmadja</author>
    public sealed class OdbTransaction : ITransaction
    {
        /// <summary>
        ///   the log module name
        /// </summary>
        public static readonly string LogId = "Transaction";

        /// <summary>
        ///   To indicate if transaction is read only
        /// </summary>
        private readonly bool _readOnlyMode;

        /// <summary>
        ///   When this flag is set,the transaction will not be deleted, but will be flagged as executed
        /// </summary>
        private bool _archiveLog;

        /// <summary>
        ///   The transaction creation time
        /// </summary>
        private long _creationDateTime;

        /// <summary>
        ///   The same write action is reused for successive writes
        /// </summary>
        private IWriteAction _currentWriteAction;

        /// <summary>
        ///   The position of the next write for WriteAction
        /// </summary>
        private long _currentWritePositionInWa;

        /// <summary>
        ///   A file interface to the transaction file - used to read/write the file
        /// </summary>
        private IFileSystemInterface _fsi;

        /// <summary>
        ///   A file interface to the engine main file
        /// </summary>
        private IFileSystemInterface _fsiToApplyWriteActions;

        /// <summary>
        ///   To indicate if all write actions are in memory - if not, transaction must read them from transaction file o commit the transaction
        /// </summary>
        private bool _hasAllWriteActionsInMemory;

        /// <summary>
        ///   To indicate if transaction has already been persisted in file
        /// </summary>
        private bool _hasBeenPersisted;

        /// <summary>
        ///   To indicate if transaction was confirmed = committed
        /// </summary>
        private bool _isCommited;

        /// <summary>
        ///   The number of write actions
        /// </summary>
        private int _numberOfWriteActions;

        private ICoreProvider _provider;

        /// <summary>
        ///   The transaction session
        /// </summary>
        private ISession _session;

        /// <summary>
        ///   To indicate if transaction was rollbacked
        /// </summary>
        private bool _wasRollbacked;

        /// <summary>
        ///   All the pending writing that must be applied to actually commit the transaction
        /// </summary>
        private IOdbList<IWriteAction> _writeActions;

        /// <summary>
        ///   The main constructor
        /// </summary>
        /// <param name="session"> The transaction session </param>
        /// <exception cref="System.IO.IOException">System.IO.IOException</exception>
        public OdbTransaction(ISession session)
        {
            Init(session);
        }

        public OdbTransaction(ISession session, IFileSystemInterface fsiToApplyTransaction)
        {
            _fsiToApplyWriteActions = fsiToApplyTransaction;
            Init(session);
            _readOnlyMode = false;
        }

        #region ITransaction Members

        public void Clear()
        {
            if (_writeActions != null)
            {
                _writeActions.Clear();
                _writeActions = null;
            }
        }

        /// <summary>
        ///   Reset the transaction
        /// </summary>
        public void Reset()
        {
            Clear();
            Init(_session);
            _fsi = null;
        }

        public string GetName()
        {
            var parameters = _fsiToApplyWriteActions.GetFileIdentification();
            if (parameters is FileIdentification)
            {
                var ifp = (FileIdentification) _fsiToApplyWriteActions.GetFileIdentification();
                var buffer =
                    new StringBuilder(ifp.Id).Append("-").Append(_creationDateTime).Append("-").Append(
                        _session.GetId()).Append(".transaction");

                return buffer.ToString();
            }

            throw new OdbRuntimeException(NDatabaseError.UnsupportedIoType.AddParameter(parameters.GetType().FullName));
        }

        public bool IsCommited()
        {
            return _isCommited;
        }

        public void Rollback()
        {
            _wasRollbacked = true;
            if (_fsi != null)
            {
                _fsi.Close();
                Delete();
            }
        }

        public void Commit()
        {
            if (OdbConfiguration.IsDebugEnabled(LogId))
            {
                DLogger.Info(string.Format("Commiting {0} write actions - In Memory : {1} - sid={2}",
                                           _numberOfWriteActions, _hasAllWriteActionsInMemory, _session.GetId()));
            }

            // Check if database has been rollbacked
            CheckRollback();

            // call the commit listeners
            ManageCommitListenersBefore();

            if (_currentWriteAction != null && !_currentWriteAction.IsEmpty())
            {
                AddWriteAction(_currentWriteAction);
                _currentWriteAction = null;
            }

            if (_fsi == null && _numberOfWriteActions != 0)
                throw new OdbRuntimeException(NDatabaseError.TransactionAlreadyCommitedOrRollbacked);

            if (_numberOfWriteActions == 0 || _readOnlyMode)
            {
                // FIXME call commitMetaModel in realOnlyMode?
                CommitMetaModel();
                // Nothing to do
                if (_fsi != null)
                {
                    _fsi.Close();
                    _fsi = null;
                }
                if (_session != null)
                    _session.GetCache().ClearOnCommit();
                return;
            }

            // Marks the transaction as committed
            SetCommited(true);

            // Apply the write actions the main database file
            ApplyTo();

            // Commit Meta Model changes
            CommitMetaModel();
            if (_archiveLog)
            {
                _fsi.SetWritePositionNoVerification(0, false);
                _fsi.WriteByte(2, false);
                _fsi.GetIo().EnableAutomaticDelete(false);
                _fsi.Close();
                _fsi = null;
            }
            else
            {
                _fsi.Close();
                Delete();
                _fsi = null;
            }

            if (_session != null)
                _session.GetCache().ClearOnCommit();

            ManageCommitListenersAfter();
        }

        public void SetFsiToApplyWriteActions(IFileSystemInterface fsi)
        {
            _fsiToApplyWriteActions = fsi;
        }

        public bool IsArchiveLog()
        {
            return _archiveLog;
        }

        public void SetArchiveLog(bool archiveLog)
        {
            _archiveLog = archiveLog;
        }

        /// <returns> Returns the numberOfWriteActions. </returns>
        public int GetNumberOfWriteActions()
        {
            if (_currentWriteAction != null && !_currentWriteAction.IsEmpty())
                return _numberOfWriteActions + 1;
            return _numberOfWriteActions;
        }

        /// <summary>
        ///   Set the write position (position in main database file).
        /// </summary>
        /// <remarks>
        ///   Set the write position (position in main database file). This is used to know if the next write can be appended to the previous one (in the same current Write Action) or not.
        /// </remarks>
        /// <param name="position"> </param>
        public void SetWritePosition(long position)
        {
            if (position != _currentWritePositionInWa)
            {
                _currentWritePositionInWa = position;
                if (_currentWriteAction != null)
                    AddWriteAction(_currentWriteAction);
                _currentWriteAction = new WriteAction(position);
            }
            else
            {
                if (_currentWriteAction == null)
                {
                    _currentWriteAction = new WriteAction(position);
                    _currentWritePositionInWa = position;
                }
            }
        }

        public void ManageWriteAction(long position, byte[] bytes)
        {
            if (_currentWritePositionInWa == position)
            {
                if (_currentWriteAction == null)
                    _currentWriteAction = _provider.GetWriteAction(position, null);
                _currentWriteAction.AddBytes(bytes);
                _currentWritePositionInWa += bytes.Length;
            }
            else
            {
                if (_currentWriteAction != null)
                    AddWriteAction(_currentWriteAction);
                _currentWriteAction = _provider.GetWriteAction(position, bytes);
                _currentWritePositionInWa = position + bytes.Length;
            }
        }

        #endregion

        public void Init(ISession session)
        {
            _provider = OdbConfiguration.GetCoreProvider();
            _session = session;
            _isCommited = false;
            _creationDateTime = OdbTime.GetCurrentTimeInTicks();
            _writeActions = new OdbArrayList<IWriteAction>(1000);
            _hasAllWriteActionsInMemory = true;
            _numberOfWriteActions = 0;
            _hasBeenPersisted = false;
            _wasRollbacked = false;
            _currentWritePositionInWa = -1;
        }

        /// <summary>
        ///   Adds a write action to the transaction
        /// </summary>
        /// <param name="writeAction"> The write action to be added </param>
        public void AddWriteAction(IWriteAction writeAction)
        {
            AddWriteAction(writeAction, true);
        }

        /// <summary>
        ///   Adds a write action to the transaction
        /// </summary>
        /// <param name="writeAction"> The write action to be added </param>
        /// <param name="persistWriteAcion"> To indicate if write action must be persisted </param>
        public void AddWriteAction(IWriteAction writeAction, bool persistWriteAcion)
        {
            if (OdbConfiguration.IsDebugEnabled(LogId))
                DLogger.Info(string.Format("Adding WA in Transaction of session {0}", _session.GetId()));

            if (writeAction.IsEmpty())
                return;

            CheckRollback();
            if (!_hasBeenPersisted && persistWriteAcion)
                Persist();

            if (persistWriteAcion)
                writeAction.Persist(_fsi, _numberOfWriteActions + 1);

            // Only adds the writeaction to the list if the transaction keeps all in
            // memory
            if (_hasAllWriteActionsInMemory)
                _writeActions.Add(writeAction);

            _numberOfWriteActions++;

            if (_hasAllWriteActionsInMemory &&
                _numberOfWriteActions > OdbConfiguration.GetMaxNumberOfWriteObjectPerTransaction())
            {
                _hasAllWriteActionsInMemory = false;
                IEnumerator iterator = _writeActions.GetEnumerator();
                while (iterator.MoveNext())
                {
                    var defaultWriteAction = (WriteAction) iterator.Current;
                    defaultWriteAction.Clear();
                }

                _writeActions.Clear();

                if (OdbConfiguration.IsDebugEnabled(LogId))
                {
                    DLogger.Info(
                        string.Format(
                            "Number of objects has exceeded the max number {0}/{1}: switching to persistent transaction managment",
                            _numberOfWriteActions, OdbConfiguration.GetMaxNumberOfWriteObjectPerTransaction()));
                }
            }
        }

        internal IFileIdentification GetParameters()
        {
            var parameters = _fsiToApplyWriteActions.GetFileIdentification();

            if (parameters is FileIdentification)
            {
                var ifp = (FileIdentification) _fsiToApplyWriteActions.GetFileIdentification();
                var buffer =
                    new StringBuilder(ifp.Directory).Append("/").Append(ifp.Id).Append("-").Append(
                        _creationDateTime).Append("-").Append(_session.GetId()).Append(".transaction");

                return new FileIdentification(buffer.ToString());
            }

            throw new OdbRuntimeException(NDatabaseError.UnsupportedIoType.AddParameter(parameters.GetType().FullName));
        }

        private void CheckFileAccess()
        {
            CheckFileAccess(null);
        }

        private void CheckFileAccess(string fileName)
        {
            lock (this)
            {
                if (_fsi == null)
                {
                    // to unable direct junit test of FileSystemInterface
                    var parameters = _fsiToApplyWriteActions == null
                                         ? new FileIdentification(fileName)
                                         : GetParameters();

                    _fsi = new FileSystemInterface("transaction", parameters,
                                                        MultiBuffer.DefaultBufferSizeForTransaction, _session);
                }
            }
        }

        private void Persist()
        {
            CheckFileAccess();

            if (OdbConfiguration.IsDebugEnabled(LogId))
                DLogger.Debug(string.Format("# Persisting transaction {0}", GetName()));

            _fsi.SetWritePosition(0, false);
            _fsi.WriteBoolean(_isCommited, false);
            _fsi.WriteLong(_creationDateTime, false, "creation date", WriteAction.DirectWriteAction);

            // Size
            _fsi.WriteLong(0, false, "size", WriteAction.DirectWriteAction);
            _hasBeenPersisted = true;
        }

        public IOdbList<IWriteAction> GetWriteActions()
        {
            return _writeActions;
        }

        public long GetCreationDateTime()
        {
            return _creationDateTime;
        }

        /// <summary>
        ///   Mark te transaction file as committed
        /// </summary>
        /// <param name="isConfirmed"> </param>
        private void SetCommited(bool isConfirmed)
        {
            _isCommited = isConfirmed;
            CheckFileAccess();

            // TODO Check atomicity
            // Writes the number of write actions after the byte and date
            _fsi.SetWritePositionNoVerification(OdbType.Byte.GetSize() + OdbType.Long.GetSize(), false);
            _fsi.WriteLong(_numberOfWriteActions, false, "nb write actions", WriteAction.DirectWriteAction);
            // FIXME The fsi.flush should not be called after the last write?
            _fsi.Flush();
            // Only set useBuffer = false when it is a local database to avoid
            // net io overhead

            _fsi.UseBuffer(false);
            _fsi.SetWritePositionNoVerification(0, false);
            _fsi.WriteByte(1, false);
        }

        private void CheckRollback()
        {
            if (_wasRollbacked)
                throw new OdbRuntimeException(NDatabaseError.OdbHasBeenRollbacked);
        }

        private void ManageCommitListenersAfter()
        {
            var listeners = _session.GetStorageEngine().GetCommitListeners();
            if (listeners == null || listeners.IsEmpty())
                return;

            var iterator = listeners.GetEnumerator();
            while (iterator.MoveNext())
            {
                var commitListener = iterator.Current;
                commitListener.AfterCommit();
            }
        }

        private void ManageCommitListenersBefore()
        {
            var listeners = _session.GetStorageEngine().GetCommitListeners();
            if (listeners == null || listeners.IsEmpty())
                return;

            foreach (var commitListener in listeners)
                commitListener.BeforeCommit();
        }

        /// <summary>
        ///   Used to commit meta model : classes This is useful when running in client server mode TODO Check this
        /// </summary>
        private void CommitMetaModel()
        {
            var sessionMetaModel = _session.GetMetaModel();
            // If meta model has not been modified, there is nothing to do
            if (!sessionMetaModel.HasChanged())
                return;

            if (OdbConfiguration.IsDebugEnabled(LogId))
                DLogger.Debug("Start commitMetaModel");

            // In local mode, we must not reload the meta model as there is no
            // concurrent access
            var lastCommitedMetaModel = sessionMetaModel;

            // Gets the classes that have changed (that have modified ,deleted or
            // inserted objects)
            var enumerator = sessionMetaModel.GetChangedClassInfo().GetEnumerator();
            var writer = _session.GetStorageEngine().GetObjectWriter();

            // for all changes between old and new meta model
            while (enumerator.MoveNext())
            {
                var newClassInfo = enumerator.Current;
                ClassInfo lastCommittedCI;

                if (lastCommitedMetaModel.ExistClass(newClassInfo.GetFullClassName()))
                {
                    // The last CI represents the last committed meta model of the
                    // database
                    lastCommittedCI = lastCommitedMetaModel.GetClassInfoFromId(newClassInfo.GetId());
                    // Just be careful to keep track of current CI committed zone
                    // deleted objects
                    lastCommittedCI.GetCommitedZoneInfo().SetNbDeletedObjects(
                        newClassInfo.GetCommitedZoneInfo().GetNbDeletedObjects());
                }
                else
                    lastCommittedCI = newClassInfo;

                var lastCommittedObjectOIDOfThisTransaction = newClassInfo.GetCommitedZoneInfo().Last;
                var lastCommittedObjectOIDOfPrevTransaction = lastCommittedCI.GetCommitedZoneInfo().Last;
                var lastCommittedObjectOID = lastCommittedObjectOIDOfPrevTransaction;

                // If some object have been created then
                if (lastCommittedObjectOIDOfPrevTransaction != null)
                {
                    // Checks if last object of committed meta model has not been
                    // deleted
                    if (_session.GetCache().IsDeleted(lastCommittedObjectOIDOfPrevTransaction))
                    {
                        // TODO This is wrong: if a committed transaction deleted a
                        // committed object and creates x new
                        // objects, then all these new objects will be lost:
                        // if it has been deleted then use the last object of the
                        // session class info
                        lastCommittedObjectOID = lastCommittedObjectOIDOfThisTransaction;
                        newClassInfo.GetCommitedZoneInfo().Last = lastCommittedObjectOID;
                    }
                }

                // Connect Unconnected zone to connected zone
                // make next oid of last committed object point to first
                // uncommitted object
                // make previous oid of first uncommitted object point to
                // last committed object
                if (lastCommittedObjectOID != null && newClassInfo.GetUncommittedZoneInfo().HasObjects())
                {
                    if (newClassInfo.GetCommitedZoneInfo().HasObjects())
                    {
                        // these 2 updates are executed directly without
                        // transaction, because
                        // We are in the commit process.
                        writer.UpdateNextObjectFieldOfObjectInfo(lastCommittedObjectOID,
                                                                 newClassInfo.GetUncommittedZoneInfo().First, false);
                        writer.UpdatePreviousObjectFieldOfObjectInfo(newClassInfo.GetUncommittedZoneInfo().First,
                                                                     lastCommittedObjectOID, false);
                    }
                    else
                    {
                        // Committed zone has 0 object
                        writer.UpdatePreviousObjectFieldOfObjectInfo(newClassInfo.GetUncommittedZoneInfo().First, null, false);
                    }
                }

                // The number of committed objects must be updated with the number
                // of the last committed CI because a transaction may have been
                // committed changing this number.
                // Notice that the setNbObjects receive the full CommittedCIZoneInfo
                // object
                // because it will set the number of objects and the number of
                // deleted objects
                newClassInfo.GetCommitedZoneInfo().SetNbObjects(lastCommittedCI.GetCommitedZoneInfo());

                // and don't forget to set the deleted objects
                // This sets the number of objects, the first object OID and the
                // last object OID
                newClassInfo = BuildClassInfoForCommit(newClassInfo);

                writer.FileSystemProcessor.UpdateInstanceFieldsOfClassInfo(newClassInfo, false);

                if (OdbConfiguration.IsDebugEnabled(LogId))
                {
                    DLogger.Debug(string.Format("Analysing class {0}", newClassInfo.GetFullClassName()));
                    DLogger.Debug(string.Format("\t-Commited CI   = {0}", newClassInfo));
                    DLogger.Debug(
                        string.Format("\t-connect last commited object with oid {0} to first uncommited object {1}",
                                      lastCommittedObjectOID, newClassInfo.GetUncommittedZoneInfo().First));
                    DLogger.Debug(string.Format("\t-Commiting new Number of objects = {0}", newClassInfo.GetNumberOfObjects()));
                }
            }

            sessionMetaModel.ResetChangedClasses();
        }

        /// <summary>
        ///   Shift all unconnected infos to connected (committed) infos
        /// </summary>
        /// <param name="classInfo"> </param>
        /// <returns> The updated class info </returns>
        public ClassInfo BuildClassInfoForCommit(ClassInfo classInfo)
        {
            var nbObjects = classInfo.GetNumberOfObjects();
            classInfo.GetCommitedZoneInfo().SetNbObjects(nbObjects);
            if (classInfo.GetCommitedZoneInfo().First == null)
            {
                // nothing to change
                classInfo.GetCommitedZoneInfo().First = classInfo.GetUncommittedZoneInfo().First;
            }

            if (classInfo.GetUncommittedZoneInfo().Last != null)
                classInfo.GetCommitedZoneInfo().Last = classInfo.GetUncommittedZoneInfo().Last;

            // Resets the unconnected zone info
            classInfo.GetUncommittedZoneInfo().Set(new CIZoneInfo(classInfo, null, null, 0));

            return classInfo;
        }

        public void LoadWriteActions(string filename, bool apply)
        {
            if (OdbConfiguration.IsDebugEnabled(LogId))
                DLogger.Debug(string.Format("Load write actions of {0}", filename));

            CheckFileAccess(filename);
            _fsi.UseBuffer(true);
            _fsi.SetReadPosition(0);
            _isCommited = _fsi.ReadByte() == 1;
            _creationDateTime = _fsi.ReadLong();
            var totalNumberOfWriteActions = _fsi.ReadLong();

            if (OdbConfiguration.IsDebugEnabled(LogId))
                DLogger.Info(string.Format("{0} write actions in file", _writeActions.Count));

            for (var i = 0; i < totalNumberOfWriteActions; i++)
            {
                var defaultWriteAction = WriteAction.Read(_fsi, i + 1);

                if (apply)
                {
                    defaultWriteAction.ApplyTo(_fsiToApplyWriteActions, i + 1);
                    defaultWriteAction.Clear();
                }
                else
                    AddWriteAction(defaultWriteAction, false);

            }

            if (apply)
                _fsiToApplyWriteActions.Flush();
        }

        public void LoadWriteActionsBackwards(string filename, bool apply)
        {
            var executedWriteAction = 0;
            if (OdbConfiguration.IsDebugEnabled(LogId))
                DLogger.Debug(string.Format("Load write actions of {0}", filename));

            CheckFileAccess(filename);
            _fsi.UseBuffer(true);
            _fsi.SetReadPosition(0);
            _isCommited = _fsi.ReadByte() == 1;
            _creationDateTime = _fsi.ReadLong();

            IDictionary<long, long> writtenPositions = null;
            if (apply)
                writtenPositions = new OdbHashMap<long, long>();

            var i = _numberOfWriteActions;
            var previousWriteActionPosition = _fsi.GetLength();

            while (i > 0)
            {
                // Sets the position 8 bytes backwards
                _fsi.SetReadPosition(previousWriteActionPosition - OdbType.Long.GetSize());

                // And then the read a long, this will be the previous write
                // action position
                previousWriteActionPosition = _fsi.ReadLong();

                // Then sets the read position to read the write action
                _fsi.SetReadPosition(previousWriteActionPosition);

                IWriteAction writeAction = WriteAction.Read(_fsi, i + 1);

                if (apply)
                {
                    var position = writeAction.GetPosition();
                    if (writtenPositions.ContainsKey(position))
                    {
                        // It has already been written something more recent at
                        // this position, do not write again
                        i--;
                        continue;
                    }
                    writeAction.ApplyTo(_fsiToApplyWriteActions, i + 1);
                    writtenPositions.Add(position, position);
                    executedWriteAction++;
                }
                else
                    AddWriteAction(writeAction, false);
                i--;
            }
            if (apply)
            {
                _fsiToApplyWriteActions.Flush();
                if (OdbConfiguration.IsDebugEnabled(LogId))
                    DLogger.Debug(string.Format("Total Write actions : {0} / position cache = {1}", i, writtenPositions.Count));

                DLogger.Info(string.Format("Total write actions = {0} : executed = {1}", _numberOfWriteActions, executedWriteAction));

                writtenPositions.Clear();
            }
        }

        /// <summary>
        ///   deletes the transaction file
        /// </summary>
        /// <exception cref="System.IO.IOException">System.IO.IOException</exception>
        private static void Delete()
        {
            //TODO: check it
        }

        // The delete is done automatically by underlying api
        public override string ToString()
        {
            var buffer = new StringBuilder();
            buffer.Append("state=").Append(_isCommited).Append(" | creation=").Append(_creationDateTime).Append(
                " | write actions numbers=").Append(_numberOfWriteActions);
            return buffer.ToString();
        }

        private void ApplyTo()
        {
            if (!_isCommited)
            {
                DLogger.Info("can not execute a transaction that is not confirmed");
                return;
            }

            if (_hasAllWriteActionsInMemory)
            {
                for (var i = 0; i < _writeActions.Count; i++)
                {
                    var wa = (WriteAction) _writeActions[i];
                    wa.ApplyTo(_fsiToApplyWriteActions, i + 1);
                    wa.Clear();
                }
                _fsiToApplyWriteActions.Flush();
            }
            else
            {
                LoadWriteActions(GetName(), true);
                _fsiToApplyWriteActions.Flush();
            }
        }

        public IFileSystemInterface GetFsi()
        {
            if (_fsi == null)
                CheckFileAccess();
            return _fsi;
        }
    }
}
