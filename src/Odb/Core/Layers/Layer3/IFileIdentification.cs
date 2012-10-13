namespace NDatabase2.Odb.Core.Layers.Layer3
{
    /// <summary>
    ///   An interface to get info about database parameters
    /// </summary>
    public interface IFileIdentification
    {
        string Id { get; }
        string Directory { get; }
        string FileName { get; }

        bool IsNew();
    }
}