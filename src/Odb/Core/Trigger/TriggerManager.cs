using System;
using System.Collections.Generic;
using NDatabase2.Odb.Core.Layers.Layer2.Meta;
using NDatabase2.Odb.Core.Layers.Layer3;
using NDatabase2.Odb.Main;
using NDatabase2.Tool;
using NDatabase2.Tool.Wrappers.List;
using NDatabase2.Tool.Wrappers.Map;

namespace NDatabase2.Odb.Core.Trigger
{
    internal sealed class TriggerManager : ITriggerManager
    {
        private readonly IStorageEngine _storageEngine;

        /// <summary>
        ///   key is class Name, value is the collection of triggers for the class
        /// </summary>
        private readonly IDictionary<Type, IOdbList<Trigger>> _listOfDeleteTriggers;

        /// <summary>
        ///   key is class Name, value is the collection of triggers for the class
        /// </summary>
        private readonly IDictionary<Type, IOdbList<Trigger>> _listOfInsertTriggers;

        /// <summary>
        ///   key is class Name, value is the collection of triggers for the class
        /// </summary>
        private readonly IDictionary<Type, IOdbList<Trigger>> _listOfSelectTriggers;

        /// <summary>
        ///   key is class Name, value is the collection of triggers for the class
        /// </summary>
        private readonly IDictionary<Type, IOdbList<Trigger>> _listOfUpdateTriggers;

        public TriggerManager(IStorageEngine engine)
        {
            _storageEngine = engine;
            _listOfUpdateTriggers = new OdbHashMap<Type, IOdbList<Trigger>>();
            _listOfDeleteTriggers = new OdbHashMap<Type, IOdbList<Trigger>>();
            _listOfSelectTriggers = new OdbHashMap<Type, IOdbList<Trigger>>();
            _listOfInsertTriggers = new OdbHashMap<Type, IOdbList<Trigger>>();
        }

        #region ITriggerManager Members

        public void AddUpdateTriggerFor(Type type, UpdateTrigger trigger)
        {
            AddTriggerFor(type, trigger, _listOfUpdateTriggers);
        }

        public void AddInsertTriggerFor(Type type, InsertTrigger trigger)
        {
            AddTriggerFor(type, trigger, _listOfInsertTriggers);
        }

        public void AddDeleteTriggerFor(Type type, DeleteTrigger trigger)
        {
            AddTriggerFor(type, trigger, _listOfDeleteTriggers);
        }

        public void AddSelectTriggerFor(Type type, SelectTrigger trigger)
        {
            AddTriggerFor(type, trigger, _listOfSelectTriggers);
        }

        public bool HasDeleteTriggersFor(Type type)
        {
            return _listOfDeleteTriggers.ContainsKey(type) || _listOfDeleteTriggers.ContainsKey(typeof(object));
        }

        public bool HasInsertTriggersFor(Type type)
        {
            return _listOfInsertTriggers.ContainsKey(type) || _listOfInsertTriggers.ContainsKey(typeof(object));
        }

        public bool HasSelectTriggersFor(Type type)
        {
            return _listOfSelectTriggers.ContainsKey(type) || _listOfSelectTriggers.ContainsKey(typeof(object));
        }

        public bool HasUpdateTriggersFor(Type type)
        {
            return _listOfUpdateTriggers.ContainsKey(type) || _listOfUpdateTriggers.ContainsKey(typeof(object));
        }

        public bool ManageInsertTriggerBefore(Type type, object @object)
        {
            if (HasInsertTriggersFor(type))
            {
                foreach (InsertTrigger trigger in GetListOfInsertTriggersFor(type))
                {
                    if (trigger.Odb == null)
                        trigger.Odb = new OdbForTrigger(_storageEngine);

                    try
                    {
                        if (@object != null)
                            trigger.BeforeInsert(Transform(@object));
                    }
                    catch (Exception e)
                    {
                        var warning =
                            NDatabaseError.BeforeInsertTriggerHasThrownException.AddParameter(trigger.GetType().FullName)
                                .AddParameter(e.ToString());

                        if (OdbConfiguration.IsLoggingEnabled())
                            DLogger.Warning(warning);
                    }
                }
            }

            return true;
        }

        public void ManageInsertTriggerAfter(Type type, object @object, OID oid)
        {
            if (!HasInsertTriggersFor(type))
                return;

            foreach (InsertTrigger trigger in GetListOfInsertTriggersFor(type))
            {
                if (trigger.Odb == null)
                    trigger.Odb = new OdbForTrigger(_storageEngine);

                try
                {
                    trigger.AfterInsert(Transform(@object), oid);
                }
                catch (Exception e)
                {
                    var warning =
                        NDatabaseError.AfterInsertTriggerHasThrownException.AddParameter(trigger.GetType().FullName).
                            AddParameter(e.ToString());

                    if (OdbConfiguration.IsLoggingEnabled())
                        DLogger.Warning(warning);
                }
            }
        }

        public bool ManageUpdateTriggerBefore(Type type, NonNativeObjectInfo oldNnoi, object newObject, OID oid)
        {
            if (HasUpdateTriggersFor(type))
            {
                foreach (UpdateTrigger trigger in GetListOfUpdateTriggersFor(type))
                {
                    if (trigger.Odb == null)
                        trigger.Odb = new OdbForTrigger(_storageEngine);

                    try
                    {
                        trigger.BeforeUpdate(new ObjectRepresentation(oldNnoi), Transform(newObject), oid);
                    }
                    catch (Exception e)
                    {
                        var warning =
                            NDatabaseError.BeforeUpdateTriggerHasThrownException.AddParameter(trigger.GetType().FullName)
                                .AddParameter(e.ToString());

                        if (OdbConfiguration.IsLoggingEnabled())
                            DLogger.Warning(warning);
                    }
                }
            }
            return true;
        }

        public void ManageUpdateTriggerAfter(Type type, NonNativeObjectInfo oldNnoi, object newObject, OID oid)
        {
            if (!HasUpdateTriggersFor(type))
                return;

            foreach (UpdateTrigger trigger in GetListOfUpdateTriggersFor(type))
            {
                if (trigger.Odb == null)
                    trigger.Odb = new OdbForTrigger(_storageEngine);

                try
                {
                    trigger.AfterUpdate(new ObjectRepresentation(oldNnoi), Transform(newObject), oid);
                }
                catch (Exception e)
                {
                    var warning =
                        NDatabaseError.AfterUpdateTriggerHasThrownException.AddParameter(trigger.GetType().FullName).
                            AddParameter(e.ToString());

                    if (OdbConfiguration.IsLoggingEnabled())
                        DLogger.Warning(warning);
                }
            }
        }

        public bool ManageDeleteTriggerBefore(Type type, object @object, OID oid)
        {
            if (HasDeleteTriggersFor(type))
            {
                foreach (DeleteTrigger trigger in GetListOfDeleteTriggersFor(type))
                {
                    if (trigger.Odb == null)
                        trigger.Odb = new OdbForTrigger(_storageEngine);

                    try
                    {
                        trigger.BeforeDelete(Transform(@object), oid);
                    }
                    catch (Exception e)
                    {
                        var warning =
                            NDatabaseError.BeforeDeleteTriggerHasThrownException.AddParameter(trigger.GetType().FullName)
                                .AddParameter(e.ToString());

                        if (OdbConfiguration.IsLoggingEnabled())
                            DLogger.Warning(warning);
                    }
                }
            }
            return true;
        }

        public void ManageDeleteTriggerAfter(Type type, object @object, OID oid)
        {
            if (!HasDeleteTriggersFor(type))
                return;

            foreach (DeleteTrigger trigger in GetListOfDeleteTriggersFor(type))
            {
                if (trigger.Odb == null)
                    trigger.Odb = new OdbForTrigger(_storageEngine);

                try
                {
                    trigger.AfterDelete(Transform(@object), oid);
                }
                catch (Exception e)
                {
                    var warning =
                        NDatabaseError.AfterDeleteTriggerHasThrownException.AddParameter(trigger.GetType().FullName).
                            AddParameter(e.ToString());

                    if (OdbConfiguration.IsLoggingEnabled())
                        DLogger.Warning(warning);
                }
            }
        }

        public void ManageSelectTriggerAfter(Type type, object @object, OID oid)
        {
            if (!HasSelectTriggersFor(type))
                return;

            foreach (SelectTrigger trigger in GetListOfSelectTriggersFor(type))
            {
                if (trigger.Odb == null)
                    trigger.Odb = new OdbForTrigger(_storageEngine);

                if (@object != null)
                    trigger.AfterSelect(Transform(@object), oid);
            }
        }

        /// <summary>
        ///   For the default object trigger, no transformation is needed
        /// </summary>
        public object Transform(object @object)
        {
            return @object;
        }

        #endregion

        private static void AddTriggerFor<TTrigger>(Type type, TTrigger trigger,
                                                    IDictionary<Type, IOdbList<Trigger>> listOfTriggers)
            where TTrigger : Trigger
        {
            var triggers = listOfTriggers[type];

            if (triggers == null)
            {
                triggers = new OdbList<Trigger>();
                listOfTriggers.Add(type, triggers);
            }

            triggers.Add(trigger);
        }

        /// <summary>
        ///   FIXME try to cache l1+l2
        /// </summary>
        /// <param name="type"> </param>
        /// <returns> </returns>
        public IOdbList<Trigger> GetListOfDeleteTriggersFor(Type type)
        {
            return GetListOfTriggersFor(type, _listOfDeleteTriggers);
        }

        public IOdbList<Trigger> GetListOfInsertTriggersFor(Type type)
        {
            return GetListOfTriggersFor(type, _listOfInsertTriggers);
        }

        public IOdbList<Trigger> GetListOfSelectTriggersFor(Type type)
        {
            return GetListOfTriggersFor(type, _listOfSelectTriggers);
        }

        public IOdbList<Trigger> GetListOfUpdateTriggersFor(Type type)
        {
            return GetListOfTriggersFor(type, _listOfUpdateTriggers);
        }

        private static IOdbList<Trigger> GetListOfTriggersFor(Type type,
                                                              IDictionary<Type, IOdbList<Trigger>> listOfTriggers)
        {
            var listOfTriggersBuClassName = listOfTriggers[type];
            var listOfTriggersByAllClassTrigger = listOfTriggers[typeof(object)];

            if (listOfTriggersByAllClassTrigger != null)
            {
                var size = listOfTriggersByAllClassTrigger.Count;
                if (listOfTriggersBuClassName != null)
                    size = size + listOfTriggersBuClassName.Count;

                IOdbList<Trigger> listOfTriggersToReturn = new OdbList<Trigger>(size);

                if (listOfTriggersBuClassName != null)
                    listOfTriggersToReturn.AddAll(listOfTriggersBuClassName);

                listOfTriggersToReturn.AddAll(listOfTriggersByAllClassTrigger);
                return listOfTriggersToReturn;
            }

            return listOfTriggersBuClassName;
        }
    }
}
