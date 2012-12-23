using NDatabase2.Odb.Core.Layers.Layer2.Meta;

namespace NDatabase2.Odb.Core.Layers.Layer3.Engine
{
    /// <summary>
    ///   Some Storage engine constants about offset position for object writing/reading.
    /// </summary>
    internal static class StorageEngineConstant
    {
        public const long NullObjectIdId = 0;

        public const long DeletedObjectPosition = 0;

        public const long NullObjectPosition = 0;

        public const long ObjectIsNotInCache = -1;

        public const long PositionNotInitialized = -1;

        public const long ObjectDoesNotExist = -2;

        /// <summary>
        ///   this occurs when a class has been refactored adding a field.
        /// </summary>
        /// <remarks>
        ///   this occurs when a class has been refactored adding a field. Old objects do not the new field
        /// </remarks>
        public const long FieldDoesNotExist = -1;

        public const byte Version2 = 2;

        public const byte Version3 = 3;

        public const byte Version4 = 4;

        public const byte Version5 = 5;

        public const byte Version6 = 6;

        public const byte Version7 = 7;

        public const int Version8 = 8;

        /// <summary>
        ///   1.9 file format
        /// </summary>
        public const int Version9 = 9;

        /// <summary>
        ///   2.0 - 2.3 file format
        /// </summary>
        private const int Version20 = 20;

        public const int CurrentFileFormatVersion = Version20;

        private const long ClassOffsetBlockSize = 0;
        private const long ObjectOffsetBlockSize = 0;

        /// <summary>
        ///   pull id type (byte),id(long),
        /// </summary>
        private const long BlockIdRepetitionIdType = 0;

        private const long NativeObjectOffsetBlockSize = 0;
        public const byte NoEncryption = 0;

        public const byte WithEncryption = 1;

        /// <summary>
        ///   Used to make an attribute reference a null object - setting its id to zero
        /// </summary>
        public static readonly OID NullObjectId = null;

        /// <summary>
        ///   File format version : 1 int (4 bytes)
        /// </summary>
        public const int DatabaseHeaderVersionPosition = 0;

        /// <summary>
        ///   The Database ID : 4 Long (4*8 bytes)
        /// </summary>
        public static readonly int DatabaseHeaderDatabaseIdPosition = OdbType.Integer.Size;

        /// <summary>
        ///   The last Transaction ID 2 long (2*4*8 bytes)
        /// </summary>
        public static readonly int DatabaseHeaderLastTransactionId = DatabaseHeaderDatabaseIdPosition +
                                                                          4 * OdbType.Long.Size;

        /// <summary>
        ///   The number of classes in the meta model 1 long (4*8 bytes)
        /// </summary>
        public static readonly int DatabaseHeaderNumberOfClassesPosition = DatabaseHeaderLastTransactionId +
                                                                           2 * OdbType.Long.Size;

        /// <summary>
        ///   The first class OID : 1 Long (8 bytes)
        /// </summary>
        public static readonly int DatabaseHeaderFirstClassOid = DatabaseHeaderNumberOfClassesPosition +
                                                                 OdbType.Long.Size;

        /// <summary>
        ///   The last ODB close status.
        /// </summary>
        /// <remarks>
        ///   The last ODB close status. Used to detect if the transaction is ok : 1 byte
        /// </remarks>
        public static readonly int DatabaseHeaderLastCloseStatusPosition = DatabaseHeaderFirstClassOid +
                                                                           OdbType.Long.Size;

        public static readonly int DatabaseHeaderEmptySpaceWhichCouldBeUsedInTheFuture =
            DatabaseHeaderLastCloseStatusPosition + OdbType.Byte.Size;

        /// <summary>
        ///   The Database character encoding : 50 bytes
        /// </summary>
        public static readonly int DatabaseHeaderDatabaseCharacterEncodingPosition =
            DatabaseHeaderEmptySpaceWhichCouldBeUsedInTheFuture + 120 * OdbType.Byte.Size;

        /// <summary>
        ///   The position of the current id block: 1 long
        /// </summary>
        public static readonly int DatabaseHeaderCurrentIdBlockPosition =
            DatabaseHeaderDatabaseCharacterEncodingPosition + 58 * OdbType.Byte.Size;
            

        /// <summary>
        ///   First ID Block position
        /// </summary>
        public static readonly int DatabaseHeaderFirstIdBlockPosition = DatabaseHeaderCurrentIdBlockPosition +
                                                                        OdbType.Long.Size;

        public static readonly int DatabaseHeaderProtectedZoneSize = DatabaseHeaderCurrentIdBlockPosition;

        public static readonly int[] DatabaseHeaderPositions = new[]
            {
                DatabaseHeaderVersionPosition, DatabaseHeaderDatabaseIdPosition, DatabaseHeaderLastTransactionId,
                DatabaseHeaderNumberOfClassesPosition, DatabaseHeaderFirstClassOid,
                DatabaseHeaderLastCloseStatusPosition, DatabaseHeaderDatabaseCharacterEncodingPosition, DatabaseHeaderEmptySpaceWhichCouldBeUsedInTheFuture
            };

        private static readonly long ClassOffsetBlockType = ClassOffsetBlockSize + OdbType.Integer.Size;

        private static readonly long ClassOffsetCategory = ClassOffsetBlockType + OdbType.Byte.Size;

        private static readonly long ClassOffsetId = ClassOffsetCategory + OdbType.Byte.Size;

        private static readonly long ClassOffsetPreviousClassPosition = ClassOffsetId + OdbType.Long.Size;

        public static readonly long ClassOffsetNextClassPosition = ClassOffsetPreviousClassPosition +
                                                                   OdbType.Long.Size;

        public static readonly long ClassOffsetClassNbObjects = ClassOffsetNextClassPosition + OdbType.Long.Size;

        public static readonly long ClassOffsetFullClassNameSize = ClassOffsetNextClassPosition + OdbType.Long.Size;

        public static readonly long ObjectOffsetBlockType = ObjectOffsetBlockSize + OdbType.Integer.Size;

        private static readonly long ObjectOffsetObjectId = ObjectOffsetBlockType + OdbType.Byte.Size;

        public static readonly long ObjectOffsetClassInfoId = ObjectOffsetObjectId + OdbType.Long.Size;

        public static readonly long ObjectOffsetPreviousObjectOid = ObjectOffsetClassInfoId + OdbType.Long.Size;

        public static readonly long ObjectOffsetNextObjectOid = ObjectOffsetPreviousObjectOid + OdbType.Long.Size;

        private static readonly long ObjectOffsetCreationDate = ObjectOffsetNextObjectOid + OdbType.Long.Size;

        private static readonly long ObjectOffsetUpdateDate = ObjectOffsetCreationDate + OdbType.Long.Size;

        private static readonly long ObjectOffsetVersion = ObjectOffsetUpdateDate + OdbType.Long.Size;

        private static readonly long ObjectOffsetReferencePointer = ObjectOffsetVersion + OdbType.Integer.Size;

        private static readonly long ObjectOffsetIsExternallySynchronized = ObjectOffsetReferencePointer +
                                                                           OdbType.Long.Size;

        public static readonly long ObjectOffsetNbAttributes = ObjectOffsetIsExternallySynchronized +
                                                               OdbType.Boolean.Size;

        /// <summary>
        ///   <pre>ID Block Header :
        ///     Block size             : 1 int
        ///     Block type             : 1 byte
        ///     Block status           : 1 byte
        ///     Prev block position    : 1 long
        ///     Next block position    : 1 long
        ///     Block number           : 1 int
        ///     Max id                 : 1 long
        ///     Total size = 34</pre>
        /// </summary>
        public static readonly long BlockIdOffsetForBlockStatus = OdbType.Integer.Size + OdbType.Byte.Size;

        private static readonly long BlockIdOffsetForPrevBlock = BlockIdOffsetForBlockStatus + OdbType.Byte.Size;

        public static readonly long BlockIdOffsetForNextBlock = BlockIdOffsetForPrevBlock + OdbType.Long.Size;

        public static readonly long BlockIdOffsetForBlockNumber = BlockIdOffsetForNextBlock + OdbType.Long.Size;

        public static readonly long BlockIdOffsetForMaxId = BlockIdOffsetForBlockNumber + OdbType.Integer.Size;

        public static readonly long BlockIdOffsetForStartOfRepetition = BlockIdOffsetForMaxId + OdbType.Long.Size;

        private static readonly long BlockIdRepetitionId = BlockIdRepetitionIdType + OdbType.Byte.Size;

        public static readonly long BlockIdRepetitionIdStatus = BlockIdRepetitionId + OdbType.Long.Size;

        public static readonly long BlockIdRepetitionObjectPosition = BlockIdRepetitionIdStatus + OdbType.Byte.Size;

        public static readonly long NativeObjectOffsetBlockType = NativeObjectOffsetBlockSize +
                                                                  OdbType.Integer.Size;

        private static readonly long NativeObjectOffsetOdbTypeId = NativeObjectOffsetBlockType + OdbType.Byte.Size;

        private static readonly long NativeObjectOffsetObjectIsNull = NativeObjectOffsetOdbTypeId +
                                                                     OdbType.Integer.Size;

        public static readonly long NativeObjectOffsetDataArea = NativeObjectOffsetObjectIsNull +
                                                                 OdbType.Boolean.Size;

        // ********************************************************
        // DATABASE HEADER
        // ********************************************************
        // **********************************************************
        // END OF DATABASE HEADER
        // *********************************************************
        // CLASS OFFSETS
        // OBJECT OFFSETS - update this section when modifying the odb file format 
    }
}
