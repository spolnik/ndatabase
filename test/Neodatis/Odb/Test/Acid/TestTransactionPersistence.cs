using System;
using IO;
using NDatabase.Odb;
using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Layers.Layer3.Engine;
using NDatabase.Odb.Core.Transaction;
using NDatabase.Odb.Impl.Core.Layers.Layer3.Engine;
using NDatabase.Odb.Impl.Core.Transaction;
using NUnit.Framework;
using Test.Odb.Test;

namespace Acid
{
    [TestFixture]
    public class TestTransactionPersistence : ODBTest
    {
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.TypeLoadException"></exception>
        [Test]
        public virtual void Test4()
        {
            var byteArrayConverter = OdbConfiguration.GetCoreProvider().GetByteArrayConverter();
            IWriteAction wa1 = new DefaultWriteAction(1, byteArrayConverter.IntToByteArray(1), "size");
            AssertEquals(wa1.GetBytes(0).Length, 4);

            IWriteAction wa2 = new DefaultWriteAction(1,
                                                      byteArrayConverter.StringToByteArray("ol√° chico", true, -1, true),
                                                      "size");
            AssertEquals(wa2.GetBytes(0).Length, 29);

            IWriteAction wa3 = new DefaultWriteAction(1,
                                                      byteArrayConverter.BigDecimalToByteArray(
                                                          new Decimal(1.123456789), true), "size");
            AssertEquals(wa3.GetBytes(0).Length, 27);
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test5()
        {
            var byteArrayConverter = OdbConfiguration.GetCoreProvider().GetByteArrayConverter();
            var size = 1000;
            ISession session = new MockSession("test2.neodatis");
            IFileSystemInterface fsi = new LocalFileSystemInterface("test", session,
                                                                    new IOFileParameter("test2.neodatis",
                                                                                        true), false,
                                                                    OdbConfiguration.GetDefaultBufferSizeForData());
            var transaction = new DefaultTransaction(session, fsi);
            transaction.SetArchiveLog(true);
            for (var i = 0; i < size; i++)
            {
                // 155 : to avoid protected zone
                transaction.ManageWriteAction(300 + i * 4, byteArrayConverter.IntToByteArray(i));
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
