using System;

namespace NDatabase.Northwind.Domain
{
    public class Order
    {
        long orderID;
        Customer customerID;
        Employee employeeID;
        DateTime orderDate;
        DateTime requiredDate;
        DateTime shippedDate;
        Shipper shipVia;
        double freight;
        string shipName;
        string shipAddress;
        string shipCity;
        string shipRegion;
        string shipPostalCode;
        string shipCountry;

        public long OrderID
        {
            get { return orderID; }
            set { orderID = value; }
        }

        public Customer CustomerID
        {
            get { return customerID; }
            set { customerID = value; }
        }

        public Employee EmployeeID
        {
            get { return employeeID; }
            set { employeeID = value; }
        }

        public DateTime OrderDate
        {
            get { return orderDate; }
            set { orderDate = value; }
        }

        public DateTime RequiredDate
        {
            get { return requiredDate; }
            set { requiredDate = value; }
        }

        public DateTime ShippedDate
        {
            get { return shippedDate; }
            set { shippedDate = value; }
        }

        public Shipper ShipVia
        {
            get { return shipVia; }
            set { shipVia = value; }
        }

        public double Freight
        {
            get { return freight; }
            set { freight = value; }
        }

        public string ShipName
        {
            get { return shipName; }
            set { shipName = value; }
        }

        public string ShipAddress
        {
            get { return shipAddress; }
            set { shipAddress = value; }
        }

        public string ShipCity
        {
            get { return shipCity; }
            set { shipCity = value; }
        }

        public string ShipRegion
        {
            get { return shipRegion; }
            set { shipRegion = value; }
        }

        public string ShipPostalCode
        {
            get { return shipPostalCode; }
            set { shipPostalCode = value; }
        }

        public string ShipCountry
        {
            get { return shipCountry; }
            set { shipCountry = value; }
        }
    }
}
