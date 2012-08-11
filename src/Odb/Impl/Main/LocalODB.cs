using NDatabase.Odb.Core.Layers.Layer3;

namespace NDatabase.Odb.Impl.Main
{
    /// <summary>
    ///   The Local ODB implementation.
    /// </summary>
    /// <remarks>
    ///   The Local ODB implementation.
    /// </remarks>
    /// <author>osmadja</author>
    internal class LocalOdb : OdbAdapter
    {
        /// <summary>
        ///   protected Constructor with user and password
        /// </summary>
        protected LocalOdb(string fileName)
            : base(OdbConfiguration.GetCoreProvider().GetStorageEngine(new IOFileParameter(fileName, true)))
        {
        }

        internal static LocalOdb GetInstance(string fileName)
        {
            return new LocalOdb(fileName);
        }
    }
}
