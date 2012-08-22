namespace NDatabase.Odb.Impl.Tool
{
    /// <summary>
    ///   To manage logging level
    /// </summary>
    /// <author>osmadja</author>
    public static class LogUtil
    {
        public static readonly string ObjectWriter = "ObjectWriter";

        public static readonly string ObjectReader = "ObjectReader";

        public static readonly string FileSystemInterface = "FileSystemInterface";

        public static readonly string IdManager = "IdManager";

        public static readonly string Transaction = "Transaction";

        public static readonly string BufferedIo = "BufferedIO";

        public static readonly string MultiBufferedIo = "MultiBufferedIO";

        public static readonly string WriteAction = "WriteAction";

        public static void ObjectWriterOn(bool yes)
        {
            if (yes)
                OdbConfiguration.AddLogId(ObjectWriter);
            else
                OdbConfiguration.RemoveLogId(ObjectWriter);
        }

        public static void ObjectReaderOn(bool yes)
        {
            if (yes)
                OdbConfiguration.AddLogId(ObjectReader);
            else
                OdbConfiguration.RemoveLogId(ObjectReader);
        }

        public static void FileSystemOn(bool yes)
        {
            if (yes)
                OdbConfiguration.AddLogId(FileSystemInterface);
            else
                OdbConfiguration.RemoveLogId(FileSystemInterface);
        }

        public static void IdManagerOn(bool yes)
        {
            if (yes)
                OdbConfiguration.AddLogId(IdManager);
            else
                OdbConfiguration.RemoveLogId(IdManager);
        }

        public static void TransactionOn(bool yes)
        {
            if (yes)
                OdbConfiguration.AddLogId(Transaction);
            else
                OdbConfiguration.RemoveLogId(Transaction);
        }

        public static void LogOn(string logId, bool yes)
        {
            if (yes)
                OdbConfiguration.AddLogId(logId);
            else
                OdbConfiguration.RemoveLogId(logId);
        }

        public static void AllOn(bool yes)
        {
            OdbConfiguration.SetLogAll(yes);
        }

        public static void Enable(string logId)
        {
            OdbConfiguration.AddLogId(logId);
        }
    }
}
