using System;
using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Layers.Layer3.Engine;
using NDatabase.Odb.Core.Layers.Layer3.IO;
using NDatabase.Odb.Core.Transaction;
using NDatabase.Odb.Impl.Core.Layers.Layer3.Engine;
using NDatabase.Odb.Impl.Core.Transaction;
using NUnit.Framework;
using Test.NDatabase.Odb.Test.IO;

namespace Test.NDatabase.Odb.Test.Acid
{
    [TestFixture]
    public class TestTransactionPersistence : ODBTest
    {
        [Test]
        public virtual void Test4()
        {
            IWriteAction wa1 = new WriteAction(1, ByteArrayConverter.IntToByteArray(1));
            AssertEquals(wa1.GetBytes(0).Length, 4);

            IWriteAction wa2 = new WriteAction(1, ByteArrayConverter.StringToByteArray("ol√° chico", -1));
            AssertEquals(wa2.GetBytes(0).Length, 29);

            IWriteAction wa3 = new WriteAction(1,
                                               ByteArrayConverter.DecimalToByteArray(new Decimal(1.123456789)));
            AssertEquals(wa3.GetBytes(0).Length, 16);
        }

        [Test]
        [Ignore("Requires better mocking engine")]
        public virtual void Test5()
        {
            var size = 1000;
            ISession session = new MockSession("test2.neodatis");
            IFileSystemInterface fsi = new FileSystemInterface(new FileIdentification("test2.neodatis"), MultiBuffer.DefaultBufferSizeForData,
                                                               session);
            var transaction = new OdbTransaction(session, fsi);
            transaction.SetArchiveLog(true);
            for (var i = 0; i < size; i++)
            {
                // 155 : to avoid protected zone
                transaction.ManageWriteAction(300 + i * 4, ByteArrayConverter.IntToByteArray(i));
            }
            // All write action were together so the transaction should have
            // appended all the bytes
            // in one WriteAction. As it as not been committed, the current
            // writeAction
            AssertEquals(0, transaction.GetWriteActions().Count);
            transaction.Commit();
            fsi.Close();
            DeleteBase("test2.neodatis");
            DeleteBase(transaction.GetName());
        }
    }
}
