using System.Collections.Generic;

namespace NDatabase.Tool
{
    internal class Hashtable : Dictionary<object, object>
    {
        public Hashtable()
        {
        }

        public Hashtable(int capacity)
            : base(capacity)
        {
        }

        public bool Contains(object key)
        {
            return ContainsKey(key);
        }
    }
}