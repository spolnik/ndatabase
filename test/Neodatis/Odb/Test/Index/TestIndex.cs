using System;
using NDatabase.Btree.Exception;
using NDatabase.Odb;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Query;
using NDatabase.Odb.Core.Query.Criteria;
using NDatabase.Odb.Impl.Core.Btree;
using NDatabase.Odb.Impl.Core.Layers.Layer3.Engine;
using NDatabase.Odb.Impl.Core.Query.Criteria;
using NDatabase.Tool.Wrappers;
using NUnit.Framework;
using Test.Odb.Test;
using Test.Odb.Test.VO.Login;

namespace Index
{
    [TestFixture]
    public class TestIndex : ODBTest
    {
        [Test]
        public void SimpleUniqueIndex()
        {
            var baseName = GetBaseName();
            DeleteBase(baseName);
            var odb = Open(baseName);
            var clazz = odb.GetClassRepresentation(typeof (Function));
            var indexFields = new[] {"name"};
            clazz.AddUniqueIndexOn("index", indexFields, true);
            odb.Close();
            odb = Open(baseName);
            // inserting 3 objects with 3 different index keys
            odb.Store(new Function("function1"));
            odb.Store(new Function("function2"));
            odb.Store(new Function("function3"));
            odb.Close();
            odb = Open(baseName);
            try
            {
                // Tries to store another function with name function1 => send an
                // exception because of duplicated keys
                odb.Store(new Function("function1"));
                Fail("Should have thrown Exception");
            }
            catch (DuplicatedKeyException)
            {
                Assert.Pass();
                odb.Close();
                DeleteBase(baseName);
            }
        }

        [Test]
        public void TestIndexExist1()
        {
            var baseName = GetBaseName();
            DeleteBase(baseName);
            var odb = Open(baseName);
            var clazz = odb.GetClassRepresentation(typeof (Function));
            var indexFields = new[] {"name"};
            clazz.AddUniqueIndexOn("my-index", indexFields, true);
            odb.Store(new Function("test"));
            odb.Close();
            odb = Open(baseName);
            AssertTrue(odb.GetClassRepresentation(typeof (Function)).ExistIndex("my-index"));
            AssertFalse(odb.GetClassRepresentation(typeof (Function)).ExistIndex("my-indexdhfdjkfhdjkhj"));
            odb.Close();
        }

        [Test]
        public void TestIndexExist2()
        {
            var baseName = GetBaseName();
            DeleteBase(baseName);
            var odb = Open(baseName);
            var clazz = odb.GetClassRepresentation(typeof (Function));
            var indexFields = new[] {"name"};
            clazz.AddUniqueIndexOn("my-index", indexFields, true);
            odb.Close();
            odb = Open(baseName);
            AssertTrue(odb.GetClassRepresentation(typeof (Function)).ExistIndex("my-index"));
            AssertFalse(odb.GetClassRepresentation(typeof (Function)).ExistIndex("my-indexdhfdjkfhdjkhj"));
            odb.Close();
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public void TestIndexWithOneFieldAndQueryWithTwoFields()
        {
            var baseName = GetBaseName();
            DeleteBase(baseName);
            var @base = Open(baseName);
            var clazz = @base.GetClassRepresentation(typeof (IndexedObject));
            var indexFields = new[] {"name"};
            clazz.AddUniqueIndexOn("index1", indexFields, true);
            @base.Close();
            @base = Open(baseName);
            var io1 = new IndexedObject("olivier", 15, new DateTime());
            @base.Store(io1);
            @base.Close();
            @base = Open(baseName);
            IQuery q = new CriteriaQuery(typeof (IndexedObject),
                                         Where.And().Add(Where.Equal("name", "olivier")).Add(Where.Equal("duration", 15)));
            var objects = @base.GetObjects<IndexedObject>(q, true);
            @base.Close();
            Println(q.GetExecutionPlan().ToString());
            AssertEquals(false, q.GetExecutionPlan().UseIndex());
            AssertEquals(1, objects.Count);
            DeleteBase(baseName);
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public void TestInsertAndDeleteWithIndex()
        {
            var baseName = GetBaseName();
            DeleteBase(baseName);
            var @base = Open(baseName);
            // base.store(new IndexedObject());
            var clazz = @base.GetClassRepresentation(typeof (IndexedObject));
            var indexFields = new[] {"name"};
            clazz.AddUniqueIndexOn("index1", indexFields, true);
            @base.Close();
            @base = Open(baseName);
            var size = 1000;
            var start0 = OdbTime.GetCurrentTimeInMs();
            for (var i = 0; i < size; i++)
            {
                var io1 = new IndexedObject("olivier" + (i + 1), 15 + i, new DateTime());
                @base.Store(io1);
                if (i % 10 == 0)
                    Println(i);
            }
            var tt0 = OdbTime.GetCurrentTimeInMs();
            @base.Close();
            var tt1 = OdbTime.GetCurrentTimeInMs();
            var end0 = OdbTime.GetCurrentTimeInMs();
            Println(string.Format("NU={0}", AbstractObjectWriter.GetNbNormalUpdates()));
            Println("inserting time with index=" + (end0 - start0));
            Println("commit time=" + (tt1 - tt0));
            Println(LazyOdbBtreePersister.Counters());
            @base = Open(baseName);
            long totalTime = 0;
            long maxTime = 0;
            long minTime = 100000;
            for (var i = 0; i < size; i++)
            {
                IQuery query = new CriteriaQuery(typeof (IndexedObject), Where.Equal("name", "olivier" + (i + 1)));
                var start = OdbTime.GetCurrentTimeInMs();
                var objects = @base.GetObjects<IndexedObject>(query, true);
                var end = OdbTime.GetCurrentTimeInMs();
                AssertEquals(1, objects.Count);
                var io2 = objects.GetFirst();
                AssertEquals("olivier" + (i + 1), io2.GetName());
                AssertEquals(15 + i, io2.GetDuration());
                var d = end - start;
                totalTime += d;
                if (d > maxTime)
                    maxTime = d;
                if (d < minTime)
                    minTime = d;
                @base.Delete(io2);
            }
            @base.Close();
            @base = Open(baseName);
            IQuery q = new CriteriaQuery(typeof (IndexedObject));
            var oos = @base.GetObjects<IndexedObject>(q, true);
            for (var i = 0; i < size; i++)
            {
                q = new CriteriaQuery(typeof (IndexedObject), Where.Equal("name", "olivier" + (i + 1)));
                oos = @base.GetObjects<IndexedObject>(q, true);
                AssertEquals(0, oos.Count);
            }
            @base.Close();
            DeleteBase(baseName);
            Println("total duration=" + totalTime + " / " + (double) totalTime / size);
            Println("duration max=" + maxTime + " / min=" + minTime);
            if (testPerformance)
            {
                AssertTrue(totalTime / size < 0.9);
                
                AssertTrue(maxTime < 20);
                AssertTrue(minTime == 0);
            }
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public void TestInsertAndDeleteWithIndex1()
        {
            var baseName = GetBaseName();
            DeleteBase(baseName);
            var @base = Open(baseName);
            // base.store(new IndexedObject());
            var clazz = @base.GetClassRepresentation(typeof (IndexedObject));
            var indexFields = new[] {"name"};
            clazz.AddUniqueIndexOn("index1", indexFields, true);
            @base.Close();
            @base = Open(baseName);
            var size = 1400;
            for (var i = 0; i < size; i++)
            {
                var io1 = new IndexedObject("olivier" + (i + 1), 15 + i, new DateTime());
                @base.Store(io1);
            }
            @base.Close();
            Console.Out.WriteLine("----ola");
            @base = Open(baseName);
            IQuery q = new CriteriaQuery(typeof (IndexedObject));
            var objects = @base.GetObjects<IndexedObject>(q);
            while (objects.HasNext())
            {
                var io = objects.Next();
                Println(io);
                @base.Delete(io);
            }
            @base.Close();
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public void TestInsertAndDeleteWithIndexWith1000()
        {
            var baseName = GetBaseName();

            DeleteBase(baseName);
            var @base = Open(baseName);
            // base.store(new IndexedObject());
            var clazz = @base.GetClassRepresentation(typeof (IndexedObject));
            var indexFields = new[] {"name"};
            clazz.AddUniqueIndexOn("index1", indexFields, true);
            @base.Close();
            @base = Open(baseName);
            var size = 1000;
            var start0 = OdbTime.GetCurrentTimeInMs();
            for (var i = 0; i < size; i++)
            {
                var io1 = new IndexedObject("olivier" + (i + 1), 15 + i, new DateTime());
                @base.Store(io1);
                if (i % 100 == 0)
                    Println(i);
            }
            var tt0 = OdbTime.GetCurrentTimeInMs();
            @base.Close();
            var tt1 = OdbTime.GetCurrentTimeInMs();
            var end0 = OdbTime.GetCurrentTimeInMs();
            Println(string.Format("NU={0}", AbstractObjectWriter.GetNbNormalUpdates()));
            Println("inserting time with index=" + (end0 - start0));
            Println("commit time=" + (tt1 - tt0));
            Println(LazyOdbBtreePersister.Counters());
            @base = Open(baseName);
            long totalSelectTime = 0;
            long maxTime = 0;
            long minTime = 100000;
            var t0 = OdbTime.GetCurrentTimeInMs();
            long t1 = 0;
            long ta1 = 0;
            long ta2 = 0;
            long totalTimeDelete = 0;
            long totalTimeSelect = 0;
            for (var j = 0; j < size; j++)
            {
                IQuery q = new CriteriaQuery(typeof (IndexedObject), Where.Equal("name", "olivier" + (j + 1)));
                var start = OdbTime.GetCurrentTimeInMs();
                var objects = @base.GetObjects<IndexedObject>(q, true);
                var end = OdbTime.GetCurrentTimeInMs();
                AssertEquals(1, objects.Count);
                var io2 = objects.GetFirst();
                AssertEquals("olivier" + (j + 1), io2.GetName());
                AssertEquals(15 + j, io2.GetDuration());
                var d = end - start;
                totalSelectTime += d;
                if (d > maxTime)
                    maxTime = d;
                if (d < minTime)
                    minTime = d;
                ta1 = OdbTime.GetCurrentTimeInMs();
                @base.Delete(io2);
                ta2 = OdbTime.GetCurrentTimeInMs();
                totalTimeDelete += (ta2 - ta1);
                totalTimeSelect += (end - start);
                if (j % 100 == 0 && j > 0)
                {
                    t1 = OdbTime.GetCurrentTimeInMs();
                    Println(j + " - t= " + (t1 - t0) + " - delete=" + (totalTimeDelete / j) + " / select=" +
                            (totalTimeSelect / j));
                    Println(LazyOdbBtreePersister.Counters());
                    LazyOdbBtreePersister.ResetCounters();
                    t0 = t1;
                }
            }
            @base.Close();
            Println("total select=" + totalSelectTime + " / " + (double) totalSelectTime / size);
            Println("total delete=" + totalTimeDelete + " / " + (double) totalTimeDelete / size);
            Println("duration max=" + maxTime + " / min=" + minTime);
            @base = Open(baseName);
            for (var i = 0; i < size; i++)
            {
                IQuery q = new CriteriaQuery(typeof (IndexedObject), Where.Equal("name", "olivier" + (i + 1)));
                var start = OdbTime.GetCurrentTimeInMs();
                var objects = @base.GetObjects<IndexedObject>(q, true);
                var end = OdbTime.GetCurrentTimeInMs();
                AssertEquals(0, objects.Count);
                if (i % 100 == 0)
                    Println(i);
            }
            @base.Close();
            DeleteBase(baseName);
            var timePerObject = totalSelectTime / (float) size;
            Println("Time per object = " + timePerObject);
            if (timePerObject > 1)
                Println("Time per object = " + timePerObject);
            AssertTrue(timePerObject < 1);
            // TODO Try to get maxTime < 10!
            AssertTrue(maxTime < 250);
            AssertTrue(minTime < 1);
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public void TestInsertAndDeleteWithIndexWith40Elements()
        {
            var baseName = GetBaseName();
            OdbConfiguration.SetDefaultIndexBTreeDegree(3);
            DeleteBase(baseName);
            var @base = Open(baseName);
            // base.store(new IndexedObject());
            var clazz = @base.GetClassRepresentation(typeof (IndexedObject));
            var indexFields = new[] {"name"};
            clazz.AddUniqueIndexOn("index1", indexFields, true);
            @base.Close();
            @base = Open(baseName);
            var size = 6;
            var start0 = OdbTime.GetCurrentTimeInMs();
            for (var i = 0; i < size; i++)
            {
                var io1 = new IndexedObject("olivier" + (i + 1), 15 + i, new DateTime());
                @base.Store(io1);
                if (i % 1000 == 0)
                    Println(i);
            }
            var tt0 = OdbTime.GetCurrentTimeInMs();
            @base.Close();
            var tt1 = OdbTime.GetCurrentTimeInMs();
            var end0 = OdbTime.GetCurrentTimeInMs();
            Println(string.Format("NU={0}", AbstractObjectWriter.GetNbNormalUpdates()));
            Println("inserting time with index=" + (end0 - start0));
            Println("commit time=" + (tt1 - tt0));
            Println(LazyOdbBtreePersister.Counters());
            @base = Open(baseName);
            long totalTime = 0;
            long maxTime = 0;
            long minTime = 100000;
            var t0 = OdbTime.GetCurrentTimeInMs();
            long t1 = 0;
            for (var i = 0; i < size; i++)
            {
                IQuery q = new CriteriaQuery(typeof (IndexedObject), Where.Equal("name", "olivier" + (i + 1)));
                var start = OdbTime.GetCurrentTimeInMs();
                var objects = @base.GetObjects<IndexedObject>(q, true);
                var end = OdbTime.GetCurrentTimeInMs();
                AssertEquals(1, objects.Count);
                var io2 = objects.GetFirst();
                AssertEquals("olivier" + (i + 1), io2.GetName());
                AssertEquals(15 + i, io2.GetDuration());
                var d = end - start;
                totalTime += d;
                if (d > maxTime)
                    maxTime = d;
                if (d < minTime)
                    minTime = d;
                @base.Delete(io2);
                if (i % 100 == 0)
                {
                    t1 = OdbTime.GetCurrentTimeInMs();
                    Println(i + " - t= " + (t1 - t0));
                    t0 = t1;
                }
            }
            // println(new BTreeDisplay().build(cii.getBTree(), true));
            @base.Close();
            @base = Open(baseName);
            for (var i = 0; i < size; i++)
            {
                IQuery q = new CriteriaQuery(typeof (IndexedObject), Where.Equal("name", "olivier" + (i + 1)));
                var start = OdbTime.GetCurrentTimeInMs();
                var objects = @base.GetObjects<IndexedObject>(q, true);
                var end = OdbTime.GetCurrentTimeInMs();
                AssertEquals(0, objects.Count);
                if (i % 100 == 0)
                    Println(i);
            }
            var unitTime = (double) totalTime / size;
            Println("total duration=" + totalTime + " / " + unitTime);
            Println("duration max=" + maxTime + " / min=" + minTime);
            @base.Close();
            DeleteBase(baseName);
            AssertTrue(unitTime < 10);
            // TODO Try to get maxTime < 10!
            if (testPerformance)
            {
                AssertTrue(maxTime < 250);
                AssertTrue(minTime <= 1);
            }
            OdbConfiguration.SetDefaultIndexBTreeDegree(20);
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public void TestInsertAndDeleteWithIndexWith4Elements()
        {
            var baseName = GetBaseName();
            DeleteBase(baseName);
            var @base = Open(baseName);
            // base.store(new IndexedObject());
            var clazz = @base.GetClassRepresentation(typeof (IndexedObject));
            var indexFields = new[] {"name"};
            clazz.AddUniqueIndexOn("index1", indexFields, true);
            @base.Close();
            @base = Open(baseName);
            var size = 4;
            var start0 = OdbTime.GetCurrentTimeInMs();
            for (var i = 0; i < size; i++)
            {
                var io1 = new IndexedObject("olivier" + (i + 1), 15 + i, new DateTime());
                @base.Store(io1);
                if (i % 1000 == 0)
                    Println(i);
            }
            var tt0 = OdbTime.GetCurrentTimeInMs();
            @base.Close();
            var tt1 = OdbTime.GetCurrentTimeInMs();
            var end0 = OdbTime.GetCurrentTimeInMs();
            Println(string.Format("NU={0}", AbstractObjectWriter.GetNbNormalUpdates()));
            Println("inserting time with index=" + (end0 - start0));
            Println("commit time=" + (tt1 - tt0));
            Println(LazyOdbBtreePersister.Counters());
            @base = Open(baseName);
            long totalTime = 0;
            long maxTime = 0;
            long minTime = 100000;
            var t0 = OdbTime.GetCurrentTimeInMs();
            long t1 = 0;
            for (var i = 0; i < size; i++)
            {
                IQuery q = new CriteriaQuery(typeof (IndexedObject), Where.Equal("name", "olivier" + (i + 1)));
                var start = OdbTime.GetCurrentTimeInMs();
                var objects = @base.GetObjects<IndexedObject>(q, true);
                var end = OdbTime.GetCurrentTimeInMs();
                AssertEquals(1, objects.Count);
                var io2 = objects.GetFirst();
                AssertEquals("olivier" + (i + 1), io2.GetName());
                AssertEquals(15 + i, io2.GetDuration());
                var d = end - start;
                totalTime += d;
                if (d > maxTime)
                    maxTime = d;
                if (d < minTime)
                    minTime = d;
                @base.Delete(io2);
                if (i % 100 == 0)
                {
                    t1 = OdbTime.GetCurrentTimeInMs();
                    Println(i + " - t= " + (t1 - t0));
                    t0 = t1;
                }
            }
            @base.Close();
            @base = Open(baseName);
            for (var i = 0; i < size; i++)
            {
                IQuery q = new CriteriaQuery(typeof (IndexedObject), Where.Equal("name", "olivier" + (i + 1)));
                var start = OdbTime.GetCurrentTimeInMs();
                var objects = @base.GetObjects<IndexedObject>(q, true);
                var end = OdbTime.GetCurrentTimeInMs();
                AssertEquals(0, objects.Count);
                if (i % 100 == 0)
                    Println(i);
            }
            @base.Close();
            DeleteBase(baseName);
            var unitTime = (double) totalTime / size;
            Println("total duration=" + totalTime + " / " + (double) totalTime / size);
            Println("duration max=" + maxTime + " / min=" + minTime);
            AssertTrue(unitTime < 1);
            // TODO Try to get maxTime < 10!
            if (testPerformance)
            {
                AssertTrue(maxTime < 250);
                AssertTrue(minTime <= 1);
            }
        }

        /// <summary>
        ///   Test with two key index
        /// </summary>
        /// <exception cref="System.Exception"></exception>
        [Test]
        public void TestInsertWith3Indexes()
        {
            var baseName = GetBaseName();
            DeleteBase(baseName);
            var @base = Open(baseName);
            // Configuration.setUseLazyCache(true);
            // base.store(new IndexedObject());
            var clazz = @base.GetClassRepresentation(typeof (IndexedObject));
            var indexFields3 = new[] {"name"};
            clazz.AddUniqueIndexOn("index3", indexFields3, true);
            var indexFields2 = new[] {"name", "creation"};
            clazz.AddUniqueIndexOn("index2", indexFields2, true);
            var indexField4 = new[] {"duration", "creation"};
            clazz.AddUniqueIndexOn("inde3", indexField4, true);
            @base.Close();
            @base = Open(baseName);
            var size = 1000;
            var start0 = OdbTime.GetCurrentTimeInMs();
            var dates = new DateTime[size];
            for (var i = 0; i < size; i++)
            {
                // println(i);
                dates[i] = new DateTime();
                var io1 = new IndexedObject("olivier" + (i + 1), i, dates[i]);
                @base.Store(io1);
                if (i % 100 == 0)
                    Println(i);
            }
            @base.Close();
            var end0 = OdbTime.GetCurrentTimeInMs();
            Println(string.Format("NU={0}", AbstractObjectWriter.GetNbNormalUpdates()));
            Println("inserting time with index=" + (end0 - start0));
            @base = Open(baseName);
            var start = OdbTime.GetCurrentTimeInMs();
            for (var i = 0; i < size; i++)
            {
                IQuery q = new CriteriaQuery(typeof (IndexedObject),
                                             Where.And().Add(Where.Equal("duration", i)).Add(Where.Equal("creation",
                                                                                                         dates[i])));
                var objects = @base.GetObjects<IndexedObject>(q, true);
                AssertEquals(1, objects.Count);
                AssertTrue(q.GetExecutionPlan().UseIndex());
            }
            var end = OdbTime.GetCurrentTimeInMs();
            double duration = (end - start);
            duration = duration / size;
            Println("duration=" + duration);
            @base.Close();
            DeleteBase(baseName);

            Println(duration);
            var d = 2;

            if (duration > d)
                Fail("Time of search in index is greater than " + d + " ms : " + duration);
        }

        /// <summary>
        ///   Test with 3 indexes
        /// </summary>
        /// <exception cref="System.Exception"></exception>
        [Test]
        public void TestInsertWith3IndexesCheckAll()
        {
            var baseName = GetBaseName();
            // LogUtil.logOn(LazyOdbBtreePersister.LOG_ID, true);
            DeleteBase(baseName);
            var @base = Open(baseName);

            // base.store(new IndexedObject());
            var clazz = @base.GetClassRepresentation(typeof (IndexedObject));
            var indexFields = new[] {"duration"};
            clazz.AddIndexOn("index1", indexFields, true);
            var indexFields2 = new[] {"creation"};
            clazz.AddIndexOn("index2", indexFields2, true);
            var indexFields3 = new[] {"name"};
            clazz.AddIndexOn("index3", indexFields3, true);
            @base.Close();
            @base = Open(baseName);
            var size = 130;
            var commitInterval = 10;
            var start0 = OdbTime.GetCurrentTimeInMs();
            for (var i = 0; i < size; i++)
            {
                var io1 = new IndexedObject("olivier" + (i + 1), i, new DateTime());
                @base.Store(io1);
                if (i % commitInterval == 0)
                {
                    @base.Commit();
                    Println(i + " : commit / " + size);
                }
            }
            @base.Close();
            var end0 = OdbTime.GetCurrentTimeInMs();
            Println(string.Format("NU={0}", AbstractObjectWriter.GetNbNormalUpdates()));
            // ObjectWriter.getNbNormalUpdates());
            // println("inserting time with index=" + (end0 - start0));
            @base = Open(baseName);
            var start = OdbTime.GetCurrentTimeInMs();
            for (var i = 0; i < size; i++)
            {
                IQuery q = new CriteriaQuery(typeof (IndexedObject), Where.Equal("duration", i));
                var objects = @base.GetObjects<IndexedObject>(q, false);
                // println("olivier" + (i+1));
                AssertEquals(1, objects.Count);
            }
            var end = OdbTime.GetCurrentTimeInMs();
            try
            {
                var duration = (end - start) / (float) size;
                Println(duration);
                var d = 0.144;

                if (testPerformance && duration > d)
                    Fail("Time of search in index is greater than " + d + " ms : " + duration);
            }
            finally
            {
                @base.Close();
                DeleteBase(baseName);
            }
        }

        /// <summary>
        ///   Test index with 3 keys .
        /// </summary>
        /// <remarks>
        ///   Test index with 3 keys .
        ///   Select using only one field to verify that query does not use index, then
        ///   execute a query with the 3 fields and checks than index is used
        /// </remarks>
        /// <exception cref="System.Exception"></exception>
        [Test]
        public void TestInsertWith3Keys()
        {
            var baseName = GetBaseName();
            DeleteBase(baseName);
            var @base = Open(baseName);
            // base.store(new IndexedObject());
            var clazz = @base.GetClassRepresentation(typeof (IndexedObject));
            var indexFields = new[] {"name", "duration", "creation"};
            clazz.AddUniqueIndexOn("index", indexFields, true);
            @base.Close();
            @base = Open(baseName);
            var size = 500;
            var commitInterval = 10000;
            var start0 = OdbTime.GetCurrentTimeInMs();
            for (var i = 0; i < size; i++)
            {
                var io2 = new IndexedObject("olivier" + (i + 1), i + 15 + size, new DateTime());
                @base.Store(io2);
                if (i % commitInterval == 0)
                {
                    var t0 = OdbTime.GetCurrentTimeInMs();
                    @base.Commit();
                    var t1 = OdbTime.GetCurrentTimeInMs();
                    Println(i + " : commit - ctime " + (t1 - t0) + " -ttime=");
                    // println(LazyOdbBtreePersister.counters());
                    LazyOdbBtreePersister.ResetCounters();
                }
            }
            var theDate = new DateTime();
            var theName = "name indexed";
            var theDuration = 45;
            var io1 = new IndexedObject(theName, theDuration, theDate);
            @base.Store(io1);
            @base.Close();
            @base = Open(baseName);
            // first search without index
            IQuery q = new CriteriaQuery(typeof (IndexedObject), Where.Equal("name", theName));
            var objects = @base.GetObjects<IndexedObject>(q, true);
            AssertFalse(q.GetExecutionPlan().UseIndex());
            Println(q.GetExecutionPlan().GetDetails());
            AssertEquals(1, objects.Count);
            var io3 = objects.GetFirst();
            AssertEquals(theName, io3.GetName());
            AssertEquals(theDuration, io3.GetDuration());
            AssertEquals(theDate, io3.GetCreation());
            @base.Close();
            @base = Open(baseName);
            // Then search usin index
            q = new CriteriaQuery(typeof (IndexedObject),
                                  Where.And().Add(Where.Equal("name", theName)).Add(Where.Equal("creation", theDate)).
                                      Add(Where.Equal("duration", theDuration)));
            objects = @base.GetObjects<IndexedObject>(q, true);
            AssertTrue(q.GetExecutionPlan().UseIndex());
            AssertEquals("index", q.GetExecutionPlan().GetIndex().Name);
            Println(q.GetExecutionPlan().GetDetails());
            AssertEquals(1, objects.Count);
            io3 = objects.GetFirst();
            AssertEquals(theName, io3.GetName());
            AssertEquals(theDuration, io3.GetDuration());
            AssertEquals(theDate, io3.GetCreation());
            @base.Close();
        }

        /// <summary>
        ///   Test with two key index
        /// </summary>
        /// <exception cref="System.Exception"></exception>
        [Test]
        public void TestInsertWith4IndexesAndCommits()
        {
            var baseName = GetBaseName();

            DeleteBase(baseName);
            var @base = Open(baseName);
            // Configuration.setUseLazyCache(true);
            // base.store(new IndexedObject());
            var clazz = @base.GetClassRepresentation(typeof (IndexedObject));
            var indexField1 = new[] {"duration"};
            clazz.AddUniqueIndexOn("inde1", indexField1, true);
            var indexFields3 = new[] {"name"};
            clazz.AddUniqueIndexOn("index3", indexFields3, true);
            var indexFields2 = new[] {"name", "creation"};
            clazz.AddUniqueIndexOn("index2", indexFields2, true);
            var indexField4 = new[] {"duration", "creation"};
            clazz.AddUniqueIndexOn("inde4", indexField4, true);
            @base.Close();
            @base = Open(baseName);
            var size = 100;
            var commitInterval = 10;
            var start0 = OdbTime.GetCurrentTimeInMs();
            for (var i = 0; i < size; i++)
            {
                // println(i);
                var io1 = new IndexedObject("olivier" + (i + 1), i, new DateTime());
                @base.Store(io1);
                if (i % 10 == 0)
                    Println(i);
                if (i % commitInterval == 0)
                    @base.Commit();
            }
            @base.Close();
            var end0 = OdbTime.GetCurrentTimeInMs();
            Println(string.Format("NU={0}", AbstractObjectWriter.GetNbNormalUpdates()));
            Println("inserting time with index=" + (end0 - start0));
            @base = Open(baseName);
            var start = OdbTime.GetCurrentTimeInMs();
            for (var i = 0; i < size; i++)
            {
                IQuery q = new CriteriaQuery(typeof (IndexedObject), Where.Equal("duration", i));
                var objects = @base.GetObjects<IndexedObject>(q, false);
                // println("olivier" + (i+1));
                AssertEquals(1, objects.Count);
            }
            var end = OdbTime.GetCurrentTimeInMs();
            var duration = end - start;
            Println("duration=" + duration);
            @base.Close();
            DeleteBase(baseName);

            if (testPerformance && duration > 111)
                Fail("Time of search in index : " + duration + ", should be less than 111");
        }

        /// <summary>
        ///   Test with one key index
        /// </summary>
        /// <exception cref="System.Exception"></exception>
        [Test]
        public void TestInsertWithDateIndex3CheckAll()
        {
            var baseName = GetBaseName();
            // LogUtil.logOn(LazyOdbBtreePersister.LOG_ID, true);
            DeleteBase(baseName);
            var @base = Open(baseName);

            // base.store(new IndexedObject());
            var clazz = @base.GetClassRepresentation(typeof (IndexedObject));
            var indexFields = new[] {"creation"};
            clazz.AddUniqueIndexOn("index1", indexFields, true);
            @base.Close();
            @base = Open(baseName);
            var size = 1300;
            var commitInterval = 1000;
            var start0 = OdbTime.GetCurrentTimeInMs();
            for (var i = 0; i < size; i++)
            {
                var io1 = new IndexedObject("olivier" + (i + 1), i, new DateTime(start0 + i));
                @base.Store(io1);
                if (i % commitInterval == 0)
                    @base.Commit();
            }
            // println(i+" : commit / " + size);
            @base.Close();
            var end0 = OdbTime.GetCurrentTimeInMs();
            Println(string.Format("NU={0}", AbstractObjectWriter.GetNbNormalUpdates()));
            // ObjectWriter.getNbNormalUpdates());
            // println("inserting time with index=" + (end0 - start0));
            @base = Open(baseName);
            var start = OdbTime.GetCurrentTimeInMs();
            for (var i = 0; i < size; i++)
            {
                IQuery q = new CriteriaQuery(typeof (IndexedObject), Where.Equal("creation", new DateTime(start0 + i)));
                var objects = @base.GetObjects<IndexedObject>(q, false);
                // println("olivier" + (i+1));
                AssertEquals(1, objects.Count);
            }
            var end = OdbTime.GetCurrentTimeInMs();
            try
            {
                var duration = (end - start) / (float) size;
                Println(duration);
                var d = 0.144;

                if (testPerformance && duration > d)
                    Fail("Time of search in index is greater than " + d + " ms : " + duration);
            }
            finally
            {
                @base.Close();
                DeleteBase(baseName);
            }
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public void TestInsertWithIndex()
        {
            var baseName = GetBaseName();
            DeleteBase(baseName);
            var @base = Open(baseName);
            var clazz = @base.GetClassRepresentation(typeof (IndexedObject));
            var indexFields = new[] {"name", "duration"};
            clazz.AddUniqueIndexOn("index1", indexFields, true);
            @base.Close();
            @base = Open(baseName);
            var io1 = new IndexedObject("olivier", 15, new DateTime());
            @base.Store(io1);
            @base.Close();
            @base = Open(baseName);
            IQuery q = new CriteriaQuery(typeof (IndexedObject), Where.IsNotNull("name"));
            var objects = @base.GetObjects<IndexedObject>(q, true);
            @base.Close();
            AssertEquals(1, objects.Count);
            var io2 = objects.GetFirst();
            AssertEquals("olivier", io2.GetName());
            AssertEquals(15, io2.GetDuration());
            AssertFalse(q.GetExecutionPlan().GetDetails().IndexOf("index1") != -1);
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public void TestInsertWithIndex1()
        {
            var baseName = GetBaseName();
            DeleteBase(baseName);
            var @base = Open(baseName);
            // base.store(new IndexedObject());
            var clazz = @base.GetClassRepresentation(typeof (IndexedObject));
            var indexFields = new[] {"name"};
            clazz.AddUniqueIndexOn("index1", indexFields, true);
            @base.Close();
            @base = Open(baseName);
            var size = 1000;
            var start0 = OdbTime.GetCurrentTimeInMs();
            for (var i = 0; i < size; i++)
            {
                var io1 = new IndexedObject("olivier" + (i + 1), 15 + i, new DateTime());
                @base.Store(io1);
                if (i % 100 == 0)
                    Println(i);
            }
            var tt0 = OdbTime.GetCurrentTimeInMs();
            @base.Close();
            var tt1 = OdbTime.GetCurrentTimeInMs();
            var end0 = OdbTime.GetCurrentTimeInMs();
            Println(string.Format("NU={0}", AbstractObjectWriter.GetNbNormalUpdates()));
            Println("inserting time with index=" + (end0 - start0));
            Println("commit time=" + (tt1 - tt0));
            Println(LazyOdbBtreePersister.Counters());
            @base = Open(baseName);
            long totalTime = 0;
            long maxTime = 0;
            long minTime = 100000;
            for (var i = 0; i < size; i++)
            {
                IQuery q = new CriteriaQuery(typeof (IndexedObject), Where.Equal("name", "olivier" + (i + 1)));
                var start = OdbTime.GetCurrentTimeInMs();
                var objects = @base.GetObjects<IndexedObject>(q, true);
                var end = OdbTime.GetCurrentTimeInMs();
                AssertEquals(1, objects.Count);
                var io2 = objects.GetFirst();
                AssertEquals("olivier" + (i + 1), io2.GetName());
                AssertEquals(15 + i, io2.GetDuration());
                var d = end - start;
                totalTime += d;
                if (d > maxTime)
                    maxTime = d;
                if (d < minTime)
                    minTime = d;
            }
            @base.Close();
            DeleteBase(baseName);
            Println("total duration=" + totalTime + " / " + (double) totalTime / size);
            Println("duration max=" + maxTime + " / min=" + minTime);
            if (testPerformance && totalTime / size > 2)
                Fail("Total/size is > than 2 : " + totalTime);
            if (testPerformance)
            {
                // TODO Try to get maxTime < 10!
                AssertTrue(maxTime < 100);
                AssertTrue(minTime < 1);
            }
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public void TestInsertWithIndex2()
        {
            var baseName = GetBaseName();

            DeleteBase(baseName);
            var @base = Open(baseName);
            // base.store(new IndexedObject());
            var clazz = @base.GetClassRepresentation(typeof (IndexedObject));
            var indexFields = new[] {"name"};
            clazz.AddUniqueIndexOn("index1", indexFields, true);
            var size = 1000;
            var start0 = OdbTime.GetCurrentTimeInMs();
            for (var i = 0; i < size; i++)
            {
                var io1 = new IndexedObject("olivier" + (i + 1), 15 + i, new DateTime());
                @base.Store(io1);
                if (i % 100 == 0)
                    Println(i);
            }
            var tt0 = OdbTime.GetCurrentTimeInMs();
            @base.Close();
            var tt1 = OdbTime.GetCurrentTimeInMs();
            var end0 = OdbTime.GetCurrentTimeInMs();
            Println(string.Format("NU={0}", AbstractObjectWriter.GetNbNormalUpdates()));
            Println("inserting time with index=" + (end0 - start0));
            Println("commit time=" + (tt1 - tt0));
            Println(LazyOdbBtreePersister.Counters());
            @base = Open(baseName);
            long totalTime = 0;
            long maxTime = 0;
            long minTime = 100000;
            var t0 = OdbTime.GetCurrentTimeInMs();
            long t1 = 0;
            for (var i = 0; i < size; i++)
            {
                IQuery q = new CriteriaQuery(typeof (IndexedObject), Where.Equal("name", "olivier" + (i + 1)));
                var start = OdbTime.GetCurrentTimeInMs();
                var objects = @base.GetObjects<IndexedObject>(q, true);
                var end = OdbTime.GetCurrentTimeInMs();
                AssertEquals(1, objects.Count);
                var io2 = objects.GetFirst();
                AssertEquals("olivier" + (i + 1), io2.GetName());
                AssertEquals(15 + i, io2.GetDuration());
                var d = end - start;
                totalTime += d;
                if (d > maxTime)
                    maxTime = d;
                if (d < minTime)
                    minTime = d;
                if (i % 100 == 0)
                {
                    t1 = OdbTime.GetCurrentTimeInMs();
                    Println("i=" + i + " - time=" + (t1 - t0));
                    t0 = t1;
                }
            }
            // /println(LazyOdbBtreePersister.counters());
            @base.Close();
            DeleteBase(baseName);
            // println("total duration=" + totalTime + " / " + (double) totalTime /
            // size);
            // println("duration max=" + maxTime + " / min=" + minTime);
            if (totalTime / size > 1)
                Fail("Total/size is > than 1 : " + (totalTime / (float) size));
            Println("Max time=" + maxTime);
            Println("Min time=" + minTime);
            // TODO Try to get maxTime < 10!
            AssertTrue(maxTime < 250);
            AssertTrue(minTime < 1);
        }

        /// <summary>
        ///   Test with on e key index
        /// </summary>
        /// <exception cref="System.Exception"></exception>
        [Test]
        public void TestInsertWithIndex3()
        {
            var baseName = GetBaseName();
            // LogUtil.logOn(LazyOdbBtreePersister.LOG_ID, true);
            DeleteBase(baseName);
            var @base = Open(baseName);

            // base.store(new IndexedObject());
            var clazz = @base.GetClassRepresentation(typeof (IndexedObject));
            var indexFields = new[] {"name"};
            clazz.AddUniqueIndexOn("index1", indexFields, true);
            @base.Close();
            @base = Open(baseName);
            var size = 1300;
            var commitInterval = 10;
            var start0 = OdbTime.GetCurrentTimeInMs();
            var engine = Dummy.GetEngine(@base);
            for (var i = 0; i < size; i++)
            {
                var io1 = new IndexedObject("olivier" + (i + 1), 15 + size, new DateTime());
                @base.Store(io1);
                if (i % commitInterval == 0)
                {
                    @base.Commit();
                    @base.Close();
                    @base = Open(baseName);
                    engine = Dummy.GetEngine(@base);
                }
                if (io1.GetName().Equals("olivier" + size))
                    Println("Ola chico");
            }
            engine = Dummy.GetEngine(@base);
            // println(new
            // BTreeDisplay().build(engine.getSession(true).getMetaModel().getClassInfo(IndexedObject.class.Name,
            // true).getIndex(0).getBTree(), true));
            @base.Close();
            var end0 = OdbTime.GetCurrentTimeInMs();
            Println(string.Format("NU={0}", AbstractObjectWriter.GetNbNormalUpdates()));
            // ObjectWriter.getNbNormalUpdates());
            Console.WriteLine("inserting time with index=" + (end0 - start0));
            @base = Open(baseName);
            engine = Dummy.GetEngine(@base);
            // println("After load = unconnected : "+
            // engine.getSession(true).getMetaModel().getClassInfo(IndexedObject.class.Name,
            // true).getUncommittedZoneInfo());
            // println("After Load = connected : "+
            // engine.getSession(true).getMetaModel().getClassInfo(IndexedObject.class.Name,
            // true).getCommitedZoneInfo());
            // println(new
            // BTreeDisplay().build(engine.getSession(true).getMetaModel().getClassInfo(IndexedObject.class.Name,
            // true).getIndex(0).getBTree(), true));
            IQuery q = new CriteriaQuery(typeof (IndexedObject), Where.Equal("name", "olivier" + size));
            var start = OdbTime.GetCurrentTimeInMs();
            var objects = @base.GetObjects<IndexedObject>(q, false);
            var end = OdbTime.GetCurrentTimeInMs();
            try
            {
                AssertEquals(1, objects.Count);
                var io2 = objects.GetFirst();
                AssertEquals("olivier" + size, io2.GetName());
                AssertEquals(15 + size, io2.GetDuration());
                var duration = end - start;
                Println("duration=" + duration);

                if (testPerformance)
                {
                    if (duration > 2)
                        Fail("Time of search in index is greater than 2ms : " + duration);
                }
            }
            finally
            {
                @base.Close();
                DeleteBase(baseName);
            }
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public void TestInsertWithIndex3Part1()
        {
            var baseName = "index.neodatis";
            // LogUtil.logOn(LazyOdbBtreePersister.LOG_ID, true);
            DeleteBase(baseName);
            var @base = Open(baseName);

            // base.store(new IndexedObject());
            var clazz = @base.GetClassRepresentation(typeof (IndexedObject));
            var indexFields = new[] {"name"};
            clazz.AddUniqueIndexOn("index1", indexFields, true);
            @base.Close();
            @base = Open(baseName);
            var size = 1300;
            var commitInterval = 10;
            var start0 = OdbTime.GetCurrentTimeInMs();
            var engine = Dummy.GetEngine(@base);
            for (var i = 0; i < size; i++)
            {
                var io1 = new IndexedObject("olivier" + (i + 1), 15 + size, new DateTime());
                @base.Store(io1);
                if (i % commitInterval == 0)
                {
                    @base.Commit();
                    @base.Close();
                    @base = Open(baseName);
                    engine = Dummy.GetEngine(@base);
                }
                if (io1.GetName().Equals("olivier" + size))
                    Println("Ola chico");
            }
            engine = Dummy.GetEngine(@base);
            // println(new
            // BTreeDisplay().build(engine.getSession(true).getMetaModel().getClassInfo(IndexedObject.class.Name,
            // true).getIndex(0).getBTree(), true));
            @base.Close();
            var end0 = OdbTime.GetCurrentTimeInMs();
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public void TestInsertWithIndex3Part2()
        {
            var baseName = "index.neodatis";
            var size = 1300;
            var @base = Open(baseName);
            IQuery q = new CriteriaQuery(typeof (IndexedObject), Where.Equal("name", "olivier" + size));
            var start = OdbTime.GetCurrentTimeInMs();
            var objects = @base.GetObjects<IndexedObject>(q, false);
            var end = OdbTime.GetCurrentTimeInMs();
            try
            {
                AssertEquals(1, objects.Count);
                var io2 = objects.GetFirst();
                AssertEquals("olivier" + size, io2.GetName());
                AssertEquals(15 + size, io2.GetDuration());
                var duration = end - start;
                Println("duration=" + duration);

                if (testPerformance)
                {
                    if (duration > 2)
                        Fail("Time of search in index is greater than 2ms : " + duration);
                }
            }
            finally
            {
                @base.Close();
            }
        }

        /// <summary>
        ///   Test with two key index
        /// </summary>
        /// <exception cref="System.Exception"></exception>
        [Test]
        public void TestInsertWithIndex4()
        {
            var baseName = GetBaseName();
            DeleteBase(baseName);
            var @base = Open(baseName);
            // base.store(new IndexedObject());
            var clazz = @base.GetClassRepresentation(typeof (IndexedObject));
            var indexFields3 = new[] {"name"};
            clazz.AddUniqueIndexOn("index3", indexFields3, true);
            var indexFields2 = new[] {"name", "creation"};
            clazz.AddUniqueIndexOn("index2", indexFields2, true);
            var indexField4 = new[] {"duration", "creation"};
            clazz.AddUniqueIndexOn("inde3", indexField4, true);
            @base.Close();
            @base = Open(baseName);
            var size = 500;
            var commitInterval = 1000;
            var start0 = OdbTime.GetCurrentTimeInMs();
            for (var i = 0; i < size; i++)
            {
                // println(i);
                var ioio = new IndexedObject("olivier" + (i + 1), i + 15 + size, new DateTime());
                @base.Store(ioio);
                if (i % commitInterval == 0)
                {
                    var t0 = OdbTime.GetCurrentTimeInMs();
                    @base.Commit();
                    var t1 = OdbTime.GetCurrentTimeInMs();
                    Println(i + " : commit - ctime " + (t1 - t0) + " -ttime=");
                    // println(LazyOdbBtreePersister.counters());
                    LazyOdbBtreePersister.ResetCounters();
                }
            }
            var theDate = new DateTime();
            var theName = "name indexed";
            var io1 = new IndexedObject(theName, 45, theDate);
            @base.Store(io1);
            @base.Close();
            var end0 = OdbTime.GetCurrentTimeInMs();
            Println(string.Format("NU={0}", AbstractObjectWriter.GetNbNormalUpdates()));
            Println("inserting time with index=" + (end0 - start0));
            @base = Open(baseName);
            // IQuery q = new
            // CriteriaQuery(IndexedObject.class,Restrictions.and().add(Restrictions.equal("name",theName)).add(Restrictions.equal("creation",
            // theDate)));
            IQuery q = new CriteriaQuery(typeof (IndexedObject), Where.Equal("name", theName));
            var start = OdbTime.GetCurrentTimeInMs();
            var objects = @base.GetObjects<IndexedObject>(q, true);
            var end = OdbTime.GetCurrentTimeInMs();
            AssertEquals("index3", q.GetExecutionPlan().GetIndex().Name);
            AssertEquals(1, objects.Count);
            var io2 = objects.GetFirst();
            AssertEquals(theName, io2.GetName());
            AssertEquals(45, io2.GetDuration());
            AssertEquals(theDate, io2.GetCreation());
            var duration = end - start;
            Println("duration=" + duration);
            @base.Close();
            DeleteBase(baseName);

            if (testPerformance && duration > 1)
                Fail("Time of search in index > 1 : " + duration);
        }

        // deleteBase(baseName);
        /// <summary>
        ///   Test with one key index
        /// </summary>
        /// <exception cref="System.Exception"></exception>
        [Test]
        public void TestInsertWithIntIndex3CheckAll()
        {
            var baseName = GetBaseName();
            // LogUtil.logOn(LazyOdbBtreePersister.LOG_ID, true);
            DeleteBase(baseName);
            var @base = Open(baseName);

            // base.store(new IndexedObject());
            var clazz = @base.GetClassRepresentation(typeof (IndexedObject));
            var indexFields = new[] {"duration"};
            clazz.AddUniqueIndexOn("index1", indexFields, true);
            @base.Close();
            @base = Open(baseName);
            var size = 1300;
            var commitInterval = 10;
            var start0 = OdbTime.GetCurrentTimeInMs();
            for (var i = 0; i < size; i++)
            {
                var io1 = new IndexedObject("olivier" + (i + 1), i, new DateTime());
                @base.Store(io1);
                if (i % commitInterval == 0)
                    @base.Commit();
            }
            // println(i+" : commit / " + size);
            @base.Close();
            var end0 = OdbTime.GetCurrentTimeInMs();
            Println(string.Format("NU={0}", AbstractObjectWriter.GetNbNormalUpdates()));
            Console.WriteLine("inserting time with index=" + (end0 - start0));
            @base = Open(baseName);
            var start = OdbTime.GetCurrentTimeInMs();
            for (var i = 0; i < size; i++)
            {
                IQuery q = new CriteriaQuery(typeof (IndexedObject), Where.Equal("duration", i));
                var objects = @base.GetObjects<IndexedObject>(q, false);
                // println("olivier" + (i+1));
                AssertEquals(1, objects.Count);
            }
            var end = OdbTime.GetCurrentTimeInMs();
            try
            {
                var duration = (end - start) / (float) size;
                if (testPerformance && duration > 2)
                    Fail("Time of search in index is greater than 2ms : " + duration);
            }
            finally
            {
                @base.Close();
                DeleteBase(baseName);
            }
        }

        /// <summary>
        ///   Test with on e key index
        /// </summary>
        /// <exception cref="System.Exception"></exception>
        [Test]
        public void TestInsertWithoutIndex3()
        {
            var baseName = GetBaseName();

            DeleteBase(baseName);
            var @base = Open(baseName);
            var size = 3000;
            var commitInterval = 1000;
            var start0 = OdbTime.GetCurrentTimeInMs();
            for (var i = 0; i < size; i++)
            {
                var io1 = new IndexedObject("olivier" + (i + 1), 15 + size, new DateTime());
                @base.Store(io1);
                if (i % commitInterval == 0)
                    @base.Commit();
            }
            // println(i+" : commit");
            @base.Close();
            var end0 = OdbTime.GetCurrentTimeInMs();
            Println(string.Format("NU={0}", AbstractObjectWriter.GetNbNormalUpdates()));
            Println("inserting time with index=" + (end0 - start0));
            @base = Open(baseName);
            IQuery q = new CriteriaQuery(typeof (IndexedObject), Where.Equal("name", "olivier" + size));
            var start = OdbTime.GetCurrentTimeInMs();
            var objects = @base.GetObjects<IndexedObject>(q, false);
            var end = OdbTime.GetCurrentTimeInMs();
            AssertEquals(1, objects.Count);
            var io2 = objects.GetFirst();
            AssertEquals("olivier" + size, io2.GetName());
            AssertEquals(15 + size, io2.GetDuration());
            var duration = end - start;
            Println("duration=" + duration);
            @base.Close();
            DeleteBase(baseName);

            Println(duration);
            double d = 408;

            if (duration > d)
                Fail("Time of search in index is greater than " + d + " ms : " + duration);
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public void TestSaveIndex()
        {
            var baseName = GetBaseName();
            DeleteBase(baseName);
            var @base = Open(baseName);
            var clazz = @base.GetClassRepresentation(typeof (IndexedObject));
            var indexFields = new[] {"name", "duration"};
            clazz.AddUniqueIndexOn("index1", indexFields, true);
            var indexFields2 = new[] {"name", "creation"};
            clazz.AddUniqueIndexOn("index2", indexFields2, true);
            var indexFields3 = new[] {"duration", "creation"};
            clazz.AddUniqueIndexOn("index3", indexFields3, true);
            @base.Close();
            @base = Open(baseName);
            var session = Dummy.GetEngine(@base).GetSession(true);
            var metaModel = session.GetStorageEngine().GetSession(true).GetMetaModel();
            var ci = metaModel.GetClassInfo(typeof (IndexedObject), true);
            AssertEquals(3, ci.GetNumberOfIndexes());
            AssertEquals(ci.GetIndex(0).Name, "index1");
            AssertEquals(1, ci.GetIndex(0).AttributeIds[0]);
            AssertEquals(ClassInfoIndex.Enabled, ci.GetIndex(0).Status);
            AssertEquals(ci.GetIndex(1).Name, "index2");
            AssertEquals(1, ci.GetIndex(1).AttributeIds[0]);
            AssertEquals(ClassInfoIndex.Enabled, ci.GetIndex(1).Status);
            AssertEquals(ci.GetIndex(2).Name, "index3");
            AssertEquals(2, ci.GetIndex(2).AttributeIds[0]);
            AssertEquals(ClassInfoIndex.Enabled, ci.GetIndex(0).Status);
            @base.Close();
            DeleteBase(baseName);
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public void TestSizeBTree()
        {
            var baseName = GetBaseName();
            DeleteBase(baseName);
            var @base = Open(baseName);
            // base.store(new IndexedObject());
            var clazz = @base.GetClassRepresentation(typeof (IndexedObject));
            var indexFields = new[] {"name"};
            clazz.AddUniqueIndexOn("index1", indexFields, true);
            @base.Close();
            @base = Open(baseName);
            var size = 4;
            for (var i = 0; i < size; i++)
            {
                var io1 = new IndexedObject("olivier" + (i + 1), 15 + i, new DateTime());
                @base.Store(io1);
                if (i % 1000 == 0)
                    Println(i);
            }
            @base.Close();
            @base = Open(baseName);
            var e = Dummy.GetEngine(@base);
            var cii = e.GetSession(true).GetMetaModel().GetClassInfo(typeof (IndexedObject), true).GetIndex(0);
            @base.Close();
            DeleteBase(baseName);
            AssertEquals(size, cii.BTree.GetSize());
        }

        /// <summary>
        ///   Test index.
        /// </summary>
        /// <remarks>
        ///   Test index. Creates 1000 objects. Take 10 objects to update 10000 times.
        ///   Then check if all objects are ok
        /// </remarks>
        /// <exception cref="System.Exception"></exception>
        [Test]
        public void TestXUpdatesWithIndex()
        {
            var baseName = GetBaseName();
            DeleteBase(baseName);
            var @base = Open(baseName);
            // base.store(new IndexedObject());
            var clazz = @base.GetClassRepresentation(typeof (IndexedObject));
            var indexFields = new[] {"name"};
            clazz.AddUniqueIndexOn("index", indexFields, true);
            @base.Close();
            @base = Open(baseName);
            var start = OdbTime.GetCurrentTimeInMs();
            var size = 100;
            var nbObjects = 10;
            var nbUpdates = 10;
            for (var i = 0; i < size; i++)
            {
                var io1 = new IndexedObject("IO-" + i + "-0", i + 15 + size, new DateTime());
                @base.Store(io1);
            }
            @base.Close();
            Println("Time of insert " + size + " objects = " + size);
            var indexes = new[]
                {
                    "IO-0-0", "IO-10-0", "IO-20-0", "IO-30-0", "IO-40-0", "IO-50-0", "IO-60-0", "IO-70-0",
                    "IO-80-0", "IO-90-0"
                };
            long t1 = 0;
            long t2 = 0;
            long t3 = 0;
            long t4 = 0;
            long t5 = 0;
            long t6 = 0;
            for (var i = 0; i < nbUpdates; i++)
            {
                start = OdbTime.GetCurrentTimeInMs();
                for (var j = 0; j < nbObjects; j++)
                {
                    t1 = OdbTime.GetCurrentTimeInMs();
                    @base = Open(baseName);
                    t2 = OdbTime.GetCurrentTimeInMs();
                    IQuery q = new CriteriaQuery(typeof (IndexedObject), Where.Equal("name", indexes[j]));
                    var os = @base.GetObjects<IndexedObject>(q);
                    t3 = OdbTime.GetCurrentTimeInMs();
                    AssertTrue(q.GetExecutionPlan().UseIndex());
                    AssertEquals(1, os.Count);
                    // check if index has been used
                    AssertTrue(q.GetExecutionPlan().UseIndex());
                    var io = os.GetFirst();
                    if (i > 0)
                        AssertTrue(io.GetName().EndsWith(("-" + (i - 1))));
                    io.SetName(io.GetName() + "-updated-" + i);
                    @base.Store(io);
                    t4 = OdbTime.GetCurrentTimeInMs();
                    if (j == 0)
                    {
                        var engine = Dummy.GetEngine(@base);
                        var ci = engine.GetSession(true).GetMetaModel().GetClassInfo(
                            typeof (IndexedObject), true);
                        var cii = ci.GetIndex(0);
                        AssertEquals(size, cii.BTree.GetSize());
                    }
                    indexes[j] = io.GetName();
                    AssertEquals(new Decimal(size), @base.Count(new CriteriaQuery(typeof (IndexedObject))));
                    t5 = OdbTime.GetCurrentTimeInMs();
                    @base.Commit();
                    @base.Close();
                    t6 = OdbTime.GetCurrentTimeInMs();
                }
                var end = OdbTime.GetCurrentTimeInMs();
                Console.Out.WriteLine("Nb Updates of " + nbObjects + " =" + i + " - " + (end - start) +
                                      "ms  -- open=" + (t2 - t1) + " - getObjects=" + (t3 - t2) + " - update=" +
                                      (t4 - t3) + " - count=" + (t5 - t4) + " - close=" + (t6 - t5));
            }
        }
    }
}
