using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Layers.Layer3.Engine;

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
            : base(new StorageEngine(new FileIdentification(fileName)))
        {
        }

        private Odb()
            : base(new StorageEngine(new InMemoryIdentification()))
        {
        }

        internal static Odb GetInstance(string fileName)
        {
            return new Odb(fileName);
        }

        internal static Odb GetInMemoryInstance()
        {
            return new Odb();
        }
    }
}
