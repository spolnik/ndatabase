namespace NDatabase2.Odb.Core.Trigger
{
    public abstract class InsertTrigger : Trigger
    {
        public abstract bool BeforeInsert(object @object);
        public abstract void AfterInsert(object @object, OID oid);
    }
}
