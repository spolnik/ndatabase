using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Transaction;
using NDatabase.Odb.Core.Trigger;
using NDatabase.Tool.Wrappers;

namespace NDatabase.Odb.Core.Layers.Layer1.Introspector
{
    internal sealed class InstrumentationCallbackForStore : IIntrospectionCallback
    {
        private readonly ICrossSessionCache _crossSessionCache;
        private readonly IStorageEngine _engine;
        private readonly bool _isUpdate;
        private readonly ITriggerManager _triggerManager;

        public InstrumentationCallbackForStore(IStorageEngine engine, ITriggerManager triggerManager,
                                                      bool isUpdate)
        {
            _engine = engine;
            _triggerManager = triggerManager;
            _isUpdate = isUpdate;
            // Just for junits
            if (engine != null)
            {
                string identification = engine.GetBaseIdentification().Id;
                _crossSessionCache = CrossSessionCache.GetInstance(identification);
            }
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
