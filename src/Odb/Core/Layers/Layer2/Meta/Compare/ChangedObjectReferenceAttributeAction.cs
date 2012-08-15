using System.Text;

namespace NDatabase.Odb.Core.Layers.Layer2.Meta.Compare
{
    /// <summary>
    ///   Used to store informations about object changes when the change is only a reference change
    /// </summary>
    /// <author>osmadja</author>
    public class ChangedObjectReferenceAttributeAction : IChangedAttribute
    {
        private readonly ObjectReference _objectReference;

        private readonly int _recursionLevel;
        private readonly long _updatePosition;

        public ChangedObjectReferenceAttributeAction(long position, ObjectReference oref, int recursionLevel)
        {
            _updatePosition = position;
            _objectReference = oref;
            _recursionLevel = recursionLevel;
        }

        public override string ToString()
        {
            var buffer = new StringBuilder();

            buffer.Append("update position=").Append(_updatePosition).Append(" - new obj ref=").Append(
                _objectReference.GetOid()).Append(" - level=").Append(_recursionLevel);

            return buffer.ToString();
        }

        public virtual OID GetNewId()
        {
            return _objectReference.GetOid();
        }

        public virtual int GetRecursionLevel()
        {
            return _recursionLevel;
        }

        public virtual long GetUpdatePosition()
        {
            return _updatePosition;
        }

        public virtual bool IsString()
        {
            return false;
        }
    }
}
