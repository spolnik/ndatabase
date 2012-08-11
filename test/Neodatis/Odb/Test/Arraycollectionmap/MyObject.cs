namespace Test.Odb.Test.Arraycollectionmap
{
	public class MyObject
	{
		private string name;

		private MyList list;

		public MyObject(string name, MyList list) : 
			base()
		{
			this.name = name;
			this.list = list;
		}

		public virtual MyList GetList()
		{
			return list;
		}

		public virtual void SetList(MyList list)
		{
			this.list = list;
		}

		public virtual string GetName()
		{
			return name;
		}

		public virtual void SetName(string name)
		{
			this.name = name;
		}
	}
}
