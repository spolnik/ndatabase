using System;
using System.Text.RegularExpressions;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Impl.Core.Layers.Layer2.Meta.Serialization;
using NUnit.Framework;
using Test.Odb.Test;

namespace Serialization
{
    [TestFixture]
    public class TestSerialization : ODBTest
    {
        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void TestAtomicNativeCollectionBigDecimal()
        {
            var bd = Convert.ToDecimal("123456789.987654321");
            AtomicNativeObjectInfo anoi = null;
            anoi = new AtomicNativeObjectInfo(bd, OdbType.BigDecimalId);
            var s = Serializer.GetInstance().ToString(anoi);
            // println(s);
            var anoi2 = (AtomicNativeObjectInfo) Serializer.GetInstance().FromOneString(s);
            AssertEquals(anoi, anoi2);
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void TestAtomicNativeCollectionDate()
        {
            var date = new DateTime();
            AtomicNativeObjectInfo anoi = null;
            anoi = new AtomicNativeObjectInfo(date, OdbType.DateId);
            var s = Serializer.GetInstance().ToString(anoi);
            // println(s);
            var anoi2 = (AtomicNativeObjectInfo) Serializer.GetInstance().FromOneString(s);
            AssertEquals(anoi, anoi2);
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void TestAtomicNativeCollectionDouble()
        {
            var d = 123456789.789456123;
            AtomicNativeObjectInfo anoi = null;
            anoi = new AtomicNativeObjectInfo(d, OdbType.DoubleId);
            var s = Serializer.GetInstance().ToString(anoi);
            // println(s);
            var anoi2 = (AtomicNativeObjectInfo) Serializer.GetInstance().FromOneString(s);
            AssertEquals(anoi, anoi2);
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void TestAtomicNativeCollectionInt()
        {
            var i = 123456789;
            AtomicNativeObjectInfo anoi = null;
            anoi = new AtomicNativeObjectInfo(i, OdbType.IntegerId);
            var s = Serializer.GetInstance().ToString(anoi);
            // println(s);
            var anoi2 = (AtomicNativeObjectInfo) Serializer.GetInstance().FromOneString(s);
            AssertEquals(anoi, anoi2);
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void TestAtomicNativeCollectionString()
        {
            var s1 = "ol√° chico";
            AtomicNativeObjectInfo anoi = null;
            anoi = new AtomicNativeObjectInfo(s1, OdbType.StringId);
            var s = Serializer.GetInstance().ToString(anoi);
            // println(s);
            var anoi2 = (AtomicNativeObjectInfo) Serializer.GetInstance().FromOneString(s);
            AssertEquals(anoi, anoi2);
        }

        [Test]
        public virtual void TestRegExp()
        {
            // println("start");
            var token = "A;B;[C;D];E";
            // (*)&&^(*^)
            var pattern = "[\\[*\\]]";
            Regex p = new Regex(pattern);
            var array = p.Split(token);
            var m = p.Match(token);
            Console.WriteLine(token);
            Console.WriteLine(m.Groups.Count);

            foreach (var item in array)
                Console.WriteLine(item);

            Assert.Fail("TODO");
        }
    }
}
