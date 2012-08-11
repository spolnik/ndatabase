using System;
using System.Globalization;
using NDatabase.Tool.Wrappers;

namespace NDatabase.Odb.Impl.Core.Query.Values
{
    public class ValuesUtil
    {
        public static Decimal Convert(Decimal number)
        {
            return NDatabaseNumber.CreateDecimalFromString(number.ToString(CultureInfo.InvariantCulture));
        }
    }
}
