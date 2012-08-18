using NDatabase.Odb;
using NDatabase.Odb.Impl.Core.Query.Criteria;
using NDatabase.Tool.Wrappers;
using NUnit.Framework;
using Test.Odb.Test.VO.Login;

namespace Test.Odb.Test.Performance
{
	[TestFixture]
    public class TestLazy : ODBTest
	{
		public const int Size = 4000;

		public static readonly string Filename = "lazy.neodatis";

		/// <summary>Test the timeof lazy get</summary>
		
		[Test]
        public virtual void Test1()
		{
			DeleteBase(Filename);
			// println("Start inserting " + SIZE + " objects");
			long startinsert = OdbTime.GetCurrentTimeInTicks();
			IOdb odb = Open(Filename);
			for (int i = 0; i < Size; i++)
			{
				odb.Store(GetInstance());
			}
			odb.Close();
			long endinsert = OdbTime.GetCurrentTimeInTicks();
			// println("End inserting " + SIZE + " objects  - " +
			// (endinsert-startinsert) + " ms");
			// println("totalObjects = "+ odb.count(User.class));
			odb = Open(Filename);
			long start1 = OdbTime.GetCurrentTimeInTicks();
			IObjects<User> lazyList = odb.GetObjects<User>(false);
			long end1 = OdbTime.GetCurrentTimeInTicks();
			long startget1 = OdbTime.GetCurrentTimeInTicks();
			while (lazyList.HasNext())
			{
				// t1 = OdbTime.getCurrentTimeInMs();
				lazyList.Next();
			}
			// t2 = OdbTime.getCurrentTimeInMs();
			// println(t2-t1);
			long endget1 = OdbTime.GetCurrentTimeInTicks();
			AssertEquals(odb.Count(new CriteriaQuery(typeof(
				User))), lazyList.Count);
			odb.Close();
			long t01 = end1 - start1;
			long tget1 = endget1 - startget1;
			// long t2 = end2-start2;
			// long tget2 = endget2-startget2;
			// println("t1(lazy)="+t1 + " - " +tget1+ "      t2(memory)="+t2 +" - "
			// + tget2);
			// println("t1(lazy)="+t1 + " - " +tget1);
			// assertTrue(t1<t2);
			// println(endinsert-startinsert);
			bool c = 501 > tget1;
			Println("Time for " + Size + " lazy gets : " + (tget1));
			if (!c)
			{
				Println("Time for " + Size + " lazy gets : " + (tget1));
			}
			DeleteBase(Filename);
			if (testPerformance && !c)
			{
				Fail("Time for " + Size + " lazy gets : " + (tget1));
			}
		}

		private object GetInstance()
		{
			VO.Login.Function login = new VO.Login.Function
				("login");
			VO.Login.Function logout = new VO.Login.Function
				("logout");
			System.Collections.Generic.List<VO.Login.Function> list = new System.Collections.Generic.List<VO.Login.Function>();
			list.Add(login);
			list.Add(logout);
			Profile profile = new Profile
				("operator", list);
			User user = new User("olivier smadja"
				, "olivier@neodatis.com", profile);
			return user;
		}
	}
}
