using NDatabase.Btree.Tool;
using NDatabase.Tool;

namespace NDatabase.Odb
{
    /// <summary>
    ///   The main NDatabase ODB Configuration class.
    /// </summary>
    /// <remarks>
    ///   The main NDatabase ODB Configuration class. All engine configuration is done via this class.
    /// </remarks>
    public static class OdbConfiguration
    {
        private static bool _loggingEnabled;

        private static int _maxNumberOfWriteObjectPerTransaction = 10000;

        /// <summary>
        ///   The default btree size for index btrees
        /// </summary>
        private static int _indexBTreeDegree = 20;

        /// <summary>
        /// Get max number of write object actions per transaction
        /// </summary>
        /// <returns>Max number of write object actions per transaction</returns>
        public static int GetMaxNumberOfWriteObjectPerTransaction()
        {
            return _maxNumberOfWriteObjectPerTransaction;
        }

        /// <summary>
        /// Set max number of write object actions per transaction
        /// </summary>
        /// <param name="maxNumberOfWriteObjectPerTransaction">New value - max number of write object actions per transaction</param>
        public static void SetMaxNumberOfWriteObjectPerTransaction(int maxNumberOfWriteObjectPerTransaction)
        {
            _maxNumberOfWriteObjectPerTransaction = maxNumberOfWriteObjectPerTransaction;
        }

        /// <summary>
        /// Turn on BTree validation (more safely, but hit on performance)
        /// </summary>
        public static void TurnOnBTreeValidation()
        {
            BTreeValidator.SetOn(true);
        }

        /// <summary>
        /// Turn off BTree validation (less safely, but better performance)
        /// </summary>
        public static void TurnOffBTreeValidation()
        {
            BTreeValidator.SetOn(false);
        }

        /// <summary>
        /// Get index BTree degree (on start it is equals to 20)
        /// </summary>
        /// <returns>Degree of index BTree</returns>
        public static int GetIndexBTreeDegree()
        {
            return _indexBTreeDegree;
        }

        /// <summary>
        /// Set index BTree degree (on start it is equals to 20)
        /// </summary>
        /// <param name="indexBTreeSize">Degree of index BTree</param>
        public static void SetIndexBTreeDegree(int indexBTreeSize)
        {
            _indexBTreeDegree = indexBTreeSize;
        }

        /// <summary>
        /// Check if logging is turned on
        /// </summary>
        /// <returns>True if logging is enabled, false in other case</returns>
        public static bool IsLoggingEnabled()
        {
            return _loggingEnabled;
        }

        /// <summary>
        /// Turn on logging in NDatabase
        /// </summary>
        public static void EnableLogging()
        {
            _loggingEnabled = true;
        }

        /// <summary>
        /// Turn off logging in NDatabase
        /// </summary>
        public static void DisableLogging()
        {
            _loggingEnabled = false;
        }

        /// <summary>
        /// Enable console logger, automatically turning on the logging in NDatabase
        /// </summary>
        public static void EnableConsoleLogger()
        {
            if (!IsLoggingEnabled())
                EnableLogging();

            DLogger.Register(new ConsoleLogger());
        }

        /// <summary>
        /// Register custom logger, e.g. log4net implementation
        /// </summary>
        /// <param name="logger">Custom loger to register</param>
        public static void RegisterLogger(ILogger logger)
        {
            if (!IsLoggingEnabled())
                EnableLogging();

            DLogger.Register(logger);
        }
    }
}
