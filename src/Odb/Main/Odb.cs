using NDatabase.Odb.Core.Layers.Layer3;

namespace NDatabase.Odb.Main
{
    /// <summary>
    ///   The Local ODB implementation.
    /// </summary>
    internal sealed class Odb : OdbAdapter
    {
        /// <summary>
        ///   protected Constructor
        /// </summary>
        private Odb(string fileName)
            : base(OdbConfiguration.GetCoreProvider().GetStorageEngine(new FileIdentification(fileName)))
        {
        }

        internal static Odb GetInstance(string fileName)
        {
            return new Odb(fileName);
        }
    }
}
