namespace NDatabase.Northwind.Domain
{
    public class OrderDetail
    {
        Order orderID;
        Product productID;
        double unitPrice;
        int quantity;
        double discount;

        public string OrderDetailID
        {
            get { return orderID.OrderID.ToString()+"-"+productID.ProductID.ToString(); }
        }

        public Order OrderID
        {
            get { return orderID; }
            set { orderID = value; }
        }

        public Product ProductID
        {
            get { return productID; }
            set { productID = value; }
        }

        public double UnitPrice
        {
            get { return unitPrice; }
            set { unitPrice = value; }
        }

        public int Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }

        public double Discount
        {
            get { return discount; }
            set { discount = value; }
        }
    }
}
