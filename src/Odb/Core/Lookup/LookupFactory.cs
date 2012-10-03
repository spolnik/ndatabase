using System.Collections.Generic;
using NDatabase.Tool.Wrappers.Map;

namespace NDatabase.Odb.Core.Lookup
{
    internal static class LookupFactory
    {
        private static readonly IDictionary<string, ILookup> Lookups = new OdbHashMap<string, ILookup>();

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
