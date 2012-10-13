namespace NDatabase2.Odb.Core.Trigger
{
    public abstract class SelectTrigger : Trigger
    {
        public abstract void AfterSelect(object @object, OID oid);
    }
}
