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
                _crossSessionCache =
                    CacheFactory.GetCrossSessionCache(engine.GetBaseIdentification().Id);
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

            if (OdbConfiguration.ReconnectObjectsToSession())
                CheckIfObjectMustBeReconnected(@object);

            return true;
        }

        #endregion

        /// <summary>
        ///   Used to check if object must be reconnected to current session <pre>An object must be reconnected to session if OdbConfiguration.reconnectObjectsToSession() is true
        ///                                                                    and object is not in local cache and is in cross session cache.</pre>
        /// </summary>
        /// <remarks>
        ///   Used to check if object must be reconnected to current session <pre>An object must be reconnected to session if OdbConfiguration.reconnectObjectsToSession() is true
        ///                                                                    and object is not in local cache and is in cross session cache. In this case
        ///                                                                    we had it to local cache</pre>
        /// </remarks>
        private void CheckIfObjectMustBeReconnected(object @object)
        {
            if (_engine == null)
            {
                // This protection is for JUnit
                return;
            }
            var session = _engine.GetSession(true);
            // If object is in local cache, no need to reconnect it
            if (session.GetCache().ExistObject(@object))
                return;

            var oidCrossSession = _crossSessionCache.GetOid(@object);
            if (oidCrossSession != null)
            {
                // reconnect object
                var objectInfoHeader = _engine.GetObjectInfoHeaderFromOid(oidCrossSession);
                session.AddObjectToCache(oidCrossSession, @object, objectInfoHeader);
            }
        }
    }
}
