using NDatabase.Odb;
using NDatabase.Odb.Core.Query;
using NDatabase.Odb.Core.Query.Criteria;
using NDatabase.Odb.Impl.Core.Query.Criteria;
using NUnit.Framework;
using Test.Odb.Test.VO.Login;
using Test.Odb.Test.VO.Sport;

namespace Test.Odb.Test.Transaction
{
	[TestFixture]
    public class TestInTransaction : ODBTest
	{
		public readonly string BaseName = "transaction";

		/// <summary>Test select objects that are not yet commited</summary>
		/// <exception cref="System.Exception">System.Exception</exception>
		[Test]
        public virtual void TestSelectUnCommitedObject()
		{
			IOdb odb = null;
			try
			{
				DeleteBase(BaseName);
				odb = Open(BaseName);
				for (int i = 0; i < 4; i++)
				{
					odb.Store(new VO.Login.Function("function " + i));
				}
				odb.Close();
				// reopen the database
				odb = Open(BaseName);
				// stores a new function
				odb.Store(new VO.Login.Function("function uncommited"));
				IObjects<VO.Login.Function> functions = odb.GetObjects<VO.Login.Function>();
				AssertEquals(5, functions.Count);
			}
			finally
			{
				if (odb != null)
				{
					odb.Close();
					DeleteBase(BaseName);
				}
			}
		}

		/// <summary>Test select objects that are not yet commited</summary>
		/// <exception cref="System.Exception">System.Exception</exception>
		[Test]
        public virtual void TestSelectUnCommitedObject2()
		{
			IOdb odb = null;
			try
			{
				DeleteBase(BaseName);
				odb = Open(BaseName);
				for (int i = 0; i < 4; i++)
				{
					odb.Store(new User("user" + i, "email" + i, new Profile
						("profile" + i, new VO.Login.Function("function" + i))));
				}
				odb.Close();
				// reopen the database
				odb = Open(BaseName);
				// stores a new function
				odb.Store(new User("uncommited user", "uncommied email"
					, new Profile("uncommiedt profile", new VO.Login.Function
					("uncommited function"))));
				IObjects<User> users = odb.GetObjects<User>();
				AssertEquals(5, users.Count);
				IObjects<VO.Login.Function> functions = odb.GetObjects<VO.Login.Function>();
				AssertEquals(5, functions.Count);
				IObjects<Profile> profiles = odb.GetObjects<Profile>();
				AssertEquals(5, profiles.Count);
			}
			finally
			{
				if (odb != null)
				{
					odb.Close();
					DeleteBase(BaseName);
				}
			}
		}

		/// <summary>Test select objects that are not yet commited.</summary>
		/// <remarks>
		/// Test select objects that are not yet commited. It also test the meta
		/// model class reference for in transaction class creation
		/// </remarks>
		/// <exception cref="System.Exception">System.Exception</exception>
		[Test]
        public virtual void TestSelectUnCommitedObject3()
		{
			DeleteBase(BaseName);
			// Create instance
			Sport sport = new Sport("volley-ball"
				);
			IOdb odb = null;
			try
			{
				// Open the database
				odb = Open(BaseName);
				// Store the object
				odb.Store(sport);
			}
			finally
			{
				if (odb != null)
				{
					// Close the database
					odb.Close();
				}
			}
			try
			{
				// Open the database
				odb = Open(BaseName);
				// Let's insert a tennis player
				Player agassi = new Player(
					"Andr√© Agassi", new System.DateTime(), new Sport("Tennis"
					));
				odb.Store(agassi);
				IQuery query = new CriteriaQuery
					(typeof(Player), Where
					.Equal("favoriteSport.name", "volley-ball"));
				IObjects<Player> players = odb.GetObjects<Player>(query);
				Println("\nStep 4 : Players of Voller-ball");
				int i = 1;
				// display each object
				while (players.HasNext())
				{
					Println((i++) + "\t: " + players.Next());
				}
			}
			finally
			{
				if (odb != null)
				{
					// Close the database
					odb.Close();
				}
			}
			DeleteBase(BaseName);
		}

		/// <summary>Test select objects that are not yet commited</summary>
		/// <exception cref="System.Exception">System.Exception</exception>
		[Test]
        public virtual void TestSelectUnCommitedObject4()
		{
			DeleteBase(BaseName);
			// Create instance
			Sport sport = new Sport("volley-ball"
				);
			IOdb odb = null;
			try
			{
				// Open the database
				odb = Open(BaseName);
				// Store the object
				odb.Store(sport);
			}
			finally
			{
				if (odb != null)
				{
					// Close the database
					odb.Close();
				}
			}
			// Create instance
			Sport volleyball = new Sport
				("volley-ball");
			// Create 4 players
			Player player1 = new Player
				("olivier", new System.DateTime(), volleyball);
			Player player2 = new Player
				("pierre", new System.DateTime(), volleyball);
			Player player3 = new Player
				("elohim", new System.DateTime(), volleyball);
			Player player4 = new Player
				("minh", new System.DateTime(), volleyball);
			// Create two teams
			Team team1 = new Team("Paris"
				);
			Team team2 = new Team("Montpellier"
				);
			// Set players for team1
			team1.AddPlayer(player1);
			team1.AddPlayer(player2);
			// Set players for team2
			team2.AddPlayer(player3);
			team2.AddPlayer(player4);
			// Then create a volley ball game for the two teams
			Game game = new Game(new System.DateTime
				(), volleyball, team1, team2);
			odb = null;
			try
			{
				// Open the database
				odb = Open(BaseName);
				// Store the object
				odb.Store(game);
			}
			finally
			{
				if (odb != null)
				{
					// Close the database
					odb.Close();
				}
			}
			try
			{
				// Open the database
				odb = Open(BaseName);
				IQuery query = new CriteriaQuery
					(typeof(Player), Where
					.Equal("name", "olivier"));
				IObjects<Player> players = odb.GetObjects<Player>(query);
				Println("\nStep 3 : Players with name olivier");
				int i = 1;
				// display each object
				while (players.HasNext())
				{
					Println((i++) + "\t: " + players.Next());
				}
			}
			finally
			{
				if (odb != null)
				{
					// Close the database
					odb.Close();
				}
			}
			try
			{
				// Open the database
				odb = Open(BaseName);
				// Let's insert a tennis player
				Player agassi = new Player(
					"Andr√© Agassi", new System.DateTime(), new Sport("Tennis"
					));
				OID oid = odb.Store(agassi);
				IQuery query = new CriteriaQuery
					(typeof(Player), Where
					.Equal("favoriteSport.name", "volley-ball"));
				IObjects<Player> players = odb.GetObjects<Player>(query);
				Println("\nStep 4 : Players of Voller-ball");
				int i = 1;
				// display each object
				while (players.HasNext())
				{
					Println((i++) + "\t: " + players.Next());
				}
			}
			finally
			{
				if (odb != null)
				{
					// Close the database
					odb.Close();
				}
			}
			DeleteBase(BaseName);
		}
	}
}
