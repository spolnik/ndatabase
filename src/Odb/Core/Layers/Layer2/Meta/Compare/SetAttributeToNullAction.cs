namespace NDatabase2.Odb.Core.Layers.Layer2.Meta.Compare
{
    internal sealed class SetAttributeToNullAction
    {
        private int _attributeId;
        private NonNativeObjectInfo _nnoi;

        public SetAttributeToNullAction(NonNativeObjectInfo nnoi, int attributeId)
        {
            _nnoi = nnoi;
            _attributeId = attributeId;
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
    }
}
