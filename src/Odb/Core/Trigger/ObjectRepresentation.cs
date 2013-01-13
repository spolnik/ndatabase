using NDatabase.Exceptions;
using NDatabase.Odb.Core.Layers.Layer1.Introspector;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer3;

namespace NDatabase.Odb.Core.Trigger
{
    internal sealed class ObjectRepresentation : IObjectRepresentation
    {
        private readonly NonNativeObjectInfo _nnoi;
        private readonly IStorageEngine _storageEngine;

        public ObjectRepresentation(NonNativeObjectInfo nnoi, IStorageEngine storageEngine)
        {
            _nnoi = nnoi;
            _storageEngine = storageEngine;
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
            var introspector = (IObjectIntrospector) new ObjectIntrospector(_storageEngine);
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
