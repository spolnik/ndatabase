using NDatabase.Odb.Core.Transaction;
using NDatabase.Odb.Core.Trigger;
using NDatabase.Tool.Wrappers;

namespace NDatabase.Odb.Core.Layers.Layer1.Introspector
{
    internal sealed class InstrumentationCallbackForStore : IIntrospectionCallback
    {
        private readonly bool _isUpdate;
        private readonly ITriggerManager _triggerManager;

        public InstrumentationCallbackForStore(ITriggerManager triggerManager, bool isUpdate)
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
                    _triggerManager.ManageInsertTriggerBefore(OdbClassUtil.GetFullName(type), @object);
                }
            }

            return true;
        }

        #endregion
    }
}
