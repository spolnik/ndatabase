namespace NDatabase2.Odb.Core.Trigger
{
    public abstract class UpdateTrigger : Trigger
    {
        public abstract bool BeforeUpdate(IObjectRepresentation oldObjectRepresentation, object newObject, OID oid);
        public abstract void AfterUpdate(IObjectRepresentation oldObjectRepresentation, object newObject, OID oid);
    }
}
