using System;
using System.Collections.Concurrent;
using System.Text;
using System.Text.RegularExpressions;
using NDatabase.Odb;
using NDatabase.Odb.Core;

namespace NDatabase.Tool.Wrappers
{
    internal static class OdbString
    {
        private static readonly ConcurrentDictionary<string, Regex> Cache = new ConcurrentDictionary<string, Regex>();

        /// <summary>
        ///   Replace a string within a string
        /// </summary>
        /// <param name="inSourceString"> The String to modify </param>
        /// <param name="inTokenToReplace"> The Token to replace </param>
        /// <param name="inNewToken"> The new Token </param>
        /// <param name="inNbTimes"> The number of time, the replace operation must be done. -1 means replace all </param>
        /// <returns> String The new String </returns>
        internal static String ReplaceToken(String inSourceString, String inTokenToReplace, String inNewToken,
                                          int inNbTimes)
        {
            var nIndex = 0;
            var bHasToken = true;
            var sResult = new StringBuilder(inSourceString);

            var sTempString = sResult.ToString();
            var nOldTokenLength = inTokenToReplace.Length;
            var nTimes = 0;

            // To prevent from replace the token with a token containg Token to replace
            if (inNbTimes == -1 && inNewToken.IndexOf(inTokenToReplace, StringComparison.Ordinal) != -1)
            {
                const string message = "Can not replace by this new token because it contains token to be replaced";
                throw new OdbRuntimeException(NDatabaseError.InternalError.AddParameter(message));
            }

            while (bHasToken)
            {
                nIndex = sTempString.IndexOf(inTokenToReplace, nIndex, StringComparison.Ordinal);
                bHasToken = (nIndex != -1);

                if (bHasToken)
                {
                    // Control number of times
                    if (inNbTimes != -1)
                    {
                        if (nTimes < inNbTimes)
                            nTimes++;
                        else
                        {
                            // If we already replace the number of times asked then go out
                            break;
                        }
                    }

                    sResult.Replace(sResult.ToString(nIndex, nIndex + nOldTokenLength - nIndex), inNewToken, nIndex,
                                    nIndex + nOldTokenLength - nIndex);
                    sTempString = sResult.ToString();
                }

                nIndex = 0;
            }

            return sResult.ToString();
        }

        internal static bool Matches(string regExp, string valueToCheck)
        {
            var regex = Cache.GetOrAdd(regExp, pattern => new Regex(pattern));

            return regex.IsMatch(valueToCheck);
        }
    }
}
