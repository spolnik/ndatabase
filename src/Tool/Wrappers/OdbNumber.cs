using System;

namespace NDatabase.Tool.Wrappers
{
    public class OdbNumber
    {
        public static Decimal NewBigInteger(long number)
        {
            return new Decimal(number);
        }
    }
}