using System;
using NDatabase.Odb;
using NDatabase.Odb.Core.Layers.Layer3.Engine;
using NUnit.Framework;

namespace Test.Odb.Test.Performance
{
    [TestFixture]
    public class TestByteConversion : ODBTest
    {
        internal static IByteArrayConverter byteArrayConverter =
            OdbConfiguration.GetCoreProvider().GetByteArrayConverter();

        public const int Size = 1000;

        public const int Size0 = 1000;

        /// <exception cref="System.IO.IOException"></exception>
        [Test]
        public virtual void TestBigDecimal1()
        {
            var bd1 = new Decimal(10);
            var b2 = byteArrayConverter.BigDecimalToByteArray(bd1, true);
            var bd2 = byteArrayConverter.ByteArrayToBigDecimal(b2, true);
            AssertEquals(bd1, bd2);
        }

        /// <exception cref="System.IO.IOException"></exception>
        [Test]
        public virtual void TestBigDecimal10()
        {
            var bd1 = new Decimal(123456789123456789123456789.123456789123456789);
            var b2 = byteArrayConverter.BigDecimalToByteArray(bd1, true);
            var bd2 = byteArrayConverter.ByteArrayToBigDecimal(b2, true);
            AssertEquals(bd1, bd2);
        }

        /// <exception cref="System.IO.IOException"></exception>
        [Test]
        public virtual void TestBigDecimal11()
        {
            var bd1 = new Decimal(-0.00000);
            var b2 = byteArrayConverter.BigDecimalToByteArray(bd1, true);
            var bd2 = byteArrayConverter.ByteArrayToBigDecimal(b2, true);
            AssertEquals(bd1, bd2);
        }

        /// <exception cref="System.IO.IOException"></exception>
        [Test]
        public virtual void TestBigDecimal2()
        {
            var bd1 = new Decimal(10.123456789123456789);
            var b2 = byteArrayConverter.BigDecimalToByteArray(bd1, true);
            var bd2 = byteArrayConverter.ByteArrayToBigDecimal(b2, true);
            AssertEquals(bd1, bd2);
        }

        /// <exception cref="System.IO.IOException"></exception>
        [Test]
        public virtual void TestBigDecimal3()
        {
            var bd1 = new Decimal(0);
            var b2 = byteArrayConverter.BigDecimalToByteArray(bd1, true);
            var bd2 = byteArrayConverter.ByteArrayToBigDecimal(b2, true);
            AssertEquals(bd1, bd2);
        }

        /// <exception cref="System.IO.IOException"></exception>
        [Test]
        public virtual void TestBigDecimal4()
        {
            var bd1 = new Decimal(10);
            var b2 = byteArrayConverter.BigDecimalToByteArray(bd1, true);
            var bd2 = byteArrayConverter.ByteArrayToBigDecimal(b2, true);
            AssertEquals(bd1, bd2);
        }

        /// <exception cref="System.IO.IOException"></exception>
        [Test]
        public virtual void TestBigDecimal5()
        {
            var bd1 = new Decimal(0.000);
            var b2 = byteArrayConverter.BigDecimalToByteArray(bd1, true);
            var bd2 = byteArrayConverter.ByteArrayToBigDecimal(b2, true);
            AssertEquals(bd1, bd2);
        }

        /// <exception cref="System.IO.IOException"></exception>
        [Test]
        public virtual void TestBigDecimal6()
        {
            var bd1 = new Decimal(0.000000000000000123456789);
            var b2 = byteArrayConverter.BigDecimalToByteArray(bd1, true);
            var bd2 = byteArrayConverter.ByteArrayToBigDecimal(b2, true);
            AssertEquals(bd1, bd2);
        }

        /// <exception cref="System.IO.IOException"></exception>
        [Test]
        public virtual void TestBigDecimal7()
        {
            var bd1 = new Decimal(-1);
            var b2 = byteArrayConverter.BigDecimalToByteArray(bd1, true);
            var bd2 = byteArrayConverter.ByteArrayToBigDecimal(b2, true);
            AssertEquals(bd1, bd2);
        }

        /// <exception cref="System.IO.IOException"></exception>
        [Test]
        public virtual void TestBigDecimal8()
        {
            var bd1 = new Decimal(-123456789);
            var b2 = byteArrayConverter.BigDecimalToByteArray(bd1, true);
            var bd2 = byteArrayConverter.ByteArrayToBigDecimal(b2, true);
            AssertEquals(bd1, bd2);
        }

        /// <exception cref="System.IO.IOException"></exception>
        [Test]
        public virtual void TestBigDecimal9()
        {
            var bd1 = new Decimal(-0.000000000000000000000000000000123456789);
            var b2 = byteArrayConverter.BigDecimalToByteArray(bd1, true);
            var bd2 = byteArrayConverter.ByteArrayToBigDecimal(b2, true);
            AssertEquals(bd1, bd2);
        }

        /// <exception cref="System.IO.IOException"></exception>
        [Test]
        public virtual void TestBoolean()
        {
            var b1 = true;
            var b2 = byteArrayConverter.BooleanToByteArray(b1);
            var b3 = byteArrayConverter.ByteArrayToBoolean(b2, 0);
            AssertEquals(b1, b3);
            b1 = false;
            b2 = byteArrayConverter.BooleanToByteArray(b1);
            b3 = byteArrayConverter.ByteArrayToBoolean(b2, 0);
            AssertEquals(b1, b3);
        }

        /// <exception cref="System.IO.IOException"></exception>
        [Test]
        public virtual void TestChar()
        {
            var c = '\u00E1';
            var b2 = byteArrayConverter.CharToByteArray(c);
            var c1 = byteArrayConverter.ByteArrayToChar(b2);
            AssertEquals(c, c1);
        }

        [Test]
        public virtual void TestDouble()
        {
            var l1 = 785412.4875;
            var b2 = byteArrayConverter.DoubleToByteArray(l1);
            var l2 = byteArrayConverter.ByteArrayToDouble(b2);
            AssertEquals(l1, l2, 0);
        }

        [Test]
        public virtual void TestFloat()
        {
            var l1 = (float) 785412.4875;
            var b2 = byteArrayConverter.FloatToByteArray(l1);
            var l2 = byteArrayConverter.ByteArrayToFloat(b2);
            AssertEquals(l1, l2, 0);
        }

        [Test]
        public virtual void TestInt()
        {
            var l1 = 785412;
            var b = byteArrayConverter.IntToByteArray(l1);
            var l2 = byteArrayConverter.ByteArrayToInt(b, 0);
            AssertEquals(l1, l2);
        }

        [Test]
        public virtual void TestLong()
        {
            long l1 = 785412;
            var b = byteArrayConverter.LongToByteArray(l1);
            var l2 = byteArrayConverter.ByteArrayToLong(b, 0);
            AssertEquals(l1, l2);
            l1 = long.MaxValue;
            b = byteArrayConverter.LongToByteArray(l1);
            l2 = byteArrayConverter.ByteArrayToLong(b, 0);
            AssertEquals(l1, l2);
            l1 = long.MinValue;
            b = byteArrayConverter.LongToByteArray(l1);
            l2 = byteArrayConverter.ByteArrayToLong(b, 0);
            AssertEquals(l1, l2);
        }

        /// <exception cref="System.IO.IOException"></exception>
        [Test]
        public virtual void TestShort()
        {
            short s = 4598;
            var b2 = byteArrayConverter.ShortToByteArray(s);
            var s2 = byteArrayConverter.ByteArrayToShort(b2);
            // assertEquals(s,s2);
            s = 10000;
            b2 = byteArrayConverter.ShortToByteArray(s);
            s2 = byteArrayConverter.ByteArrayToShort(b2);
            AssertEquals(s, s2);
            s = short.MaxValue;
            b2 = byteArrayConverter.ShortToByteArray(s);
            s2 = byteArrayConverter.ByteArrayToShort(b2);
            AssertEquals(s, s2);
            s = short.MinValue;
            b2 = byteArrayConverter.ShortToByteArray(s);
            s2 = byteArrayConverter.ByteArrayToShort(b2);
            AssertEquals(s, s2);
        }

        /// <exception cref="System.IO.IOException"></exception>
        [Test]
        public virtual void TestString()
        {
            var s = "test1";
            var b2 = byteArrayConverter.StringToByteArray(s, true, -1, true);
            var s2 = byteArrayConverter.ByteArrayToString(b2, true, true);
            AssertEquals(s, s2);
        }
    }
}
