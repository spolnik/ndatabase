using NDatabase.Odb;
using NDatabase.Odb.Core.Layers.Layer3;
using NUnit.Framework;

namespace Test.NDatabase.Odb.Test.IO
{
    [TestFixture]
    public class TestReservingSpace : ODBTest
    {
        /// <exception cref="System.IO.IOException"></exception>
        public virtual void Write(IFileSystemInterface fsi, bool writeInTransaction)
        {
            fsi.WriteInt(1, writeInTransaction, "1");
        }

        [Test]
        public virtual void TestSize()
        {
            DeleteBase("writing.neodatis");
            DeleteBase("writing");
            DeleteBase("reserving.neodatis");
            DeleteBase("reserving");
            var engine1 =
                OdbConfiguration.GetCoreProvider().GetStorageEngine(new FileIdentification("writing.neodatis"));
            var engine2 =
                OdbConfiguration.GetCoreProvider().GetStorageEngine(new FileIdentification("reserving.neodatis"));
            var writingFsi = engine1.GetObjectWriter().FileSystemProcessor.FileSystemInterface;
            var reservingFsi = engine2.GetObjectWriter().FileSystemProcessor.FileSystemInterface;
            AssertEquals(writingFsi.GetLength(), reservingFsi.GetLength());
            Write(writingFsi, false);
            Write(reservingFsi, true);
            AssertEquals(writingFsi.GetLength(), reservingFsi.GetLength());
            engine1.Commit();
            engine1.Close();
            engine2.Commit();
            engine2.Close();
            DeleteBase("writing.neodatis");
            DeleteBase("reserving.neodatis");
        }
    }
}
