using System.Text;

namespace NDatabase.Odb.Core.Layers.Layer2.Meta.Compare
{
    /// <summary>
    ///   Used to store that a new Object was created when comparing to Objects.
    /// </summary>
    /// <remarks>
    ///   Used to store that a new Object was created when comparing to Objects.
    /// </remarks>
    /// <author>osmadja</author>
    public class NewNonNativeObjectAction
    {
        private readonly string _attributeName;
        private readonly NonNativeObjectInfo _nnoi;

        private readonly int _recursionLevel;
        private readonly long _updatePosition;

        public NewNonNativeObjectAction(long position, NonNativeObjectInfo nnoi, int recursionLevel,
                                        string attributeName)
        {
            _updatePosition = position;
            _nnoi = nnoi;
            _recursionLevel = recursionLevel;
            _attributeName = attributeName;
        }

        public override string ToString()
        {
            var buffer = new StringBuilder();

            buffer.Append("field ").Append(_attributeName).Append(" - update reference position=").Append(_updatePosition)
                .Append(" - new nnoi=").Append(_nnoi).Append(" - level=").Append(_recursionLevel);

            return buffer.ToString();
        }

        public virtual NonNativeObjectInfo GetNnoi()
        {
            return _nnoi;
        }

        public virtual int GetRecursionLevel()
        {
            return _recursionLevel;
        }

        public virtual long GetUpdatePosition()
        {
            return _updatePosition;
        }
    }
}