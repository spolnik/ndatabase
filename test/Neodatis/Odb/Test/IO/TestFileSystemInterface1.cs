using System;
using NDatabase.Odb;
using NDatabase.Odb.Core.Layers.Layer3.Engine;
using NDatabase.Odb.Impl.Core.Layers.Layer3.Engine;
using NDatabase.Odb.Impl.Core.Transaction;
using NUnit.Framework;
using Test.Odb.Test;

namespace IO
{
    [TestFixture]
    public class TestFileSystemInterface1 : ODBTest
    {
        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void TestBigDecimal()
        {
            var bd = Convert.ToDecimal("-128451.1234567899876543210");
            IFileSystemInterface fsi = new LocalFileSystemInterface("data", new MockSession("test"),
                                                                    "testBigDecimal.neodatis", true,
                                                                    true, OdbConfiguration.GetDefaultBufferSizeForData());
            fsi.SetWritePosition(0, false);
            fsi.WriteBigDecimal(bd, false);
            fsi.Close();
            fsi = new LocalFileSystemInterface("data", new MockSession("test"),
                                               "testBigDecimal.neodatis", false, false,
                                               OdbConfiguration.GetDefaultBufferSizeForData());
            fsi.SetReadPosition(0);
            var bd2 = fsi.ReadBigDecimal();
            AssertEquals(bd, bd2);
            fsi.Close();
            DeleteBase("testBigDecimal.neodatis");
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void TestBigInteger()
        {
            var bd = Convert.ToDecimal("-128451");
            IFileSystemInterface fsi = new LocalFileSystemInterface("data", new MockSession("test"),
                                                                    "testBigDecimal.neodatis", true,
                                                                    true, OdbConfiguration.GetDefaultBufferSizeForData());
            fsi.SetWritePosition(0, false);
            fsi.WriteBigDecimal(bd, false);
            fsi.Close();
            fsi = new LocalFileSystemInterface("data", new MockSession("test"),
                                               "testBigDecimal.neodatis", false, false,
                                               OdbConfiguration.GetDefaultBufferSizeForData());
            fsi.SetReadPosition(0);
            var bd2 = fsi.ReadBigDecimal();
            AssertEquals(bd, bd2);
            fsi.Close();
            DeleteBase("testBigDecimal.neodatis");
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void TestBoolean()
        {
            var b1 = true;
            var b2 = false;
            IFileSystemInterface fsi = new LocalFileSystemInterface("data", new MockSession("test"),
                                                                    "testBoolean.neodatis", true,
                                                                    true, OdbConfiguration.GetDefaultBufferSizeForData());
            fsi.SetWritePosition(0, false);
            fsi.WriteBoolean(b1, false);
            fsi.WriteBoolean(b2, false);
            fsi.Close();
            fsi = new LocalFileSystemInterface("data", new MockSession("test"),
                                               "testBoolean.neodatis", false, false,
                                               OdbConfiguration.GetDefaultBufferSizeForData());
            fsi.SetReadPosition(0);
            var b11 = fsi.ReadBoolean();
            var b22 = fsi.ReadBoolean();
            AssertEquals(b1, b11);
            AssertEquals(b2, b22);
            fsi.Close();
            DeleteBase("testBoolean.neodatis");
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void TestByte()
        {
            byte b = 127;
            IFileSystemInterface fsi = new LocalFileSystemInterface("data", new MockSession("test"), "testByte.neodatis",
                                                                    true, true,
                                                                    OdbConfiguration.GetDefaultBufferSizeForData());
            fsi.SetWritePosition(0, false);
            fsi.WriteByte(b, false);
            fsi.Close();
            fsi = new LocalFileSystemInterface("data", new MockSession("test"), "testByte.neodatis", false, false,
                                               OdbConfiguration.GetDefaultBufferSizeForData());
            fsi.SetReadPosition(0);
            var b2 = fsi.ReadByte();
            AssertEquals(b, b2);
            fsi.Close();
            DeleteBase("testByte.neodatis");
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void TestChar()
        {
            var c = '\u00E1';
            IFileSystemInterface fsi = new LocalFileSystemInterface("data", new MockSession("test"), "testChar.neodatis",
                                                                    true, true,
                                                                    OdbConfiguration.GetDefaultBufferSizeForData());
            fsi.SetWritePosition(0, false);
            fsi.WriteChar(c, false);
            fsi.Close();
            fsi = new LocalFileSystemInterface("data", new MockSession("test"), "testChar.neodatis", false, false,
                                               OdbConfiguration.GetDefaultBufferSizeForData());
            fsi.SetReadPosition(0);
            var c2 = fsi.ReadChar();
            AssertEquals(c, c2);
            fsi.Close();
            DeleteBase("testChar.neodatis");
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void TestFloat()
        {
            var f = (float) 12544548.12454;
            IFileSystemInterface fsi = new LocalFileSystemInterface("data", new MockSession("test"),
                                                                    "testFloat.neodatis", true, true,
                                                                    OdbConfiguration.GetDefaultBufferSizeForData());
            fsi.SetWritePosition(0, false);
            fsi.WriteFloat(f, false);
            fsi.Close();
            fsi = new LocalFileSystemInterface("data", new MockSession("test"), "testFloat.neodatis",
                                               false, false, OdbConfiguration.GetDefaultBufferSizeForData());
            fsi.SetReadPosition(0);
            var f2 = fsi.ReadFloat();
            AssertTrue(f == f2);
            fsi.Close();
            DeleteBase("testFloat.neodatis");
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void TestInt()
        {
            var i = 259998;
            IFileSystemInterface fsi = new LocalFileSystemInterface("data", new MockSession("test"), "testInt.neodatis",
                                                                    true, true,
                                                                    OdbConfiguration.GetDefaultBufferSizeForData());
            fsi.SetWritePosition(0, false);
            fsi.WriteInt(i, false, "i");
            fsi.Close();
            fsi = new LocalFileSystemInterface("data", new MockSession("test"), "testInt.neodatis", false, false,
                                               OdbConfiguration.GetDefaultBufferSizeForData());
            fsi.SetReadPosition(0);
            var i2 = fsi.ReadInt();
            AssertEquals(i, i2);
            fsi.Close();
            DeleteBase("testInt.neodatis");
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void TestLong()
        {
            long i = 259999865;
            IFileSystemInterface fsi = new LocalFileSystemInterface("data", new MockSession("test"),
                                                                    "testLong.neodatis", true, true,
                                                                    OdbConfiguration.GetDefaultBufferSizeForData());
            fsi.SetWritePosition(0, false);
            fsi.WriteLong(i, false, "i", DefaultWriteAction.PointerWriteAction);
            fsi.Close();
            fsi = new LocalFileSystemInterface("data", new MockSession("test"), "testLong.neodatis",
                                               false, false, OdbConfiguration.GetDefaultBufferSizeForData());
            fsi.SetReadPosition(0);
            var i2 = fsi.ReadLong();
            AssertEquals(i, i2);
            fsi.Close();
            DeleteBase("testLong.neodatis");
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void TestShort()
        {
            short s = 4598;
            IFileSystemInterface fsi = new LocalFileSystemInterface("data", new MockSession("test"),
                                                                    "testShort.neodatis", true, true,
                                                                    OdbConfiguration.GetDefaultBufferSizeForData());
            fsi.SetWritePosition(0, false);
            fsi.WriteShort(s, false);
            fsi.Close();
            fsi = new LocalFileSystemInterface("data", new MockSession("test"), "testShort.neodatis",
                                               false, false, OdbConfiguration.GetDefaultBufferSizeForData());
            fsi.SetReadPosition(0);
            var s2 = fsi.ReadShort();
            AssertEquals(s, s2);
            fsi.Close();
            DeleteBase("testShort.neodatis");
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void TestString()
        {
            var baseName = GetBaseName();
            var s = "ola chico, como voc√™ est√° ??? eu estou bem at√© amanh√£ de manh√£, √°√°√°√°'";
            IFileSystemInterface fsi = new LocalFileSystemInterface("data", new MockSession("test"), baseName, true,
                                                                    true, OdbConfiguration.GetDefaultBufferSizeForData());
            fsi.SetWritePosition(0, false);
            fsi.WriteString(s, false, true);
            fsi.Close();
            fsi = new LocalFileSystemInterface("data", new MockSession("test"), baseName, false, false,
                                               OdbConfiguration.GetDefaultBufferSizeForData());
            fsi.SetReadPosition(0);
            var s2 = fsi.ReadString(true);
            fsi.Close();
            DeleteBase(baseName);
            AssertEquals(s, s2);
        }
    }
}
