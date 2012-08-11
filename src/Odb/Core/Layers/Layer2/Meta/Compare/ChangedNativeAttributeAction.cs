using System.Text;

namespace NDatabase.Odb.Core.Layers.Layer2.Meta.Compare
{
    /// <summary>
    ///   Used to store informations about object changes at attribute level
    /// </summary>
    /// <author>osmadja</author>
    public class ChangedNativeAttributeAction : IChangedAttribute
    {
        private readonly string _attributeName;

        /// <summary>
        ///   The new object meta representation: is case of no in place update
        /// </summary>
        private readonly NonNativeObjectInfo _newNoi;

        private readonly NativeObjectInfo _noiWithNewValue;

        /// <summary>
        ///   The old object meta representation: is case of no in place update
        /// </summary>
        private readonly NonNativeObjectInfo _oldNnoi;

        /// <summary>
        ///   This boolean value is set to true when original object is null, is this case there is no way to do in place update
        /// </summary>
        private readonly bool _reallyCantDoInPlaceUpdate;

        private readonly int _recursionLevel;
        private readonly long _updatePosition;

        public ChangedNativeAttributeAction(NonNativeObjectInfo oldNnoi, NonNativeObjectInfo newNnoi, long position,
                                            NativeObjectInfo newNoi, int recursionLevel, bool canDoInPlaceUpdate,
                                            string attributeName)
        {
            _oldNnoi = oldNnoi;
            _newNoi = newNnoi;
            _updatePosition = position;
            _noiWithNewValue = newNoi;
            _recursionLevel = recursionLevel;
            _reallyCantDoInPlaceUpdate = canDoInPlaceUpdate;
            _attributeName = attributeName;
        }

        public override string ToString()
        {
            var buffer = new StringBuilder();
            buffer.Append("field : '").Append(_attributeName).Append("' - update position=").Append(_updatePosition).
                Append(" - new value=").Append(_noiWithNewValue).Append(" - level=").Append(_recursionLevel);

            return buffer.ToString();
        }

        public virtual NativeObjectInfo GetNoiWithNewValue()
        {
            return _noiWithNewValue;
        }

        public virtual int GetRecursionLevel()
        {
            return _recursionLevel;
        }

        public virtual long GetUpdatePosition()
        {
            return _updatePosition;
        }

        public virtual bool ReallyCantDoInPlaceUpdate()
        {
            return _reallyCantDoInPlaceUpdate;
        }

        public virtual bool InPlaceUpdateIsGuaranteed()
        {
            return !_reallyCantDoInPlaceUpdate && _noiWithNewValue.IsAtomicNativeObject() &&
                   _noiWithNewValue.GetOdbTypeId() != OdbType.StringId;
        }

        public virtual bool IsString()
        {
            return _noiWithNewValue.GetOdbTypeId() == OdbType.StringId;
        }

        public virtual NonNativeObjectInfo GetOldNnoi()
        {
            return _oldNnoi;
        }

        public virtual NonNativeObjectInfo GetNewNoi()
        {
            return _newNoi;
        }
    }
}
