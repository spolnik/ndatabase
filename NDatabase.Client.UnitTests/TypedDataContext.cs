using System.Collections.Generic;
using System.Linq;
using NDatabase.Client.UnitTests.Data;
using NDatabase2.Odb;

namespace NDatabase.Client.UnitTests
{
    public class TypedDataContext
    {
        public IEnumerable<Address> Addresses
        {
            get
            {
                IList<Address> result;
                using (var odb = OdbFactory.Open("working_with_linq.ndb"))
                {
                    result = odb.Query<Address>().Execute<Address>().ToList();
                }
                return result;
            }
        }

        public IEnumerable<User> Users
        {
            get
            {
                IList<User> result;
                using (var odb = OdbFactory.Open("working_with_linq.ndb"))
                {
                    result = odb.Query<User>().Execute<User>().ToList();
                }
                return result;
            }
        }

        public IEnumerable<City> Cities
        {
            get
            {
                IList<City> result;
                using (var odb = OdbFactory.Open("working_with_linq.ndb"))
                {
                    result = odb.Query<City>().Execute<City>().ToList();
                }
                return result;
            }
        }
    }
}