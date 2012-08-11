using NDatabase.Odb.Core.Query.Criteria;
using NDatabase.Odb.Impl.Core.Query.Criteria;
using NUnit.Framework;
using Test.Odb.Test.Newbie.VO;

namespace Test.Odb.Test.Newbie
{
    /// <summary>
    ///   It is just a simple test to help the newbies
    /// </summary>
    /// <author>mayworm at
    ///   <xmpp://mayworm@gmail.com>
    /// </author>
    public class UpdateTest : ODBTest
    {
        protected static readonly string NewbieOdb = "newbie.neodatis";

        [Test]
        public virtual void TestUpdate()
        {
            DeleteBase(NewbieOdb);

            using (var odb = Open(NewbieOdb))
            {
                var marcelo = new Driver("marcelo");
                var car = new Car("car1", 4, "ranger", marcelo);
                odb.Store(car);

                var query = new CriteriaQuery(Where.Equal("Driver.Name", "marcelo"));
                var newCar = odb.GetObjects<Car>(query).GetFirst();

                newCar.Driver = new Driver("dani");
                odb.Store(newCar);
            }

            using (var odb = Open(NewbieOdb))
            {
                var query = new CriteriaQuery(Where.Equal("Driver.Name", "dani"));
                AssertEquals(1, odb.GetObjects<Car>(query).Count);
            }

            DeleteBase(NewbieOdb);
        }
    }
}