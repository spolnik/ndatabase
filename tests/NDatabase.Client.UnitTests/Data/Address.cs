namespace NDatabase.Client.UnitTests.Data
{
    public class Address
    {
        public Address(string street, City city, int number)
        {
            Street = street;
            City = city;
            Number = number;
        }

        public string Street { get; private set; }
        public City City { get; private set; }
        public int Number { get; private set; }
    }
}