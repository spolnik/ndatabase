using System;
using NDatabase.Tool.Wrappers;

namespace NDatabase.Core.Query
{
    /// <summary>
    ///   A simple compare key : an object that contains various values used for indexing query result <p></p>
    /// </summary>
    internal sealed class SimpleCompareKey : IOdbComparable
    {
        private readonly IComparable _key;

        public SimpleCompareKey(IComparable key)
        {
            _key = key;
        }

        public int CompareTo(object o)
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
