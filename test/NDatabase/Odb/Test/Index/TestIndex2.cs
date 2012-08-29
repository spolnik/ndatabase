using System;
using NDatabase.Btree.Exception;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Query;
using NDatabase.Odb.Core.Query.Criteria;
using NDatabase.Odb.Impl.Core.Layers.Layer3.Engine;
using NDatabase.Odb.Impl.Core.Query.Criteria;
using NUnit.Framework;

namespace Test.NDatabase.Odb.Test.Index
{
    [TestFixture]
    public class TestIndex2 : ODBTest
    {
        /// <summary>
        ///   Opens a connection C1, then create the index in another connection C2 and
        ///   then stores the object in connection C1
        /// </summary>
        /// <exception cref="System.Exception">System.Exception</exception>
        [Test]
        [Ignore("No Support for multiple access to db in the same time for now")]
        public virtual void TestCreateIndexInOtherConnection()
        {
            var baseName = GetBaseName();
            DeleteBase(baseName);
            var odb1 = Open(baseName);
            var odb2 = Open(baseName);
            var clazz = odb2.GetClassRepresentation(typeof (IndexedObject3));
            var indexFields1 = new[] {"i1"};
            clazz.AddUniqueIndexOn("index1", indexFields1, true);
            odb2.Close();
            for (var i = 0; i < 10; i++)
            {
                var io = new IndexedObject3(1 + i, 2, 3, "1" + i, "2", "3", new DateTime(2009, i, 1), new DateTime(),
                                            new DateTime());
                odb1.Store(io);
            }
            odb1.Close();
            var odb = Open(baseName);
            IQuery q = new CriteriaQuery(typeof (IndexedObject3), Where.Equal("i1", 1));
            var iis = odb.GetObjects<IndexedObject3>(q);
            odb.Close();
            AssertEquals(1, iis.Count);
            AssertTrue(q.GetExecutionPlan().UseIndex());
            DeleteBase(baseName);
        }

        /// <summary>
        ///   Opens a connection C1, then create the index in another connection C2 and
        ///   then stores the object in connection C1
        /// </summary>
        /// <exception cref="System.Exception">System.Exception</exception>
        [Test]
        [Ignore("No Support for multiple access to db in the same time for now")]
        public virtual void TestCreateIndexInOtherConnectionNoClose()
        {
            var baseName = GetBaseName();
            DeleteBase(baseName);
            var odb1 = Open(baseName);
            var odb2 = Open(baseName);
            var clazz = odb2.GetClassRepresentation(typeof (IndexedObject3));
            var indexFields1 = new[] {"i1"};
            clazz.AddUniqueIndexOn("index1", indexFields1, true);
            odb2.Commit();
            for (var i = 0; i < 10; i++)
            {
                var io = new IndexedObject3(1 + i, 2, 3, "1" + i, "2", "3", new DateTime(2009, i, 1), new DateTime(),
                                            new DateTime());
                odb1.Store(io);
            }
            odb1.Close();
            var odb = Open(baseName);
            IQuery q = new CriteriaQuery(typeof (IndexedObject3), Where.Equal("i1", 1));
            var iis = odb.GetObjects<IndexedObject3>(q);
            odb.Close();
            odb2.Close();
            AssertEquals(1, iis.Count);
            AssertTrue(q.GetExecutionPlan().UseIndex());
            DeleteBase(baseName);
        }

        /// <summary>
        ///   Opens a connection C1, then create the index in another connection C2 and
        ///   then stores the object in connection C1
        /// </summary>
        /// <exception cref="System.Exception">System.Exception</exception>
        [Test]
        [Ignore("No Support for multiple access to db in the same time for now")]
        public virtual void TestCreateIndexInOtherConnectionNoCommit1()
        {
            var baseName = GetBaseName();
            DeleteBase(baseName);
            var odb1 = Open(baseName);
            var odb2 = Open(baseName);
            var clazz = odb2.GetClassRepresentation(typeof (IndexedObject3));
            var indexFields1 = new[] {"i1"};
            clazz.AddUniqueIndexOn("index1", indexFields1, true);
            for (var i = 0; i < 10; i++)
            {
                var io = new IndexedObject3(1 + i, 2, 3, "1" + i, "2", "3", new DateTime(2009, i, 1), new DateTime(),
                                            new DateTime());
                odb1.Store(io);
            }
            odb2.Close();
            odb1.Close();
            var odb = Open(baseName);
            IQuery q = new CriteriaQuery(typeof (IndexedObject3), Where.Equal("i1", 1));
            var iis = odb.GetObjects<IndexedObject3>(q);
            odb.Close();
            AssertEquals(1, iis.Count);
            AssertTrue(q.GetExecutionPlan().UseIndex());
            DeleteBase(baseName);
        }

        /// <summary>
        ///   Opens a connection C1, then create the index in another connection C2 and
        ///   then stores the object in connection C1
        /// </summary>
        /// <exception cref="System.Exception">System.Exception</exception>
        [Test]
        [Ignore("No Support for multiple access to db in the same time for now")]
        public virtual void TestCreateIndexInOtherConnectionNoCommit2()
        {
            var baseName = GetBaseName();
            DeleteBase(baseName);
            var odb1 = Open(baseName);
            var odb2 = Open(baseName);
            var clazz = odb2.GetClassRepresentation(typeof (IndexedObject3));
            var indexFields1 = new[] {"i1"};
            clazz.AddUniqueIndexOn("index1", indexFields1, true);
            for (var i = 0; i < 10; i++)
            {
                var io = new IndexedObject3(1 + i, 2, 3, "1" + i, "2", "3", new DateTime(2009, i, 1), new DateTime(),
                                            new DateTime());
                odb1.Store(io);
            }
            odb1.Close();
            odb2.Close();
            var odb = Open(baseName);
            IQuery q = new CriteriaQuery(typeof (IndexedObject3), Where.Equal("i1", 1));
            var iis = odb.GetObjects<IndexedObject3>(q);
            odb.Close();
            AssertEquals(1, iis.Count);
            AssertTrue(q.GetExecutionPlan().UseIndex());
            DeleteBase(baseName);
        }

        /// <summary>
        ///   Test index creation without commit
        /// </summary>
        /// <exception cref="System.Exception">System.Exception</exception>
        [Test]
        public virtual void TestCreateIndexWithoutCommit()
        {
            var baseName = GetBaseName();
            DeleteBase(baseName);
            var @base = Open(baseName);
            var clazz = @base.GetClassRepresentation(typeof (IndexedObject3));
            var indexFields1 = new[] {"i1"};
            clazz.AddUniqueIndexOn("index1", indexFields1, true);
            for (var i = 0; i < 10; i++)
            {
                var io = new IndexedObject3(1 + i, 2, 3, "1" + i, "2", "3", new DateTime(2009, i + 1, 1), new DateTime(),
                                            new DateTime());
                @base.Store(io);
            }
            @base.Close();
            @base = Open(baseName);
            IQuery q = new CriteriaQuery(typeof (IndexedObject3), Where.Equal("i1", 1));
            var iis = @base.GetObjects<IndexedObject3>(q);
            @base.Close();
            AssertEquals(1, iis.Count);
            AssertTrue(q.GetExecutionPlan().UseIndex());
            DeleteBase(baseName);
        }

        /// <summary>
        ///   Create objects, then create index, then execute a select with index, then
        ///   rebuild index e execute
        /// </summary>
        /// <exception cref="System.Exception">System.Exception</exception>
        [Test]
        public virtual void TestDeleteIndex()
        {
            var baseName = GetBaseName();
            DeleteBase(baseName);
            var @base = Open(baseName);
            for (var i = 0; i < 250; i++)
            {
                var io = new IndexedObject3(1 + i, 2, 3, "1" + i, "2", "3", new DateTime(2009, (i % 12) + 1, 1),
                                            new DateTime(), new DateTime());
                @base.Store(io);
            }
            @base.Close();
            @base = Open(baseName);
            var clazz = @base.GetClassRepresentation(typeof (IndexedObject3));
            var indexFields1 = new[] {"i1", "i2", "i3"};
            clazz.AddUniqueIndexOn("index1", indexFields1, true);
            @base.Close();
            @base = Open(baseName);
            var session = Dummy.GetEngine(@base).GetSession(true);
            var metaModel = session.GetStorageEngine().GetSession(true).GetMetaModel();
            var ci = metaModel.GetClassInfo(typeof (IndexedObject3), true);
            AssertEquals(1, ci.GetNumberOfIndexes());
            AssertEquals(ci.GetIndex(0).Name, "index1");
            AssertEquals(3, ci.GetIndex(0).AttributeIds.Length);
            AssertEquals(ClassInfoIndex.Enabled, ci.GetIndex(0).Status);
            IQuery q = new CriteriaQuery(typeof (IndexedObject3),
                                         Where.And().Add(Where.Equal("i1", 10)).Add(Where.Equal("i2", 2)).Add(
                                             Where.Equal("i3", 3)));
            var objects = @base.GetObjects<IndexedObject3>(q);
            AssertEquals(true, q.GetExecutionPlan().UseIndex());
            @base.GetClassRepresentation(typeof (IndexedObject3)).DeleteIndex("index1", true);
            @base.Close();
            @base = Open(baseName);
            objects = @base.GetObjects<IndexedObject3>(q);
            AssertEquals(false, q.GetExecutionPlan().UseIndex());
            @base.Close();
            DeleteBase(baseName);
        }

        [Test]
        public virtual void TestIndexFail()
        {
            var baseName = GetBaseName();
            DeleteBase(baseName);
            var @base = Open(baseName);
            var indexName = "index1";
            var clazz = @base.GetClassRepresentation(typeof (IndexedObject3));
            var indexFields1 = new[] {"i1", "i2", "i3"};
            clazz.AddUniqueIndexOn(indexName, indexFields1, true);
            @base.Close();
            @base = Open(baseName);
            var io = new IndexedObject3(1, 2, 3, "1", "2", "3", new DateTime(), new DateTime(), new DateTime());
            @base.Store(io);
            try
            {
                var io2 = new IndexedObject3(1, 2, 3, "1", "2", "3", new DateTime(), new DateTime(), new DateTime());
                @base.Store(io2);
            }
            catch (DuplicatedKeyException e)
            {
                Console.WriteLine(e.Message);
                Assert.Pass();
            }
            // println(e.getMessage());
            @base.Close();
            @base = Open(baseName);
            var oo3 = @base.GetObjects<IndexedObject3>();
            @base.Close();
            AssertEquals(0, oo3.Count);
            DeleteBase(baseName);
        }

        /// <summary>
        ///   Create objects, then create index, then execute a select with index, then
        ///   rebuild index e execute
        /// </summary>
        /// <exception cref="System.Exception">System.Exception</exception>
        [Test]
        public virtual void TestRebuildIndex()
        {
            var baseName = GetBaseName();
            DeleteBase(baseName);
            var @base = Open(baseName);
            for (var i = 0; i < 250; i++)
            {
                var io = new IndexedObject3(1 + i, 2, 3, "1" + i, "2", "3", new DateTime(2009, (i % 12) + 1, 1),
                                            new DateTime(), new DateTime());
                @base.Store(io);
            }
            @base.Close();
            @base = Open(baseName);
            var clazz = @base.GetClassRepresentation(typeof (IndexedObject3));
            var indexFields1 = new[] {"i1", "i2", "i3"};
            clazz.AddUniqueIndexOn("index1", indexFields1, true);
            @base.Close();
            @base = Open(baseName);
            var session = Dummy.GetEngine(@base).GetSession(true);
            var metaModel = session.GetStorageEngine().GetSession(true).GetMetaModel();
            var ci = metaModel.GetClassInfo(typeof (IndexedObject3), true);
            AssertEquals(1, ci.GetNumberOfIndexes());
            AssertEquals(ci.GetIndex(0).Name, "index1");
            AssertEquals(3, ci.GetIndex(0).AttributeIds.Length);
            AssertEquals(ClassInfoIndex.Enabled, ci.GetIndex(0).Status);
            IQuery q = new CriteriaQuery(typeof (IndexedObject3),
                                         Where.And().Add(Where.Equal("i1", 10)).Add(Where.Equal("i2", 2)).Add(
                                             Where.Equal("i3", 3)));
            var objects = @base.GetObjects<IndexedObject3>(q);
            AssertEquals(true, q.GetExecutionPlan().UseIndex());
            @base.GetClassRepresentation(typeof (IndexedObject3)).RebuildIndex("index1", true);
            @base.Close();
            @base = Open(baseName);
            objects = @base.GetObjects<IndexedObject3>(q);
            AssertEquals(true, q.GetExecutionPlan().UseIndex());
            @base.Close();
            DeleteBase(baseName);
        }

        [Test]
        public virtual void TestSaveIndex()
        {
            var baseName = GetBaseName();
            DeleteBase(baseName);
            var @base = Open(baseName);
            var clazz = @base.GetClassRepresentation(typeof (IndexedObject3));
            var indexFields1 = new[] {"i1", "i2", "i3"};
            clazz.AddUniqueIndexOn("index1", indexFields1, true);
            var indexFields2 = new[] {"s1", "s2", "s3"};
            clazz.AddUniqueIndexOn("index2", indexFields2, true);
            var indexFields3 = new[] {"dt1", "dt2", "dt3"};
            clazz.AddUniqueIndexOn("index3", indexFields3, true);
            var indexFields4 = new[] {"i1", "i2", "i3", "s1", "s2", "s3", "dt1", "dt2", "dt3"};
            clazz.AddUniqueIndexOn("index4", indexFields4, true);
            @base.Close();
            @base = Open(baseName);
            var session = Dummy.GetEngine(@base).GetSession(true);
            var metaModel = session.GetStorageEngine().GetSession(true).GetMetaModel();
            var ci = metaModel.GetClassInfo(typeof (IndexedObject3), true);
            AssertEquals(4, ci.GetNumberOfIndexes());
            AssertEquals(ci.GetIndex(0).Name, "index1");
            AssertEquals(3, ci.GetIndex(0).AttributeIds.Length);
            AssertEquals(ClassInfoIndex.Enabled, ci.GetIndex(0).Status);
            AssertEquals(ci.GetIndex(1).Name, "index2");
            AssertEquals(3, ci.GetIndex(1).AttributeIds.Length);
            AssertEquals(ClassInfoIndex.Enabled, ci.GetIndex(1).Status);
            AssertEquals(ci.GetIndex(2).Name, "index3");
            AssertEquals(3, ci.GetIndex(2).AttributeIds.Length);
            AssertEquals(ClassInfoIndex.Enabled, ci.GetIndex(2).Status);
            AssertEquals(ci.GetIndex(3).Name, "index4");
            AssertEquals(9, ci.GetIndex(3).AttributeIds.Length);
            AssertEquals(ClassInfoIndex.Enabled, ci.GetIndex(3).Status);
            @base.Close();
            @base = Open(baseName);
            for (var i = 0; i < 10; i++)
            {
                var io = new IndexedObject3(1 + i, 2, 3, "1" + i, "2", "3", new DateTime(2009, i + 1, 1), new DateTime(),
                                            new DateTime());
                @base.Store(io);
            }
            @base.Close();
            DeleteBase(baseName);
        }
    }
}
