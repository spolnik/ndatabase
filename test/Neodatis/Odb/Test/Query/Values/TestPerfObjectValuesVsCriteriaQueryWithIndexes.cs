using System;
using NDatabase.Odb;
using NDatabase.Odb.Core.Query;
using NDatabase.Odb.Core.Query.Criteria;
using NDatabase.Odb.Impl.Core.Query.Criteria;
using NDatabase.Odb.Impl.Core.Query.Values;
using NDatabase.Tool.Wrappers;
using NUnit.Framework;
using Test.Odb.Test;
using Test.Odb.Test.VO.Login;

namespace Query.Values
{
	[TestFixture]
    public class TestPerfObjectValuesVsCriteriaQueryWithIndexes : ODBTest
	{
        [Test]
		public virtual void Populate()
		{
			IOdb odb = Open("perfOValuesVsCriteriaIndex");
			string[] atts = new string[] { "name" };
			try
			{
				odb.GetClassRepresentation(typeof(User2)).AddUniqueIndexOn
					("Index", atts, true);
			}
			catch (System.Exception)
			{
			}
			// TODO: handle exception
			int nbProfiles = 200;
			int nbUsers = 500000;
			Profile[] profiles = new Profile
				[nbProfiles];
			User2[] users = new User2[nbUsers
				];
			int userStart = 1500000;
			int profileStart = 600;
			// First creates profiles
			for (int i = 0; i < nbProfiles; i++)
			{
				profiles[i] = new Profile("profile " + (i + profileStart
					), new Function("function Profile" + i));
				odb.Store(profiles[i]);
			}
			// Then creates users
			for (int i = 0; i < nbUsers; i++)
			{
				users[i] = new User2("user" + (i + userStart), "user mail"
					 + i, profiles[GetProfileIndex(nbProfiles)], i);
				odb.Store(users[i]);
				if (i % 10000 == 0)
				{
					Println(i);
				}
			}
			odb.Close();
		}

		private int GetProfileIndex(int nbProfiles)
		{
            return OdbRandom.GetRandomInteger() * nbProfiles;
		}

		/// <exception cref="System.Exception"></exception>
		public static void Main2(string[] args)
		{
			Query.Values.TestPerfObjectValuesVsCriteriaQueryWithIndexes t = 
				new Query.Values.TestPerfObjectValuesVsCriteriaQueryWithIndexes
				();
			// t.populate();
			t.T1est1();
			t.T1estA();
		}

        [Test]
		public virtual void T1est()
		{
			IOdb odb = Open("perfOValuesVsCriteriaIndex");
			OdbConfiguration.MonitorMemory(true);
			IQuery q = new CriteriaQuery(typeof(User2));
			System.Decimal b = odb.Count(new CriteriaQuery
				(typeof(User2)));
			Println(b);
			System.Console.Out.WriteLine(q.GetExecutionPlan().GetDetails());
			AssertEquals(Convert.ToDecimal("500000"), b);
			odb.Close();
		}

		[Test]
		public virtual void T1estA()
		{
			IOdb odb = Open("perfOValuesVsCriteriaIndex");
			OdbConfiguration.MonitorMemory(true);
			IQuery q = new CriteriaQuery
				(typeof(User2), Where
				.Equal("name", "user1999999"));
			var objects = odb.GetObjects<User2>(q, false);
			Println(objects.Count);
			System.Console.Out.WriteLine(q.GetExecutionPlan().GetDetails());
			AssertEquals(1, objects.Count);
			objects = odb.GetObjects<User2>(q, false);
			Println(objects.Count);
			System.Console.Out.WriteLine(q.GetExecutionPlan().GetDetails());
			AssertEquals(1, objects.Count);
			odb.Close();
		}

		[Test]
		public virtual void T1est1()
		{
			IOdb odb = Open("perfOValuesVsCriteriaIndex");
			OdbConfiguration.MonitorMemory(true);
			IValuesQuery q = new ValuesCriteriaQuery
				(typeof(User2), Where.Equal("name", "user1999999")).Field("name");
			IValues v = odb.GetValues(q);
			Println(v.Count);
			System.Console.Out.WriteLine(q.GetExecutionPlan().GetDetails());
			AssertEquals(1, v.Count);
			odb.Close();
		}
	}
}
