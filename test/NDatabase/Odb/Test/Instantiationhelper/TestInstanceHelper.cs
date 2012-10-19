using NDatabase2.Odb;
using NDatabase2.Odb.Core.Query.Criteria;
using NUnit.Framework;

namespace Test.NDatabase.Odb.Test.Instantiationhelper
{
    /// <summary>
    ///   Test if the ODB retrieves objects without default constructor and null
    ///   arguments
    /// </summary>
    [TestFixture]
    public class TestInstanceHelper : ODBTest
    {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            DeleteBase(Testdbname);
            odb = Open(Testdbname);
        }

        [TearDown]
        public override void TearDown()
        {
            DeleteBase(Testdbname);
        }

        #endregion

        private IOdb odb;

        private static readonly string Testdbname = "instanceHelper.neodatis";

        private void CheckCarRetrieval()
        {
            var cars = odb.Query<Car>();
            AssertEquals(1, cars.Count);
            var car = cars.GetFirst();
            AssertEquals(car.GetModel(), "Ranger");
            AssertEquals(car.GetYear(), 2006);
            var query = new CriteriaQuery<Car>( Where.Equal("model", "Ranger"));
            cars = odb.Query<Car>(query);
            car = cars.GetFirst();
            AssertEquals(car.GetModel(), "Ranger");
        }

        private void CloseAndReopenDb()
        {
            odb.Close();
            odb = Open(Testdbname);
        }

        /// <summary>
        ///   Create, store and try retrieve the object without default constructor
        /// </summary>
        [Test]
        public virtual void TestWithoutHelperUsingNoConstructor()
        {
            // create a db and store a object that has not default constructor
            var car = new Car("Ranger", 2006);
            odb.Store(car);
            CloseAndReopenDb();
            CheckCarRetrieval();
            odb.Close();
        }
    }
}
