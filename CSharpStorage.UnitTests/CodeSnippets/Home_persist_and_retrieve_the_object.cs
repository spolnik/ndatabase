using System.IO;
using NDatabase.Odb;
using NDatabase.UnitTests.CodeSnippets.Data;
using NDatabase2.Odb;
using NUnit.Framework;

namespace NDatabase.UnitTests.CodeSnippets
{
    public class Home_persist_and_retrieve_the_object
    {
        [Test]
        public void TheSnippet()
        {
            //=================================

            // Create the instance be stored
            var sport = new Sport("volley-ball");

            // Open the database
            using (var odb = OdbFactory.Open("test.db"))
            {
                // Store the object
                odb.Store(sport);
            }

            //=================================

            // Open the database
            using (var odb1 = OdbFactory.Open("test.db"))
            {
                var query = odb1.Query<Sport>();
                var sports = query.Execute<Sport>();
                // code working on sports list
                Assert.That(sports, Has.Count.EqualTo(1));
            }

            //=================================
        }

        [SetUp]
        public void SetUp()
        {
            const string testDb = "test.db";

            if (File.Exists(testDb))
                File.Delete(testDb);
        }
    }
}
