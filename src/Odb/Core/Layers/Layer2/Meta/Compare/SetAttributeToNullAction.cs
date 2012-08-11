namespace NDatabase.Odb.Core.Layers.Layer2.Meta.Compare
{
    public class SetAttributeToNullAction
    {
        private int _attributeId;
        private NonNativeObjectInfo _nnoi;

        public SetAttributeToNullAction(NonNativeObjectInfo nnoi, int attributeId)
        {
            _nnoi = nnoi;
            _attributeId = attributeId;
        }

        public virtual int GetAttributeId()
        {
            return _attributeId;
        }

        public virtual void SetAttributeId(int attributeId)
        {
            _attributeId = attributeId;
        }

        public virtual NonNativeObjectInfo GetNnoi()
        {
            return _nnoi;
        }

        public virtual void SetNnoi(NonNativeObjectInfo nnoi)
        {
            _nnoi = nnoi;
        }

        public virtual long GetUpdatePosition()
        {
            return _nnoi.GetAttributeDefinitionPosition(_attributeId);
        }
    }
}
