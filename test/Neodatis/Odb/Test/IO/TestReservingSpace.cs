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
		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void TestSize()
		{
			DeleteBase("writing.neodatis");
			DeleteBase("writing");
			DeleteBase("reserving.neodatis");
			DeleteBase("reserving");
			IStorageEngine engine1 = OdbConfiguration
				.GetCoreProvider().GetStorageEngine(new IOFileParameter(ODBTest.Directory + "writing.neodatis", true));
			IStorageEngine engine2 = OdbConfiguration
				.GetCoreProvider().GetStorageEngine(new IOFileParameter(ODBTest.Directory + "reserving.neodatis", true));
			IFileSystemInterface writingFsi = engine1.
				GetObjectWriter().GetFsi();
			IFileSystemInterface reservingFsi = engine2
				.GetObjectWriter().GetFsi();
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

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Write(IFileSystemInterface fsi, bool writeInTransaction)
		{
			fsi.WriteInt(1, writeInTransaction, "1");
		}
	}
}
