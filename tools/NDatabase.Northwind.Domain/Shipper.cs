namespace NDatabase.Northwind.Domain
{
    public class Shipper
    {
        long shipperID;
        string companyName;
        string phone;

        public long ShipperID
        {
            get { return shipperID; }
            set { shipperID = value; }
        }

        public string CompanyName
        {
            get { return companyName; }
            set { companyName = value; }
        }

        public string Phone
        {
            get { return phone; }
            set { phone = value; }
        }
    }
}
