using System;
using NDatabase.Tool.Wrappers;
using NUnit.Framework;
using Test.Odb.Test;

namespace Intropector
{
	[TestFixture]
    public class TestArray : ODBTest
	{
		[Test]
        public virtual void Test1()
		{
			string[] array = new[] { "Ola", "chico" };
			AssertEquals(true, array.GetType().IsArray);
			AssertEquals("[Ljava.lang.String;", array.GetType().FullName);
			AssertEquals("java.lang.String", array.GetType().GetElementType().FullName);
		}

		[Test]
        public virtual void Test2()
		{
			int[] array = new int[] { 1, 2 };
			AssertEquals(true, array.GetType().IsArray);
			AssertEquals("[I", array.GetType().FullName);
			AssertEquals("int", array.GetType().GetElementType().FullName);
		}

		[Test]
        public virtual void Test3()
		{
			double[] array = new double[] { 1, 2 };
			AssertEquals(true, array.GetType().IsArray);
			AssertEquals("[D", array.GetType().FullName);
			AssertEquals("double", array.GetType().GetElementType().FullName);
		}

		[Test]
        public virtual void Test4()
		{
            Array o = System.Array.CreateInstance(typeof(int),5);
			o.SetValue(0, 1);
            o.SetValue(1, 2);
			AssertEquals(true, o.GetType().IsArray);
			AssertEquals("int", o.GetType().GetElementType().FullName);
			AssertEquals(1, OdbArray.GetArrayElement(o, 0));
            AssertEquals(2, OdbArray.GetArrayElement(o, 1));
            
		}
	}
}
