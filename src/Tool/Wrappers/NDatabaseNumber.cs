using System;

namespace NDatabase.Tool.Wrappers
{
    /**To Wrap some basic number functions
   * @author olivier
   *warning the round type is not used
   */

    public class NDatabaseNumber
    {
        public static Decimal NewBigInteger(long l)
        {
            return new Decimal(l);
        }

        public static Decimal Add(Decimal d1, Decimal d2)
        {
            return Decimal.Add(d1, d2);
        }

        public static Decimal Divide(Decimal d1, Decimal d2, int roundType, int scale)
        {
            var result = Decimal.Divide(d1, d2);
            result = Decimal.Round(result, scale, MidpointRounding.ToEven);
            return result;
        }

        public static Decimal CreateDecimalFromString(string s)
        {
            return Convert.ToDecimal(s);
        }
    }
}
