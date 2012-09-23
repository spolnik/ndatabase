using NDatabase.Odb.Core.Layers.Layer2.Meta;

namespace NDatabase.Odb.Core.Trigger
{
    internal interface ITriggerManager
    {
        bool ManageInsertTriggerBefore(string className, object @object);

        void ManageInsertTriggerAfter(string className, object @object, OID oid);

        bool ManageUpdateTriggerBefore(string className, NonNativeObjectInfo oldObjectRepresentation, object newObject,
                                       OID oid);

        void ManageUpdateTriggerAfter(string className, NonNativeObjectInfo oldObjectRepresentation, object newObject,
                                      OID oid);

        bool ManageDeleteTriggerBefore(string className, object @object, OID oid);

        void ManageDeleteTriggerAfter(string className, object @object, OID oid);

        void ManageSelectTriggerAfter(string className, object @object, OID oid);

        void AddUpdateTriggerFor(string className, UpdateTrigger trigger);

        void AddInsertTriggerFor(string className, InsertTrigger trigger);

        void AddDeleteTriggerFor(string className, DeleteTrigger trigger);

        void AddSelectTriggerFor(string className, SelectTrigger trigger);

        /// <summary>
        ///   used to transform object before real trigger call.
        /// </summary>
        /// <remarks>
        ///   used to transform object before real trigger call. This is used for example, in server side trigger where the object is encapsulated in an ObjectRepresentation instance. It is only for internal use
        /// </remarks>
        object Transform(object @object);

        bool HasDeleteTriggersFor(string classsName);

        bool HasInsertTriggersFor(string className);

        bool HasSelectTriggersFor(string className);

        bool HasUpdateTriggersFor(string className);
    }
}
