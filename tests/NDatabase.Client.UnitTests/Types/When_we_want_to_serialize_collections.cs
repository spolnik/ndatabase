using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using NDatabase.Odb;
using NUnit.Framework;
using System.Linq;

namespace NDatabase.Client.UnitTests.Types
{
    public class When_we_want_to_serialize_collections
    {
        private const string DbName = "collections.ndb";

        [SetUp]
        public void SetUp()
        {
            OdbFactory.Delete(DbName);
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
        public void It_should_be_possible_to_store_list_directly()
        {
            var values = new List<string> {"One", "One", "Two", "Three", "Three", "Four"};

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

        [Test]
        public void It_should_be_possible_to_store_linked_list_directly()
        {
            var values = new LinkedList<string>();
            values.AddLast("One");
            values.AddLast("One");
            values.AddLast("Two");
            values.AddLast("Three");
            values.AddLast("Three");
            values.AddLast("Four");

            using (var odb = OdbFactory.Open(DbName))
            {
                odb.Store(values);
            }

            LinkedList<string> restoredItem;
            using (var odb = OdbFactory.Open(DbName))
            {
                restoredItem = odb.Query<LinkedList<string>>().Execute<LinkedList<string>>().FirstOrDefault();
            }

            Assert.That(restoredItem, Is.Not.Null);
            CollectionAssert.AreEqual(restoredItem, values);
        }

        [Test]
        public void It_should_be_possible_to_store_array_list_directly()
        {
            var values = new ArrayList { "One", "One", "Two", "Three", "Three", "Four" };

            using (var odb = OdbFactory.Open(DbName))
            {
                odb.Store(values);
            }

            ArrayList restoredItem;
            using (var odb = OdbFactory.Open(DbName))
            {
                restoredItem = odb.Query<ArrayList>().Execute<ArrayList>().FirstOrDefault();
            }

            Assert.That(restoredItem, Is.Not.Null);
            CollectionAssert.AreEqual(restoredItem, values);
        }

        [Test]
        public void It_should_be_possible_to_store_stack_directly()
        {
            var values = new Stack();

            values.Push("One");
            values.Push("One");
            values.Push("Two");
            values.Push("Three");
            values.Push("Three");
            values.Push("Four");

            using (var odb = OdbFactory.Open(DbName))
            {
                odb.Store(values);
            }

            Stack restoredItem;
            using (var odb = OdbFactory.Open(DbName))
            {
                restoredItem = odb.Query<Stack>().Execute<Stack>().FirstOrDefault();
            }

            Assert.That(restoredItem, Is.Not.Null);
            CollectionAssert.AreEqual(restoredItem, values);
        }

        [Test]
        [Ignore("Known issue - storing concurrent collections")]
        public void It_should_be_possible_to_store_concurrent_stack_directly()
        {
            var values = new ConcurrentStack<string>();

            values.Push("One");
            values.Push("One");
            values.Push("Two");
            values.Push("Three");
            values.Push("Three");
            values.Push("Four");

            using (var odb = OdbFactory.Open(DbName))
            {
                odb.Store(values);
            }

            ConcurrentStack<string> restoredItem;
            using (var odb = OdbFactory.Open(DbName))
            {
                restoredItem = odb.Query<ConcurrentStack<string>>().Execute<ConcurrentStack<string>>().FirstOrDefault();
            }

            Assert.That(restoredItem, Is.Not.Null);
            CollectionAssert.AreEqual(restoredItem, values);
        }

        [Test]
        public void It_should_be_possible_to_store_generic_stack_directly()
        {
            var values = new Stack<string>();

            values.Push("One");
            values.Push("One");
            values.Push("Two");
            values.Push("Three");
            values.Push("Three");
            values.Push("Four");

            using (var odb = OdbFactory.Open(DbName))
            {
                odb.Store(values);
            }

            Stack<string> restoredItem;
            using (var odb = OdbFactory.Open(DbName))
            {
                restoredItem = odb.Query<Stack<string>>().Execute<Stack<string>>().FirstOrDefault();
            }

            Assert.That(restoredItem, Is.Not.Null);
            CollectionAssert.AreEqual(restoredItem, values);
        }

        [Test]
        public void It_should_be_possible_to_store_queue_directly()
        {
            var values = new Queue();

            values.Enqueue("One");
            values.Enqueue("One");
            values.Enqueue("Two");
            values.Enqueue("Three");
            values.Enqueue("Three");
            values.Enqueue("Four");

            using (var odb = OdbFactory.Open(DbName))
            {
                odb.Store(values);
            }

            Queue restoredItem;
            using (var odb = OdbFactory.Open(DbName))
            {
                restoredItem = odb.Query<Queue>().Execute<Queue>().FirstOrDefault();
            }

            Assert.That(restoredItem, Is.Not.Null);
            CollectionAssert.AreEqual(restoredItem, values);
        }

        [Test]
        [Ignore("Known issue - storing concurrent collections")]
        public void It_should_be_possible_to_store_concurrent_queue_directly()
        {
            var values = new ConcurrentQueue<string>();

            values.Enqueue("One");
            values.Enqueue("One");
            values.Enqueue("Two");
            values.Enqueue("Three");
            values.Enqueue("Three");
            values.Enqueue("Four");

            using (var odb = OdbFactory.Open(DbName))
            {
                odb.Store(values);
            }

            ConcurrentQueue<string> restoredItem;
            using (var odb = OdbFactory.Open(DbName))
            {
                restoredItem = odb.Query<ConcurrentQueue<string>>().Execute<ConcurrentQueue<string>>().FirstOrDefault();
            }

            Assert.That(restoredItem, Is.Not.Null);
            CollectionAssert.AreEqual(restoredItem, values);
        }

        [Test]
        public void It_should_be_possible_to_store_generic_queue_directly()
        {
            var values = new Queue<string>();

            values.Enqueue("One");
            values.Enqueue("One");
            values.Enqueue("Two");
            values.Enqueue("Three");
            values.Enqueue("Three");
            values.Enqueue("Four");

            using (var odb = OdbFactory.Open(DbName))
            {
                odb.Store(values);
            }

            Queue<string> restoredItem;
            using (var odb = OdbFactory.Open(DbName))
            {
                restoredItem = odb.Query<Queue<string>>().Execute<Queue<string>>().FirstOrDefault();
            }

            Assert.That(restoredItem, Is.Not.Null);
            CollectionAssert.AreEqual(restoredItem, values);
        }

        [Test]
        public void It_should_be_possible_to_store_string_collection_directly()
        {
            var values = new StringCollection();

            values.Add("One");
            values.Add("One");
            values.Add("Two");
            values.Add("Three");
            values.Add("Three");
            values.Add("Four");

            using (var odb = OdbFactory.Open(DbName))
            {
                odb.Store(values);
            }

            StringCollection restoredItem;
            using (var odb = OdbFactory.Open(DbName))
            {
                restoredItem = odb.Query<StringCollection>().Execute<StringCollection>().FirstOrDefault();
            }

            Assert.That(restoredItem, Is.Not.Null);
            CollectionAssert.AreEqual(restoredItem, values);
        }

        [Test]
        public void It_should_be_possible_to_store_hashtable_directly()
        {
            var values = new Hashtable();

            values.Add("Key1", "One");
            values.Add("Key2", "One");
            values.Add("Key3", "Two");
            values.Add("Key4", "Three");
            values.Add("Key5", "Three");
            values.Add("Key6", "Four");

            using (var odb = OdbFactory.Open(DbName))
            {
                odb.Store(values);
            }

            Hashtable restoredItem;
            using (var odb = OdbFactory.Open(DbName))
            {
                restoredItem = odb.Query<Hashtable>().Execute<Hashtable>().FirstOrDefault();
            }

            Assert.That(restoredItem, Is.Not.Null);
            CollectionAssert.AreEqual(restoredItem.Keys, values.Keys);
            CollectionAssert.AreEqual(restoredItem.Values, values.Values);
        }

        [Test]
        public void It_should_be_possible_to_store_dictionary_directly()
        {
            var values = new Dictionary<string, string>();

            values.Add("Key1", "One");
            values.Add("Key2", "One");
            values.Add("Key3", "Two");
            values.Add("Key4", "Three");
            values.Add("Key5", "Three");
            values.Add("Key6", "Four");

            using (var odb = OdbFactory.Open(DbName))
            {
                odb.Store(values);
            }

            Dictionary<string, string> restoredItem;
            using (var odb = OdbFactory.Open(DbName))
            {
                restoredItem = odb.Query<Dictionary<string, string>>().Execute<Dictionary<string, string>>().FirstOrDefault();
            }

            Assert.That(restoredItem, Is.Not.Null);
            CollectionAssert.AreEqual(restoredItem.Keys, values.Keys);
            CollectionAssert.AreEqual(restoredItem.Values, values.Values);
        }

        [Test]
        public void It_should_be_possible_to_store_list_dictionary_directly()
        {
            var values = new ListDictionary();

            values.Add("Key1", "One");
            values.Add("Key2", "One");
            values.Add("Key3", "Two");
            values.Add("Key4", "Three");
            values.Add("Key5", "Three");
            values.Add("Key6", "Four");

            using (var odb = OdbFactory.Open(DbName))
            {
                odb.Store(values);
            }

            ListDictionary restoredItem;
            using (var odb = OdbFactory.Open(DbName))
            {
                restoredItem = odb.Query<ListDictionary>().Execute<ListDictionary>().FirstOrDefault();
            }

            Assert.That(restoredItem, Is.Not.Null);
            CollectionAssert.AreEqual(restoredItem.Keys, values.Keys);
            CollectionAssert.AreEqual(restoredItem.Values, values.Values);
        }

        [Test]
        public void It_should_be_possible_to_store_string_dictionary_directly()
        {
            var values = new StringDictionary();

            values.Add("Key1", "One");
            values.Add("Key2", "One");
            values.Add("Key3", "Two");
            values.Add("Key4", "Three");
            values.Add("Key5", "Three");
            values.Add("Key6", "Four");

            using (var odb = OdbFactory.Open(DbName))
            {
                odb.Store(values);
            }

            StringDictionary restoredItem;
            using (var odb = OdbFactory.Open(DbName))
            {
                restoredItem = odb.Query<StringDictionary>().Execute<StringDictionary>().FirstOrDefault();
            }

            Assert.That(restoredItem, Is.Not.Null);
            CollectionAssert.AreEqual(restoredItem.Keys, values.Keys);
            CollectionAssert.AreEqual(restoredItem.Values, values.Values);
        }

        [Test]
        public void It_should_be_possible_to_store_hybrid_dictionary_directly()
        {
            var values = new HybridDictionary();

            values.Add("Key1", "One");
            values.Add("Key2", "One");
            values.Add("Key3", "Two");
            values.Add("Key4", "Three");
            values.Add("Key5", "Three");
            values.Add("Key6", "Four");

            using (var odb = OdbFactory.Open(DbName))
            {
                odb.Store(values);
            }

            HybridDictionary restoredItem;
            using (var odb = OdbFactory.Open(DbName))
            {
                restoredItem = odb.Query<HybridDictionary>().Execute<HybridDictionary>().FirstOrDefault();
            }

            Assert.That(restoredItem, Is.Not.Null);
            CollectionAssert.AreEqual(restoredItem.Keys, values.Keys);
            CollectionAssert.AreEqual(restoredItem.Values, values.Values);
        }

        [Test]
        public void It_should_be_possible_to_store_sorted_dictionary_directly()
        {
            var values = new SortedDictionary<string, string>();
            
            values.Add("Key1", "One");
            values.Add("Key2", "One");
            values.Add("Key3", "Two");
            values.Add("Key4", "Three");
            values.Add("Key5", "Three");
            values.Add("Key6", "Four");

            using (var odb = OdbFactory.Open(DbName))
            {
                odb.Store(values);
            }

            SortedDictionary<string, string> restoredItem;
            using (var odb = OdbFactory.Open(DbName))
            {
                restoredItem =
                    odb.Query<SortedDictionary<string, string>>().Execute<SortedDictionary<string, string>>().
                        FirstOrDefault();
            }

            Assert.That(restoredItem, Is.Not.Null);
            CollectionAssert.AreEqual(restoredItem.Keys, values.Keys);
            CollectionAssert.AreEqual(restoredItem.Values, values.Values);
        }

        [Test]
        [Ignore("Known issue - storing concurrent collections")]
        public void It_should_be_possible_to_store_concurrent_dictionary_directly()
        {
            var values = new ConcurrentDictionary<string, string>();

            values.GetOrAdd("Key1", "One");
            values.GetOrAdd("Key2", "One");
            values.GetOrAdd("Key3", "Two");
            values.GetOrAdd("Key4", "Three");
            values.GetOrAdd("Key5", "Three");
            values.GetOrAdd("Key6", "Four");

            using (var odb = OdbFactory.Open(DbName))
            {
                odb.Store(values);
            }

            ConcurrentDictionary<string, string> restoredItem;
            using (var odb = OdbFactory.Open(DbName))
            {
                restoredItem =
                    odb.Query<ConcurrentDictionary<string, string>>().Execute<ConcurrentDictionary<string, string>>().
                        FirstOrDefault();
            }

            Assert.That(restoredItem, Is.Not.Null);
            CollectionAssert.AreEqual(restoredItem.Keys, values.Keys);
            CollectionAssert.AreEqual(restoredItem.Values, values.Values);
        }

        [Test]
        public void It_should_be_possible_to_store_sorted_list_directly()
        {
            var values = new SortedList<string, string>();

            values.Add("Key1", "One");
            values.Add("Key2", "One");
            values.Add("Key3", "Two");
            values.Add("Key4", "Three");
            values.Add("Key5", "Three");
            values.Add("Key6", "Four");

            using (var odb = OdbFactory.Open(DbName))
            {
                odb.Store(values);
            }

            SortedList<string, string> restoredItem;
            using (var odb = OdbFactory.Open(DbName))
            {
                restoredItem = odb.Query<SortedList<string, string>>().Execute<SortedList<string, string>>().FirstOrDefault();
            }

            Assert.That(restoredItem, Is.Not.Null);
            CollectionAssert.AreEqual(restoredItem.Keys, values.Keys);
            CollectionAssert.AreEqual(restoredItem.Values, values.Values);
        }
    }
}
