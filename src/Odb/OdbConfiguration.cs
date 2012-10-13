using NDatabase2.Tool;

namespace NDatabase2.Odb
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

        private static bool _useMultiBuffer = true;

        private static bool _automaticCloseFileOnExit;

        private static bool _throwExceptionWhenInconsistencyFound = true;

        /// <summary>
        ///   header(34) + 1000 * 18
        /// </summary>
        private static int _idBlockSize = 34 + NbIdsPerBlock * IdBlockRepetitionSize;

        private static bool _useCache = true;

        /// <summary>
        ///   The default btree size for index btrees
        /// </summary>
        private static int _defaultIndexBTreeDegree = 20;

        /// <summary>
        ///   Scale used for average action *
        /// </summary>
        private static int _scaleForAverageDivision = 2;

        /// <summary>
        ///   Round Type used for the average division
        /// </summary>
        private static int _roundTypeForAverageDivision;

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

        public static bool ThrowExceptionWhenInconsistencyFound()
        {
            return _throwExceptionWhenInconsistencyFound;
        }

        public static void SetThrowExceptionWhenInconsistencyFound(bool throwExceptionWhenInconsistencyFound)
        {
            _throwExceptionWhenInconsistencyFound = throwExceptionWhenInconsistencyFound;
        }

        public static int GetIdBlockSize()
        {
            return _idBlockSize;
        }

        public static void SetIdBlockSize(int idBlockSize)
        {
            _idBlockSize = idBlockSize;
        }

        public static int GetNbIdsPerBlock()
        {
            return NbIdsPerBlock;
        }

        public static int GetIdBlockRepetitionSize()
        {
            return IdBlockRepetitionSize;
        }

        public static bool UseMultiBuffer()
        {
            return _useMultiBuffer;
        }

        public static void SetUseMultiBuffer(bool useMultiBuffer)
        {
            _useMultiBuffer = useMultiBuffer;
        }

        public static bool AutomaticCloseFileOnExit()
        {
            return _automaticCloseFileOnExit;
        }

        public static void SetAutomaticCloseFileOnExit(bool automaticFileClose)
        {
            _automaticCloseFileOnExit = automaticFileClose;
        }

        public static int GetDefaultIndexBTreeDegree()
        {
            return _defaultIndexBTreeDegree;
        }

        public static void SetDefaultIndexBTreeDegree(int defaultIndexBTreeSize)
        {
            _defaultIndexBTreeDegree = defaultIndexBTreeSize;
        }

        public static bool IsUseCache()
        {
            return _useCache;
        }

        public static void SetUseCache(bool useCache)
        {
            _useCache = useCache;
        }

        public static int GetScaleForAverageDivision()
        {
            return _scaleForAverageDivision;
        }

        public static void SetScaleForAverageDivision(int scaleForAverageDivision)
        {
            _scaleForAverageDivision = scaleForAverageDivision;
        }

        public static int GetRoundTypeForAverageDivision()
        {
            return _roundTypeForAverageDivision;
        }

        public static void SetRoundTypeForAverageDivision(int roundTypeForAverageDivision)
        {
            _roundTypeForAverageDivision = roundTypeForAverageDivision;
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
