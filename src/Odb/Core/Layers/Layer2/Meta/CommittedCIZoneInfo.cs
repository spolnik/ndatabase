using System;

namespace NDatabase.Odb.Core.Layers.Layer2.Meta
{
    /// <summary>
    ///   Used for committed zone info.
    /// </summary>
    /// <remarks>
    ///   Used for committed zone info. It has one more attribute than the super class. It is used to keep track of committed deleted objects
    /// </remarks>
    [Serializable]
    public sealed class CommittedCIZoneInfo : CIZoneInfo
    {
        private long _nbDeletedObjects;

        public CommittedCIZoneInfo(ClassInfo ci, OID first, OID last, long nbObjects) : base(ci, first, last, nbObjects)
        {
            _nbDeletedObjects = 0;
        }

        public override void DecreaseNbObjects()
        {
            _nbDeletedObjects++;
        }

        public long GetNbDeletedObjects()
        {
            return _nbDeletedObjects;
        }

        public void SetNbDeletedObjects(long nbDeletedObjects)
        {
            _nbDeletedObjects = nbDeletedObjects;
        }

        public override long GetNbObjects()
        {
            return NbObjects - _nbDeletedObjects;
        }

        public override void SetNbObjects(long nb)
        {
            NbObjects = nb;
            _nbDeletedObjects = 0;
        }

        public void SetNbObjects(CommittedCIZoneInfo cizi)
        {
            NbObjects = cizi.NbObjects;
            _nbDeletedObjects = cizi._nbDeletedObjects;
        }

        public override string ToString()
        {
            return string.Format("(first={0},last={1},nb={2}-{3})", First, Last, NbObjects, _nbDeletedObjects);
        }
    }
}
