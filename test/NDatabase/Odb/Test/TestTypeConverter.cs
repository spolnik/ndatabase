using System;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NUnit.Framework;

namespace Test.Odb.Test
{
    /// <author>olivier s</author>
    [TestFixture]
    public class TestTypeConverter : ODBTest
    {
        [Test]
        public virtual void Test2()
        {
            AssertEquals(OdbType.Integer, OdbType.GetFromClass(typeof (int)));
            AssertEquals(OdbType.Boolean, OdbType.GetFromClass(typeof (bool)));
            AssertEquals(OdbType.Byte, OdbType.GetFromClass(typeof (byte)));
            AssertEquals(OdbType.Character, OdbType.GetFromClass(typeof (char)));
            AssertEquals(OdbType.Double, OdbType.GetFromClass(typeof (double)));
            AssertEquals(OdbType.Float, OdbType.GetFromClass(typeof (float)));
            AssertEquals(OdbType.Long, OdbType.GetFromClass(typeof (long)));
            AssertEquals(OdbType.Short, OdbType.GetFromClass(typeof (short)));
            AssertEquals(OdbType.String, OdbType.GetFromClass(typeof (string)));
            AssertEquals(OdbType.BigDecimal, OdbType.GetFromClass(typeof (Decimal)));
        }

        [Test]
        public virtual void Test3()
        {
            var array1 = new[] {1, 2};
            AssertEquals(OdbType.Array, OdbType.GetFromClass(array1.GetType()));
            AssertEquals(OdbType.Integer, OdbType.GetFromClass(array1.GetType()).GetSubType());
            var array2 = new[] {"1", "2"};
            AssertEquals(OdbType.Array, OdbType.GetFromClass(array2.GetType()));
            AssertEquals(OdbType.String, OdbType.GetFromClass(array2.GetType()).GetSubType());
        }

        [Test]
        public virtual void Test4()
        {
            // println(int.class.getName());
            AssertEquals(typeof (int), typeof (int));
        }
    }
}
