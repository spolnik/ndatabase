namespace NDatabase.Odb
{
    public interface ITransactionId
    {
        long GetId1();

        long GetId2();

        IDatabaseId GetDatabaseId();

        ITransactionId Next();

        ITransactionId Prev();
    }
}
