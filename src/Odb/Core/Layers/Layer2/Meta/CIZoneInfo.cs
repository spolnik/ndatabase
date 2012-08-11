using System;

namespace NDatabase.Odb.Core.Layers.Layer2.Meta
{
    /// <summary>
    ///   Class keep track of object pointers and number of objects of a class info for a specific zone. For example, to keep track of first committed and last committed object position.
    /// </summary>
    /// <remarks>
    ///   Class keep track of object pointers and number of objects of a class info for a specific zone <pre>For example, to keep track of first committed and last committed object position.</pre>
    /// </remarks>
    /// <author>osmadja</author>
    [Serializable]
    public class CIZoneInfo
    {
        private readonly ClassInfo _ci;

        protected long NbObjects;

        public CIZoneInfo(ClassInfo ci, OID first, OID last, long nbObjects)
        {
            First = first;
            Last = last;
            NbObjects = nbObjects;
            _ci = ci;
        }

        public OID First { get; set; }
        public OID Last { get; set; }

        public override string ToString()
        {
            return string.Format("(first={0},last={1},nb={2})", First, Last, NbObjects);
        }

        public virtual void Reset()
        {
            First = null;
            Last = null;
            NbObjects = 0;
        }

        public virtual void Set(CIZoneInfo zoneInfo)
        {
            NbObjects = zoneInfo.NbObjects;
            First = zoneInfo.First;
            Last = zoneInfo.Last;
        }

        public virtual void DecreaseNbObjects()
        {
            NbObjects--;
            if (NbObjects < 0)
            {
                var errorMessage = string.Format("nb objects is negative! in {0}", _ci.GetFullClassName());

                throw new OdbRuntimeException(
                    NDatabaseError.InternalError.AddParameter(errorMessage));
            }
        }

        public virtual void IncreaseNbObjects()
        {
            NbObjects++;
        }

        public virtual long GetNbObjects()
        {
            return NbObjects;
        }

        public virtual bool HasObjects()
        {
            return NbObjects != 0;
        }

        public virtual void SetNbObjects(long nb)
        {
            NbObjects = nb;
        }
    }
}
