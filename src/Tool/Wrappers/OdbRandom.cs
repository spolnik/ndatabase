using System;

namespace NDatabase.Tool.Wrappers
{
    public static class OdbRandom
    {
        private static readonly Random Random = new Random();

        public static int GetRandomInteger()
        {
            return Random.Next();
        }

        public static double GetRandomDouble()
        {
            return Random.NextDouble();
        }
    }
}