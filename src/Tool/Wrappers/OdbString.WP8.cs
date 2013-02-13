using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NDatabase.Tool.Wrappers
{
    internal static class OdbString
    {
        private static readonly Dictionary<string, Regex> Cache = new Dictionary<string, Regex>();

        internal static bool Matches(string regExp, string valueToCheck)
        {
            var regex = Cache.GetOrAdd(regExp, pattern => new Regex(pattern));

            return regex.IsMatch(valueToCheck);
        }
    }
}
