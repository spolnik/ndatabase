using NDatabase.Odb;
using NDatabase.Odb.Core.Layers.Layer2.Instance;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Tool;
using NDatabase.Tool.Wrappers;
using NDatabase.Tool.Wrappers.IO;
using NUnit.Framework;
using Test.Odb.Test.VO.Login;

namespace Test.Odb.Test.Performance
{
	public class PerformanceTest2
	{
		public static int TestSize = 10000;

		public static readonly string OdbFileName = "perf.neodatis";

		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void TestInsertSimpleObjectODB()
		{
			bool inMemory = true;
			// Deletes the database file
			OdbFile.DeleteFile(OdbFileName);
			long t1 = 0;
			long t2 = 0;
			long t3 = 0;
			long t4 = 0;
			long t5 = 0;
			long t6 = 0;
			long t7 = 0;
			long t77 = 0;
			long t8 = 0;
			IOdb odb = null;
			IObjects<User> l = null;
			User so = null;
			// Insert TEST_SIZE objects
			System.Console.Out.WriteLine("Inserting " + TestSize + " objects");
			t1 = OdbTime.GetCurrentTimeInTicks();
			odb = OdbFactory.Open(OdbFileName);
			odb.GetClassRepresentation(typeof(User)).AddFullInstantiationHelper
				(new UserFullInstantiationHelper());
			for (int i = 0; i < TestSize; i++)
			{
				object o = GetUserInstance(i);
				odb.Store(o);
			}
			t2 = OdbTime.GetCurrentTimeInTicks();
			// Closes the database
			odb.Close();
			t3 = OdbTime.GetCurrentTimeInTicks();
			System.Console.Out.WriteLine("Retrieving " + TestSize + " objects");
			// Reopen the database
			odb = OdbFactory.Open(OdbFileName);
			// Gets retrieve the TEST_SIZE objects
			l = odb.GetObjects<User>(inMemory);
			t4 = OdbTime.GetCurrentTimeInTicks();
			// Actually get objects
			while (l.HasNext())
			{
				object o = l.Next();
			}
			t5 = OdbTime.GetCurrentTimeInTicks();
			System.Console.Out.WriteLine("Updating " + TestSize + " objects");
			so = null;
			l.Reset();
			// Actually get objects
			while (l.HasNext())
			{
				so = (User)l.Next();
				// so.setName(so.getName() + " updated");
				// so.setName(so.getName() + " updated-updated-updated-updated");
				so.GetProfile().SetName(so.GetName() + " updated-updated-updated");
				odb.Store(so);
			}
			t6 = OdbTime.GetCurrentTimeInTicks();
			odb.Close();
			t7 = OdbTime.GetCurrentTimeInTicks();
			System.Console.Out.WriteLine("Deleting " + TestSize + " objects");
			odb = OdbFactory.Open(OdbFileName);
			l = odb.GetObjects<User>(inMemory);
			t77 = OdbTime.GetCurrentTimeInTicks();
			// Actually get objects
			while (l.HasNext())
			{
				so = (User)l.Next();
				odb.Delete(so);
			}
			odb.Close();
			t8 = OdbTime.GetCurrentTimeInTicks();
			odb = OdbFactory.Open(OdbFileName);
			odb.Close();
			DisplayResult("ODB " + TestSize + " User objects ", t1, t2, t3, t4, t5, t6, t7, t77
				, t8);
		}

		private object GetUserInstance(int i)
		{
			VO.Login.Function login = new VO.Login.Function("login" + i);
			VO.Login.Function logout = new VO.Login.Function("logout" + i);
            System.Collections.Generic.List<VO.Login.Function> list = new System.Collections.Generic.List<VO.Login.Function>();
			list.Add(login);
			list.Add(logout);
			Profile profile = new Profile("operator" + i, list);
			User user = new User("olivier smadja" + i, "olivier@neodatis.com", profile);
			return user;
		}

		private void DisplayResult(string @string, long t1, long t2, long t3, long t4, long
			 t5, long t6, long t7, long t77, long t8)
		{
			string s1 = " total=" + (t8 - t1);
			string s2 = " total insert=" + (t3 - t1) + " -- " + "insert=" + (t2 - t1) + " commit="
				 + (t3 - t2);
			string s3 = " total select=" + (t5 - t3) + " -- " + "select=" + (t4 - t3) + " get="
				 + (t5 - t4);
			string s4 = " total update=" + (t7 - t5) + " -- " + "update=" + (t6 - t5) + " commit="
				 + (t7 - t6);
			string s5 = " total delete=" + (t8 - t7) + " -- " + "select=" + (t77 - t7) + " - delete="
				 + (t8 - t77);
			System.Console.Out.WriteLine(@string + s1 + " | " + s2 + " | " + s3 + " | " + s4 
				+ " | " + s5);
		}

		/// <exception cref="System.Exception"></exception>
		public static void Main2(string[] args)
		{
			PerformanceTest2 pt = new PerformanceTest2
				();
			pt.TestInsertSimpleObjectODB();
		}
	}

	internal class UserFullInstantiationHelper : IFullInstantiationHelper
	{
		public virtual object Instantiate(NonNativeObjectInfo
			 nnoi)
		{
			User user = new User();
			return user;
		}
	}
}
