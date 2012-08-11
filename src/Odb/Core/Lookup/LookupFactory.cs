using System.Collections.Generic;
using NDatabase.Tool.Wrappers.Map;

namespace NDatabase.Odb.Core.Lookup
{
    /// <author>olivier</author>
    public static class LookupFactory
    {
        internal static IDictionary<string, ILookup> Lookups = new OdbHashMap<string, ILookup>();

        public static ILookup Get(string key)
        {
            lock (typeof (LookupFactory))
            {
                var lookup = Lookups[key];

                if (lookup == null)
                {
                    lookup = new LookupImpl();
                    Lookups.Add(key, lookup);
                }

                return lookup;
            }
        }
    }
}
