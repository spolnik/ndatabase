using System;

namespace NDatabase.Northwind.Domain
{
    public class Employee
    {
        long employeeID;
        string firstName;
        string lastName;
        string title;
        string titleOfCourtesy;
        DateTime birthDate;
        DateTime hireDate;
        string address;
        string city;
        string region;
        string postalCode;
        string country;
        string homePhone;
        string extension;
        string notes;
        Byte[] photo;
        string photoPath;
        Employee reportsTo;

        public long EmployeeID
        {
            get { return employeeID; }
            set { employeeID = value; }
        }

        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }

        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }

        public string TitleOfCourtesy
        {
            get { return titleOfCourtesy; }
            set { titleOfCourtesy = value; }
        }

        public DateTime BirthDate
        {
            get { return birthDate; }
            set { birthDate = value; }
        }

        public DateTime HireDate
        {
            get { return hireDate; }
            set { hireDate = value; }
        }

        public string Title
        {
            get { return title; }
            set { title = value; }
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

        public string HomePhone
        {
            get { return homePhone; }
            set { homePhone = value; }
        }

        public string Extension
        {
            get { return extension; }
            set { extension = value; }
        }

        public string Notes
        {
            get { return notes; }
            set { notes = value; }
        }

        public Byte[] Photo
        {
            get { return photo; }
            set { photo = value; }
        }

        public string PhotoPath
        {
            get { return photoPath; }
            set { photoPath = value; }
        }

        public Employee ReportsTo
        {
            get { return reportsTo; }
            set { reportsTo = value; }
        }
    }
}
