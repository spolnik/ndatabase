using NDatabase.Odb.Impl.Main;

namespace NDatabase.Odb
{
    /// <summary>
    ///   The ODBFactory to obtain the right ODB implementation.
    /// </summary>
    /// <remarks>
    ///   The ODBFactory to obtain the right ODB implementation.
    /// </remarks>
    /// <author>osmadja</author>
    public static class OdbFactory
    {
        /// <summary>
        ///   Open a non password protected ODB database
        /// </summary>
        /// <param name="fileName"> The ODB database name </param>
        /// <returns> A local ODB implementation </returns>
        public static IOdb Open(string fileName)
        {
            return LocalOdb.GetInstance(fileName);
        }
    }
}
