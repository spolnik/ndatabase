using System;
using NDatabase.Tool.Wrappers;
using NUnit.Framework;

namespace Test.Odb.Test.Intropector
{
    [TestFixture]
    public class TestArray : ODBTest
    {
        [Test]
        public virtual void Test4()
        {
            var o = Array.CreateInstance(typeof (int), 5);
            o.SetValue(1, 0);
            o.SetValue(2, 1);
            AssertEquals(true, o.GetType().IsArray);
            AssertEquals("System.Int32", o.GetType().GetElementType().FullName);
            AssertEquals(1, OdbArray.GetArrayElement(o, 0));
            AssertEquals(2, OdbArray.GetArrayElement(o, 1));
        }
    }
}
