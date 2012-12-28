using NDatabase2.Odb;

namespace NDatabase.Odb.Core.Trigger
{
    public abstract class DeleteTrigger : Trigger
    {
        public abstract bool BeforeDelete(object @object, OID oid);
        public abstract void AfterDelete(object @object, OID oid);
    }
}
