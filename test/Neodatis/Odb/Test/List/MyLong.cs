namespace Test.Odb.Test.List
{
	public class MyLong : System.IComparable
	{
		private long value;

		public MyLong(long value) : base()
		{
			this.value = value;
		}

		public virtual int CompareTo(object @object)
		{
			if (@object == null || !(@object is MyLong))
			{
				return -10;
			}
			MyLong ml = (MyLong)@object;
			return (int)(value - ml.value);
		}

		public virtual long LongValue()
		{
			return value;
		}
	}
}
