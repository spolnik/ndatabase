using System;
using System.Collections.Generic;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Main;
using NDatabase.Tool;
using NDatabase.Tool.Wrappers.List;
using NDatabase.Tool.Wrappers.Map;

namespace NDatabase.Odb.Core.Trigger
{
    public sealed class TriggerManager : ITriggerManager
    {
        private const string AllClassTrigger = "__all_class_";

        private readonly IStorageEngine _storageEngine;

        /// <summary>
        ///   key is class Name, value is the collection of triggers for the class
        /// </summary>
        private IDictionary<string, IOdbList<Odb.Core.Trigger.Trigger>> _listOfDeleteTriggers;

        /// <summary>
        ///   key is class Name, value is the collection of triggers for the class
        /// </summary>
        private IDictionary<string, IOdbList<Odb.Core.Trigger.Trigger>> _listOfInsertTriggers;

        /// <summary>
        ///   key is class Name, value is the collection of triggers for the class
        /// </summary>
        private IDictionary<string, IOdbList<Odb.Core.Trigger.Trigger>> _listOfSelectTriggers;

        /// <summary>
        ///   key is class Name, value is the collection of triggers for the class
        /// </summary>
        private IDictionary<string, IOdbList<Odb.Core.Trigger.Trigger>> _listOfUpdateTriggers;

        public TriggerManager(IStorageEngine engine)
        {
            _storageEngine = engine;
            Init();
        }

        #region ITriggerManager Members

        public void AddUpdateTriggerFor(string className, UpdateTrigger trigger)
        {
            AddTriggerFor(className, trigger, _listOfUpdateTriggers);
        }

        public void AddInsertTriggerFor(string className, InsertTrigger trigger)
        {
            AddTriggerFor(className, trigger, _listOfInsertTriggers);
        }

        public void AddDeleteTriggerFor(string className, DeleteTrigger trigger)
        {
            AddTriggerFor(className, trigger, _listOfDeleteTriggers);
        }

        public void AddSelectTriggerFor(string className, SelectTrigger trigger)
        {
            AddTriggerFor(className, trigger, _listOfSelectTriggers);
        }

        public bool HasDeleteTriggersFor(string classsName)
        {
            return _listOfDeleteTriggers.ContainsKey(classsName) || _listOfDeleteTriggers.ContainsKey(AllClassTrigger);
        }

        public bool HasInsertTriggersFor(string className)
        {
            return _listOfInsertTriggers.ContainsKey(className) || _listOfInsertTriggers.ContainsKey(AllClassTrigger);
        }

        public bool HasSelectTriggersFor(string className)
        {
            return _listOfSelectTriggers.ContainsKey(className) || _listOfSelectTriggers.ContainsKey(AllClassTrigger);
        }

        public bool HasUpdateTriggersFor(string className)
        {
            return _listOfUpdateTriggers.ContainsKey(className) || _listOfUpdateTriggers.ContainsKey(AllClassTrigger);
        }

        public bool ManageInsertTriggerBefore(string className, object @object)
        {
            if (HasInsertTriggersFor(className))
            {
                foreach (InsertTrigger trigger in GetListOfInsertTriggersFor(className))
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

                        if (OdbConfiguration.DisplayWarnings())
                            DLogger.Info(warning);
                    }
                }
            }

            return true;
        }

        public void ManageInsertTriggerAfter(string className, object @object, OID oid)
        {
            if (!HasInsertTriggersFor(className))
                return;

            foreach (InsertTrigger trigger in GetListOfInsertTriggersFor(className))
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

                    if (OdbConfiguration.DisplayWarnings())
                        DLogger.Info(warning);
                }
            }
        }

        public bool ManageUpdateTriggerBefore(string className, NonNativeObjectInfo oldNnoi, object newObject,
                                                      OID oid)
        {
            if (HasUpdateTriggersFor(className))
            {
                foreach (UpdateTrigger trigger in GetListOfUpdateTriggersFor(className))
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

                        if (OdbConfiguration.DisplayWarnings())
                            DLogger.Info(warning);
                    }
                }
            }
            return true;
        }

        public void ManageUpdateTriggerAfter(string className, NonNativeObjectInfo oldNnoi, object newObject,
                                                     OID oid)
        {
            if (!HasUpdateTriggersFor(className))
                return;

            foreach (UpdateTrigger trigger in GetListOfUpdateTriggersFor(className))
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

                    if (OdbConfiguration.DisplayWarnings())
                        DLogger.Info(warning);
                }
            }
        }

        public bool ManageDeleteTriggerBefore(string className, object @object, OID oid)
        {
            if (HasDeleteTriggersFor(className))
            {
                foreach (DeleteTrigger trigger in GetListOfDeleteTriggersFor(className))
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

                        if (OdbConfiguration.DisplayWarnings())
                            DLogger.Info(warning);
                    }
                }
            }
            return true;
        }

        public void ManageDeleteTriggerAfter(string className, object @object, OID oid)
        {
            if (!HasDeleteTriggersFor(className))
                return;

            foreach (DeleteTrigger trigger in GetListOfDeleteTriggersFor(className))
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

                    if (OdbConfiguration.DisplayWarnings())
                        DLogger.Info(warning);
                }
            }
        }

        public void ManageSelectTriggerAfter(string className, object @object, OID oid)
        {
            if (!HasSelectTriggersFor(className))
                return;

            foreach (SelectTrigger trigger in GetListOfSelectTriggersFor(className))
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

        private static void AddTriggerFor<TTrigger>(string className, TTrigger trigger,
                                                    IDictionary<string, IOdbList<Odb.Core.Trigger.Trigger>>
                                                        listOfTriggers) where TTrigger : Odb.Core.Trigger.Trigger
        {
            if (className == null)
                className = AllClassTrigger;
            var triggers = listOfTriggers[className];

            if (triggers == null)
            {
                triggers = new OdbList<Odb.Core.Trigger.Trigger>();
                listOfTriggers.Add(className, triggers);
            }

            triggers.Add(trigger);
        }

        private void Init()
        {
            _listOfUpdateTriggers = new OdbHashMap<string, IOdbList<Odb.Core.Trigger.Trigger>>();
            _listOfDeleteTriggers = new OdbHashMap<string, IOdbList<Odb.Core.Trigger.Trigger>>();
            _listOfSelectTriggers = new OdbHashMap<string, IOdbList<Odb.Core.Trigger.Trigger>>();
            _listOfInsertTriggers = new OdbHashMap<string, IOdbList<Odb.Core.Trigger.Trigger>>();
        }

        /// <summary>
        ///   FIXME try to cache l1+l2
        /// </summary>
        /// <param name="className"> </param>
        /// <returns> </returns>
        public IOdbList<Odb.Core.Trigger.Trigger> GetListOfDeleteTriggersFor(string className)
        {
            return GetListOfTriggersFor(className, _listOfDeleteTriggers);
        }

        public IOdbList<Odb.Core.Trigger.Trigger> GetListOfInsertTriggersFor(string className)
        {
            return GetListOfTriggersFor(className, _listOfInsertTriggers);
        }

        public IOdbList<Odb.Core.Trigger.Trigger> GetListOfSelectTriggersFor(string className)
        {
            return GetListOfTriggersFor(className, _listOfSelectTriggers);
        }

        public IOdbList<Odb.Core.Trigger.Trigger> GetListOfUpdateTriggersFor(string className)
        {
            return GetListOfTriggersFor(className, _listOfUpdateTriggers);
        }

        private static IOdbList<Odb.Core.Trigger.Trigger> GetListOfTriggersFor(string className,
                                                                               IDictionary<string, IOdbList<Odb.Core.Trigger.Trigger>> listOfTriggers)
        {
            var listOfTriggersBuClassName = listOfTriggers[className];
            var listOfTriggersByAllClassTrigger = listOfTriggers[AllClassTrigger];

            if (listOfTriggersByAllClassTrigger != null)
            {
                var size = listOfTriggersByAllClassTrigger.Count;
                if (listOfTriggersBuClassName != null)
                    size = size + listOfTriggersBuClassName.Count;

                IOdbList<Odb.Core.Trigger.Trigger> listOfTriggersToReturn = new OdbList<Odb.Core.Trigger.Trigger>(size);

                if (listOfTriggersBuClassName != null)
                    listOfTriggersToReturn.AddAll(listOfTriggersBuClassName);

                listOfTriggersToReturn.AddAll(listOfTriggersByAllClassTrigger);
                return listOfTriggersToReturn;
            }

            return listOfTriggersBuClassName;
        }
    }
}
