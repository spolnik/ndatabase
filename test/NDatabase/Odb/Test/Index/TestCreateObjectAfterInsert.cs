using System;
using NDatabase.Odb;
using NDatabase.Odb.Core.Query.Criteria;
using NDatabase.Odb.Impl.Tool;
using NDatabase.Tool.Wrappers;
using NUnit.Framework;

namespace Test.NDatabase.Odb.Test.Index
{
    [TestFixture]
    public class TestCreateObjectAfterInsert : ODBTest
    {
        /// <summary>
        ///   Test the creation of an index after having created objects.
        /// </summary>
        /// <remarks>
        ///   Test the creation of an index after having created objects. In this case
        ///   ODB should creates the index and update it with already existing objects
        /// </remarks>
        /// <exception cref="System.Exception">System.Exception</exception>
        [Test]
        public virtual void Test1000Objects()
        {
            var OdbFileName = "index2.test1.odb";
            IOdb odb = null;
            var size = 1000;
            var start = OdbTime.GetCurrentTimeInMs();

            try
            {
                DeleteBase(OdbFileName);
                odb = Open(OdbFileName);
                for (var i = 0; i < size; i++)
                {
                    var io = new IndexedObject("name" + i, i, new DateTime());
                    odb.Store(io);
                    if (i % 100 == 0)
                        MemoryMonitor.DisplayCurrentMemory(i + " objects created", true);
                }
                odb.Close();
                Println("\n\n END OF INSERT \n\n");
                odb = Open(OdbFileName);
                var names = new[] {"name"};
                odb.GetClassRepresentation<IndexedObject>().AddUniqueIndexOn("index1", names, true);
                Println("\n\n after create index\n\n");
                var objects =
                    odb.GetObjects<IndexedObject>(
                        new CriteriaQuery<IndexedObject>( Where.Equal("name", "name0")), true);
                Println("\n\nafter get Objects\n\n");
                AssertEquals(1, objects.Count);
                objects =
                    odb.GetObjects<IndexedObject>(
                        new CriteriaQuery<IndexedObject>( Where.Equal("duration", 9)), true);
                AssertEquals(1, objects.Count);
                objects = odb.GetObjects<IndexedObject>(new CriteriaQuery<IndexedObject>(), true);
                AssertEquals(size, objects.Count);
            }
            finally
            {
                var end = OdbTime.GetCurrentTimeInMs();
                Println((end - start) + "ms");
                
                odb.Close();
            }
        }

        /// <summary>
        ///   Test the creation of an index after having created objects.
        /// </summary>
        /// <remarks>
        ///   Test the creation of an index after having created objects. In this case
        ///   ODB should creates the index and update it with already existing objects
        /// </remarks>
        /// <exception cref="System.Exception">System.Exception</exception>
        [Test]
        public virtual void Test100ObjectsIntiNdex()
        {
            var OdbFileName = "index2.test2.odb";
            IOdb odb = null;
            var size = 100;
            var start = OdbTime.GetCurrentTimeInMs();
            
            try
            {
                DeleteBase(OdbFileName);
                odb = Open(OdbFileName);
                for (var i = 0; i < size; i++)
                {
                    var io = new IndexedObject("name" + i, i, new DateTime());
                    odb.Store(io);
                    if (i % 10 == 0)
                        MemoryMonitor.DisplayCurrentMemory(i + " objects created", true);
                }
                odb.Close();
                Println("\n\n END OF INSERT \n\n");
                odb = Open(OdbFileName);
                var names = new[] {"duration"};
                odb.GetClassRepresentation<IndexedObject>().AddUniqueIndexOn("index1", names, true);
                Println("\n\n after create index\n\n");
                var objects =
                    odb.GetObjects<IndexedObject>(
                        new CriteriaQuery<IndexedObject>( Where.Equal("name", "name0")), true);
                Println("\n\nafter get Objects\n\n");
                AssertEquals(1, objects.Count);
                objects =
                    odb.GetObjects<IndexedObject>(
                        new CriteriaQuery<IndexedObject>( Where.Equal("duration", 10)), true);
                AssertEquals(1, objects.Count);
                objects = odb.GetObjects<IndexedObject>(new CriteriaQuery<IndexedObject>(), true);
                AssertEquals(size, objects.Count);
            }
            finally
            {
                var end = OdbTime.GetCurrentTimeInMs();
                Println((end - start) + "ms");
            }
        }

        /// <summary>
        ///   Test the creation of an index after having created objects.
        /// </summary>
        /// <remarks>
        ///   Test the creation of an index after having created objects. In this case
        ///   ODB should creates the index and update it with already existing objects
        /// </remarks>
        /// <exception cref="System.Exception">System.Exception</exception>
        [Test]
        public virtual void Test1Object()
        {
            const string odbFileName = "index2.test3.odb";

            DeleteBase(odbFileName);
            using (var odb = Open(odbFileName))
            {
                var io = new IndexedObject("name", 5, new DateTime());
                odb.Store(io);
            }

            using (var odb = Open(odbFileName))
            {
                var names = new[] {"name"};
                odb.GetClassRepresentation<IndexedObject>().AddUniqueIndexOn("index1", names, true);
                var objects =
                    odb.GetObjects<IndexedObject>(
                        new CriteriaQuery<IndexedObject>( Where.Equal("name", "name")), true);
                AssertEquals(1, objects.Count);
            }
        }

        /// <summary>
        ///   Test the creation of an index after having created objects.
        /// </summary>
        /// <remarks>
        ///   Test the creation of an index after having created objects. In this case
        ///   ODB should creates the index and update it with already existing objects
        /// </remarks>
        /// <exception cref="System.Exception">System.Exception</exception>
        [Test]
        public virtual void Test2000Objects()
        {
            var OdbFileName = "index2.test4.odb";
            var start = OdbTime.GetCurrentTimeInMs();
            IOdb odb = null;
            var size = 2000;
            try
            {
                DeleteBase(OdbFileName);
                odb = Open(OdbFileName);
                for (var i = 0; i < size; i++)
                {
                    var io = new IndexedObject("name" + i, i, new DateTime());
                    odb.Store(io);
                }
                odb.Close();
                odb = Open(OdbFileName);
                var names = new[] {"name"};
                odb.GetClassRepresentation<IndexedObject>().AddUniqueIndexOn("index1", names, true);
                var objects =
                    odb.GetObjects<IndexedObject>(
                        new CriteriaQuery<IndexedObject>( Where.Equal("name", "name0")), true);
                AssertEquals(1, objects.Count);
                objects = odb.GetObjects<IndexedObject>(new CriteriaQuery<IndexedObject>(), true);
                MemoryMonitor.DisplayCurrentMemory("BTREE", true);
                AssertEquals(size, objects.Count);
            }
            finally
            {
                if (odb != null)
                    odb.Close();
                var end = OdbTime.GetCurrentTimeInMs();
                Println((end - start) + "ms");
            }
        }
    }
}
