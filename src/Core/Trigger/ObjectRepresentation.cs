using NDatabase.Api;
using NDatabase.Core.Layers.Layer1.Introspector;
using NDatabase.Core.Layers.Layer2.Meta;
using NDatabase.Exceptions;

namespace NDatabase.Core.Trigger
{
    internal sealed class ObjectRepresentation : IObjectRepresentation
    {
        private readonly NonNativeObjectInfo _nnoi;
        private readonly IObjectIntrospectionDataProvider _classInfoProvider;

        public ObjectRepresentation(NonNativeObjectInfo nnoi, IObjectIntrospectionDataProvider classInfoProvider)
        {
            _nnoi = nnoi;
            _classInfoProvider = classInfoProvider;
        }

        #region IObjectRepresentation Members

        public object GetValueOf(string attributeName)
        {
            if (_nnoi.IsNull())
            {
                throw new OdbRuntimeException(
                    NDatabaseError.TriggerCalledOnNullObject.AddParameter(_nnoi.GetClassInfo().FullClassName).
                        AddParameter(attributeName));
            }
            return _nnoi.GetValueOf(attributeName);
        }

        public void SetValueOf(string attributeName, object value)
        {
            var introspector = (IObjectIntrospector) new ObjectIntrospector(_classInfoProvider);
            var aoi = introspector.GetMetaRepresentation(value, true, null, new DefaultInstrumentationCallback());
            _nnoi.SetValueOf(attributeName, aoi);
        }

        public OID GetOid()
        {
            return _nnoi.GetOid();
        }

        public string GetObjectClassName()
        {
            return _nnoi.GetClassInfo().FullClassName;
        }

        #endregion
    }
}
