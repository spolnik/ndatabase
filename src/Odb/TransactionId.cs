namespace NDatabase2.Odb
{
    public interface ITransactionId
    {
        long GetId1();

        long GetId2();

        IDatabaseId GetDatabaseId();

        ITransactionId Next();
    }
}
