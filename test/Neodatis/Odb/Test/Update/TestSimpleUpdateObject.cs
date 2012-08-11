using NDatabase.Odb;
using NDatabase.Odb.Core.Query.Criteria;
using NDatabase.Odb.Impl.Core.Query.Criteria;
using NUnit.Framework;
using Test.Odb.Test.VO.Login;

namespace Test.Odb.Test.Update
{
	[TestFixture]
    public class TestSimpleUpdateObject : ODBTest
	{
		/// <exception cref="System.Exception"></exception>
		public override void TearDown()
		{
		}

		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void Test1()
		{
			DeleteBase("t1u.neodatis");
			IOdb odb = Open("t1u.neodatis");
			VO.Login.Function login = new VO.Login.Function
				("login");
			VO.Login.Function logout = new VO.Login.Function
				("logout");
			odb.Store(login);
			Println("--------");
			odb.Store(login);
			odb.Store(logout);
			// odb.commit();
			odb.Close();
			odb = Open("t1u.neodatis");
			IObjects<VO.Login.Function> l = odb.GetObjects<VO.Login.Function>(true);
			VO.Login.Function f2 = (VO.Login.Function)l.GetFirst
				();
			f2.SetName("login function");
			odb.Store(f2);
			odb.Close();
			IOdb odb2 = Open("t1u.neodatis");
			VO.Login.Function f = (VO.Login.Function)odb2
				.GetObjects<VO.Login.Function>().GetFirst();
			AssertEquals("login function", f.GetName());
			odb2.Close();
			DeleteBase("t1u.neodatis");
		}

		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void Test2()
		{
			DeleteBase("t2.neodatis");
			IOdb odb = Open("t2.neodatis");
			int nbUsers = odb.GetObjects<User>().Count;
			int nbProfiles = odb.GetObjects<Profile>(true).Count;
			int nbFunctions = odb.GetObjects<VO.Login.Function>(true).Count;
			VO.Login.Function login = new VO.Login.Function
				("login");
			VO.Login.Function logout = new VO.Login.Function
				("logout");
            System.Collections.Generic.IList<VO.Login.Function> list = new System.Collections.Generic.List<VO.Login.Function>();
			list.Add(login);
			list.Add(logout);
			Profile profile = new Profile("operator", list);
			User olivier = new User("olivier smadja"
				, "olivier@neodatis.com", profile);
			User aisa = new User("A√≠sa Galv√£o Smadja"
				, "aisa@neodRMuatis.com", profile);
			odb.Store(olivier);
			odb.Store(aisa);
			odb.Commit();
			IObjects<User> users = odb.GetObjects<User>(true);
			IObjects<Profile> profiles = odb.GetObjects<Profile>(true);
			IObjects<VO.Login.Function> functions = odb.GetObjects<VO.Login.Function>(true);
			odb.Close();
			// println("Users:"+users);
			Println("Profiles:" + profiles);
			Println("Functions:" + functions);
			odb = Open("t2.neodatis");
			IObjects<User> l = odb.GetObjects<User>(true);
			odb.Close();
			AssertEquals(nbUsers + 2, users.Count);
			User user2 = (User)users.GetFirst
				();
			AssertEquals(olivier.ToString(), user2.ToString());
			AssertEquals(nbProfiles + 1, profiles.Count);
			AssertEquals(nbFunctions + 2, functions.Count);
			IOdb odb2 = Open("t2.neodatis");
			IObjects<VO.Login.Function> l2 = odb2.GetObjects<VO.Login.Function>(true);
			VO.Login.Function function = l2.GetFirst();
			function.SetName("login function");
			odb2.Store(function);
			odb2.Close();
			IOdb odb3 = Open("t2.neodatis");
			IObjects<User> l3 = odb3.GetObjects<User>(true);
			int i = 0;
			while (l3.HasNext() && i < System.Math.Min(2, l3.Count))
			{
				User user = (User)l3.Next();
				AssertEquals("login function", string.Empty + user.GetProfile().GetFunctions()[0]
					);
				i++;
			}
			odb3.Close();
			DeleteBase("t2.neodatis");
		}

		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void Test3()
		{
			DeleteBase("t1u2.neodatis");
			IOdb odb = Open("t1u2.neodatis");
			VO.Login.Function login = new VO.Login.Function
				(null);
			odb.Store(login);
			odb.Close();
			odb = Open("t1u2.neodatis");
			login = (VO.Login.Function)odb.GetObjects<VO.Login.Function>(new CriteriaQuery(Where.IsNull("name"))).GetFirst();
			AssertTrue(login.GetName() == null);
			login.SetName("login");
			odb.Store(login);
			odb.Close();
			odb = Open("t1u2.neodatis");
			login = (VO.Login.Function)odb.GetObjects<VO.Login.Function>().GetFirst();
			AssertTrue(login.GetName().Equals("login"));
			odb.Close();
			DeleteBase("t1u2.neodatis");
		}

		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void Test5()
		{
			DeleteBase("t5.neodatis");
			IOdb odb = Open("t5.neodatis");
			long nbFunctions = odb.Count(new CriteriaQuery(typeof(VO.Login.Function)));
			long nbProfiles = odb.Count(new CriteriaQuery(typeof(Profile)));
			long nbUsers = odb.Count(new CriteriaQuery(typeof(User)));
			VO.Login.Function login = new VO.Login.Function
				("login");
			VO.Login.Function logout = new VO.Login.Function
				("logout");
            System.Collections.Generic.List<VO.Login.Function> list = new System.Collections.Generic.List<VO.Login.Function>();
			list.Add(login);
			list.Add(logout);
			Profile profile = new Profile("operator", list);
			User olivier = new User("olivier smadja"
				, "olivier@neodatis.com", profile);
			User aisa = new User("A√≠sa Galv√£o Smadja"
				, "aisa@neodatis.com", profile);
			odb.Store(olivier);
			odb.Store(profile);
			odb.Commit();
			odb.Close();
			odb = Open("t5.neodatis");
			IObjects<User> users = odb.GetObjects<User>(true);
			IObjects<Profile> profiles = odb.GetObjects<Profile>(true);
			IObjects<VO.Login.Function> functions = odb.GetObjects<VO.Login.Function>( true);
			odb.Close();
			AssertEquals(nbUsers + 1, users.Count);
			AssertEquals(nbProfiles + 1, profiles.Count);
			AssertEquals(nbFunctions + 2, functions.Count);
		}

		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void Test6()
		{
			// LogUtil.objectWriterOn(true);
			DeleteBase("t6.neodatis");
			IOdb odb = Open("t6.neodatis");
			VO.Login.Function login = new VO.Login.Function
				("login");
			VO.Login.Function logout = new VO.Login.Function
				("logout");
            System.Collections.Generic.List<VO.Login.Function> list = new System.Collections.Generic.List<VO.Login.Function>();
			list.Add(login);
			list.Add(logout);
			Profile profile = new Profile
				("operator", list);
			User olivier = new User("olivier smadja"
				, "olivier@neodatis.com", profile);
			odb.Store(olivier);
			odb.Close();
			Println("----------");
			odb = Open("t6.neodatis");
			IObjects<User> users = odb.GetObjects<User>(true);
			User u1 = (User)users.GetFirst
				();
			u1.GetProfile().SetName("operator 234567891011121314");
			odb.Store(u1);
			odb.Close();
			odb = Open("t6.neodatis");
			IObjects<Profile> profiles = odb.GetObjects<Profile>(true);
			AssertEquals(1, profiles.Count);
			Profile p1 = (Profile)profiles
				.GetFirst();
			AssertEquals(u1.GetProfile().GetName(), p1.GetName());
		}
	}
}
