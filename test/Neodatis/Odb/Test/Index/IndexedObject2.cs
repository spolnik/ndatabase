using NUnit.Framework;
namespace Index
{
	public class IndexedObject2
	{
		private string name;

		private Index.IndexedObject @object;

		public IndexedObject2() : base()
		{
		}

		public IndexedObject2(string name, Index.IndexedObject @object)
			 : base()
		{
			this.name = name;
			this.@object = @object;
		}

		public virtual string GetName()
		{
			return name;
		}

		public virtual void SetName(string name)
		{
			this.name = name;
		}

		public virtual Index.IndexedObject GetObject()
		{
			return @object;
		}

		public virtual void SetObject(Index.IndexedObject @object)
		{
			this.@object = @object;
		}
	}
}
