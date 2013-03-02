using System;
using System.Collections.Generic;

namespace NDatabase.Tool
{
<<<<<<< HEAD
    public static class DictionaryExtensions
    {
         public static TItem GetOrAdd<TKey, TItem>(this Dictionary<TKey, TItem> self, TKey key, Func<TKey, TItem> produce)
         {
             TItem value;
             var success = self.TryGetValue(key, out value);
             if (success)
                 return value;

             value = produce(key);
             self.Add(key, value);

             return value;
         }

         public static TItem GetOrAdd<TKey, TItem>(this Dictionary<TKey, TItem> self, TKey key, TItem item)
         {
             TItem value;
             var success = self.TryGetValue(key, out value);
             if (success)
                 return value;

             self.Add(key, item);

             return item;
         }
=======
    internal static class DictionaryExtensions
    {
        internal static TItem GetOrAdd<TKey, TItem>(this Dictionary<TKey, TItem> self, TKey key, Func<TKey, TItem> produce)
        {
            TItem value;
            var success = self.TryGetValue(key, out value);
            if (success)
                return value;

            value = produce(key);
            self.Add(key, value);

            return value;
        }

        internal static TItem GetOrAdd<TKey, TItem>(this Dictionary<TKey, TItem> self, TKey key, TItem item)
        {
            TItem value;
            var success = self.TryGetValue(key, out value);
            if (success)
                return value;

            self.Add(key, item);

            return item;
        }
>>>>>>> master
    }
}