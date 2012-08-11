using NDatabase.Odb.Core;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Impl.Core.Layers.Layer1.Introspector;

namespace NDatabase.Odb.Impl.Core.Trigger
{
    public class DefaultObjectRepresentation : IObjectRepresentation
    {
        private readonly NonNativeObjectInfo _nnoi;

        public DefaultObjectRepresentation(NonNativeObjectInfo nnoi)
        {
            _nnoi = nnoi;
        }

        #region IObjectRepresentation Members

        public virtual object GetValueOf(string attributeName)
        {
            if (_nnoi.IsNull())
            {
                throw new OdbRuntimeException(
                    NDatabaseError.TriggerCalledOnNullObject.AddParameter(_nnoi.GetClassInfo().GetFullClassName()).
                        AddParameter(attributeName));
            }
            return _nnoi.GetValueOf(attributeName);
        }

        public virtual void SetValueOf(string attributeName, object value)
        {
            //fixme : storage engine is null?
            var introspector = OdbConfiguration.GetCoreProvider().GetLocalObjectIntrospector(null);
            var aoi = introspector.GetMetaRepresentation(value, null, true, null, new DefaultInstrumentationCallback());
            _nnoi.SetValueOf(attributeName, aoi);
        }

        public virtual OID GetOid()
        {
            return _nnoi.GetOid();
        }

        public virtual string GetObjectClassName()
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
