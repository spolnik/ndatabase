namespace NDatabase.Northwind.Domain
{
    public class EmployeeTerritory
    {
        Employee employeeID;
        Territory territoryID;

        public string EmployeeTerritoryID
        {
            get { return employeeID.EmployeeID.ToString() + "-" + territoryID.TerritoryID.ToString(); }
        }

        public Employee EmployeeID
        {
            get { return employeeID; }
            set { employeeID = value; }
        }

        public Territory TerritoryID
        {
            get { return territoryID; }
            set { territoryID = value; }
        }
    }
}
