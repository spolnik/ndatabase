using NDatabase.Odb;
using NDatabase.Odb.Core.Layers.Layer3.Engine;
using NDatabase.Odb.Core.Query.Criteria;
using NUnit.Framework;
using Test.NDatabase.Odb.Test.VO.Country;
using Test.NDatabase.Odb.Test.VO.Login;

namespace Test.NDatabase.Odb.Test.Cyclic
{
    [TestFixture]
    public class TestCyclicReference : ODBTest
    {
        #region Setup/Teardown

        [TearDown]
        public override void TearDown()
        {
            DeleteBase("cyclic.neodatis");
        }

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            DeleteBase("cyclic.neodatis");
            var odb = Open("cyclic.neodatis");
            for (var i = 0; i < 1; i++)
            {
                var brasilia = new City("Brasilia" + i);
                var brasil = new Country2("Brasil" + i);
                brasilia.SetCountry(brasil);
                brasil.SetCapital(brasilia);
                brasil.SetPopulation(450000);
                odb.Store(brasil);
            }
            odb.Store(new User("name", "email", new Profile("profile")));
            odb.Close();
        }

        #endregion

        [Test]
        public virtual void Test1()
        {
            var odb = Open("cyclic.neodatis");
            var l = odb.GetObjects<Country2>(true);
            var country = l.GetFirst();
            AssertEquals("Brasil0", country.GetName());
            AssertEquals("Brasilia0", country.GetCapital().GetName());
            odb.Close();
        }

        [Test]
        public virtual void Test10()
        {
            IOdb odb = null;
            try
            {
                var baseName = GetBaseName();
                DeleteBase(baseName);
                odb = Open(baseName);
                var ca = new ClassA();
                var cb = new ClassB(ca, "b");
                ca.SetClassb(cb);
                ca.SetName("a");
                odb.Store(ca);
                var ci = Dummy.GetEngine(odb).GetSession(true).GetMetaModel().GetClassInfo(typeof (ClassA), true);
                AssertTrue(ci.HasCyclicReference());
            }
            finally
            {
                if (odb != null)
                    odb.Close();
            }
        }

        [Test]
        public virtual void Test11()
        {
            IOdb odb = null;
            try
            {
                odb = Open("cyclic.neodatis");
                var ci = Dummy.GetEngine(odb).GetSession(true).GetMetaModel().GetClassInfo(typeof (User), true);
                AssertFalse(ci.HasCyclicReference());
            }
            finally
            {
                if (odb != null)
                    odb.Close();
            }
        }

        [Test]
        public virtual void Test15()
        {
            Println("-------------------");
            // LogUtil.logOn(ObjectWriter.LOG_ID, true);
            // LogUtil.logOn(ObjectReader.LOG_ID, true);
            var odb = Open("cyclic.neodatis");
            var l = odb.GetObjects<Country2>(true);
            var country = l.GetFirst();
            var city = country.GetCapital();
            city.SetName("rio de janeiro");
            country.SetCapital(city);
            odb.Store(country);
            odb.Close();
            odb = Open("cyclic.neodatis");
            l = odb.GetObjects<Country2>(true);
            country = l.GetFirst();
            AssertEquals("rio de janeiro", country.GetCapital().GetName());
            var cities = odb.GetObjects<City>(new CriteriaQuery(typeof (City), Where.Equal("name", "rio de janeiro")));
            AssertEquals(1, cities.Count);
            var cities2 = odb.GetObjects<City>(new CriteriaQuery(typeof (City)));
            AssertEquals(1, cities2.Count);
            odb.Close();
        }

        [Test]
        public virtual void Test2()
        {
            var odb = Open("cyclic.neodatis");
            var l = odb.GetObjects<Country2>(true);
            var country = l.GetFirst();
            var city = new City("rio de janeiro");
            country.SetCapital(city);
            odb.Store(country);
            odb.Close();
            odb = Open("cyclic.neodatis");
            l = odb.GetObjects<Country2>(true);
            country = l.GetFirst();
            AssertEquals("rio de janeiro", country.GetCapital().GetName());
            var cities = odb.GetObjects<City>(new CriteriaQuery(typeof (City), Where.Equal("name", "rio de janeiro")));
            AssertEquals(1, cities.Count);

            var cities2 = odb.GetObjects<City>(new CriteriaQuery(typeof (City)));
            AssertEquals(2, cities2.Count);
            odb.Close();
        }

        [Test]
        public virtual void TestUniqueInstance1()
        {
            var odb = Open("cyclic.neodatis");
            var cities = odb.GetObjects<City>(true);
            var countries = odb.GetObjects<Country2>(true);
            var country = countries.GetFirst();
            var city = cities.GetFirst();
            AssertTrue(country == city.GetCountry());
            AssertTrue(city == country.GetCities()[0]);
            AssertTrue(city == country.GetCapital());
            odb.Close();
        }

        [Test]
        public virtual void TestUniqueInstance2()
        {
            var odb = Open("cyclic.neodatis");
            var countries = odb.GetObjects<Country2>(true);
            var cities = odb.GetObjects<City>(true);
            var country = countries.GetFirst();
            var city = cities.GetFirst();
            AssertTrue(country == city.GetCountry());
            AssertTrue(city == country.GetCities()[0]);
            AssertTrue(city == country.GetCapital());
            odb.Close();
        }
    }
}
