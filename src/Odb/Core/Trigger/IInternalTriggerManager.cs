using System;
using NDatabase2.Odb.Core.Layers.Layer2.Meta;

namespace NDatabase2.Odb.Core.Trigger
{
    internal interface IInternalTriggerManager
    {
        bool ManageInsertTriggerBefore(Type type, object @object);

        void ManageInsertTriggerAfter(Type type, object @object, OID oid);

        bool ManageUpdateTriggerBefore(Type type, NonNativeObjectInfo oldObjectRepresentation, object newObject,
                                       OID oid);

        void ManageUpdateTriggerAfter(Type type, NonNativeObjectInfo oldObjectRepresentation, object newObject,
                                      OID oid);

        bool ManageDeleteTriggerBefore(Type type, object @object, OID oid);

        void ManageDeleteTriggerAfter(Type type, object @object, OID oid);

        void ManageSelectTriggerAfter(Type type, object @object, OID oid);

        void AddUpdateTriggerFor(Type type, UpdateTrigger trigger);

        void AddInsertTriggerFor(Type type, InsertTrigger trigger);

        void AddDeleteTriggerFor(Type type, DeleteTrigger trigger);

        void AddSelectTriggerFor(Type type, SelectTrigger trigger);

        bool HasDeleteTriggersFor(Type type);

        bool HasInsertTriggersFor(Type type);

        bool HasSelectTriggersFor(Type type);

        bool HasUpdateTriggersFor(Type type);
    }
}
