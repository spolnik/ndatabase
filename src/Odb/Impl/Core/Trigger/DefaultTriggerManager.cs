using System;
using System.Collections.Generic;
using NDatabase.Odb.Core;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Trigger;
using NDatabase.Odb.Impl.Main;
using NDatabase.Tool;
using NDatabase.Tool.Wrappers.List;
using NDatabase.Tool.Wrappers.Map;

namespace NDatabase.Odb.Impl.Core.Trigger
{
    public class DefaultTriggerManager : ITriggerManager
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

        public DefaultTriggerManager(IStorageEngine engine)
        {
            _storageEngine = engine;
            Init();
        }

        #region ITriggerManager Members

        public virtual void AddUpdateTriggerFor(string className, UpdateTrigger trigger)
        {
            AddTriggerFor(className, trigger, _listOfUpdateTriggers);
        }

        public virtual void AddInsertTriggerFor(string className, InsertTrigger trigger)
        {
            AddTriggerFor(className, trigger, _listOfInsertTriggers);
        }

        public virtual void AddDeleteTriggerFor(string className, DeleteTrigger trigger)
        {
            AddTriggerFor(className, trigger, _listOfDeleteTriggers);
        }

        public virtual void AddSelectTriggerFor(string className, SelectTrigger trigger)
        {
            AddTriggerFor(className, trigger, _listOfSelectTriggers);
        }

        public virtual bool HasDeleteTriggersFor(string classsName)
        {
            return _listOfDeleteTriggers.ContainsKey(classsName) || _listOfDeleteTriggers.ContainsKey(AllClassTrigger);
        }

        public virtual bool HasInsertTriggersFor(string className)
        {
            return _listOfInsertTriggers.ContainsKey(className) || _listOfInsertTriggers.ContainsKey(AllClassTrigger);
        }

        public virtual bool HasSelectTriggersFor(string className)
        {
            return _listOfSelectTriggers.ContainsKey(className) || _listOfSelectTriggers.ContainsKey(AllClassTrigger);
        }

        public virtual bool HasUpdateTriggersFor(string className)
        {
            return _listOfUpdateTriggers.ContainsKey(className) || _listOfUpdateTriggers.ContainsKey(AllClassTrigger);
        }

        public virtual bool ManageInsertTriggerBefore(string className, object @object)
        {
            if (HasInsertTriggersFor(className))
            {
                foreach (InsertTrigger trigger in GetListOfInsertTriggersFor(className))
                {
                    if (trigger.Odb == null)
                        trigger.Odb = new OdbForTrigger(_storageEngine);

                    try
                    {
                        if (!IsNull(@object))
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

        public virtual void ManageInsertTriggerAfter(string className, object @object, OID oid)
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

        public virtual bool ManageUpdateTriggerBefore(string className, NonNativeObjectInfo oldNnoi, object newObject,
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
                        trigger.BeforeUpdate(new DefaultObjectRepresentation(oldNnoi), Transform(newObject), oid);
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

        public virtual void ManageUpdateTriggerAfter(string className, NonNativeObjectInfo oldNnoi, object newObject,
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
                    trigger.AfterUpdate(new DefaultObjectRepresentation(oldNnoi), Transform(newObject), oid);
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

        public virtual bool ManageDeleteTriggerBefore(string className, object @object, OID oid)
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

        public virtual void ManageDeleteTriggerAfter(string className, object @object, OID oid)
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

        public virtual void ManageSelectTriggerAfter(string className, object @object, OID oid)
        {
            if (!HasSelectTriggersFor(className))
                return;

            foreach (SelectTrigger trigger in GetListOfSelectTriggersFor(className))
            {
                if (trigger.Odb == null)
                    trigger.Odb = new OdbForTrigger(_storageEngine);

                if (!IsNull(@object))
                    trigger.AfterSelect(Transform(@object), oid);
            }
        }

        /// <summary>
        ///   For the default object trigger, no transformation is needed
        /// </summary>
        public virtual object Transform(object @object)
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
                triggers = new OdbArrayList<Odb.Core.Trigger.Trigger>();
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
        public virtual IOdbList<Odb.Core.Trigger.Trigger> GetListOfDeleteTriggersFor(string className)
        {
            var l1 = _listOfDeleteTriggers[className];
            var l2 = _listOfDeleteTriggers[AllClassTrigger];

            if (l2 != null)
            {
                var size = l2.Count;
                if (l1 != null)
                    size = size + l1.Count;

                IOdbList<Odb.Core.Trigger.Trigger> r = new OdbArrayList<Odb.Core.Trigger.Trigger>(size);

                if (l1 != null)
                    r.AddAll(l1);

                r.AddAll(l2);
                return r;
            }
            return l1;
        }

        public virtual IOdbList<Odb.Core.Trigger.Trigger> GetListOfInsertTriggersFor(string className)
        {
            var l1 = _listOfInsertTriggers[className];
            var l2 = _listOfInsertTriggers[AllClassTrigger];
            if (l2 != null)
            {
                var size = l2.Count;
                if (l1 != null)
                    size = size + l1.Count;
                IOdbList<Odb.Core.Trigger.Trigger> r = new OdbArrayList<Odb.Core.Trigger.Trigger>(size);
                if (l1 != null)
                    r.AddAll(l1);
                r.AddAll(l2);
                return r;
            }
            return l1;
        }

        public virtual IOdbList<Odb.Core.Trigger.Trigger> GetListOfSelectTriggersFor(string className)
        {
            var l1 = _listOfSelectTriggers[className];
            var l2 = _listOfSelectTriggers[AllClassTrigger];
            if (l2 != null)
            {
                var size = l2.Count;
                if (l1 != null)
                    size = size + l1.Count;
                IOdbList<Odb.Core.Trigger.Trigger> r = new OdbArrayList<Odb.Core.Trigger.Trigger>(size);
                if (l1 != null)
                    r.AddAll(l1);
                r.AddAll(l2);
                return r;
            }
            return l1;
        }

        public virtual IOdbList<Odb.Core.Trigger.Trigger> GetListOfUpdateTriggersFor(string className)
        {
            var l1 = _listOfUpdateTriggers[className];
            var l2 = _listOfUpdateTriggers[AllClassTrigger];
            if (l2 != null)
            {
                var size = l2.Count;
                if (l1 != null)
                    size = size + l1.Count;
                IOdbList<Odb.Core.Trigger.Trigger> r = new OdbArrayList<Odb.Core.Trigger.Trigger>(size);
                if (l1 != null)
                    r.AddAll(l1);
                r.AddAll(l2);
                return r;
            }
            return l1;
        }

        protected virtual bool IsNull(object @object)
        {
            return @object == null;
        }
    }
}
