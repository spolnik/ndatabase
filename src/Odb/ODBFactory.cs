using NDatabase.Odb.Impl.Main;

namespace NDatabase.Odb
{
    /// <summary>
    ///   The NDatabase Factory to open new instance of local odb.
    /// </summary>
    public static class OdbFactory
    {
        /// <summary>
        ///   Open a ODB database instance with a given name
        /// </summary>
        /// <param name="fileName"> The ODB database name </param>
        /// <returns> A local ODB implementation </returns>
        public static IOdb Open(string fileName)
        {
            return Impl.Main.Odb.GetInstance(fileName);
        }
    }
}
