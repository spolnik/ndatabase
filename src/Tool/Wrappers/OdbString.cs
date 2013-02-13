using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace NDatabase.Tool.Wrappers
{
    internal static class OdbString
    {
        private static readonly ConcurrentDictionary<string, Regex> Cache = new ConcurrentDictionary<string, Regex>();

        internal static bool Matches(string regExp, string valueToCheck)
        {
            var regex = Cache.GetOrAdd(regExp, pattern => new Regex(pattern));

            return regex.IsMatch(valueToCheck);
        }
    }
}
