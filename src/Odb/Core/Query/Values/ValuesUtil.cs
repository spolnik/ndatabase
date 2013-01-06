using System;
using System.Globalization;

namespace NDatabase.Odb.Core.Query.Values
{
    internal static class ValuesUtil
    {
        internal static Decimal Convert(Decimal number)
        {
            return System.Convert.ToDecimal(number.ToString(CultureInfo.InvariantCulture));
        }
    }
}
