using System.IO;
using NDatabase.Tool.Wrappers.IO;
using NUnit.Framework;
using Test.Odb.Test;

namespace IO
{
    [TestFixture]
    public class TestFileLock : ODBTest
    {
        /// <exception cref="System.IO.IOException"></exception>
        [Test]
        public virtual void Test1()
        {
            var raf = new FileStream("testLock1", FileMode.OpenOrCreate);
            raf.Seek(1024, SeekOrigin.Begin);
            raf.Write(new byte[] {10}, 0, 1);
            //Java.Nio.Channels.FileLock fileLock = raf.GetChannel().Lock(0, 1024, false);
            //AssertEquals(true, fileLock != null);
            //System.IO.FileStream raf2 = new System.IO.FileStream(ODBTest.Directory		 + "testLock1", FileMode.OpenOrCreate);
            //Java.Nio.Channels.FileLock fileLock2 = raf2.GetChannel().Lock(0, 1024, false);
        }

        /// <exception cref="System.IO.IOException"></exception>
        [Test]
        public virtual void Test2NoWrite()
        {
            var raf = new FileStream("testLock1", FileMode.OpenOrCreate);
            raf.Seek(1024, SeekOrigin.Begin);
            //Java.Nio.Channels.FileLock fileLock = raf.GetChannel().Lock(0, 1, false);
            //AssertEquals(true, fileLock != null);
            raf.Close();
        }

        /// <summary>
        ///   Simple lock
        /// </summary>
        /// <exception cref="System.IO.IOException">System.IO.IOException</exception>
        [Test]
        public virtual void TestOdbFileIo()
        {
            var fileIO = new OdbFileIO("testLock1", true, null);
            fileIO.Seek(1024);
            fileIO.Write(10);
            fileIO.LockFile();
            AssertEquals(true, fileIO.IsLocked());
            fileIO.Close();
        }

        /// <summary>
        ///   Simple lock
        /// </summary>
        /// <exception cref="System.IO.IOException">System.IO.IOException</exception>
        [Test]
        public virtual void TestOdbFileIo2()
        {
            var fileIO = new OdbFileIO("testLock2", true, null);
            fileIO.Seek(1024);
            fileIO.Write(10);
            var isLocked = fileIO.LockFile();
            AssertTrue(isLocked);
            var fileIO2 = new OdbFileIO("testLock2", true, null);
            fileIO2.LockFile();
            AssertEquals(true, fileIO2.IsLocked());
            fileIO.Close();
        }
    }
}
