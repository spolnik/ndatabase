namespace NDatabase.UnitTests.CodeSnippets.Data
{
    public class Sport
    {
        public Sport(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}