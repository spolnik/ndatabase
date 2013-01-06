namespace NDatabase.Odb
{
    internal interface ITransactionId
    {
        long GetId1();

        long GetId2();

        ITransactionId Next();
    }
}
