using System;
using System.Collections.Generic;

namespace NDatabase.Tool
{
    internal static class DictionaryExtensions
    {
<<<<<<< HEAD
        public static TItem GetOrAdd<TKey, TItem>(this Dictionary<TKey, TItem> self, TKey key, Func<TKey, TItem> produce)
=======
        internal static TItem GetOrAdd<TKey, TItem>(this Dictionary<TKey, TItem> self, TKey key, Func<TKey, TItem> produce)
>>>>>>> master
        {
            TItem value;
            var success = self.TryGetValue(key, out value);
            if (success)
                return value;

            value = produce(key);
            self.Add(key, value);

            return value;
        }

<<<<<<< HEAD
        public static TItem GetOrAdd<TKey, TItem>(this Dictionary<TKey, TItem> self, TKey key, TItem item)
=======
        internal static TItem GetOrAdd<TKey, TItem>(this Dictionary<TKey, TItem> self, TKey key, TItem item)
>>>>>>> master
        {
            TItem value;
            var success = self.TryGetValue(key, out value);
            if (success)
                return value;

            self.Add(key, item);

            return item;
        }
    }
}