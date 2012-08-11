namespace Test.Odb.Test.List
{
	public class MyInt : System.IComparable
	{
		private int value;

		public MyInt(int value) : base()
		{
			this.value = value;
		}

		public virtual int CompareTo(object @object)
		{
			if (@object == null || !(@object is MyInt))
			{
				return -10;
			}
			MyInt ml = (MyInt)@object;
			return (int)(value - ml.value);
		}

		public virtual int IntValue()
		{
			return value;
		}
	}
}
