namespace Test.Odb.Test.Oid
{
	public class A
	{
		private string name;

		private B b;

		public A(string name, B b) : base()
		{
			this.name = name;
			this.b = b;
		}

		public virtual string GetName()
		{
			return name;
		}

		public virtual B GetB()
		{
			return b;
		}
	}
}
