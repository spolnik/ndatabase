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
    }
}
