using NDatabase.Odb.Core.Layers.Layer3.IO;

namespace NDatabase.Odb
{
    /// <summary>
    ///   The NDatabase Factory to open new instance of local odb.
    /// </summary>
    public static class OdbFactory
    {
        private static string _last;

        /// <summary>
        ///   Open a ODB database instance with a given name
        /// </summary>
        /// <param name="fileName"> The ODB database name </param>
        /// <returns> A local ODB implementation </returns>
        public static IOdb Open(string fileName)
        {
            _last = fileName;
            return Impl.Main.Odb.GetInstance(fileName);
        }

        /// <summary>
        ///   Open a ODB database instance with a last given name
        /// </summary>
        /// <returns> A local ODB implementation </returns>
        public static IOdb OpenLast()
        {
            return Impl.Main.Odb.GetInstance(_last);
        }

        /// <summary>
        ///  Remove existing ODB database with a given name
        /// </summary>
        /// <param name="fileName">Database file name</param>
        /// <returns>The result of operation - true if succcess</returns>
        public static bool Delete(string fileName)
        {
            var file = new OdbFile(fileName);
            return file.Delete();
        }
    }
}
