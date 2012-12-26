using System.Collections.Generic;
using NDatabase2.Odb;
using NUnit.Framework;
using System.Linq;

namespace NDatabase.Client.UnitTests.Bugs
{
    public class When_we_want_to_serialize_class_with_HashSet_as_the_property
    {
        private const string DbName = "hashset.ndb";

        [SetUp]
        public void SetUp()
        {
            OdbFactory.Delete(DbName);
        }

        [Test]
        public void It_should_be_properly_stored_and_then_queried()
        {
            var item = PrepareData();

            using (var odb = OdbFactory.Open(DbName))
            {
                odb.Store(item);
            }

            TestCaseWithHashSet restoredItem;
            using (var odb = OdbFactory.Open(DbName))
            {
                restoredItem = odb.Query<TestCaseWithHashSet>().Execute<TestCaseWithHashSet>().FirstOrDefault();
            }

            Assert.That(restoredItem, Is.Not.Null);
            Assert.That(restoredItem.Value, Is.EqualTo(item.Value));
            CollectionAssert.AreEqual(restoredItem.DistinctValues, item.DistinctValues);
        }

        [Test]
        public void It_should_be_possible_to_store_hashset_directly()
        {
            var distinctValues = new HashSet<string> {"One", "One", "Two", "Three", "Three", "Four"};

            using (var odb = OdbFactory.Open(DbName))
            {
                odb.Store(distinctValues);
            }

            HashSet<string> restoredItem;
            using (var odb = OdbFactory.Open(DbName))
            {
                restoredItem = odb.Query<HashSet<string>>().Execute<HashSet<string>>().FirstOrDefault();
            }

            Assert.That(restoredItem, Is.Not.Null);
            CollectionAssert.AreEqual(restoredItem, distinctValues);
        }

        [Test]
        public void It_should_be_possible_to_store_list_directly_too()
        {
            var values = new List<string> { "One", "One", "Two", "Three", "Three", "Four" };

            using (var odb = OdbFactory.Open(DbName))
            {
                odb.Store(values);
            }

            List<string> restoredItem;
            using (var odb = OdbFactory.Open(DbName))
            {
                restoredItem = odb.Query<List<string>>().Execute<List<string>>().FirstOrDefault();
            }

            Assert.That(restoredItem, Is.Not.Null);
            CollectionAssert.AreEqual(restoredItem, values);
        }

        private static TestCaseWithHashSet PrepareData()
        {
            var withHashSet = new TestCaseWithHashSet {Value = "Checking"};

            withHashSet.DistinctValues.Add("One");
            withHashSet.DistinctValues.Add("One");
            withHashSet.DistinctValues.Add("Two");
            withHashSet.DistinctValues.Add("Three");
            withHashSet.DistinctValues.Add("Three");
            withHashSet.DistinctValues.Add("Four");

            return withHashSet;
        }


        class TestCaseWithHashSet
        {
            public TestCaseWithHashSet()
            {
                DistinctValues = new HashSet<string>();
            }

            public string Value { get; set; }

            public HashSet<string> DistinctValues { get; set; }
        }
    }
}