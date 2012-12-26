namespace NDatabase.Northwind.Domain
{
    public class Region
    {
        long regionID;
        string regionDescription;

        public long RegionID
        {
            get { return regionID; }
            set { regionID = value; }
        }

        public string RegionDescription
        {
            get { return regionDescription; }
            set { regionDescription = value; }
        }
    }
}
