using System;
using System.Diagnostics;
using NDatabase.UnitTests.CodeSnippets.Data;
using NDatabase2.Odb;
using NUnit.Framework;

namespace NDatabase.UnitTests.CodeSnippets
{
    public class Documentation_indexes
    {
        [Test]
        public void Add_index_then_query()
        {
            OdbFactory.Delete("index1.ndb");
            using (var odb = OdbFactory.Open("index1.ndb"))
            {
                var fields = new[] { "Name" };
                odb.IndexManagerFor<Player>().AddUniqueIndexOn("nameIndex", fields);
                
                for (var i = 0; i < 50; i++)
                {
                    var player = new Player("Player" + i, DateTime.Now, new Sport("Sport" + i));
                    odb.Store(player);
                }
            }

            using (var odb = OdbFactory.OpenLast())
            {
                var count = odb.Query<Player>().Count;
                Assert.That(count, Is.EqualTo(50));
            }
        }

        [Test]
        public void Test_perf_of_query_with_index()
        {
            OdbFactory.Delete("index1perf.ndb");
            using (var odb = OdbFactory.Open("index1perf.ndb"))
            {
                var fields = new[] { "Name" };
                odb.IndexManagerFor<Player>().AddUniqueIndexOn("nameIndex", fields);

                for (var i = 0; i < 5000; i++)
                {
                    var player = new Player("Player" + i, DateTime.Now, new Sport("Sport" + i));
                    odb.Store(player);
                }
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            using (var odb = OdbFactory.OpenLast())
            {
                var query = odb.CreateCriteriaQuery<Player>();
                query.Descend("Name").Equal("Player20");
                
                var count = query.Execute<Player>().Count;

                Assert.That(count, Is.EqualTo(1));
            }
            stopwatch.Stop();
            Console.WriteLine("Elapsed {0} ms", stopwatch.ElapsedMilliseconds);

            stopwatch.Reset();
            stopwatch.Start();
            using (var odb = OdbFactory.OpenLast())
            {
                var query = odb.CreateCriteriaQuery<Player>();
                query.Descend("Name").Equal("Player1234");
                var count = query.Execute<Player>().Count;
                Assert.That(count, Is.EqualTo(1));
            }
            stopwatch.Stop();
            Console.WriteLine("Elapsed {0} ms", stopwatch.ElapsedMilliseconds);

            stopwatch.Reset();
            stopwatch.Start();
            using (var odb = OdbFactory.OpenLast())
            {
                var query = odb.CreateCriteriaQuery<Player>();
                query.Descend("Name").Equal("Player4444");
                var count = query.Execute<Player>().Count;
                Assert.That(count, Is.EqualTo(1));
            }
            stopwatch.Stop();
            Console.WriteLine("Elapsed {0} ms", stopwatch.ElapsedMilliseconds);

            stopwatch.Reset();
            stopwatch.Start();
            using (var odb = OdbFactory.OpenLast())
            {
                var query = odb.CreateCriteriaQuery<Player>();
                query.Descend("Name").Equal("Player3211");
                var count = query.Execute<Player>().Count;
                Assert.That(count, Is.EqualTo(1));
            }
            stopwatch.Stop();
            Console.WriteLine("Elapsed {0} ms", stopwatch.ElapsedMilliseconds);
        }

        [Test]
        public void Test_perf_of_query_without_index()
        {
            OdbFactory.Delete("index1perf.ndb");
            using (var odb = OdbFactory.Open("index1perf.ndb"))
            {
                for (var i = 0; i < 5000; i++)
                {
                    var player = new Player("Player" + i, DateTime.Now, new Sport("Sport" + i));
                    odb.Store(player);
                }
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            using (var odb = OdbFactory.OpenLast())
            {
                var query = odb.CreateCriteriaQuery<Player>();
                query.Descend("Name").Equal("Player20");
                var count = query.Execute<Player>().Count;
                Assert.That(count, Is.EqualTo(1));
            }
            stopwatch.Stop();
            Console.WriteLine("Elapsed {0} ms", stopwatch.ElapsedMilliseconds);

            stopwatch.Reset();
            stopwatch.Start();
            using (var odb = OdbFactory.OpenLast())
            {
                var query = odb.CreateCriteriaQuery<Player>();
                query.Descend("Name").Equal("Player1234");
                var count = query.Execute<Player>().Count;
                Assert.That(count, Is.EqualTo(1));
            }
            stopwatch.Stop();
            Console.WriteLine("Elapsed {0} ms", stopwatch.ElapsedMilliseconds);

            stopwatch.Reset();
            stopwatch.Start();
            using (var odb = OdbFactory.OpenLast())
            {
                var query = odb.CreateCriteriaQuery<Player>();
                query.Descend("Name").Equal("Player4444");
                var count = query.Execute<Player>().Count;
                Assert.That(count, Is.EqualTo(1));
            }
            stopwatch.Stop();
            Console.WriteLine("Elapsed {0} ms", stopwatch.ElapsedMilliseconds);

            stopwatch.Reset();
            stopwatch.Start();
            using (var odb = OdbFactory.OpenLast())
            {
                var query = odb.CreateCriteriaQuery<Player>();
                query.Descend("Name").Equal("Player3211");
                var count = query.Execute<Player>().Count;
                Assert.That(count, Is.EqualTo(1));
            }
            stopwatch.Stop();
            Console.WriteLine("Elapsed {0} ms", stopwatch.ElapsedMilliseconds);
        }
    }
}