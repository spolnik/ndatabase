namespace NDatabase2.Odb
{
    public interface IExternalOID : OID
    {
        IDatabaseId GetDatabaseId();
    }
}
