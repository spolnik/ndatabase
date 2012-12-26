namespace NDatabase.Northwind.Domain
{
    public class Supplier
    {
        long supplierID;
        string companyName;
        string contactName;
        string contactTitle;
        string address;
        string city;
        string region;
        string postalCode;
        string country;
        string phone;
        string fax;
        string homePage;

        public long SupplierID
        {
            get { return supplierID; }
            set { supplierID = value; }
        }

        public string CompanyName
        {
            get { return companyName; }
            set { companyName = value; }
        }

        public string ContactName
        {
            get { return contactName; }
            set { contactName = value; }
        }

        public string ContactTitle
        {
            get { return contactTitle; }
            set { contactTitle = value; }
        }

        public string Address
        {
            get { return address; }
            set { address = value; }
        }

        public string City
        {
            get { return city; }
            set { city = value; }
        }

        public string Region
        {
            get { return region; }
            set { region = value; }
        }

        public string PostalCode
        {
            get { return postalCode; }
            set { postalCode = value; }
        }

        public string Country
        {
            get { return country; }
            set { country = value; }
        }

        public string Phone
        {
            get { return phone; }
            set { phone = value; }
        }

        public string Fax
        {
            get { return fax; }
            set { fax = value; }
        }

        public string HomePage
        {
            get { return homePage; }
            set { homePage = value; }
        }
    }
}
