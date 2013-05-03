using NDatabase.Core.Layer3;
using NDatabase.Core.Layer3.Engine;

namespace NDatabase.Core
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
