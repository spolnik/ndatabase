using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Transaction;
using NDatabase.Tool.Wrappers;

namespace NDatabase.Odb.Impl.Core.Transaction
{
    /// <summary>
    ///   The session object used when ODB is used in local/embedded mode
    /// </summary>
    /// <author>olivier s</author>
    public sealed class LocalSession : Session
    {
        private IFileSystemInterface _fsiToApplyTransaction;
        private IStorageEngine _storageEngine;
        private ITransaction _transaction;

        public LocalSession(IStorageEngine engine, string sessionId)
            : base(sessionId, engine.GetBaseIdentification().GetIdentification())
        {
            _storageEngine = engine;
        }

        public LocalSession(IStorageEngine engine)
            : this(engine, string.Format("local {0}{1}", OdbTime.GetCurrentTimeInTicks(), OdbRandom.GetRandomInteger()))
        {
        }

        public override void SetFileSystemInterfaceToApplyTransaction(IFileSystemInterface fsi)
        {
            _fsiToApplyTransaction = fsi;
            if (_transaction != null)
                _transaction.SetFsiToApplyWriteActions(_fsiToApplyTransaction);
        }

        public override ITransaction GetTransaction()
        {
            return _transaction ??
                   (_transaction = OdbConfiguration.GetCoreProvider().GetTransaction(this, _fsiToApplyTransaction));
        }

        public override bool TransactionIsPending()
        {
            if (_transaction == null)
                return false;
            return _transaction.GetNumberOfWriteActions() != 0;
        }

        private void ResetTranstion()
        {
            if (_transaction != null)
            {
                _transaction.Clear();
                _transaction = null;
            }
        }

        public override void Commit()
        {
            if (_transaction != null)
            {
                _transaction.Commit();
                _transaction.Reset();
            }
        }

        public override void Rollback()
        {
            if (_transaction != null)
            {
                _transaction.Rollback();
                ResetTranstion();
            }
            base.Rollback();
        }

        public override IStorageEngine GetStorageEngine()
        {
            return _storageEngine;
        }

        public override void Clear()
        {
            base.Clear();
            if (_transaction != null)
                _transaction.Clear();
            _storageEngine = null;
        }
    }
}
