namespace Test.Odb.Test.Inheritance
{
	public class Class1 : IInterface
	{
		private string name;

		public Class1(string name)
		{
			this.name = name;
		}

		public virtual string GetName()
		{
			return name;
		}
	}
}
