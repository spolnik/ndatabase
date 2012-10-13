using System;

namespace NDatabase2.Odb.Core.Query
{
    /// <summary>
    ///   A simple compare key : an object that contains various values used for indexing query result <p></p>
    /// </summary>
    public sealed class SimpleCompareKey : CompareKey
    {
        private readonly IComparable _key;

        public SimpleCompareKey(IComparable key)
        {
            _key = key;
        }

        public override int CompareTo(object o)
        {
            if (o == null || o.GetType() != typeof (SimpleCompareKey))
                return -1;

            var ckey = (SimpleCompareKey) o;
            return _key.CompareTo(ckey._key);
        }

        public override string ToString()
        {
            return _key.ToString();
        }

        public override bool Equals(object o)
        {
            if (o == null || !(o is SimpleCompareKey))
                return false;

            var c = (SimpleCompareKey) o;
            return _key.Equals(c._key);
        }

        public override int GetHashCode()
        {
            return _key.GetHashCode();
        }
    }
}
