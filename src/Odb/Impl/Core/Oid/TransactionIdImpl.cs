using System;
using System.Globalization;
using System.Text;

namespace NDatabase.Odb.Impl.Core.Oid
{
    [Serializable]
    public class TransactionIdImpl : ITransactionId
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

        public virtual long GetId1()
        {
            return _id1;
        }

        public virtual IDatabaseId GetDatabaseId()
        {
            return _databaseId;
        }

        public virtual long GetId2()
        {
            return _id2;
        }

        public virtual ITransactionId Next()
        {
            return new TransactionIdImpl(_databaseId, _id1, _id2 + 1);
        }

        public virtual ITransactionId Prev()
        {
            return new TransactionIdImpl(_databaseId, _id1, _id2 - 1);
        }

        #endregion

        public override string ToString()
        {
            var buffer =
                new StringBuilder("tid=").Append(_id1.ToString(CultureInfo.InvariantCulture)).Append(
                    _id2.ToString(CultureInfo.InvariantCulture));

            buffer.Append(" - dbid=").Append(_databaseId);
            return buffer.ToString();
        }

        public override bool Equals(object @object)
        {
            if (@object == null || @object.GetType() != typeof (TransactionIdImpl))
                return false;

            var tid = (TransactionIdImpl) @object;
            return _id1 == tid._id1 && _id2 == tid._id2 && _databaseId.Equals(tid._databaseId);
        }
    }
}
