using NDatabase2.Odb.Core.Query;
using NDatabase2.Odb.Core.Query.Criteria;
using NUnit.Framework;
using Test.NDatabase.Odb.Test.VO.Arraycollectionmap;

namespace Test.NDatabase.Odb.Test.Arraycollectionmap
{
    [TestFixture]
    public class TestMap : ODBTest
    {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp()
        {
            DeleteBase("map.neodatis");
            var odb = Open("map.neodatis");
            var dictionnary1 = new Dictionnary("test1");
            dictionnary1.AddEntry("olivier", "Smadja");
            dictionnary1.AddEntry("kiko", "vidal");
            dictionnary1.AddEntry("karine", "galvao");
            var dictionnary2 = new Dictionnary("test2");
            dictionnary2.AddEntry("f1", new VO.Login.Function("function1"));
            dictionnary2.AddEntry("f2", new VO.Login.Function("function2"));
            dictionnary2.AddEntry("f3", new VO.Login.Function("function3"));
            dictionnary2.AddEntry(dictionnary1, new VO.Login.Function("function4"));

            dictionnary2.AddEntry("f4", null);
            odb.Store(dictionnary1);
            odb.Store(dictionnary2);
            odb.Store(new VO.Login.Function("login"));
            odb.Close();
        }

        #endregion

        public override void TearDown()
        {
            DeleteBase("map.neodatis");
        }

        [Test]
        public virtual void Test1()
        {
            var odb = Open("map.neodatis");
            var l = odb.Query<Dictionnary>(true);
            // assertEquals(2,l.size());
            var dictionnary = l.GetFirst();
            AssertEquals("Smadja", dictionnary.Get("olivier"));
            odb.Close();
        }

        [Test]
        public virtual void Test2()
        {
            var odb = Open("map.neodatis");
            var l = odb.Query<Dictionnary>();
            var aq = new CriteriaQuery<Dictionnary>(Where.Equal("name", "test2"));
            l = odb.Query<Dictionnary>(aq);
            var dictionnary = l.GetFirst();
            AssertEquals("function2", ((VO.Login.Function) dictionnary.Get("f2")).GetName());
            odb.Close();
        }

        [Test]
        public virtual void Test3()
        {
            var odb = Open("map.neodatis");
            var size = odb.Count(new CriteriaQuery<Dictionnary>());
            var dictionnary1 = new Dictionnary("test1");
            dictionnary1.SetMap(null);
            odb.Store(dictionnary1);
            odb.Close();
            odb = Open("map.neodatis");
            AssertEquals(size + 1, odb.Query<Dictionnary>().Count);
            AssertEquals(size + 1, odb.Count(new CriteriaQuery<Dictionnary>()));
            odb.Close();
        }

        [Test]
        public virtual void Test4()
        {
            var odb = Open("map.neodatis");
            var n = odb.Count(new CriteriaQuery<Dictionnary>());
            IQuery query = new CriteriaQuery<Dictionnary>(Where.Equal("name", "test2"));
            var l = odb.Query<Dictionnary>(query);
            var dictionnary = l.GetFirst();
            dictionnary.SetMap(null);
            odb.Store(dictionnary);
            odb.Close();
            odb = Open("map.neodatis");
            AssertEquals(n, odb.Count(new CriteriaQuery<Dictionnary>()));
            var dic = odb.Query<Dictionnary>(query).GetFirst();
            AssertEquals(null, dic.GetMap());
            odb.Close();
        }

        [Test]
        public virtual void Test5updateIncreasingSize()
        {
            var odb = Open("map.neodatis");
            var n = odb.Count(new CriteriaQuery<Dictionnary>());
            IQuery query = new CriteriaQuery<Dictionnary>(Where.Equal("name", "test2"));
            var l = odb.Query<Dictionnary>(query);
            var dictionnary = l.GetFirst();
            dictionnary.SetMap(null);
            odb.Store(dictionnary);
            odb.Close();
            odb = Open("map.neodatis");
            AssertEquals(n, odb.Count(new CriteriaQuery<Dictionnary>()));
            var dic = odb.Query<Dictionnary>(query).GetFirst();
            AssertNull(dic.GetMap());
            odb.Close();
            odb = Open("map.neodatis");
            dic = odb.Query<Dictionnary>(query).GetFirst();
            dic.AddEntry("olivier", "Smadja");
            odb.Store(dic);
            odb.Close();
            odb = Open("map.neodatis");
            dic = odb.Query<Dictionnary>(query).GetFirst();
            AssertNotNull(dic.GetMap());
            AssertEquals("Smadja", dic.GetMap()["olivier"]);
            odb.Close();
        }

        [Test]
        public virtual void Test6updateChangingKeyValue()
        {
            // to monitor updates
            var odb = Open("map.neodatis");
            var n = odb.Count(new CriteriaQuery<Dictionnary>());
            IQuery query = new CriteriaQuery<Dictionnary>(Where.Equal("name", "test2"));
            var l = odb.Query<Dictionnary>(query);
            var dictionnary = l.GetFirst();
            dictionnary.GetMap()["f1"] = "changed function";
            odb.Store(dictionnary);
            odb.Close();
            odb = Open("map.neodatis");
            AssertEquals(n, odb.Count(new CriteriaQuery<Dictionnary>()));
            var dic = odb.Query<Dictionnary>(query).GetFirst();
            AssertEquals("changed function", dic.GetMap()["f1"]);
            odb.Close();
        }

        [Test]
        public virtual void Test6updateDecreasingSize()
        {
            var odb = Open("map.neodatis");
            var n = odb.Count(new CriteriaQuery<Dictionnary>());
            IQuery query = new CriteriaQuery<Dictionnary>(Where.Equal("name", "test2"));
            var l = odb.Query<Dictionnary>(query);
            var dictionnary = l.GetFirst();
            var mapSize = dictionnary.GetMap().Count;
            dictionnary.GetMap().Remove("f1");
            odb.Store(dictionnary);
            odb.Close();
            odb = Open("map.neodatis");
            AssertEquals(n, odb.Count(new CriteriaQuery<Dictionnary>()));
            var dic = odb.Query<Dictionnary>(query).GetFirst();
            AssertEquals(mapSize - 1, dic.GetMap().Count);
            odb.Close();
        }

//        [Test]
//        public virtual void TestNonGenericMap()
//        {
//            NDatabase.Odb.ODB odb = Open("map.neodatis");
//            ClassWithNonGenericMap cm = new ClassWithNonGenericMap("test1");
//            cm.Add("key1", "value1");
//            cm.Add("key2", "value2");
//            odb.Store(cm);
//            odb.Close();
//            odb = Open("map.neodatis");
//            AssertEquals(1, odb.Count(new CriteriaQuery (typeof(ClassWithNonGenericMap))));
//            ClassWithNonGenericMap cm2 = odb.GetObjects<ClassWithNonGenericMap>(new CriteriaQuery(typeof(ClassWithNonGenericMap),Where.Equal("name","test1"))).GetFirst();
//            AssertEquals("test1", cm2.GetName());
//            AssertEquals(2, cm2.Size());
//            AssertEquals("value1", cm2.Get("key1"));
//            AssertEquals("value2", cm2.Get("key2"));
//            odb.Close();
//        }
    }
}
