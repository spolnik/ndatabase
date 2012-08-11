using NDatabase.Tool.Wrappers.IO;

namespace NDatabase.Tool
{
    /// <summary>
    ///   Delete file function
    /// </summary>
    /// <author>osmadja</author>
    public static class IOUtil
    {
        public static bool DeleteFile(string fileName)
        {
            var file = new OdbFile(fileName);
            return file.Delete();
        }

        public static bool ExistFile(string fileName)
        {
            var file = new OdbFile(fileName);
            return file.Exists();
        }
    }
}