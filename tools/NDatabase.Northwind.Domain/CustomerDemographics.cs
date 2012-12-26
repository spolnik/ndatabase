namespace NDatabase.Northwind.Domain
{
    public class CustomerDemographics
    {
        string customerTypeID;
        string customerDesc;

        public string CustomerTypeID
        {
            get { return customerTypeID; }
            set { customerTypeID = value; }
        }

        public string CustomerDesc
        {
            get { return customerDesc; }
            set { customerDesc = value; }
        }
    }
}
