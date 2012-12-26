namespace NDatabase.Northwind.Domain
{
    public class CustomerCustomerDemo
    {
        Customer customerID;
        CustomerDemographics customerTypeID;

        public string CustomerCustomerDemoID
        {
            get { return customerID.CustomerID.ToString() + "-" + customerTypeID.CustomerTypeID.ToString(); }
        }

        public CustomerDemographics CustomerTypeID
        {
            get { return customerTypeID; }
            set { customerTypeID = value; }
        }

        public Customer CustomerID
        {
            get { return customerID; }
            set { customerID = value; }
        }
    }
}
