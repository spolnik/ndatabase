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
        private const int NbIdsPerBlock = 1000;

        private const int IdBlockRepetitionSize = 18;
        
        private static bool _loggingEnabled;

        private static int _maxNumberOfWriteObjectPerTransaction = 10000;

        /// <summary>
        ///   The default btree size for index btrees
        /// </summary>
        private static int _defaultIndexBTreeDegree = 20;

        /// <summary>
        ///   To activate or desactivate the use of index
        /// </summary>
        private static bool _useIndex = true;

        public static int GetMaxNumberOfWriteObjectPerTransaction()
        {
            return _maxNumberOfWriteObjectPerTransaction;
        }

        public static void SetMaxNumberOfWriteObjectPerTransaction(int maxNumberOfWriteObjectPerTransaction)
        {
            _maxNumberOfWriteObjectPerTransaction = maxNumberOfWriteObjectPerTransaction;
        }

        public static void TurnOnBTreeValidation()
        {
            BTreeValidator.SetOn(true);
        }

        public static void TurnOffBTreeValidation()
        {
            BTreeValidator.SetOn(false);
        }

        public static int GetNbIdsPerBlock()
        {
            return NbIdsPerBlock;
        }

        public static int GetIdBlockRepetitionSize()
        {
            return IdBlockRepetitionSize;
        }

        public static int GetDefaultIndexBTreeDegree()
        {
            return _defaultIndexBTreeDegree;
        }

        public static void SetDefaultIndexBTreeDegree(int defaultIndexBTreeSize)
        {
            _defaultIndexBTreeDegree = defaultIndexBTreeSize;
        }

        public static bool UseIndex()
        {
            return _useIndex;
        }

        public static void SetUseIndex(bool useIndex)
        {
            _useIndex = useIndex;
        }

        public static bool IsLoggingEnabled()
        {
            return _loggingEnabled;
        }

        public static void EnableLogging()
        {
            _loggingEnabled = true;
        }

        public static void DisableLogging()
        {
            _loggingEnabled = false;
        }

        public static void EnableConsoleLogger()
        {
            if (!IsLoggingEnabled())
                EnableLogging();

            DLogger.Register(new ConsoleLogger());
        }

        public static void RegisterLogger(ILogger logger)
        {
            if (!IsLoggingEnabled())
                EnableLogging();

            DLogger.Register(logger);
        }
    }
}
