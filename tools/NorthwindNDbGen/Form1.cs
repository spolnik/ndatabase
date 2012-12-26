using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using NDatabase.Northwind.Domain;
using NDatabase2.Odb;
using NorthwindNDb.NorthwindDataSetTableAdapters;

namespace NorthwindNDb
{
    public partial class Form1 : Form
    {
        private static readonly string basePath =
            (Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase)).Substring(6);

        private static readonly string NDB_FILE = basePath + "\\northwind.ndb";
        private NorthwindDataSet ds;
        private IOdb odb;

        public Form1()
        {
            InitializeComponent();
        }

        public void Init()
        {
            OdbFactory.Delete(NDB_FILE);

            try
            {
                ds = new NorthwindDataSet();
                odb = OdbFactory.Open(NDB_FILE);

                CopyCustomers();
                CopyCategories();
                CopySuppliers();
                CopyShippers();
                CopyRegions();
                CopyEmployees();
                CopyCustomerDemographics();
                CopyCustomerCustomerDemo();
                CopyTerritories();
                CopyEmployeeTerritories();
                CopyProducts();
                CopyOrders();
                CopyOrderDetails();

                SetUpIndexes(odb);
            }
            finally
            {
                Terminate();
            }
        }

        private void SetUpIndexes(IOdb odb)
        {
            //Set indexes
            odb.IndexManagerFor<Customer>().AddUniqueIndexOn("Customer_CustomerID_PK_index", "customerID");
            odb.IndexManagerFor<Category>().AddUniqueIndexOn("Category_CategoryID_PK_index", "categoryID");
            odb.IndexManagerFor<Supplier>().AddUniqueIndexOn("Supplier_SupplierID_PK_index", "supplierID");
            odb.IndexManagerFor<Region>().AddUniqueIndexOn("Region_RegionID_PK_index", "regionID");
            odb.IndexManagerFor<Employee>().AddUniqueIndexOn("Employee_EmployeeID_PK_index", "employeeID");
            odb.IndexManagerFor<Shipper>().AddUniqueIndexOn("Shipper_ShipperID_PK_index", "shipperID");
            odb.IndexManagerFor<CustomerDemographics>().AddUniqueIndexOn(
                "CustomerDemographics_CustomerTypeID_PK_index", "customerTypeID");
            odb.IndexManagerFor<CustomerCustomerDemo>().AddUniqueIndexOn(
                "CustomerCustomerDemo_CustomerID_CustomerTypeID_PK_index", "customerID", "customerTypeID");
            odb.IndexManagerFor<Territory>().AddUniqueIndexOn("Territory_TerritoryID_PK_index", "territoryID");
            odb.IndexManagerFor<Territory>().AddIndexOn("Territory_RegionID_FK_index", "regionID");
            odb.IndexManagerFor<EmployeeTerritory>().AddUniqueIndexOn(
                "EmployeeTerritory_EmployeeID_TerritoryID_PK_index", "territoryID", "employeeID");
            odb.IndexManagerFor<Product>().AddUniqueIndexOn("Product_ProductID_PK_index", "productID");
            odb.IndexManagerFor<Product>().AddIndexOn("Product_SupplierID_FK_index", "supplierID");
            odb.IndexManagerFor<Product>().AddIndexOn("Product_CategoryID_FK_index", "categoryID");
            odb.IndexManagerFor<Order>().AddUniqueIndexOn("Order_OrderID_PK_index", "orderID");
            odb.IndexManagerFor<Order>().AddIndexOn("Order_CustomerID_FK_index", "customerID");
            odb.IndexManagerFor<Order>().AddIndexOn("Order_EmployeeID_FK_index", "employeeID");
            odb.IndexManagerFor<Order>().AddIndexOn("Order_ShipperID_FK_index", "shipVia");
            odb.IndexManagerFor<OrderDetail>().AddUniqueIndexOn("OrderDetail_OrderID_ProductID_PK_index", "orderID",
                                                                "productID");
        }

        public void Terminate()
        {
            if (ds != null)
            {
                ds.Dispose();
                ds = null;
            }
            if (odb != null)
            {
                odb.Close();
                odb = null;
            }
        }

        private void LogMessage(string msg, bool linefeed)
        {
            if (linefeed)
                msg = msg + Environment.NewLine;
            textBox1.AppendText(msg);
        }

        public void CopyCustomers()
        {
            //Processing Customers
            LogMessage("Reading Customers...", false);
            var adapter1 = new CustomersTableAdapter();
            var table1 = adapter1.GetData();
            LogMessage("processing " + table1.Count.ToString() + " rows", true);
            foreach (var row in table1)
            {
                LogMessage("Customer: " + row.CustomerID + " ...", false);

                var c = new Customer();

                c.CustomerID = row.CustomerID;
                c.CompanyName = row.CompanyName;
                c.ContactName = row.IsContactNameNull() ? null : row.ContactName;
                c.ContactTitle = row.IsContactTitleNull() ? null : row.ContactTitle;
                c.Address = row.IsAddressNull() ? null : row.Address;
                c.City = row.IsCityNull() ? null : row.City;
                c.Region = row.IsRegionNull() ? null : row.Region;
                c.PostalCode = row.IsPostalCodeNull() ? null : row.PostalCode;
                c.Country = row.IsCountryNull() ? null : row.Country;
                c.Phone = row.IsPhoneNull() ? null : row.Phone;
                c.Fax = row.IsFaxNull() ? null : row.Fax;

                odb.Store(c);
                LogMessage("saved", true);
            }
            odb.Commit();
            long objectCount = NDbUtil.GetAllInstances<Customer>(odb).Count;
            if (table1.Count == objectCount)
                LogMessage(table1.Count + " objects saved", true);
            else
                LogMessage("Error: " + table1.Count + " rows retrieved but " + objectCount + " objects were saved", true);
            LogMessage("Done with Customers" + Environment.NewLine, true);
        }

        public void CopyCategories()
        {
            //Processing Categories
            LogMessage("Reading Categories...", false);
            var adapter1 = new CategoriesTableAdapter();
            var table1 = adapter1.GetData();
            LogMessage("processing " + table1.Count.ToString() + " rows", true);
            foreach (var row in table1)
            {
                LogMessage("Categories: " + row.CategoryID.ToString() + " ...", false);

                var c = new Category();

                c.CategoryID = row.CategoryID;
                c.CategoryName = row.CategoryName;
                c.Description = row.IsDescriptionNull() ? null : row.Description;
                c.Picture = row.IsPictureNull() ? null : row.Picture;

                odb.Store(c);
                LogMessage("saved", true);
            }
            odb.Commit();
            long objectCount = NDbUtil.GetAllInstances<Category>(odb).Count;
            if (table1.Count == objectCount)
                LogMessage(table1.Count + " objects saved", true);
            else
                LogMessage("Error: " + table1.Count + " rows retrieved but " + objectCount + " objects were saved", true);
            LogMessage("Done with Categories" + Environment.NewLine, true);
        }

        public void CopySuppliers()
        {
            //Processing Suppliers
            LogMessage("Reading Suppliers...", false);
            var adapter1 = new SuppliersTableAdapter();
            var table1 = adapter1.GetData();
            LogMessage("processing " + table1.Count.ToString() + " rows", true);
            foreach (var row in table1)
            {
                LogMessage("Supplier: " + row.SupplierID.ToString() + " ...", false);

                var s = new Supplier();

                s.SupplierID = row.SupplierID;
                s.CompanyName = row.CompanyName;
                s.ContactName = row.IsContactNameNull() ? null : row.ContactName;
                s.ContactTitle = row.IsContactTitleNull() ? null : row.ContactTitle;
                s.Address = row.IsAddressNull() ? null : row.Address;
                s.City = row.IsCityNull() ? null : row.City;
                s.Region = row.IsRegionNull() ? null : row.Region;
                s.PostalCode = row.IsPostalCodeNull() ? null : row.PostalCode;
                s.Country = row.IsCountryNull() ? null : row.Country;
                s.Phone = row.IsPhoneNull() ? null : row.Phone;
                s.Fax = row.IsFaxNull() ? null : row.Fax;
                s.HomePage = row.IsHomePageNull() ? null : row.HomePage;

                odb.Store(s);
                LogMessage("saved", true);
            }
            odb.Commit();
            long objectCount = NDbUtil.GetAllInstances<Supplier>(odb).Count;
            if (table1.Count == objectCount)
                LogMessage(table1.Count + " objects saved", true);
            else
                LogMessage("Error: " + table1.Count + " rows retrieved but " + objectCount + " objects were saved", true);
            LogMessage("Done with Suppliers" + Environment.NewLine, true);
        }

        public void CopyShippers()
        {
            //Processing Shippers
            LogMessage("Reading Shippers...", false);
            var adapter1 = new ShippersTableAdapter();
            var table1 = adapter1.GetData();
            LogMessage("processing " + table1.Count.ToString() + " rows", true);
            foreach (var row in table1)
            {
                LogMessage("Shippers: " + row.ShipperID.ToString() + " ...", false);

                var s = new Shipper();

                s.ShipperID = row.ShipperID;
                s.CompanyName = row.CompanyName;
                s.Phone = row.IsPhoneNull() ? null : row.Phone;

                odb.Store(s);
                LogMessage("saved", true);
            }
            odb.Commit();
            long objectCount = NDbUtil.GetAllInstances<Shipper>(odb).Count;
            if (table1.Count == objectCount)
                LogMessage(table1.Count + " objects saved", true);
            else
                LogMessage("Error: " + table1.Count + " rows retrieved but " + objectCount + " objects were saved", true);
            LogMessage("Done with Shippers" + Environment.NewLine, true);
        }

        public void CopyRegions()
        {
            //Processing Regions
            LogMessage("Reading Regions...", false);
            var adapter1 = new RegionTableAdapter();
            var table1 = adapter1.GetData();
            LogMessage("processing " + table1.Count.ToString() + " rows", true);
            foreach (var row in table1)
            {
                LogMessage("Regions: " + row.RegionID.ToString() + " ...", false);

                var r = new Region();

                r.RegionID = row.RegionID;
                r.RegionDescription = row.RegionDescription;

                odb.Store(r);
                LogMessage("saved", true);
            }
            odb.Commit();
            long objectCount = NDbUtil.GetAllInstances<Region>(odb).Count;
            if (table1.Count == objectCount)
                LogMessage(table1.Count + " objects saved", true);
            else
                LogMessage("Error: " + table1.Count + " rows retrieved but " + objectCount + " objects were saved", true);
            LogMessage("Done with Regions" + Environment.NewLine, true);
        }

        public void CopyCustomerDemographics()
        {
            //Processing CustomerDemographics
            LogMessage("Reading CustomerDemographics...", false);
            var adapter1 = new CustomerDemographicsTableAdapter();
            var table1 = adapter1.GetData();
            LogMessage("processing " + table1.Count.ToString() + " rows", true);
            foreach (var row in table1)
            {
                LogMessage("CustomerDemographics: " + row.CustomerTypeID + " ...", false);

                var cd = new CustomerDemographics();

                cd.CustomerTypeID = row.CustomerTypeID;
                cd.CustomerDesc = row.IsCustomerDescNull() ? null : row.CustomerDesc;

                odb.Store(cd);
                LogMessage("saved", true);
            }
            odb.Commit();
            long objectCount = NDbUtil.GetAllInstances<CustomerDemographics>(odb).Count;
            if (table1.Count == objectCount)
                LogMessage(table1.Count + " objects saved", true);
            else
                LogMessage("Error: " + table1.Count + " rows retrieved but " + objectCount + " objects were saved", true);
            LogMessage("Done with CustomerDemographics" + Environment.NewLine, true);
        }

        public void CopyEmployees()
        {
            var employees = new List<Employee>();
            var reportingEmployees = new Hashtable();
            //Processing Employees
            LogMessage("Reading Employees...", false);
            var adapter1 = new EmployeesTableAdapter();
            var table1 = adapter1.GetData();
            LogMessage("processing " + table1.Count.ToString() + " rows", true);
            foreach (var row in table1)
            {
                var e = new Employee();

                e.EmployeeID = row.EmployeeID;
                e.FirstName = row.FirstName;
                e.LastName = row.LastName;
                e.Title = row.IsTitleNull() ? null : row.Title;
                e.TitleOfCourtesy = row.IsTitleOfCourtesyNull() ? null : row.TitleOfCourtesy;
                if (!row.IsBirthDateNull())
                    e.BirthDate = row.BirthDate;
                if (!row.IsHireDateNull())
                    e.HireDate = row.HireDate;
                e.Address = row.IsAddressNull() ? null : row.Address;
                e.City = row.IsCityNull() ? null : row.City;
                e.Region = row.IsRegionNull() ? null : row.Region;
                e.PostalCode = row.IsPostalCodeNull() ? null : row.PostalCode;
                e.Country = row.IsCountryNull() ? null : row.Country;
                e.HomePhone = row.IsHomePhoneNull() ? null : row.HomePhone;
                e.Extension = row.IsExtensionNull() ? null : row.Extension;
                e.Notes = row.IsNotesNull() ? null : row.Notes;
                e.Photo = row.IsPhotoNull() ? null : row.Photo;
                e.PhotoPath = row.IsPhotoPathNull() ? null : row.PhotoPath;
                if (!row.IsReportsToNull())
                    reportingEmployees.Add(e.EmployeeID, row.ReportsTo);

                employees.Add(e);
            }
            foreach (var e in employees)
            {
                LogMessage("Employee: " + e.EmployeeID.ToString() + " ...", false);
                if (reportingEmployees.ContainsKey(e.EmployeeID))
                {
                    LogMessage("linking member...", false);
                    var reportsToID = Convert.ToInt64(reportingEmployees[e.EmployeeID]);
                    var found = employees.Find(delegate(Employee p) { return p.EmployeeID == reportsToID; });
                    e.ReportsTo = found;
                }
                odb.Store(e);
                LogMessage("saved (" + e.EmployeeID.ToString() + ")", true);
            }
            odb.Commit();
            long objectCount = NDbUtil.GetAllInstances<Employee>(odb).Count;
            if (table1.Count == objectCount)
                LogMessage(table1.Count + " objects saved", true);
            else
                LogMessage("Error: " + table1.Count + " rows retrieved but " + objectCount + " objects were saved", true);
            LogMessage("Done with Employees" + Environment.NewLine, true);
        }

        public void CopyCustomerCustomerDemo()
        {
            //Processing CustomerCustomerDemo
            LogMessage("Reading CustomerCustomerDemo...", false);
            var adapter1 = new CustomerCustomerDemoTableAdapter();
            var table1 = adapter1.GetData();
            LogMessage("processing " + table1.Count.ToString() + " rows", true);
            foreach (var row in table1)
            {
                LogMessage("CustomerCustomerDemo: " + row.CustomerID + "/" + row.CustomerTypeID + " ...", false);

                var ccd = new CustomerCustomerDemo();
                LogMessage("linking members...", false);
                ccd.CustomerID = NDbUtil.GetByStringID<Customer>(odb, "customerID", row.CustomerID);
                ccd.CustomerTypeID = NDbUtil.GetByStringID<CustomerDemographics>(odb, "customerTypeID",
                                                                                 row.CustomerTypeID);

                odb.Store(ccd);
                LogMessage("saved (" + ccd.CustomerID.CustomerID + "/" + ccd.CustomerTypeID.CustomerTypeID + ")", true);
            }
            odb.Commit();
            long objectCount = NDbUtil.GetAllInstances<CustomerCustomerDemo>(odb).Count;
            if (table1.Count == objectCount)
                LogMessage(table1.Count + " objects saved", true);
            else
                LogMessage("Error: " + table1.Count + " rows retrieved but " + objectCount + " objects were saved", true);
            LogMessage("Done with CustomerCustomerDemo" + Environment.NewLine, true);
        }

        public void CopyTerritories()
        {
            //Processing Territories
            LogMessage("Reading Territories...", false);
            var adapter1 = new TerritoriesTableAdapter();
            var table1 = adapter1.GetData();
            LogMessage("processing " + table1.Count.ToString() + " rows", true);
            foreach (var row in table1)
            {
                LogMessage("Territories: " + row.TerritoryID + " ...", false);

                var t = new Territory();

                t.TerritoryID = row.TerritoryID;
                t.TerritoryDescription = row.TerritoryDescription;
                LogMessage("linking member...", false);
                t.RegionID = NDbUtil.GetByNumericalID<Region>(odb, "regionID", row.RegionID);

                odb.Store(t);
                LogMessage("saved (" + t.TerritoryID + ")", true);
            }
            odb.Commit();
            long objectCount = NDbUtil.GetAllInstances<Territory>(odb).Count;
            if (table1.Count == objectCount)
                LogMessage(table1.Count + " objects saved", true);
            else
                LogMessage("Error: " + table1.Count + " rows retrieved but " + objectCount + " objects were saved", true);
            LogMessage("Done with Territories" + Environment.NewLine, true);
        }

        public void CopyEmployeeTerritories()
        {
            //Processing EmployeeTerritories
            LogMessage("Reading EmployeeTerritories...", false);
            var adapter1 = new EmployeeTerritoriesTableAdapter();
            var table1 = adapter1.GetData();
            LogMessage("processing " + table1.Count.ToString() + " rows", true);
            foreach (var row in table1)
            {
                LogMessage("EmployeeTerritories: " + row.EmployeeID.ToString() + "/" + row.TerritoryID + " ...", false);

                var et = new EmployeeTerritory();
                LogMessage("linking members...", false);
                et.EmployeeID = NDbUtil.GetByNumericalID<Employee>(odb, "employeeID", row.EmployeeID);
                et.TerritoryID = NDbUtil.GetByStringID<Territory>(odb, "territoryID", row.TerritoryID);

                odb.Store(et);
                LogMessage("saved (" + et.EmployeeID.EmployeeID.ToString() + "/" + et.TerritoryID.TerritoryID + ")",
                           true);
            }
            odb.Commit();
            long objectCount = NDbUtil.GetAllInstances<EmployeeTerritory>(odb).Count;
            if (table1.Count == objectCount)
                LogMessage(table1.Count + " objects saved", true);
            else
                LogMessage("Error: " + table1.Count + " rows retrieved but " + objectCount + " objects were saved", true);
            LogMessage("Done with EmployeeTerritories" + Environment.NewLine, true);
        }

        public void CopyProducts()
        {
            var products = new List<Product>();
            var suppliers = new Hashtable();
            var categories = new Hashtable();
            //Processing Products
            LogMessage("Reading Products...", false);
            var adapter1 = new ProductsTableAdapter();
            var table1 = adapter1.GetData();
            LogMessage("processing " + table1.Count.ToString() + " rows", true);
            foreach (var row in table1)
            {
                var p = new Product();

                p.ProductID = row.ProductID;
                p.ProductName = row.ProductName;
                if (!row.IsSupplierIDNull())
                    suppliers.Add(p.ProductID, row.SupplierID);
                if (!row.IsCategoryIDNull())
                    categories.Add(p.ProductID, row.CategoryID);
                p.QuantityPerUnit = row.IsQuantityPerUnitNull() ? null : row.QuantityPerUnit;
                p.UnitPrice = row.IsUnitPriceNull() ? 0 : Convert.ToDouble(row.UnitPrice);
                p.UnitsInStock = row.IsUnitsInStockNull() ? 0 : row.UnitsInStock;
                p.UnitsOnOrder = row.IsUnitsOnOrderNull() ? 0 : row.UnitsOnOrder;
                p.ReorderLevel = row.IsReorderLevelNull() ? 0 : row.ReorderLevel;
                p.Discontinued = row.Discontinued;

                products.Add(p);
            }
            foreach (var p in products)
            {
                LogMessage("Product: " + p.ProductID.ToString() + " ...", false);
                if (suppliers.ContainsKey(p.ProductID))
                {
                    LogMessage("linking member...", false);
                    var supplierID = Convert.ToInt64(suppliers[p.ProductID]);
                    var found = NDbUtil.GetByNumericalID<Supplier>(odb, "supplierID", supplierID);
                    p.SupplierID = found;
                }
                if (categories.ContainsKey(p.ProductID))
                {
                    LogMessage("linking member...", false);
                    var categoryID = Convert.ToInt64(categories[p.ProductID]);
                    var found = NDbUtil.GetByNumericalID<Category>(odb, "categoryID", categoryID);
                    p.CategoryID = found;
                }
                odb.Store(p);
                LogMessage("saved (" + p.ProductID.ToString() + ")", true);
            }
            odb.Commit();
            long objectCount = NDbUtil.GetAllInstances<Product>(odb).Count;
            if (table1.Count == objectCount)
                LogMessage(table1.Count + " objects saved", true);
            else
                LogMessage("Error: " + table1.Count + " rows retrieved but " + objectCount + " objects were saved", true);
            LogMessage("Done with Products" + Environment.NewLine, true);
        }

        public void CopyOrders()
        {
            var orders = new List<Order>();
            var customers = new Hashtable();
            var employees = new Hashtable();
            var shippers = new Hashtable();
            //Processing Orders
            LogMessage("Reading Orders...", false);
            var adapter1 = new OrdersTableAdapter();
            var table1 = adapter1.GetData();
            LogMessage("processing " + table1.Count.ToString() + " rows", true);
            foreach (var row in table1)
            {
                var o = new Order();

                o.OrderID = row.OrderID;

                if (!row.IsCustomerIDNull())
                    customers.Add(o.OrderID, row.CustomerID);
                if (!row.IsEmployeeIDNull())
                    employees.Add(o.OrderID, row.EmployeeID);
                if (!row.IsShipViaNull())
                    shippers.Add(o.OrderID, row.ShipVia);
                if (!row.IsOrderDateNull())
                    o.OrderDate = row.OrderDate;
                if (!row.IsRequiredDateNull())
                    o.RequiredDate = row.RequiredDate;
                if (!row.IsShippedDateNull())
                    o.ShippedDate = row.ShippedDate;
                o.Freight = row.IsFreightNull() ? 0 : Convert.ToDouble(row.Freight);
                o.ShipName = row.IsShipNameNull() ? null : row.ShipName;
                o.ShipAddress = row.IsShipAddressNull() ? null : row.ShipAddress;
                o.ShipCity = row.IsShipCityNull() ? null : row.ShipCity;
                o.ShipRegion = row.IsShipRegionNull() ? null : row.ShipRegion;
                o.ShipPostalCode = row.IsShipPostalCodeNull() ? null : row.ShipPostalCode;
                o.ShipCountry = row.IsShipCountryNull() ? null : row.ShipCountry;

                orders.Add(o);
            }
            foreach (var o in orders)
            {
                LogMessage("Order: " + o.OrderID.ToString() + " ...", false);
                if (customers.ContainsKey(o.OrderID))
                {
                    LogMessage("linking member...", false);
                    var customerID = Convert.ToString(customers[o.OrderID]);
                    var found = NDbUtil.GetByStringID<Customer>(odb, "customerID", customerID);
                    o.CustomerID = found;
                }
                if (employees.ContainsKey(o.OrderID))
                {
                    LogMessage("linking member...", false);
                    var employeeID = Convert.ToInt64(employees[o.OrderID]);
                    var found = NDbUtil.GetByNumericalID<Employee>(odb, "employeeID", employeeID);
                    o.EmployeeID = found;
                }
                if (shippers.ContainsKey(o.OrderID))
                {
                    LogMessage("linking member...", false);
                    var shipperID = Convert.ToInt64(shippers[o.OrderID]);
                    var found = NDbUtil.GetByNumericalID<Shipper>(odb, "shipperID", shipperID);
                    o.ShipVia = found;
                }
                odb.Store(o);
                LogMessage("saved (" + o.OrderID.ToString() + ")", true);
            }
            odb.Commit();
            long objectCount = NDbUtil.GetAllInstances<Order>(odb).Count;
            if (table1.Count == objectCount)
                LogMessage(table1.Count + " objects saved", true);
            else
                LogMessage("Error: " + table1.Count + " rows retrieved but " + objectCount + " objects were saved", true);
            LogMessage("Done with Orders" + Environment.NewLine, true);
        }

        public void CopyOrderDetails()
        {
            //Processing OrderDetails
            LogMessage("Reading OrderDetails...", false);
            var adapter1 = new Order_DetailsTableAdapter();
            var table1 = adapter1.GetData();
            LogMessage("processing " + table1.Count.ToString() + " rows", true);
            foreach (var row in table1)
            {
                LogMessage("OrderDetails: " + row.OrderID.ToString() + "/" + row.ProductID.ToString() + " ...", false);

                var od = new OrderDetail();
                LogMessage("linking members...", false);
                od.OrderID = NDbUtil.GetByNumericalID<Order>(odb, "orderID", row.OrderID);
                od.ProductID = NDbUtil.GetByNumericalID<Product>(odb, "productID", row.ProductID);
                od.UnitPrice = Convert.ToDouble(row.UnitPrice);
                od.Quantity = row.Quantity;
                od.Discount = Convert.ToDouble(row.Discount);

                odb.Store(od);
                LogMessage("saved (" + od.OrderID.OrderID.ToString() + "/" + od.ProductID.ProductID.ToString() + ")",
                           true);
            }
            odb.Commit();
            long objectCount = NDbUtil.GetAllInstances<OrderDetail>(odb).Count;
            if (table1.Count == objectCount)
                LogMessage(table1.Count + " objects saved", true);
            else
                LogMessage("Error: " + table1.Count + " rows retrieved but " + objectCount + " objects were saved", true);
            LogMessage("Done with OrderDetails" + Environment.NewLine, true);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Terminate();
        }

        private void StartClick(object sender, EventArgs e)
        {
            Init();
        }
    }
}