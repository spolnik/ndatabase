using System;
using System.Collections.Generic;
using NDatabase.Odb.Core;
using NDatabase.Odb.Impl;
using NDatabase.Tool.Wrappers.Map;

namespace NDatabase.Odb
{
    /// <summary>
    ///   The main NDatabase ODB Configuration class.
    /// </summary>
    /// <remarks>
    ///   The main NDatabase ODB Configuration class. All engine configuration is done via this class.
    /// </remarks>
    /// <author>osmadja</author>
    public static class OdbConfiguration
    {
        private const int NbIdsPerBlock = 1000;

        private const int IdBlockRepetitionSize = 18;
        private static bool _coreProviderInit;

        private static bool _debugEnabled;

        private static bool _logAll;

        private static int _debugLevel = 100;

        private static IDictionary<string, string> _logIds;

        private static bool _infoEnabled;

        private static bool _enableAfterWriteChecking;

        private static int _maxNumberOfWriteObjectPerTransaction = 10000;

        private static long _maxNumberOfObjectInCache = 3000000;

        private static bool _useMultiBuffer = true;

        private static bool _automaticCloseFileOnExit;

        private static bool _throwExceptionWhenInconsistencyFound = true;

        /// <summary>
        ///   header(34) + 1000 * 18
        /// </summary>
        private static int _idBlockSize = 34 + NbIdsPerBlock * IdBlockRepetitionSize;

        private static int _stringSpaceReserveFactor = 1;

        private static bool _checkModelCompatibility = true;

        /// <summary>
        ///   a boolean value to specify if ODBFactory waits a little to re-open a file when a file is locked
        /// </summary>
        private static bool _retryIfFileIsLocked = true;

        /// <summary>
        ///   How many times ODBFactory tries to open the file when it is locked
        /// </summary>
        private static int _numberOfRetryToOpenFile = 5;

        /// <summary>
        ///   How much time (in ms) ODBFactory waits between each retry
        /// </summary>
        private static int _retryTimeout = 100;

        /// <summary>
        ///   How much time (in ms) ODBFactory waits to be sure a file has been created
        /// </summary>
        private static long _defaultFileCreationTime = 500;

        /// <summary>
        ///   Automatically increase cache size when it is full
        /// </summary>
        private static bool _automaticallyIncreaseCacheSize;

        private static bool _useCache = true;

        /// <summary>
        ///   The default btree size for index btrees
        /// </summary>
        private static int _defaultIndexBTreeDegree = 20;

        /// <summary>
        ///   To indicate if warning must be displayed
        /// </summary>
        private static bool _displayWarnings = true;

        /// <summary>
        ///   Scale used for average action *
        /// </summary>
        private static int _scaleForAverageDivision = 2;

        /// <summary>
        ///   Round Type used for the average division
        /// </summary>
        private static int _roundTypeForAverageDivision;

        /// <summary>
        ///   The core provider is the provider of core object implementation for ODB
        /// </summary>
        private static readonly ICoreProvider CoreProvider = new DefaultCoreProvider();

        /// <summary>
        ///   To specify if NDatabase must automatically reconnect objects loaded in previous session.
        /// </summary>
        /// <remarks>
        ///   To specify if NDatabase must automatically reconnect objects loaded in previous session. With with flag on, user does not need to manually reconnect an object. Default value = true
        /// </remarks>
        private static bool _reconnectObjectsToSession;

        /// <summary>
        ///   To activate or desactivate the use of index
        /// </summary>
        private static bool _useIndex = true;

        // For multi thread
        /// <returns> </returns>
        public static bool ReconnectObjectsToSession()
        {
            return _reconnectObjectsToSession;
        }

        public static void SetReconnectObjectsToSession(bool reconnectObjectsToSession)
        {
            _reconnectObjectsToSession = reconnectObjectsToSession;
        }

        public static void AddLogId(string logId)
        {
            if (_logIds == null)
                _logIds = new OdbHashMap<string, string>();
            _logIds.Add(logId, logId);
        }

        public static void RemoveLogId(string logId)
        {
            if (_logIds == null)
                _logIds = new OdbHashMap<string, string>();
            _logIds.Remove(logId);
        }

        public static bool IsDebugEnabled(string logId)
        {
            if (!_debugEnabled)
                return false;
            if (_logAll)
                return true;
            if (_logIds == null || _logIds.Count == 0)
                return false;
            return _logIds.ContainsKey(logId);
        }

        public static void SetDebugEnabled(int level, bool debug)
        {
            _debugEnabled = debug;
            _debugLevel = level;
        }

        public static bool IsEnableAfterWriteChecking()
        {
            return _enableAfterWriteChecking;
        }

        public static bool IsInfoEnabled()
        {
            return _infoEnabled;
        }

        public static bool IsInfoEnabled(string logId)
        {
            // return false;
            if (_logAll)
                return true;
            if (_logIds == null || _logIds.Count == 0)
                return false;
            return _logIds.ContainsKey(logId);
        }

        // return false;
        public static void SetInfoEnabled(bool infoEnabled)
        {
            _infoEnabled = infoEnabled;
        }

        public static void SetEnableAfterWriteChecking(bool enableAfterWriteChecking)
        {
            _enableAfterWriteChecking = enableAfterWriteChecking;
        }

        public static int GetMaxNumberOfWriteObjectPerTransaction()
        {
            return _maxNumberOfWriteObjectPerTransaction;
        }

        public static void SetMaxNumberOfWriteObjectPerTransaction(int maxNumberOfWriteObjectPerTransaction)
        {
            _maxNumberOfWriteObjectPerTransaction = maxNumberOfWriteObjectPerTransaction;
        }

        public static long GetMaxNumberOfObjectInCache()
        {
            return _maxNumberOfObjectInCache;
        }

        public static void SetMaxNumberOfObjectInCache(long maxNumberOfObjectInCache)
        {
            _maxNumberOfObjectInCache = maxNumberOfObjectInCache;
        }

        public static int GetNumberOfRetryToOpenFile()
        {
            return _numberOfRetryToOpenFile;
        }

        public static void SetNumberOfRetryToOpenFile(int numberOfRetryToOpenFile)
        {
            _numberOfRetryToOpenFile = numberOfRetryToOpenFile;
        }

        public static long GetRetryTimeout()
        {
            return _retryTimeout;
        }

        public static void SetRetryTimeout(int retryTimeout)
        {
            _retryTimeout = retryTimeout;
        }

        public static bool RetryIfFileIsLocked()
        {
            return _retryIfFileIsLocked;
        }

        public static void SetRetryIfFileIsLocked(bool retryIfFileIsLocked)
        {
            _retryIfFileIsLocked = retryIfFileIsLocked;
        }

        public static long GetDefaultFileCreationTime()
        {
            return _defaultFileCreationTime;
        }

        public static void SetDefaultFileCreationTime(long defaultFileCreationTime)
        {
            _defaultFileCreationTime = defaultFileCreationTime;
        }

        public static bool IsMultiThread()
        {
            return _retryIfFileIsLocked;
        }

        public static void UseMultiThread(bool yes)
        {
            UseMultiThread(yes, _numberOfRetryToOpenFile);
        }

        public static void UseMultiThread(bool yes, int numberOfThreads)
        {
            SetRetryIfFileIsLocked(yes);
            if (yes)
            {
                SetNumberOfRetryToOpenFile(numberOfThreads * 10);
                SetRetryTimeout(50);
            }
        }

        public static bool ThrowExceptionWhenInconsistencyFound()
        {
            return _throwExceptionWhenInconsistencyFound;
        }

        public static void SetThrowExceptionWhenInconsistencyFound(bool throwExceptionWhenInconsistencyFound)
        {
            _throwExceptionWhenInconsistencyFound = throwExceptionWhenInconsistencyFound;
        }

        public static bool AutomaticallyIncreaseCacheSize()
        {
            return _automaticallyIncreaseCacheSize;
        }

        public static void SetAutomaticallyIncreaseCacheSize(bool automaticallyIncreaseCache)
        {
            _automaticallyIncreaseCacheSize = automaticallyIncreaseCache;
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

        /// <returns> Returns the stringSpaceReserveFactor. </returns>
        public static int GetStringSpaceReserveFactor()
        {
            return _stringSpaceReserveFactor;
        }

        /// <param name="stringSpaceReserveFactor"> The stringSpaceReserveFactor to set. </param>
        public static void SetStringSpaceReserveFactor(int stringSpaceReserveFactor)
        {
            _stringSpaceReserveFactor = stringSpaceReserveFactor;
        }

        /// <returns> Returns the debugLevel. </returns>
        public static int GetDebugLevel()
        {
            return _debugLevel;
        }

        /// <param name="debugLevel"> The debugLevel to set. </param>
        public static void SetDebugLevel(int debugLevel)
        {
            _debugLevel = debugLevel;
        }

        public static bool UseMultiBuffer()
        {
            return _useMultiBuffer;
        }

        public static void SetUseMultiBuffer(bool useMultiBuffer)
        {
            _useMultiBuffer = useMultiBuffer;
        }

        public static bool CheckModelCompatibility()
        {
            return _checkModelCompatibility;
        }

        public static void SetCheckModelCompatibility(bool checkModelCompatibility)
        {
            _checkModelCompatibility = checkModelCompatibility;
        }

        public static bool AutomaticCloseFileOnExit()
        {
            return _automaticCloseFileOnExit;
        }

        public static void SetAutomaticCloseFileOnExit(bool automaticFileClose)
        {
            _automaticCloseFileOnExit = automaticFileClose;
        }

        public static bool IsLogAll()
        {
            return _logAll;
        }

        public static void SetLogAll(bool logAll)
        {
            _logAll = logAll;
        }

        public static int GetDefaultIndexBTreeDegree()
        {
            return _defaultIndexBTreeDegree;
        }

        public static void SetDefaultIndexBTreeDegree(int defaultIndexBTreeSize)
        {
            _defaultIndexBTreeDegree = defaultIndexBTreeSize;
        }

        /// <returns> the useCache </returns>
        public static bool IsUseCache()
        {
            return _useCache;
        }

        /// <param name="useCache"> the useCache to set </param>
        public static void SetUseCache(bool useCache)
        {
            _useCache = useCache;
        }

        public static bool DisplayWarnings()
        {
            return _displayWarnings;
        }

        public static void SetDisplayWarnings(bool yesOrNo)
        {
            _displayWarnings = yesOrNo;
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

        public static ICoreProvider GetCoreProvider()
        {
            if (!_coreProviderInit)
            {
                _coreProviderInit = true;
                try
                {
                    CoreProvider.Init2();
                }
                catch (Exception e)
                {
                    throw new OdbRuntimeException(
                        NDatabaseError.ErrorInCoreProviderInitialization.AddParameter("Core Provider"), e);
                }
            }
            return CoreProvider;
        }

        public static bool UseIndex()
        {
            return _useIndex;
        }

        public static void SetUseIndex(bool useIndex)
        {
            _useIndex = useIndex;
        }

        public static bool IsDebugEnabled()
        {
            return _debugEnabled;
        }

        public static void SetDebugEnabled(bool debugEnabled)
        {
            _debugEnabled = debugEnabled;
        }
    }
}
