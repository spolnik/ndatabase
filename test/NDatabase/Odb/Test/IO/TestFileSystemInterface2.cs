using NDatabase.Odb;
using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Layers.Layer3.Engine;
using NDatabase.Odb.Core.Layers.Layer3.IO;
using NDatabase.Tool.Wrappers;
using NUnit.Framework;

namespace Test.NDatabase.Odb.Test.IO
{
    [TestFixture]
    public class TestFileSystemInterface2 : ODBTest
    {
        [Test]
        public virtual void TestReadWrite()
        {
            DeleteBase("testReadWrite.neodatis");
            IFileSystemInterface fsi = new FileSystemInterface("data", new FileIdentification("testReadWrite.neodatis"),
                                                               MultiBuffer.DefaultBufferSizeForData,
                                                               new MockSession("test"));
            fsi.SetWritePosition(fsi.GetLength(), false);
            for (var i = 0; i < 10000; i++)
            {
                fsi.WriteInt(i, false, "int");
                var currentPosition = fsi.GetPosition();
                if (i == 8000)
                {
                    currentPosition = fsi.GetPosition();
                    fsi.UseBuffer(false);
                    // Using the for transaction method to avoid protected area
                    // verification, check the setWritePosition method
                    fsi.SetWritePositionNoVerification(4, false);
                    AssertEquals(1, fsi.ReadInt());
                    fsi.UseBuffer(true);
                    fsi.SetWritePositionNoVerification(currentPosition, false);
                }
                if (i == 9000)
                {
                    currentPosition = fsi.GetPosition();
                    fsi.UseBuffer(false);
                    fsi.SetWritePositionNoVerification(8, false);
                    fsi.WriteInt(12, false, "int");
                    fsi.UseBuffer(true);
                    fsi.SetWritePositionNoVerification(currentPosition, false);
                }
            }
            fsi.SetReadPosition(0);
            for (var i = 0; i < 10000; i++)
            {
                var j = fsi.ReadInt();
                if (i == 2)
                    AssertEquals(12, j);
                else
                    AssertEquals(i, j);
            }
            fsi.Close();
            DeleteBase("testReadWrite.neodatis");
        }

        /// <exception cref="Java.IO.UnsupportedEncodingException"></exception>
        [Test]
        public virtual void TestStringGetBytesWithoutEncoding()
        {
            var byteArrayConverter = OdbConfiguration.GetCoreProvider().GetByteArrayConverter();
            var test = "How are you my friend?";
            var size = 1000000;
            var t0 = OdbTime.GetCurrentTimeInMs();
            // Execute with encoding
            for (var i = 0; i < size; i++)
                byteArrayConverter.StringToByteArray(test, true, -1, true);
            var t1 = OdbTime.GetCurrentTimeInMs();
            // Execute without encoding
            for (var i = 0; i < size; i++)
            {
            }
            // byteArrayConverter.stringToByteArray(test, false, -1, false);
            var t2 = OdbTime.GetCurrentTimeInMs();
            Println("With Encoding=" + (t1 - t0) + " / Without Encoding=" + (t2 - t1));
        }
    }
}
