namespace Test.Odb.Test.VO.Arraycollectionmap.Catalog
{
	/// <author>olivier</author>
	public class Catalog
	{
		private string name;

		private System.Collections.Generic.IList<ProductCategory
			> categories;

		public Catalog(string name)
		{
			this.name = name;
			categories = new System.Collections.Generic.List<ProductCategory
				>();
		}

		public virtual string GetName()
		{
			return name;
		}

		public virtual void SetName(string name)
		{
			this.name = name;
		}

		public virtual System.Collections.Generic.IList<ProductCategory
			> GetCategories()
		{
			return categories;
		}

		public virtual void SetCategories(System.Collections.Generic.IList<ProductCategory
			> categories)
		{
			this.categories = categories;
		}
	}
}
