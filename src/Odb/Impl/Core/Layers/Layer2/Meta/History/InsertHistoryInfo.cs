namespace NDatabase.Odb.Impl.Core.Layers.Layer2.Meta.History
{
    public class InsertHistoryInfo : IHistoryInfo
    {
        private readonly OID _next;
        private readonly OID _oid;
        private readonly long _position;

        private readonly OID _prev;
        private readonly string _type;

        public InsertHistoryInfo(string type, OID oid, long position, OID prev, OID next)
        {
            _type = type;
            _position = position;
            _oid = oid;
            _next = next;
            _prev = prev;
        }

        public virtual OID GetNext()
        {
            return _next;
        }

        public virtual long GetPosition()
        {
            return _position;
        }

        public virtual OID GetPrev()
        {
            return _prev;
        }

        public new virtual string GetType()
        {
            return _type;
        }

        public virtual OID GetOid()
        {
            return _oid;
        }

        public override string ToString()
        {
            return string.Format("{0} - oid={1} - pos={2} - prev={3} - next={4}", _type, _oid, _position, _prev, _next);
        }
    }
}
