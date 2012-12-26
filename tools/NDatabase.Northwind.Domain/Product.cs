namespace NDatabase.Northwind.Domain
{
    public class Product
    {
        long productID;
        string productName;
        Supplier supplierID;
        Category categoryID;
        string quantityPerUnit;
        double unitPrice;
        int unitsInStock;
        int unitsOnOrder;
        int reorderLevel;
        bool discontinued;

        public long ProductID
        {
            get { return productID; }
            set { productID = value; }
        }

        public string ProductName
        {
            get { return productName; }
            set { productName = value; }
        }

        public Supplier SupplierID
        {
            get { return supplierID; }
            set { supplierID = value; }
        }

        public Category CategoryID
        {
            get { return categoryID; }
            set { categoryID = value; }
        }

        public string QuantityPerUnit
        {
            get { return quantityPerUnit; }
            set { quantityPerUnit = value; }
        }

        public double UnitPrice
        {
            get { return unitPrice; }
            set { unitPrice = value; }
        }

        public int UnitsInStock
        {
            get { return unitsInStock; }
            set { unitsInStock = value; }
        }

        public int UnitsOnOrder
        {
            get { return unitsOnOrder; }
            set { unitsOnOrder = value; }
        }

        public int ReorderLevel
        {
            get { return reorderLevel; }
            set { reorderLevel = value; }
        }

        public bool Discontinued
        {
            get { return discontinued; }
            set { discontinued = value; }
        }

    }
}
