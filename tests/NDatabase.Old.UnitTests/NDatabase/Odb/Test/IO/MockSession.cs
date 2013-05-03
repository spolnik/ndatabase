using System;
using NDatabase.Core.Layers.Layer3;
using NDatabase.Core.Layers.Layer3.Engine;
using NDatabase.Core.Transaction;

namespace Test.NDatabase.Odb.Test.IO
{
    internal class MockSession : global::NDatabase.Core.Transaction.Session
    {
        public MockSession(String baseIdentification) : base("mock", baseIdentification)
        {
        }

        public override void Commit()
        {
        }

        public override IStorageEngine GetStorageEngine()
        {
            try
            {
                return new StorageEngine(new MockFileIdentification());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return null;
        }

        public override ITransaction GetTransaction()
        {
            return null;
        }

        public override void SetFileSystemInterfaceToApplyTransaction(IFileSystemInterface fsi)
        {
        }

        public override bool TransactionIsPending()
        {
            return false;
        }
    }
}
