using System.Collections.Generic;

namespace NDatabase.Tool.Wrappers.Map
{
    public class OdbHashMap3<TKey, TValue> : Dictionary<TKey, TValue> where TValue : class
    {
        public OdbHashMap3()
        {
        }


        public OdbHashMap3(IDictionary<TKey, TValue> dictionary)
            : base(dictionary)
        {
        }

        public OdbHashMap3(int capacity)
            : base(capacity)
        {
        }

        public virtual bool PutAll(IDictionary<TKey, TValue> map)
        {
            var keys = map.Keys;
            foreach (var key in keys)
                Add(key, map[key]);

            return true;
        }

        public virtual bool RemoveAll(IDictionary<TKey, TValue> map)
        {
            var keys = map.Keys;
            foreach (var key in keys)
                Remove(key);

            return true;
        }

        public TValue Remove2(TKey key)
        {
            TValue value;
            TryGetValue(key, out value);

            if (value != null)
                Remove(key);

            return value;
        }
    }
}