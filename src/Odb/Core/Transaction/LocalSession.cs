using NDatabase.Tool.Wrappers;
using NDatabase2.Odb.Core.Layers.Layer3;

namespace NDatabase.Odb.Core.Transaction
{
    /// <summary>
    ///   The session object used when ODB is used in local/embedded mode
    /// </summary>
    internal sealed class LocalSession : Session
    {
        private IFileSystemInterface _fsiToApplyTransaction;
        private IStorageEngine _storageEngine;
        private ITransaction _transaction;

        private static string GetSessionId()
        {
            return
                string.Concat("local ", OdbTime.GetCurrentTimeInTicks().ToString(),
                              OdbRandom.GetRandomInteger().ToString());
        }

        public LocalSession(IStorageEngine engine)
            : base(GetSessionId(), engine.GetBaseIdentification().Id)
        {
            _storageEngine = engine;
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
