using System;
using NDatabase.Odb;
using NDatabase.Odb.Core.Query;
using NDatabase.Odb.Core.Query.Criteria;
using NDatabase.Odb.Impl.Core.Query.Criteria;
using NDatabase.Tool.Wrappers;
using NUnit.Framework;

namespace Test.NDatabase.Odb.Test.Index
{
    /// <summary>
    ///   Nunit to test indexing an object when the index field is an object and not a
    ///   native attribute
    /// </summary>
    [TestFixture]
    public class TestIndexingByObject : ODBTest
    {
        [Test]
        public virtual void Test1()
        {
            Println("************START OF TEST1***************");
            var baseName = GetBaseName();
            DeleteBase("index-object");
            var odb = Open("index-object");
            var fields = new[] {"object"};
            odb.GetClassRepresentation(typeof (IndexedObject2)).AddUniqueIndexOn("index1", fields, true);
            var o1 = new IndexedObject2("Object1", new IndexedObject("Inner Object 1", 10, new DateTime()));
            odb.Store(o1);
            odb.Close();
            odb = Open("index-object");
            // First get the object used to index
            var objects = odb.GetObjects<IndexedObject>();
            var io = objects.GetFirst();

            IQuery q = odb.CriteriaQuery(typeof (IndexedObject2), Where.Equal("object", io));

            var objects2 = odb.GetObjects<IndexedObject2>(q);
            var o2 = objects2.GetFirst();

            odb.Close();
            AssertEquals(o1.GetName(), o2.GetName());
            Println(q.GetExecutionPlan().GetDetails());
            AssertFalse(q.GetExecutionPlan().GetDetails().IndexOf("index1") == -1);

            DeleteBase("index-object");
            Println("************END OF TEST1***************");
        }

        [Test]
        public virtual void Test2()
        {
            Println("************START OF TEST2***************");
            DeleteBase("index-object");
            var odb = Open("index-object");
            var fields = new[] {"object"};
            odb.GetClassRepresentation(typeof (IndexedObject2)).AddUniqueIndexOn("index1", fields, true);
            var size = 500;
            for (var i = 0; i < size; i++)
                odb.Store(new IndexedObject2("Object " + i, new IndexedObject("Inner Object " + i, i, new DateTime())));
            odb.Close();
            odb = Open("index-object");
            IQuery q = new CriteriaQuery(typeof (IndexedObject), Where.Equal("name", "Inner Object " + (size - 1)));
            // First get the object used to index, the last one. There is no index
            // on the class and field
            var start0 = OdbTime.GetCurrentTimeInMs();
            var objects = odb.GetObjects<IndexedObject>(q);
            var end0 = OdbTime.GetCurrentTimeInMs();
            var io = objects.GetFirst();
            Println("d0=" + (end0 - start0));
            Println(q.GetExecutionPlan().GetDetails());
            q = odb.CriteriaQuery(typeof (IndexedObject2), Where.Equal("object", io));
            var start = OdbTime.GetCurrentTimeInMs();

            var objects2 = odb.GetObjects<IndexedObject2>(q);
            var end = OdbTime.GetCurrentTimeInMs();
            Println("d=" + (end - start));
            var o2 = objects2.GetFirst();
            odb.Close();
            AssertEquals("Object " + (size - 1), o2.GetName());
            Println(q.GetExecutionPlan().GetDetails());
            AssertTrue(q.GetExecutionPlan().UseIndex());
            DeleteBase("index-object");
            Println("************END OF TEST2***************");
        }

        [Test]
        public virtual void Test3_BadAttributeInIndex()
        {
            var baseName = GetBaseName();
            IOdb odb = null;
            var fieldName = "fkjdsfkjdhfjkdhjkdsh";
            try
            {
                odb = Open(baseName);
                var fields = new[] {fieldName};
                odb.GetClassRepresentation(typeof (IndexedObject2)).AddUniqueIndexOn("index1", fields, true);
                Fail("Should have thrown an exception because the field " + fieldName + " does not exist");
            }
            catch (Exception)
            {
            }
            finally
            {
                // normal
                odb.Close();
                DeleteBase(baseName);
            }
        }

        [Test]
        public virtual void Test4()
        {
            var baseName = GetBaseName();
            var odb = Open(baseName);
            var fields = new[] {"object"};
            odb.GetClassRepresentation(typeof (IndexedObject2)).AddUniqueIndexOn("index1", fields, true);
            var fields2 = new[] {"name"};
            odb.GetClassRepresentation(typeof (IndexedObject)).AddUniqueIndexOn("index2", fields2, true);
            var size = 500;
            for (var i = 0; i < size; i++)
                odb.Store(new IndexedObject2("Object " + i, new IndexedObject("Inner Object " + i, i, new DateTime())));
            odb.Close();
            odb = Open(baseName);
            IQuery q = new CriteriaQuery(typeof (IndexedObject), Where.Equal("name", "Inner Object " + (size - 1)));
            // First get the object used to index, the last one. There is no index
            // on the class and field
            var start0 = OdbTime.GetCurrentTimeInMs();
            var objects = odb.GetObjects<IndexedObject>(q);
            var end0 = OdbTime.GetCurrentTimeInMs();
            // check if index has been used
            AssertTrue(q.GetExecutionPlan().UseIndex());
            var io = objects.GetFirst();
            Println("d0=" + (end0 - start0));
            Println(q.GetExecutionPlan().GetDetails());
            q = odb.CriteriaQuery(typeof (IndexedObject2), Where.Equal("object", io));
            var start = OdbTime.GetCurrentTimeInMs();

            var objects2 = odb.GetObjects<IndexedObject2>(q);
            var end = OdbTime.GetCurrentTimeInMs();
            Println("d=" + (end - start));
            var o2 = objects2.GetFirst();
            odb.Close();
            AssertEquals("Object " + (size - 1), o2.GetName());
            Println(q.GetExecutionPlan().GetDetails());
            AssertTrue(q.GetExecutionPlan().UseIndex());
            DeleteBase(baseName);
        }
    }
}
