using NDatabase.Odb;
using NUnit.Framework;
using Test.Odb.Test.VO.Attribute;
using Test.Odb.Test.VO.Login;

namespace Test.Odb.Test.Acid
{
	[TestFixture]
    public class TestConsistency : ODBTest
	{
		public static string OdbFileName = "consistency.neodatis";

		/// <exception cref="System.Exception"></exception>
		public virtual void CreateInconsistentFile()
		{
			IOdb odb = Open(OdbFileName);
			for (int i = 0; i < 10; i++)
			{
				object o = GetUserInstance();
				odb.Store(o);
			}
			odb.Close();
			odb = Open(OdbFileName);
			for (int i = 0; i < 10; i++)
			{
				object o = GetUserInstance();
				odb.Store(o);
			}
		}

		private TestClass GetTestClassInstance()
		{
			TestClass tc = new TestClass
				();
			tc.SetBigDecimal1(new System.Decimal(1.123456789));
			tc.SetBoolean1(true);
			tc.SetChar1('d');
			tc.SetDouble1(154.78998989);
			tc.SetInt1(78964);
			tc.SetString1("Ola chico como vc est√° ???");
			tc.SetDate1(new System.DateTime());
			return tc;
		}

		private object GetUserInstance()
		{
			VO.Login.Function login = new VO.Login.Function
				("login");
			VO.Login.Function logout = new VO.Login.Function
				("logout");
			System.Collections.Generic.IList<VO.Login.Function> list = new 
				System.Collections.Generic.List<VO.Login.Function>();
			list.Add(login);
			list.Add(logout);
			Profile profile = new Profile
				("operator", list);
			User user = new User("olivier smadja"
				, "olivier@neodatis.com", profile);
			return user;
		}
		[Test]
        public virtual void Test1()
		{
			AssertTrue(true);
		}

		/// <exception cref="System.Exception"></exception>
		public static void Main2(string[] args)
		{
			new TestConsistency().CreateInconsistentFile();
		}
		// new TestConsistency().openFile();
	}
}
