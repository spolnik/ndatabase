using System;
using IO;
using NDatabase.Odb;
using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Layers.Layer3.Engine;
using NDatabase.Odb.Core.Transaction;
using NDatabase.Odb.Impl.Core.Layers.Layer3.Engine;
using NDatabase.Odb.Impl.Core.Transaction;
using NDatabase.Tool;
using NDatabase.Tool.Wrappers;
using NUnit.Framework;
using Test.Odb.Test;

namespace Acid
{
    [TestFixture]
    public class TestTransactionPersistence : ODBTest
    {
        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test2B()
        {
            var byteArrayConverter = OdbConfiguration.GetCoreProvider().GetByteArrayConverter();
            DeleteBase("test78.neodatis");
            // 155 : to avoid protected zone
            IWriteAction wa1 = new DefaultWriteAction(300 + 1, byteArrayConverter.IntToByteArray(1), "size");
            IWriteAction wa2 = new DefaultWriteAction(300 + 15,
                                                      byteArrayConverter.StringToByteArray(" 1 - ol√° chico! - 1", true,
                                                                                           -1, true), "name");
            IWriteAction wa3 = new DefaultWriteAction(300 + 1, byteArrayConverter.IntToByteArray(2), "size");
            IWriteAction wa4 = new DefaultWriteAction(300 + 15,
                                                      byteArrayConverter.StringToByteArray(" 2 - ol√° chico! - 2", true,
                                                                                           -1, true), "name");
            var se =
                OdbConfiguration.GetCoreProvider().GetStorageEngine(new IOFileParameter(Directory + "test78.neodatis",
                                                                                        true));
            // se.close();
            var fsi = se.GetObjectWriter().GetFsi();
            // new FileSystemInterface(null,se.getSession(),new
            // IOFileParameter("test.neodatis",true),false,Configuration.getDefaultBufferSizeForData());
            var transaction = se.GetSession(true).GetTransaction();
            transaction.SetArchiveLog(true);
            transaction.ManageWriteAction(wa1.GetPosition(), wa1.GetBytes(0));
            transaction.ManageWriteAction(wa2.GetPosition(), wa2.GetBytes(0));
            transaction.ManageWriteAction(wa3.GetPosition(), wa3.GetBytes(0));
            transaction.ManageWriteAction(wa4.GetPosition(), wa4.GetBytes(0));
            // transaction.getFsi().flush();
            var wat1 = ((DefaultTransaction) transaction).GetWriteActions()[2];
            var bytes = wat1.GetBytes(0);
            transaction.Commit();
            var transaction2 = DefaultTransaction.Read(Directory + transaction.GetName());
            var wat2 = transaction2.GetWriteActions()[2];
            AssertEquals(DisplayUtility.ByteArrayToString(bytes), DisplayUtility.ByteArrayToString(wat2.GetBytes(0)));
            AssertEquals(wat1.GetPosition(), wat2.GetPosition());
            transaction2.Rollback();
            fsi.Close();
            DeleteBase("test78.neodatis");
            DeleteBase(transaction.GetName());
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test3()
        {
            var byteArrayConverter = OdbConfiguration.GetCoreProvider().GetByteArrayConverter();
            var size = 1000;
            ISession session = new MockSession(Directory + "test2.neodatis");
            IFileSystemInterface fsi = new LocalFileSystemInterface("test", session,
                                                                    new IOFileParameter(Directory + "test2.neodatis",
                                                                                        true), false,
                                                                    OdbConfiguration.GetDefaultBufferSizeForData());
            var transaction = new DefaultTransaction(session, fsi);
            transaction.SetArchiveLog(true);
            for (var i = 0; i < size; i++)
            {
                // 155 : to avoid protected zone
                transaction.ManageWriteAction(300 + i * 4 * 2, byteArrayConverter.IntToByteArray(i));
            }
            var wa1 = transaction.GetWriteActions()[size - 2];
            var bytes = wa1.GetBytes(0);
            transaction.Commit();
            var start = OdbTime.GetCurrentTimeInMs();
            var transaction2 = DefaultTransaction.Read(Directory + transaction.GetName());
            var t = OdbTime.GetCurrentTimeInMs() - start;
            var wa2 = transaction2.GetWriteActions()[size - 2];
            AssertEquals(DisplayUtility.ByteArrayToString(bytes), DisplayUtility.ByteArrayToString(wa2.GetBytes(0)));
            AssertEquals(wa1.GetPosition(), wa2.GetPosition());
            transaction2.Rollback();
            fsi.Close();
            DeleteBase("test2.neodatis");
            DeleteBase(transaction.GetName());
        }

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
            IWriteAction wa3 = new DefaultWriteAction(1,
                                                      byteArrayConverter.BigDecimalToByteArray(
                                                          new Decimal(1.123456789), true), "size");
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test5()
        {
            var byteArrayConverter = OdbConfiguration.GetCoreProvider().GetByteArrayConverter();
            var size = 1000;
            ISession session = new MockSession(Directory + "test2.neodatis");
            IFileSystemInterface fsi = new LocalFileSystemInterface("test", session,
                                                                    new IOFileParameter(Directory + "test2.neodatis",
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
            DeleteBase("test2.neodatis");
            DeleteBase(transaction.GetName());
        }
    }
}
