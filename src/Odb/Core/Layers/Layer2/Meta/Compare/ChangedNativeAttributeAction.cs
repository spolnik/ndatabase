using System.Text;

namespace NDatabase.Odb.Core.Layers.Layer2.Meta.Compare
{
    /// <summary>
    ///   Used to store informations about object changes at attribute level
    /// </summary>
    internal sealed class ChangedNativeAttributeAction : IChangedAttribute
    {
        private readonly string _attributeName;

        /// <summary>
        ///   The new object meta representation
        /// </summary>
        private readonly NonNativeObjectInfo _newNoi;

        private readonly NativeObjectInfo _noiWithNewValue;

        /// <summary>
        ///   The old object meta representation
        /// </summary>
        private readonly NonNativeObjectInfo _oldNnoi;

        private readonly int _recursionLevel;
        private readonly long _updatePosition;

        public ChangedNativeAttributeAction(NonNativeObjectInfo oldNnoi, NonNativeObjectInfo newNnoi, long position,
                                            NativeObjectInfo newNoi, int recursionLevel,
                                            string attributeName)
        {
            _oldNnoi = oldNnoi;
            _newNoi = newNnoi;
            _updatePosition = position;
            _noiWithNewValue = newNoi;
            _recursionLevel = recursionLevel;
            _attributeName = attributeName;
        }

        public override string ToString()
        {
            var buffer = new StringBuilder();
            buffer.Append("field : '").Append(_attributeName).Append("' - update position=").Append(_updatePosition).
                Append(" - new value=").Append(_noiWithNewValue).Append(" - level=").Append(_recursionLevel);

            return buffer.ToString();
        }

        public NativeObjectInfo GetNoiWithNewValue()
        {
            return _noiWithNewValue;
        }

        public int GetRecursionLevel()
        {
            return _recursionLevel;
        }

        public long GetUpdatePosition()
        {
            return _updatePosition;
        }

        public bool IsString()
        {
            return _noiWithNewValue.GetOdbTypeId() == OdbType.StringId;
        }

        public NonNativeObjectInfo GetOldNnoi()
        {
            return _oldNnoi;
        }

        public NonNativeObjectInfo GetNewNoi()
        {
            return _newNoi;
        }
    }
}
