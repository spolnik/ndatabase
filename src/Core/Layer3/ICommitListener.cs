namespace NDatabase.Core.Layer3
{
    internal interface ICommitListener
    {
        void BeforeCommit();

        void AfterCommit();
    }
}