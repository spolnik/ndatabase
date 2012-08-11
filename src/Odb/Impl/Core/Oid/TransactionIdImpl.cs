using System;
using System.Globalization;
using System.Text;

namespace NDatabase.Odb.Impl.Core.Oid
{
    [Serializable]
    public sealed class TransactionIdImpl : ITransactionId
    {
        private readonly IDatabaseId _databaseId;
        private readonly long _id1;
        private readonly long _id2;

        public TransactionIdImpl(IDatabaseId databaseId, long id1, long id2)
        {
            _databaseId = databaseId;
            _id1 = id1;
            _id2 = id2;
        }

        #region ITransactionId Members

        public long GetId1()
        {
            return _id1;
        }

        public IDatabaseId GetDatabaseId()
        {
            return _databaseId;
        }

        public long GetId2()
        {
            return _id2;
        }

        public ITransactionId Next()
        {
            return new TransactionIdImpl(_databaseId, _id1, _id2 + 1);
        }

        #endregion

        private bool Equals(TransactionIdImpl other)
        {
            return _databaseId.Equals(other._databaseId) && _id1 == other._id1 && _id2 == other._id2;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj is TransactionIdImpl && Equals((TransactionIdImpl) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (_databaseId != null
                                    ? _databaseId.GetHashCode()
                                    : 0);
                hashCode = (hashCode * 397) ^ _id1.GetHashCode();
                hashCode = (hashCode * 397) ^ _id2.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            var buffer =
                new StringBuilder("tid=").Append(_id1.ToString(CultureInfo.InvariantCulture)).Append(
                    _id2.ToString(CultureInfo.InvariantCulture));

            buffer.Append(" - dbid=").Append(_databaseId);
            return buffer.ToString();
        }
    }
}
