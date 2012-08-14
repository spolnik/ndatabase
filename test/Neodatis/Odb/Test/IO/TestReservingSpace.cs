using NDatabase.Odb;
using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Layers.Layer3.Engine;
using NUnit.Framework;
using Test.Odb.Test;

namespace IO
{
    [TestFixture]
    public class TestReservingSpace : ODBTest
    {
        /// <exception cref="System.IO.IOException"></exception>
        public virtual void Write(IFileSystemInterface fsi, bool writeInTransaction)
        {
            fsi.WriteInt(1, writeInTransaction, "1");
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void TestSize()
        {
            DeleteBase("writing.neodatis");
            DeleteBase("writing");
            DeleteBase("reserving.neodatis");
            DeleteBase("reserving");
            var engine1 =
                OdbConfiguration.GetCoreProvider().GetStorageEngine(new IOFileParameter("writing.neodatis", true));
            var engine2 =
                OdbConfiguration.GetCoreProvider().GetStorageEngine(new IOFileParameter("reserving.neodatis", true));
            var writingFsi = engine1.GetObjectWriter().GetFsi();
            var reservingFsi = engine2.GetObjectWriter().GetFsi();
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
