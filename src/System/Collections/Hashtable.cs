using System.Collections.Generic;

namespace System.Collections
{
	public class Hashtable : Dictionary<object, object>
	{
		public Hashtable()
		{
		}

		public Hashtable(int capacity) : base(capacity)
		{
		}

		public bool Contains(object key)
		{
			return ContainsKey(key);
		}
	}
}