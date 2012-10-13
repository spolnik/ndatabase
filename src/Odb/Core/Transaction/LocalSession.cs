using NDatabase2.Odb.Core.Layers.Layer3;
using NDatabase2.Tool.Wrappers;

namespace NDatabase2.Odb.Core.Transaction
{
    /// <summary>
    ///   The session object used when ODB is used in local/embedded mode
    /// </summary>
    /// <author>olivier s</author>
    internal sealed class LocalSession : Session
    {
        private IFileSystemInterface _fsiToApplyTransaction;
        private IStorageEngine _storageEngine;
        private ITransaction _transaction;

        public LocalSession(IStorageEngine engine, string sessionId)
            : base(sessionId, engine.GetBaseIdentification().Id)
        {
            _storageEngine = engine;
        }

        public LocalSession(IStorageEngine engine)
            : this(engine, string.Concat("local ", OdbTime.GetCurrentTimeInTicks().ToString(), OdbRandom.GetRandomInteger().ToString()))
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
            return _transaction ?? (_transaction = new OdbTransaction(this, _fsiToApplyTransaction));
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
