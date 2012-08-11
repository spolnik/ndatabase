using NDatabase.Tool.Wrappers;
using NUnit.Framework;

namespace Test.Odb.Test.Tools
{
	/// <author>olivier</author>
	[TestFixture]
    public class TestString : ODBTest
	{
		[Test]
        public virtual void Test4()
		{
			string s = "ola $1 ola $2";
			s = OdbString.ReplaceToken(s, "$", "param", 1);
			AssertEquals("ola param1 ola $2", s);
		}

		[Test]
        public virtual void Test6()
		{
			string s = "ola $1 ola $2 ola $3 ola $4";
			s = OdbString.ReplaceToken(s, "$", "param", 2);
			AssertEquals("ola param1 ola param2 ola $3 ola $4", s);
		}

		[Test]
        public virtual void Test8subString()
		{
			string s = "NDatabase ODB - The open source object database";
			for (int i = 0; i < 10; i++)
			{
				string s1 = s.Substring(i, i + 15 - i);
				string s2 = s.Substring(i, i + 15 - i);
				AssertEquals(s1, s2);
			}
		}

		[Test]
        public virtual void Test9subString()
		{
			string s = "NDatabase ODB - The open source object database";
			string s1 = s.Substring(0, s.Length);
			string s2 = s.Substring(0, s.Length);
			AssertEquals(s1, s2);
		}
	}
}
