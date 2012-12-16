using System;
using NDatabase2.Odb;
using NDatabase2.Odb.Core.Query;
using NDatabase2.Odb.Core.Query.Criteria;
using NUnit.Framework;
using Test.NDatabase.Odb.Test.VO.Arraycollectionmap;
using Test.NDatabase.Odb.Test.VO.Sport;

namespace Test.NDatabase.Odb.Test.Arraycollectionmap
{
    [TestFixture]
    public class TestList : ODBTest
    {
        [Test]
        public virtual void TestBigList()
        {
            var baseName = GetBaseName();
            var odb = Open(baseName);
            var size = 1000;
            var size2 = 4;
            var t0 = DateTime.Now.Ticks;
            for (var i = 0; i < size; i++)
            {
                var player = new PlayerWithList("player " + i);
                for (var j = 0; j < size2; j++)
                    player.AddGame("game " + j);
                odb.Store(player);
            }
            odb.Close();
            var t1 = DateTime.Now.Ticks;
            Console.WriteLine("insert : " + (t1 - t0) / 1000);

            var odb2 = Open(baseName);
            var l = odb2.Query<PlayerWithList>(false);
            var t2 = DateTime.Now.Ticks;
            AssertEquals(size, l.Count);
            Console.WriteLine("get objects " + l.Count + " : " + (t2 - t1) / 1000);
            odb2.Close();
            DeleteBase(baseName);
        }

        [Test]
        public virtual void TestCollectionWithContain()
        {
            IOdb odb = null;
            var baseName = GetBaseName();
            try
            {
                odb = Open(baseName);
                var nb = odb.CreateCriteriaQuery<PlayerWithList>().Count();
                var player = new PlayerWithList("kiko");
                player.AddGame("volley-ball");
                player.AddGame("squash");
                player.AddGame("tennis");
                player.AddGame("ping-pong");
                odb.Store(player);
                odb.Close();
                odb = Open(baseName);
                var query = odb.CreateCriteriaQuery<PlayerWithList>();
                query.Contain("games", "tennis");
                var l = query.Execute<PlayerWithList>();
                AssertEquals(nb + 1, l.Count);
            }
            catch (Exception)
            {
                if (odb != null)
                {
                    odb.Rollback();
                    odb = null;
                }
                throw;
            }
            finally
            {
                if (odb != null)
                    odb.Close();
            }
        }

        /// <summary>
        ///   one object has a list.
        /// </summary>
        /// <remarks>
        ///   one object has a list. we delete one of the object of the list of the
        ///   object. And the main object still has it
        /// </remarks>
        /// <exception cref="System.Exception">System.Exception</exception>
        [Test]
        public virtual void TestDeletingOneElementOfTheList()
        {
            if (!testNewFeature)
                return;
            var baseName = GetBaseName();
            DeleteBase(baseName);
            var odb = Open(baseName);
            var t1 = new Team("team1");
            t1.AddPlayer(new Player("player1", new DateTime(), new Sport("sport1")));
            t1.AddPlayer(new Player("player2", new DateTime(), new Sport("sport2")));
            odb.Store(t1);
            odb.Close();
            odb = Open(baseName);
            var teams = odb.Query<Team>();
            var team = teams.GetFirst();
            AssertEquals(2, team.GetPlayers().Count);
            var players = odb.Query<Player>();
            var p1 = players.GetFirst();
            odb.Delete(p1);
            odb.Close();
            AssertEquals(1, team.GetPlayers().Count);
        }

        [Test]
        public virtual void TestList1()
        {
            DeleteBase("list1.neodatis");
            var odb = Open("list1.neodatis");
            var nb = odb.CreateCriteriaQuery<PlayerWithList>().Count();
            var player = new PlayerWithList("kiko");
            player.AddGame("volley-ball");
            player.AddGame("squash");
            player.AddGame("tennis");
            player.AddGame("ping-pong");
            odb.Store(player);
            odb.Close();
            var odb2 = Open("list1.neodatis");
            var l = odb2.Query<PlayerWithList>(true);
            Println(l);
            AssertEquals(nb + 1, l.Count);
            // gets last player
            var player2 = l.GetFirst();
            AssertEquals(player.ToString(), player2.ToString());
            odb2.Close();
            DeleteBase("list1.neodatis");
        }

        [Test]
        public virtual void TestList1WithNull()
        {
            DeleteBase("list1.neodatis");
            var odb = Open("list1.neodatis");
            var nb = odb.CreateCriteriaQuery<PlayerWithList>().Count();
            var player = new PlayerWithList("kiko");
            player.AddGame("volley-ball");
            player.AddGame("squash");
            player.AddGame("tennis");
            player.AddGame(null);
            odb.Store(player);
            odb.Close();
            var odb2 = Open("list1.neodatis");
            var l = odb2.Query<PlayerWithList>(true);
            AssertEquals(nb + 1, l.Count);
            // gets last player
            var player2 = l.GetFirst();
            AssertEquals(player.GetGame(2), player2.GetGame(2));
            odb2.Close();
            DeleteBase("list1.neodatis");
        }

        [Test]
        public virtual void TestList2()
        {
            DeleteBase("list1.neodatis");
            var odb = Open("list1.neodatis");
            var nb = odb.CreateCriteriaQuery<PlayerWithList>().Count();
            var player = new PlayerWithList("kiko");
            player.SetGames(null);
            odb.Store(player);
            odb.Close();
            var odb2 = Open("list1.neodatis");
            var l = odb2.Query<PlayerWithList>(true);
            AssertEquals(nb + 1, l.Count);
            odb2.Close();
            DeleteBase("list1.neodatis");
        }

        [Test]
        public virtual void TestList3()
        {
            DeleteBase("list3.neodatis");
            var odb = Open("list3.neodatis");
            var nb = odb.CreateCriteriaQuery<MyObject>().Count();
            var l1 = new MyList();
            l1.Add("object1");
            l1.Add("object2");
            var myObject = new MyObject("o1", l1);
            odb.Store(myObject);
            odb.Close();
            var odb2 = Open("list3.neodatis");
            var l = odb2.Query<MyObject>(true);
            AssertEquals(nb + 1, l.Count);
            odb2.Close();
            DeleteBase("list3.neodatis");
        }

        /// <summary>
        ///   Test update object list.
        /// </summary>
        /// <remarks>
        ///   Test update object list. Removing one, adding other
        /// </remarks>
        [Test]
        public virtual void TestList4Update()
        {
            DeleteBase("list4.neodatis");
            var odb = Open("list4.neodatis");
            var nb = odb.CreateCriteriaQuery<MyObject>().Count();
            var l1 = new MyList();
            l1.Add("object1");
            l1.Add("object2");
            var myObject = new MyObject("o1", l1);
            odb.Store(myObject);
            odb.Close();
            var odb2 = Open("list4.neodatis");
            var l = odb2.Query<MyObject>(true);
            var mo = l.GetFirst();
            mo.GetList().RemoveAt(1);
            mo.GetList().Add("object 2bis");
            odb2.Store(mo);
            odb2.Close();
            odb2 = Open("list4.neodatis");
            l = odb2.Query<MyObject>(true);
            AssertEquals(nb + 1, l.Count);
            var mo2 = l.GetFirst();
            AssertEquals("object1", mo2.GetList()[0]);
            AssertEquals("object 2bis", mo2.GetList()[1]);
            odb2.Close();
            DeleteBase("list4.neodatis");
        }

        /// <summary>
        ///   Test update object list.
        /// </summary>
        /// <remarks>
        ///   Test update object list. adding 2 elements
        /// </remarks>
        [Test]
        public virtual void TestList4Update2()
        {
            DeleteBase("list4.neodatis");
            var odb = Open("list4.neodatis");
            var nb = odb.CreateCriteriaQuery<MyObject>().Count();
            var l1 = new MyList();
            l1.Add("object1");
            l1.Add("object2");
            var myObject = new MyObject("o1", l1);
            odb.Store(myObject);
            odb.Close();
            var odb2 = Open("list4.neodatis");
            var l = odb2.Query<MyObject>(true);
            var mo = l.GetFirst();
            mo.GetList().Add("object3");
            mo.GetList().Add("object4");
            odb2.Store(mo);
            odb2.Close();
            odb2 = Open("list4.neodatis");
            l = odb2.Query<MyObject>(true);
            AssertEquals(nb + 1, l.Count);
            var mo2 = l.GetFirst();
            AssertEquals(4, mo2.GetList().Count);
            AssertEquals("object1", mo2.GetList()[0]);
            AssertEquals("object2", mo2.GetList()[1]);
            AssertEquals("object3", mo2.GetList()[2]);
            AssertEquals("object4", mo2.GetList()[3]);
            odb2.Close();
            DeleteBase("list4.neodatis");
        }

        /// <summary>
        ///   Test update object list.
        /// </summary>
        /// <remarks>
        ///   Test update object list. A list of Integer
        /// </remarks>
        [Test]
        public virtual void TestList4Update3()
        {
            DeleteBase("list5.neodatis");
            var odb = Open("list5.neodatis");
            var o = new ObjectWithListOfInteger("test");
            o.GetListOfIntegers().Add(Convert.ToInt32("100"));
            odb.Store(o);
            odb.Close();
            var odb2 = Open("list5.neodatis");
            var l = odb2.Query<ObjectWithListOfInteger>(true);
            var o2 = l.GetFirst();
            o2.GetListOfIntegers().Clear();
            o2.GetListOfIntegers().Add(Convert.ToInt32("200"));
            odb2.Store(o2);
            odb2.Close();
            odb2 = Open("list5.neodatis");
            l = odb2.Query<ObjectWithListOfInteger>(true);
            AssertEquals(1, l.Count);
            var o3 = l.GetFirst();
            AssertEquals(1, o3.GetListOfIntegers().Count);
            AssertEquals(Convert.ToInt32("200"), o3.GetListOfIntegers()[0]);
            odb2.Close();
            DeleteBase("list5.neodatis");
        }

        /// <summary>
        ///   Test update object list.
        /// </summary>
        /// <remarks>
        ///   Test update object list. A list of Integer. 1000 updates
        /// </remarks>
        [Test]
        public virtual void TestList4Update4()
        {
            DeleteBase("list5.neodatis");
            var odb = Open("list5.neodatis");
            var o = new ObjectWithListOfInteger("test");
            o.GetListOfIntegers().Add(Convert.ToInt32("100"));
            odb.Store(o);
            odb.Close();
            var size = 100;
            for (var i = 0; i < size; i++)
            {
                var odb2 = Open("list5.neodatis");
                var ll = odb2.Query<ObjectWithListOfInteger>(true);
                var o2 = ll.GetFirst();
                o2.GetListOfIntegers().Clear();
                o2.GetListOfIntegers().Add(200 + i);
                odb2.Store(o2);
                odb2.Close();
            }
            var odb3 = Open("list5.neodatis");
            var l = odb3.Query<ObjectWithListOfInteger>(true);
            AssertEquals(1, l.Count);
            var o3 = l.GetFirst();
            AssertEquals(1, o3.GetListOfIntegers().Count);
            AssertEquals(200 + size - 1, o3.GetListOfIntegers()[0]);
            odb3.Close();
            DeleteBase("list5.neodatis");
        }

        /// <summary>
        ///   Test update object list.
        /// </summary>
        /// <remarks>
        ///   Test update object list. A list of Integer. 1000 updates of an object
        ///   that is the middle of the list
        /// </remarks>
        [Test]
        public virtual void TestList4Update4Middle()
        {
            DeleteBase("list5.neodatis");
            var odb = Open("list5.neodatis");
            var o = new ObjectWithListOfInteger("test1");
            o.GetListOfIntegers().Add(Convert.ToInt32("101"));
            odb.Store(o);
            o = new ObjectWithListOfInteger("test2");
            o.GetListOfIntegers().Add(Convert.ToInt32("102"));
            odb.Store(o);
            o = new ObjectWithListOfInteger("test3");
            o.GetListOfIntegers().Add(Convert.ToInt32("103"));
            odb.Store(o);
            odb.Close();
            var size = 100;
            for (var i = 0; i < size; i++)
            {
                var odb2 = Open("list5.neodatis");
                var query = odb2.CreateCriteriaQuery<ObjectWithListOfInteger>();
                query.Equal("name", "test2");
                var ll = query.Execute<ObjectWithListOfInteger>();
                var o2 = ll.GetFirst();
                o2.GetListOfIntegers().Clear();
                o2.GetListOfIntegers().Add(200 + i);
                odb2.Store(o2);
                odb2.Close();
            }
            var odb3 = Open("list5.neodatis");
            var query2 = odb3.CreateCriteriaQuery<ObjectWithListOfInteger>();
            query2.Equal("name", "test2");

            var l = query2.Execute<ObjectWithListOfInteger>();
            AssertEquals(1, l.Count);
            var o3 = l.GetFirst();
            AssertEquals(1, o3.GetListOfIntegers().Count);
            AssertEquals(200 + size - 1, o3.GetListOfIntegers()[0]);
            odb3.Close();
            DeleteBase("list5.neodatis");
        }

        /// <summary>
        ///   Test update object list.
        /// </summary>
        /// <remarks>
        ///   Test update object list. A list of Integer. 1000 updates of an object
        ///   increasing list nb elements that is the middle of the list
        /// </remarks>
        [Test]
        public virtual void TestList4Update4Middle2()
        {
            DeleteBase("list5.neodatis");
            var odb = Open("list5.neodatis");
            var o = new ObjectWithListOfInteger("test1");
            o.GetListOfIntegers().Add(Convert.ToInt32("101"));
            odb.Store(o);
            o = new ObjectWithListOfInteger("test2");
            o.GetListOfIntegers().Add(Convert.ToInt32("102"));
            odb.Store(o);
            o = new ObjectWithListOfInteger("test3");
            o.GetListOfIntegers().Add(Convert.ToInt32("103"));
            odb.Store(o);
            odb.Close();
            var size = 100;
            
            for (var i = 0; i < size; i++)
            {
                var odb2 = Open("list5.neodatis");
                var query2 = odb2.CreateCriteriaQuery<ObjectWithListOfInteger>();
                query2.Equal("name", "test2");

                var ll = query2.Execute<ObjectWithListOfInteger>();
                var o2 = ll.GetFirst();
                o2.GetListOfIntegers().Add(200 + i);
                odb2.Store(o2);
                odb2.Close();
            }
            var odb3 = Open("list5.neodatis");
            var query = odb3.CreateCriteriaQuery<ObjectWithListOfInteger>();
            query.Equal("name", "test2");
            var l = query.Execute<ObjectWithListOfInteger>();
            AssertEquals(1, l.Count);
            var o3 = l.GetFirst();
            AssertEquals(1 + size, o3.GetListOfIntegers().Count);
            odb3.Close();
            DeleteBase("list5.neodatis");
        }

        /// <summary>
        ///   Test update object list.
        /// </summary>
        /// <remarks>
        ///   Test update object list. A list of Integer. 1000 updates, increasing
        ///   number of elements
        /// </remarks>
        [Test]
        public virtual void TestList4Update5()
        {
            DeleteBase("list5.neodatis");
            var odb = Open("list5.neodatis");
            var o = new ObjectWithListOfInteger("test");
            o.GetListOfIntegers().Add(Convert.ToInt32("100"));
            odb.Store(o);
            odb.Close();
            var size = 100;
            for (var i = 0; i < size; i++)
            {
                var odb2 = Open("list5.neodatis");
                var ll = odb2.Query<ObjectWithListOfInteger>(true);
                var o2 = ll.GetFirst();
                o2.GetListOfIntegers().Add(200 + i);
                odb2.Store(o2);
                odb2.Close();
            }
            var odb3 = Open("list5.neodatis");
            var l = odb3.Query<ObjectWithListOfInteger>(true);
            AssertEquals(1, l.Count);
            var o3 = l.GetFirst();
            AssertEquals(size + 1, o3.GetListOfIntegers().Count);
            odb3.Close();
            DeleteBase("list5.neodatis");
        }
    }
}
