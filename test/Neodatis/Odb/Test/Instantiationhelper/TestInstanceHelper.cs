using System.Runtime.Serialization;
using NDatabase.Odb;
using NDatabase.Odb.Core.Layers.Layer2.Instance;
using NDatabase.Odb.Core.Query.Criteria;
using NDatabase.Odb.Impl.Core.Query.Criteria;
using NUnit.Framework;
using Test.Odb.Test;

namespace Instantiationhelper
{
    /// <summary>
    ///   Test if the ODB retrieves objects without default constructor and null
    ///   arguments
    /// </summary>
    /// <author>mayworm at
    ///   <xmpp://mayworm@gmail.com>
    /// </author>
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

        internal sealed class _InstantiationHelper_76 : IInstantiationHelper
        {
            #region IInstantiationHelper Members

            public object Instantiate()
            {
                return new Car("dummyModel", 1);
            }

            #endregion
        }

        private sealed class _ParameterHelper_84 : IParameterHelper
        {
            #region IParameterHelper Members

            public object[] Parameters()
            {
                return new object[0];
            }

            #endregion
        }

        /// <exception cref="System.Exception"></exception>
        private void CheckCarRetrieval()
        {
            var cars = odb.GetObjects<Car>();
            AssertEquals(1, cars.Count);
            var car = cars.GetFirst();
            AssertEquals(car.GetModel(), "Ranger");
            AssertEquals(car.GetYear(), 2006);
            var query = new CriteriaQuery(typeof (Car), Where.Equal("model", "Ranger"));
            cars = odb.GetObjects<Car>(query);
            car = cars.GetFirst();
            AssertEquals(car.GetModel(), "Ranger");
        }

        /// <exception cref="System.Exception"></exception>
        private void CloseAndReopenDb()
        {
            odb.Close();
            odb = Open(Testdbname);
        }

        /// <summary>
        ///   Create, store and try retrieve the object without default constructor
        /// </summary>
        /// <exception cref="System.Exception">System.Exception</exception>
        [Test]
        public virtual void TestUseInstanceHelper()
        {
            OdbConfiguration.SetEnableEmptyConstructorCreation(false);
            try
            {
                var carRepresentation = odb.GetClassRepresentation(typeof (Car));
                // create a db and store a object that has not default constructor
                var car = new Car("Ranger", 2006);
                odb.Store(car);
                odb.Commit();
                CloseAndReopenDb();
                try
                {
                    CheckCarRetrieval();
                    Fail("Expected exception");
                }
                catch (OdbRuntimeException)
                {
                }
                // expected
                CloseAndReopenDb();
                carRepresentation.AddInstantiationHelper(new _InstantiationHelper_76());
                CheckCarRetrieval();
                CloseAndReopenDb();
                carRepresentation.RemoveInstantiationHelper();
                carRepresentation.AddParameterHelper(new _ParameterHelper_84());
                try
                {
                    CheckCarRetrieval();
                    Fail("Expected Exception");
                }
                catch (OdbRuntimeException)
                {
                }
                // expected
                odb.Close();
            }
            finally
            {
                OdbConfiguration.SetEnableEmptyConstructorCreation(true);
            }
        }

        /// <summary>
        ///   Create, store and try retrieve the object without default constructor
        /// </summary>
        /// <exception cref="System.Exception">System.Exception</exception>
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

    public class When_creating_object_without_invocation_of_constructor
    {
        [Test]
        public virtual void It_should_have_the_default_state()
        {
            var car = (Car)FormatterServices.GetUninitializedObject(typeof(Car));

            Assert.That(car.GetModel(), Is.Null);
            Assert.That(car.GetYear(), Is.EqualTo(0));
        }
    }
}
