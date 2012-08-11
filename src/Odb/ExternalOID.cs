namespace NDatabase.Odb
{
    public interface IExternalOID : OID
    {
        IDatabaseId GetDatabaseId();
    }
}
