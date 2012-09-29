using System;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Layers.Layer3.Engine;
using NDatabase.Odb.Core.Transaction;

namespace Test.NDatabase.Odb.Test.IO
{
    internal class MockSession : global::NDatabase.Odb.Core.Transaction.Session
    {
        public MockSession(String baseIdentification) : base("mock", baseIdentification)
        {
            MetaModel = new MetaModel();
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

        public override MetaModel GetMetaModel()
        {
            return MetaModel;
        }
    }
}
