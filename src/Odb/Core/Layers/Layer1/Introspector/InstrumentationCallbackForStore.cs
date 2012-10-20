using NDatabase2.Odb.Core.Trigger;

namespace NDatabase2.Odb.Core.Layers.Layer1.Introspector
{
    internal sealed class InstrumentationCallbackForStore : IIntrospectionCallback
    {
        private readonly bool _isUpdate;
        private readonly IInternalTriggerManager _triggerManager;

        public InstrumentationCallbackForStore(IInternalTriggerManager triggerManager, bool isUpdate)
        {
            _triggerManager = triggerManager;
            _isUpdate = isUpdate;
        }

        #region IIntrospectionCallback Members

        public bool ObjectFound(object @object)
        {
            if (!_isUpdate)
            {
                if (_triggerManager != null)
                {
                    var type = @object.GetType();
                    _triggerManager.ManageInsertTriggerBefore(type, @object);
                }
            }

            return true;
        }

        #endregion
    }
}
