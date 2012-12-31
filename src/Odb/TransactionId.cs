namespace NDatabase.Odb
{
    public interface ITransactionId
    {
        long GetId1();

        long GetId2();

        ITransactionId Next();
    }
}
