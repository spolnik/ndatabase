using System;
using NDatabase2.Odb.Core.Layers.Layer3;
using NDatabase2.Odb.Core.Trigger;

namespace NDatabase2.Odb
{
    internal class TriggerManager<T> : ITriggerManager where T : class
    {
        private readonly IStorageEngine _storageEngine;
        private readonly Type _underlyingType;

        public TriggerManager(IStorageEngine storageEngine)
        {
            _storageEngine = storageEngine;
            _underlyingType = typeof (T);
        }

        #region ITriggerManager Members

        public void AddUpdateTrigger(UpdateTrigger trigger)
        {
            _storageEngine.AddUpdateTriggerFor(_underlyingType, trigger);
        }

        public void AddInsertTrigger(InsertTrigger trigger)
        {
            _storageEngine.AddInsertTriggerFor(_underlyingType, trigger);
        }

        public void AddDeleteTrigger(DeleteTrigger trigger)
        {
            _storageEngine.AddDeleteTriggerFor(_underlyingType, trigger);
        }

        public void AddSelectTrigger(SelectTrigger trigger)
        {
            _storageEngine.AddSelectTriggerFor(_underlyingType, trigger);
        }

        #endregion
    }
}