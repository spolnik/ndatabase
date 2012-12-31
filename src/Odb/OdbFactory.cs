using System;
using System.IO;

namespace NDatabase.Odb
{
    /// <summary>
    ///   The NDatabase Factory to open new instance of local odb.
    /// </summary>
    public static class OdbFactory
    {
        [ThreadStatic]
        private static string _last;

        /// <summary>
        ///   Open a ODB database instance with a given name
        /// </summary>
        /// <param name="fileName"> The ODB database name </param>
        /// <returns> A local ODB implementation </returns>
        public static IOdb Open(string fileName)
        {
            _last = fileName;
            return Main.Odb.GetInstance(fileName);
        }

        /// <summary>
        ///   Open a ODB database instance with a last given name
        /// </summary>
        /// <returns> A local ODB implementation </returns>
        public static IOdb OpenLast()
        {
            return Main.Odb.GetInstance(_last);
        }

        /// <summary>
        ///  Remove existing ODB database with a given name
        /// </summary>
        /// <param name="fileName">Database file name</param>
        public static void Delete(string fileName)
        {
            if (!File.Exists(fileName))
                return;

            File.Delete(fileName);
        }
    }
}
