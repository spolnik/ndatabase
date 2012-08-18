using NDatabase.Odb;
using NUnit.Framework;
using Test.Odb.Test.VO.Attribute;
using Test.Odb.Test.VO.Login;

namespace Test.Odb.Test.Insert
{
	[TestFixture]
    public class TestInsert : ODBTest
	{
		
        [Test]
        public virtual void TestCompositeCollection2DifferentObjects()
		{
			DeleteBase("ti1.neodatis");
			IOdb odb = Open("ti1.neodatis");
			int nbUsers = odb.GetObjects<User>(true).Count;
			int nbProfiles = odb.GetObjects<Profile>(true).Count;
			int nbFunctions = odb.GetObjects<VO.Login.Function>(true).Count;
			VO.Login.Function login = new VO.Login.Function("login");
			VO.Login.Function logout = new VO.Login.Function("logout");
			VO.Login.Function disconnect = new VO.Login.Function("disconnect");
			System.Collections.Generic.IList<VO.Login.Function> list = new System.Collections.Generic.List<VO.Login.Function>();
			list.Add(login);
			list.Add(logout);
            System.Collections.Generic.IList<VO.Login.Function> list2 = new System.Collections.Generic.List<VO.Login.Function>();
			list.Add(login);
			list.Add(logout);
			Profile profile1 = new Profile
				("operator 1", list);
			Profile profile2 = new Profile
				("operator 2", list2);
			User user = new User("olivier smadja"
				, "olivier@neodatis.com", profile1);
			User userB = new User("A√°sa Galv√£o Smadja"
				, "aisa@neodatis.com", profile2);
			odb.Store(user);
			odb.Store(userB);
			odb.Commit();
			IObjects<VO.Login.Function> functions = odb.GetObjects<VO.Login.Function>(true);
			IObjects<Profile> profiles = odb.GetObjects<Profile>(true);
			IObjects<User> users = odb.GetObjects<User>(true);
			odb.Close();
			// assertEquals(nbUsers+2,users.size());
			User user2 = (User)users.GetFirst
				();
			AssertEquals(user.ToString(), user2.ToString());
			AssertEquals(nbProfiles + 2, profiles.Count);
			AssertEquals(nbFunctions + 2, functions.Count);
			DeleteBase("ti1.neodatis");
		}

		
		[Test]
        public virtual void TestCompositeCollection1()
		{
			DeleteBase("t31.neodatis");
			IOdb odb = Open("t31.neodatis");
			VO.Login.Function login = new VO.Login.Function
				("login");
			System.Collections.Generic.IList<VO.Login.Function> list = new System.Collections.Generic.List<VO.Login.Function>();
			list.Add(login);
			Profile profile1 = new Profile
				("operator 1", list);
			User user = new User("olivier smadja"
				, "olivier@neodatis.com", profile1);
			odb.Store(user);
			odb.Close();
			odb = Open("t31.neodatis");
			IObjects<User> users = odb.GetObjects<User>(true);
			odb.Close();
			// assertEquals(nbUsers+2,users.size());
			User user2 = (User)users.GetFirst
				();
			AssertEquals(user.ToString(), user2.ToString());
			DeleteBase("t31.neodatis");
		}

		
        [Test]
        public virtual void Test1()
		{
			DeleteBase("t1.neodatis");
			// LogUtil.allOn(true);
			IOdb odb = Open("t1.neodatis");
			// LogUtil.objectWriterOn(true);
			VO.Login.Function login = new VO.Login.Function
				("login");
			System.Collections.Generic.IList<VO.Login.Function> list = new System.Collections.Generic.List<VO.Login.Function>();
			list.Add(login);
			Profile profile1 = new Profile
				("operator 1", list);
			User user = new User("olivier smadja"
				, "olivier@neodatis.com", profile1);
			odb.Store(user);
			odb.Close();
			odb = Open("t1.neodatis");
			IObjects<User> users = odb.GetObjects<User>(true);
			// assertEquals(nbUsers+2,users.size());
			User user2 = (User)users.GetFirst
				();
			odb.Close();
			AssertEquals(user.ToString(), user2.ToString());
			DeleteBase("t1.neodatis");
		}

		
        [Test]
        public virtual void TestCompositeCollection2()
		{
			DeleteBase("t3.neodatis");
			// LogUtil.objectWriterOn(true);
			IOdb odb = Open("t3.neodatis");
			int nbUsers = odb.GetObjects<User>(true).Count;
			int nbProfiles = odb.GetObjects<Profile>(true)
				.Count;
			int nbFunctions = odb.GetObjects<VO.Login.Function>(true).Count;
			VO.Login.Function login = new VO.Login.Function("login");
			VO.Login.Function logout = new VO.Login.Function("logout");
			System.Collections.Generic.IList<VO.Login.Function> list = new System.Collections.Generic.List<VO.Login.Function>();
			list.Add(login);
			list.Add(logout);
			Profile profile1 = new Profile
				("operator 1", list);
			Profile profile2 = new Profile
				("operator 2", list);
			User user = new User("olivier smadja"
				, "olivier@neodatis.com", profile1);
			User userB = new User("A√°sa Galv√£o Smadja"
				, "aisa@neodatis.com", profile2);
			odb.Store(user);
			odb.Store(userB);
			odb.Close();
			odb = Open("t3.neodatis");
			IObjects<User> users = odb.GetObjects<User>(true);
			IObjects<Profile> profiles = odb.GetObjects<Profile>(true);
			IObjects<VO.Login.Function> functions = odb.GetObjects<VO.Login.Function>(true);
			// assertEquals(nbUsers+2,users.size());
			User user2 = (User)users.GetFirst
				();
			AssertEquals(user.ToString(), user2.ToString());
			AssertEquals(nbProfiles + 2, profiles.Count);
			AssertEquals(nbFunctions + 2, functions.Count);
			odb.Close();
			DeleteBase("t3.neodatis");
		}

		
        [Test]
        public virtual void TestCompositeCollection3()
		{
			DeleteBase("t4.neodatis");
			IOdb odb = Open("t4.neodatis");
			// Configuration.addLogId("ObjectWriter");
			// Configuration.addLogId("ObjectReader");
			// Configuration.addLogId("FileSystemInterface");
			int nbUsers = odb.GetObjects<User>(true).Count;
			int nbProfiles = odb.GetObjects<Profile>(true).Count;
			int nbFunctions = odb.GetObjects<VO.Login.Function>(true).Count;
			VO.Login.Function login = new VO.Login.Function("login");
			VO.Login.Function logout = new VO.Login.Function("logout");
			System.Collections.Generic.IList<VO.Login.Function> list = new System.Collections.Generic.List<VO.Login.Function>();
			list.Add(login);
			list.Add(logout);
			Profile profile1 = new Profile
				("operator 1", list);
			User user = new User("olivier smadja"
				, "olivier@neodatis.com", profile1);
			User userB = new User("A√≠sa Galv√£o Smadja"
				, "aisa@neodatis.com", profile1);
			odb.Store(user);
			odb.Store(userB);
			odb.Close();
			odb = Open("t4.neodatis");
			IObjects<User> users = odb.GetObjects<User>(true);
			IObjects<Profile> profiles = odb.GetObjects<Profile>(true);
			IObjects<VO.Login.Function> functions = odb.GetObjects<VO.Login.Function>(true);
			// assertEquals(nbUsers+2,users.size());
			User user2 = (User)users.GetFirst
				();
			AssertEquals(user.ToString(), user2.ToString());
			AssertEquals(nbProfiles + 1, profiles.Count);
			AssertEquals(nbFunctions + 2, functions.Count);
			odb.Close();
			DeleteBase("t4.neodatis");
		}

		
        [Test]
        public virtual void TestCompositeCollection4()
		{
			DeleteBase("t5.neodatis");
			IOdb odb = Open("t5.neodatis");
			int nbUsers = odb.GetObjects<User>(true).Count;
			int nbProfiles = odb.GetObjects<Profile>(true).Count;
			int nbFunctions = odb.GetObjects<VO.Login.Function>(true).Count;
			VO.Login.Function login = new VO.Login.Function("login");
			VO.Login.Function logout = new VO.Login.Function("logout");
			System.Collections.Generic.IList<VO.Login.Function> list = new System.Collections.Generic.List<VO.Login.Function>();
			list.Add(login);
			list.Add(logout);
			Profile profile1 = new Profile
				("operator 1", list);
			User user = new User("olivier smadja"
				, "olivier@neodatis.com", profile1);
			User userB = new User("A√≠sa Galv√£o Smadja"
				, "aisa@neodatis.com", profile1);
			odb.Store(user);
			odb.Store(userB);
			odb.Commit();
			IObjects<User> users = odb.GetObjects<User>(true);
			IObjects<Profile> profiles = odb.GetObjects<Profile>(true);
			IObjects<VO.Login.Function> functions = odb.GetObjects<VO.Login.Function>(true);
			odb.Close();
			// assertEquals(nbUsers+2,users.size());
			User user2 = (User)users.GetFirst
				();
			AssertEquals(user.ToString(), user2.ToString());
			AssertEquals(nbProfiles + 1, profiles.Count);
			AssertEquals(nbFunctions + 2, functions.Count);
		}

		// deleteBase("t5.neodatis");
		
        [Test]
        public virtual void TestSimple()
		{
			DeleteBase("t2.neodatis");
			IOdb odb = Open("t2.neodatis");
			int nbFunctions = odb.GetObjects<VO.Login.Function>(true).Count;
			VO.Login.Function login = new VO.Login.Function("login");
			VO.Login.Function logout = new VO.Login.Function("logout");
			odb.Store(login);
			odb.Store(logout);
			odb.Close();
			odb = Open("t2.neodatis");
			IObjects<VO.Login.Function> functions = odb.GetObjects<VO.Login.Function>(true);
			VO.Login.Function f1 = (VO.Login.Function)functions.GetFirst();
			f1.SetName("login1");
			odb.Store(f1);
			odb.Close();
			odb = Open("t2.neodatis");
			functions = odb.GetObjects<VO.Login.Function>(true);
			odb.Close();
			AssertEquals(2, functions.Count);
			AssertEquals("login1", ((VO.Login.Function)functions.GetFirst()
				).GetName());
			DeleteBase("t2.neodatis");
		}

		
        [Test]
        public virtual void TestBufferSize()
		{
			int size = OdbConfiguration.GetDefaultBufferSizeForData();
			OdbConfiguration.SetDefaultBufferSizeForData(5);
			DeleteBase("ti1.neodatis");
			IOdb odb = Open("ti1.neodatis");
			System.Text.StringBuilder b = new System.Text.StringBuilder();
			for (int i = 0; i < 1000; i++)
			{
				b.Append("login - login ");
			}
			VO.Login.Function login = new VO.Login.Function
				(b.ToString());
			Profile profile1 = new Profile
				("operator 1", login);
			User user = new User("olivier smadja"
				, "olivier@neodatis.com", profile1);
			odb.Store(user);
			odb.Commit();
			IObjects<User> users = odb.GetObjects<User>(true);
			IObjects<Profile> profiles = odb.GetObjects<Profile>(true);
			IObjects<VO.Login.Function> functions = odb.GetObjects<VO.Login.Function>(true);
			odb.Close();
			// assertEquals(nbUsers+2,users.size());
			User user2 = (User)users.GetFirst
				();
			AssertEquals(user.ToString(), user2.ToString());
		    var enumerator = user2.GetProfile().GetFunctions().GetEnumerator();
		    enumerator.MoveNext();
		    AssertEquals(b.ToString(), enumerator.Current.ToString());
			DeleteBase("ti1.neodatis");
			OdbConfiguration.SetDefaultBufferSizeForData(size);
		}

		

        [Test]
        public virtual void TestDatePersistence()
		{
			IOdb odb = null;
			DeleteBase("date.neodatis");
			try
			{
				odb = Open("date.neodatis");
				TestClass tc1 = new TestClass
					();
				tc1.SetDate1(new System.DateTime());
				long t1 = tc1.GetDate1().Millisecond;
				odb.Store(tc1);
				odb.Close();
				odb = Open("date.neodatis");
				IObjects<TestClass> l = odb.GetObjects<TestClass>();
				AssertEquals(1, l.Count);
				TestClass tc2 = (TestClass)l.GetFirst();
				AssertEquals(t1, tc2.GetDate1().Millisecond);
				AssertEquals(tc1.GetDate1(), tc2.GetDate1());
			}
			finally
			{
				if (odb != null)
				{
					odb.Close();
				}
			}
			DeleteBase("date.neodatis");
		}

		
        [Test]
        public virtual void TestStringPersistence()
		{
			IOdb odb = null;
            DeleteBase("date.neodatis");
			try
			{
				odb = Open("date.neodatis");
				TestClass tc1 = new TestClass
					();
				tc1.SetString1(string.Empty);
				odb.Store(tc1);
				odb.Close();
				odb = Open("date.neodatis");
				IObjects<TestClass> l = odb.GetObjects<TestClass>();
				AssertEquals(1, l.Count);
				TestClass tc2 = (TestClass
					)l.GetFirst();
				AssertEquals(string.Empty, tc2.GetString1());
				AssertEquals(0m, tc2.GetBigDecimal1());
				AssertEquals(0d, tc2.GetDouble1());
			}
			finally
			{
				if (odb != null)
				{
					odb.Close();
				}
			}
		}

		
        [Test]
        public virtual void Test6()
		{
			DeleteBase("t1u.neodatis");
			IOdb odb = Open("t1u.neodatis");
			VO.Login.Function login = new VO.Login.Function
				("login");
			VO.Login.Function logout = new VO.Login.Function
				("logout");
			odb.Store(login);
			odb.Store(logout);
			odb.Close();
			odb = Open("t1u.neodatis");
			VO.Login.Function login2 = new VO.Login.Function
				("login2");
			VO.Login.Function logout2 = new VO.Login.Function
				("logout2");
			odb.Store(login2);
			odb.Store(logout2);
			// select without committing
			IObjects<VO.Login.Function> l = odb.GetObjects<VO.Login.Function>(true);
			AssertEquals(4, l.Count);
			// println(l);
			odb.Close();
			odb = Open("t1u.neodatis");
			l = odb.GetObjects<VO.Login.Function>(true);
			AssertEquals(4, l.Count);
			// println(l);
			odb.Close();
		}

		
        [Test]
        public virtual void Test7()
		{
			DeleteBase("t1u.neodatis");
			IOdb odb = Open("t1u.neodatis");
			VO.Login.Function login = new VO.Login.Function("login");
			VO.Login.Function logout = new VO.Login.Function("logout");
			odb.Store(login);
			odb.Store(logout);
			odb.Commit();
			VO.Login.Function input = new VO.Login.Function("input");
			odb.Store(input);
			odb.Close();
			odb = Open("t1u.neodatis");
			IObjects<VO.Login.Function> l = odb.GetObjects<VO.Login.Function>(true);
			AssertEquals(3, l.Count);
			// println(l);
			odb.Close();
		}
        
		/// <summary>Test with java util Date and java sql Date</summary>
        [Test]
        public virtual void Test8()
		{
			string baseName = GetBaseName();
			Println(baseName);
			IOdb odb = null;
			System.DateTime utilDate = new System.DateTime();
			System.DateTime sqlDate = new System.DateTime(utilDate.Millisecond + 10000);
			System.DateTime timestamp = new System.DateTime(utilDate.Millisecond + 20000);
			try
			{
				odb = Open(baseName);
				ObjectWithDates o = new ObjectWithDates
					("object1", utilDate, sqlDate, timestamp);
				odb.Store(o);
				odb.Close();
				odb = Open(baseName);
				IObjects<ObjectWithDates> dates = odb.
					GetObjects<ObjectWithDates>();
				ObjectWithDates o2 = dates.GetFirst();
				Println(o2.GetName());
				Println(o2.GetJavaUtilDate());
				Println(o2.GetJavaSqlDte());
				Println(o2.GetTimestamp());
				AssertEquals("object1", o2.GetName());
				AssertEquals(utilDate, o2.GetJavaUtilDate());
				AssertEquals(sqlDate, o2.GetJavaSqlDte());
				AssertEquals(timestamp, o2.GetTimestamp());
			}
			finally
			{
				if (odb != null)
				{
					odb.Close();
				}
			}
		}
	}
}
