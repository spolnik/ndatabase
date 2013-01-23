using System;
using System.IO;

namespace NDatabase.Odb
{
    /// <summary>
    /// The NDatabase Factory to open new instance of local odb.
    /// </summary>
    public static class OdbFactory
    {
        private static string _last;

        /// <summary>
        /// Opens the database instance with the specified file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>IOdb.</returns>
        public static IOdb Open(string fileName)
        {
            _last = fileName;
            return Main.Odb.GetInstance(fileName);
        }

        /// <summary>
        /// Opens the database instance with the last given name.
        /// </summary>
        /// <returns>IOdb.</returns>
        public static IOdb OpenLast()
        {
            return Main.Odb.GetInstance(_last);
        }

        /// <summary>
        /// Opens a database in the In-Memory mode.
        /// </summary>
        /// <returns>IOdb implementation.</returns>
        public static IOdb OpenInMemory()
        {
            return Main.Odb.GetInMemoryInstance();
        }

        /// <summary>
        /// Deletes the specified file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public static void Delete(string fileName)
        {
            if (!File.Exists(fileName))
                return;

            File.Delete(fileName);
        }
    }
}
