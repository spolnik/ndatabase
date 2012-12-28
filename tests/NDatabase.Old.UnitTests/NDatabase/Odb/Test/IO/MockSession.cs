using System;
using NDatabase.Odb.Core.Transaction;
using NDatabase2.Odb.Core.Layers.Layer3;
using NDatabase2.Odb.Core.Layers.Layer3.Engine;

namespace Test.NDatabase.Odb.Test.IO
{
    internal class MockSession : global::NDatabase.Odb.Core.Transaction.Session
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
