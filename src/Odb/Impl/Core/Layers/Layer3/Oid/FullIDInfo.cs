using System.Text;

namespace NDatabase.Odb.Impl.Core.Layers.Layer3.Oid
{
    /// <summary>
    ///   Used to obtain internal infos about all database ids
    /// </summary>
    public sealed class FullIDInfo
    {
        private readonly byte _idStatus;
        private readonly OID _nextOid;

        private readonly string _objectToString;
        private readonly long _position;

        private readonly OID _prevOid;
        private long _blockId;
        private long _id;
        private readonly string _objectClassName;

        public FullIDInfo(long id, long position, byte idStatus, long blockId, string objectClassName,
                          string objectToString, OID prevOID, OID nextOID)
        {
            _id = id;
            _position = position;
            _blockId = blockId;
            _objectClassName = objectClassName;
            _objectToString = objectToString;
            _idStatus = idStatus;
            _prevOid = prevOID;
            _nextOid = nextOID;
        }

        public long GetBlockId()
        {
            return _blockId;
        }

        public void SetBlockId(long blockId)
        {
            _blockId = blockId;
        }

        public long GetId()
        {
            return _id;
        }

        public void SetId(long id)
        {
            _id = id;
        }

        public override string ToString()
        {
            var buffer = new StringBuilder();
            buffer.Append("Id=").Append(_id).Append(" - Posi=").Append(_position).Append(" - Status=").Append(_idStatus)
                .Append(" - Block Id=").Append(_blockId);
            buffer.Append(" - Type=").Append(_objectClassName);
            buffer.Append(" - prev inst. pos=").Append(_prevOid);
            buffer.Append(" - next inst. pos=").Append(_nextOid);
            buffer.Append(" - Object=").Append(_objectToString);
            return buffer.ToString();
        }
    }
}
