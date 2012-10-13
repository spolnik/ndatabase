using NDatabase2.Odb.Core.Layers.Layer3;
using NDatabase2.Odb.Core.Layers.Layer3.Engine;

namespace NDatabase2.Odb.Main
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
            : base(new StorageEngine(new FileIdentification(fileName)))
        {
        }

        internal static Odb GetInstance(string fileName)
        {
            return new Odb(fileName);
        }
    }
}
