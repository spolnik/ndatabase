namespace NDatabase2.Odb.Core.Layers.Layer3
{
    public interface ICommitListener
    {
        void BeforeCommit();

        void AfterCommit();
    }
}