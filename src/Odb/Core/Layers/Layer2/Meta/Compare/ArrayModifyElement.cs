namespace NDatabase.Odb.Core.Layers.Layer2.Meta.Compare
{
    internal sealed class ArrayModifyElement
    {
        private readonly int _arrayElementIndexToChange;

        private readonly AbstractObjectInfo _newValue;

        /// <summary>
        ///   The array id
        /// </summary>
        private int _attributeId;

        private NonNativeObjectInfo _nnoi;

        public ArrayModifyElement(NonNativeObjectInfo nnoi, int attributeId, int index, AbstractObjectInfo newValue)
        {
            _nnoi = nnoi;
            _attributeId = attributeId;
            _newValue = newValue;
            _arrayElementIndexToChange = index;
        }

        public int GetAttributeId()
        {
            return _attributeId;
        }

        public void SetAttributeId(int attributeId)
        {
            _attributeId = attributeId;
        }

        public NonNativeObjectInfo GetNnoi()
        {
            return _nnoi;
        }

        public void SetNnoi(NonNativeObjectInfo nnoi)
        {
            _nnoi = nnoi;
        }

        public long GetUpdatePosition()
        {
            return _nnoi.GetAttributeDefinitionPosition(_attributeId);
        }

        public int GetArrayElementIndexToChange()
        {
            return _arrayElementIndexToChange;
        }

        public AbstractObjectInfo GetNewValue()
        {
            return _newValue;
        }

        /// <summary>
        ///   Return the position where the array position is stored
        /// </summary>
        /// <returns> </returns>
        public long GetArrayPositionDefinition()
        {
            return _nnoi.GetAttributeDefinitionPosition(_attributeId);
        }
    }
}
