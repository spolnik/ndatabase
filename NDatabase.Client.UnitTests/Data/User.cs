namespace NDatabase.Client.UnitTests.Data
{
    public class User
    {
        public User(string name, int age, Address address)
        {
            Name = name;
            Age = age;
            Address = address;
        }

        public string Name { get; private set; }
        public int Age { get; private set; }

        public Address Address { get; private set; } 
    }
}