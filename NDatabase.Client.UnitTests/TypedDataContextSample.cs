using System.Collections.Generic;
using System.Linq;
using NDatabase.Client.UnitTests.Data;
using NDatabase2.Odb;

namespace NDatabase.Client.UnitTests
{
    public class TypedDataContextSample
    {
        private const string DbName =
            @"D:\Workspace\NDatabase_git\ndatabase\NDatabase.Client.UnitTests\bin\Debug\working_with_linq.ndb";


        public IEnumerable<object> All
        {
            get
            {
                IList<object> result;
                using (var odb = OdbFactory.Open(DbName))
                {
                    result = odb.Query<object>().Execute<object>().ToList();
                }
                return result;
            }
        }

        public IEnumerable<Address> Addresses
        {
            get
            {
                IList<Address> result;
                using (var odb = OdbFactory.Open(DbName))
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
                using (var odb = OdbFactory.Open(DbName))
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
                using (var odb = OdbFactory.Open(DbName))
                {
                    result = odb.Query<City>().Execute<City>().ToList();
                }
                return result;
            }
        }
    }
}