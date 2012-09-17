using NDatabase.Odb.Core.Layers.Layer1.Introspector;
using NDatabase.Odb.Core.Layers.Layer2.Meta;

namespace NDatabase.Odb.Core.Trigger
{
    public sealed class ObjectRepresentation : IObjectRepresentation
    {
        private readonly NonNativeObjectInfo _nnoi;

        public ObjectRepresentation(NonNativeObjectInfo nnoi)
        {
            _nnoi = nnoi;
        }

        #region IObjectRepresentation Members

        public object GetValueOf(string attributeName)
        {
            if (_nnoi.IsNull())
            {
                throw new OdbRuntimeException(
                    NDatabaseError.TriggerCalledOnNullObject.AddParameter(_nnoi.GetClassInfo().GetFullClassName()).
                        AddParameter(attributeName));
            }
            return _nnoi.GetValueOf(attributeName);
        }

        public void SetValueOf(string attributeName, object value)
        {
            //fixme : storage engine is null?
            var introspector = OdbConfiguration.GetCoreProvider().GetLocalObjectIntrospector(null);
            var aoi = introspector.GetMetaRepresentation(value, null, true, null, new DefaultInstrumentationCallback());
            _nnoi.SetValueOf(attributeName, aoi);
        }

        public OID GetOid()
        {
            return _nnoi.GetOid();
        }

        public string GetObjectClassName()
        {
            return _nnoi.GetClassInfo().GetFullClassName();
        }

        #endregion

        public NonNativeObjectInfo GetNnoi()
        {
            return _nnoi;
        }
    }
}
