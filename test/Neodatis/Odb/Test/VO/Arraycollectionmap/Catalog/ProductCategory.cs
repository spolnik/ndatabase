namespace Test.Odb.Test.VO.Arraycollectionmap.Catalog
{
	/// <author>olivier</author>
	public class ProductCategory
	{
		private string name;

		private System.Collections.Generic.IList<Product
			> products;

		public ProductCategory(string name) : base()
		{
			this.name = name;
			products = new System.Collections.Generic.List<Product
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

		public virtual System.Collections.Generic.IList<Product
			> GetProducts()
		{
			return products;
		}

		public virtual void SetProducts(System.Collections.Generic.IList<Product
			> products)
		{
			this.products = products;
		}
	}
}
