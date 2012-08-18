using NDatabase.Odb;
using NDatabase.Tool.Wrappers.Map;
using NUnit.Framework;
using System;
using Test.Odb.Test.VO.Login;

namespace Test.Odb.Test.Commit
{
	[TestFixture]
    public class TestCommit : ODBTest
	{
		
		public override void SetUp()
		{
			base.SetUp();
		}

		
		[Test]
        public virtual void TestInsertWithCommitsSimpleObject()
		{
			DeleteBase("commits");
			IOdb odb = null;
			int size = 10000;
			int commitInterval = 1000;
			try
			{
				odb = Open("commits");
				for (int i = 0; i < size; i++)
				{
					odb.Store(new VO.Login.Function("function " + i));
					if (i % commitInterval == 0)
					{
						odb.Commit();
                        Console.WriteLine(i);
					}
				}
			}
			finally
			{
				// println("commiting "+i);
				odb.Close();
			}
			odb = Open("commits");
			IObjects<VO.Login.Function> objects = odb.GetObjects<VO.Login.Function>();
			int nbObjects = objects.Count;
            OdbHashMap<VO.Login.Function,int> map = new OdbHashMap<VO.Login.Function,int>();
			VO.Login.Function function = null;
			int j = 0;
			while (objects.HasNext())
			{
				function = (VO.Login.Function)objects.Next();
				int ii = (int)map[function];
				if (ii != 0)
				{
					Println(j + ":" + function.GetName() + " already exist at " + ii);
				}
				else
				{
					map.Add(function, j);
				}
				j++;
			}
			odb.Close();
			DeleteBase("commits");
			Println("Nb objects=" + nbObjects);
			AssertEquals(size, nbObjects);
		}

		
		[Test]
        public virtual void TestInsertWithCommitsComplexObject()
		{
			DeleteBase("commits");
			IOdb odb = null;
			int size = 5300;
			int commitInterval = 400;
			try
			{
				odb = Open("commits");
				for (int i = 0; i < size; i++)
				{
					odb.Store(GetInstance(i));
					if (i % commitInterval == 0)
					{
						odb.Commit();
					}
					// println("commiting "+i);
                    if(i%1000==0)
                    {
						Println(i);
					}
				}
			}
			finally
			{
				odb.Close();
			}
			odb = Open("commits");
			IObjects<User> users = odb.GetObjects<User>();
			IObjects<Profile> profiles = odb.GetObjects<Profile>();
			IObjects<VO.Login.Function> functions = odb.GetObjects<VO.Login.Function>();
			int nbUsers = users.Count;
			int nbProfiles = profiles.Count;
			int nbFunctions = functions.Count;
			odb.Close();
			DeleteBase("commits");
			Println("Nb users=" + nbUsers);
			Println("Nb profiles=" + nbProfiles);
			Println("Nb functions=" + nbFunctions);
			AssertEquals(size, nbUsers);
			AssertEquals(size, nbProfiles);
			AssertEquals(size * 2, nbFunctions);
		}

		private object GetInstance(int i)
		{
			VO.Login.Function login = new VO.Login.Function
				("login" + i);
			VO.Login.Function logout = new VO.Login.Function
				("logout" + i);
			System.Collections.Generic.IList<VO.Login.Function> list = new System.Collections.Generic.List<VO.Login.Function>();
			list.Add(login);
			list.Add(logout);
			Profile profile = new Profile
				("operator" + i, list);
			User user = new User("olivier"
				 + i, "olivier@neodatis.com", profile);
			return user;
		}
	}
}
