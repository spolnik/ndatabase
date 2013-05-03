namespace NDatabase.Core.Layers.Layer3
{
    internal interface ICommitListener
    {
        void BeforeCommit();

        void AfterCommit();
    }
}