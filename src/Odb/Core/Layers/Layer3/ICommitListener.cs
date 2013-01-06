namespace NDatabase.Odb.Core.Layers.Layer3
{
    internal interface ICommitListener
    {
        void BeforeCommit();

        void AfterCommit();
    }
}