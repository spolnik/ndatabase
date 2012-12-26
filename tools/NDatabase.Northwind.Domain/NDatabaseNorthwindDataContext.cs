using System.Collections.Generic;
using System.Linq;
using NDatabase2.Odb;

namespace NDatabase.Northwind.Domain
{
    public class NDatabaseNorthwindDataContext
    {
        private const string DbName =
            @"C:\Users\Jacek-Laptop\Downloads\AstoriaDemo\AstoriaDemo\NorthwindDb4o\bin\Debug\northwind.ndb";


        public IEnumerable<Customer> Customers
        {
            get
            {
                IList<Customer> result;
                using (var odb = OdbFactory.Open(DbName))
                {
                    result = odb.Query<Customer>().Execute<Customer>().ToList();
                }
                return result;
            }
        }

        public IEnumerable<Employee> Employees
        {
            get
            {
                IList<Employee> result;
                using (var odb = OdbFactory.Open(DbName))
                {
                    result = odb.Query<Employee>().Execute<Employee>().ToList();
                }
                return result;
            }
        }

        public IEnumerable<Category> Categories
        {
            get
            {
                IList<Category> result;
                using (var odb = OdbFactory.Open(DbName))
                {
                    result = odb.Query<Category>().Execute<Category>().ToList();
                }
                return result;
            }
        }

        public IEnumerable<Order> Orders
        {
            get
            {
                IList<Order> result;
                using (var odb = OdbFactory.Open(DbName))
                {
                    result = odb.Query<Order>().Execute<Order>().ToList();
                }
                return result;
            }
        }

        public IEnumerable<Product> Products
        {
            get
            {
                IList<Product> result;
                using (var odb = OdbFactory.Open(DbName))
                {
                    result = odb.Query<Product>().Execute<Product>().ToList();
                }
                return result;
            }
        }

        public IEnumerable<Region> Regions
        {
            get
            {
                IList<Region> result;
                using (var odb = OdbFactory.Open(DbName))
                {
                    result = odb.Query<Region>().Execute<Region>().ToList();
                }
                return result;
            }
        }

        public IEnumerable<Shipper> Shippers
        {
            get
            {
                IList<Shipper> result;
                using (var odb = OdbFactory.Open(DbName))
                {
                    result = odb.Query<Shipper>().Execute<Shipper>().ToList();
                }
                return result;
            }
        }

        public IEnumerable<Supplier> Suppliers
        {
            get
            {
                IList<Supplier> result;
                using (var odb = OdbFactory.Open(DbName))
                {
                    result = odb.Query<Supplier>().Execute<Supplier>().ToList();
                }
                return result;
            }
        }

        public IEnumerable<Territory> Territories
        {
            get
            {
                IList<Territory> result;
                using (var odb = OdbFactory.Open(DbName))
                {
                    result = odb.Query<Territory>().Execute<Territory>().ToList();
                }
                return result;
            }
        }
    }
}