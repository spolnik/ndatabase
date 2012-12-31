namespace NDatabase.Odb.Core.Layers.Layer3
{
    /// <summary>
    ///   An interface to get info about database parameters
    /// </summary>
    internal interface IFileIdentification
    {
        string Id { get; }
        string Directory { get; }
        string FileName { get; }

        bool IsNew();
    }
}