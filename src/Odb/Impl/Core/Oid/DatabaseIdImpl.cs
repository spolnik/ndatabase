using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace NDatabase.Odb.Impl.Core.Oid
{
    
    public sealed class DatabaseIdImpl : IDatabaseId
    {
        private readonly long[] _ids;

        public DatabaseIdImpl(long[] ids)
        {
            _ids = ids;
        }

        #region IDatabaseId Members

        public long[] GetIds()
        {
            return _ids;
        }

        #endregion

        public override string ToString()
        {
            var buffer = new StringBuilder();

            for (var i = 0; i < _ids.Length; i++)
            {
                if (i != 0)
                    buffer.Append("-");

                buffer.Append(_ids[i].ToString(CultureInfo.InvariantCulture));
            }

            return buffer.ToString();
        }

        public static IDatabaseId FromString(string sid)
        {
            var tokens = sid.Split('-');

            var ids = new long[tokens.Length];

            for (var i = 0; i < ids.Length; i++)
                ids[i] = long.Parse(tokens[i]);

            return new DatabaseIdImpl(ids);
        }

        public override bool Equals(object @object)
        {
            if (@object == null || @object.GetType() != typeof (DatabaseIdImpl))
                return false;

            var dbId = (DatabaseIdImpl) @object;

            for (var i = 0; i < _ids.Length; i++)
            {
                if (_ids[i] != dbId._ids[i])
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            var sum = _ids.Sum(val => (val ^ (UrShift(val, 32))));

            return (int)(sum ^ (UrShift(sum, 32)));
        }

        private static long UrShift(long number, int bits)
        {
            if (number >= 0)
                return number >> bits;

            return (number >> bits) + (2L << ~bits);
        }
    }
}
