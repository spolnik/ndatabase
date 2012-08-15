using NDatabase.Odb.Core.Layers.Layer2.Meta;

namespace NDatabase.Odb.Impl.Core.Layers.Layer3.Engine
{
    //TODO: Think about some buffer in the header block -> future purposes (255?)
    /// <summary>
    ///   Some Storage engine constants about offset position for object writing/reading.
    /// </summary>
    /// <remarks>
    ///   Some Storage engine constants about offset position for object writing/reading.
    /// </remarks>
    public static class StorageEngineConstant
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

        public const int CurrentFileFormatVersion = Version9;

        public const long ClassOffsetBlockSize = 0;
        public const long ObjectOffsetBlockSize = 0;

        /// <summary>
        ///   pull id type (byte),id(long),
        /// </summary>
        public const long BlockIdRepetitionIdType = 0;

        public const long NativeObjectOffsetBlockSize = 0;
        public const byte NoEncryption = 0;

        public const byte WithEncryption = 1;

        /// <summary>
        ///   Used to make an attribute reference a null object - setting its id to zero
        /// </summary>
        public static readonly OID NullObjectId = null;

        /// <summary>
        ///   File format version : 1 int (4 bytes)
        /// </summary>
        public static readonly int DatabaseHeaderVersionPosition = 0;

        /// <summary>
        ///   The Database ID : 4 Long (4*8 bytes)
        /// </summary>
        public static readonly int DatabaseHeaderDatabaseIdPosition = OdbType.Integer.GetSize();

        /// <summary>
        ///   The last Transaction ID 2 long (2*4*8 bytes)
        /// </summary>
        public static readonly int DatabaseHeaderLastTransactionId = DatabaseHeaderDatabaseIdPosition +
                                                                          4 * OdbType.Long.GetSize();

        /// <summary>
        ///   The number of classes in the meta model 1 long (4*8 bytes)
        /// </summary>
        public static readonly int DatabaseHeaderNumberOfClassesPosition = DatabaseHeaderLastTransactionId +
                                                                           2 * OdbType.Long.GetSize();

        /// <summary>
        ///   The first class OID : 1 Long (8 bytes)
        /// </summary>
        public static readonly int DatabaseHeaderFirstClassOid = DatabaseHeaderNumberOfClassesPosition +
                                                                 OdbType.Long.GetSize();

        /// <summary>
        ///   The last ODB close status.
        /// </summary>
        /// <remarks>
        ///   The last ODB close status. Used to detect if the transaction is ok : 1 byte
        /// </remarks>
        public static readonly int DatabaseHeaderLastCloseStatusPosition = DatabaseHeaderFirstClassOid +
                                                                           OdbType.Long.GetSize();

        /// <summary>
        ///   The Database character encoding : 50 bytes
        /// </summary>
        public static readonly int DatabaseHeaderDatabaseCharacterEncodingPosition =
            DatabaseHeaderLastCloseStatusPosition + OdbType.Byte.GetSize();

        /// <summary>
        ///   The position of the current id block: 1 long
        /// </summary>
        public static readonly int DatabaseHeaderCurrentIdBlockPosition =
            DatabaseHeaderDatabaseCharacterEncodingPosition + 58 * OdbType.Byte.GetSize();

        /// <summary>
        ///   First ID Block position
        /// </summary>
        public static readonly int DatabaseHeaderFirstIdBlockPosition = DatabaseHeaderCurrentIdBlockPosition +
                                                                        OdbType.Long.GetSize();

        public static readonly int DatabaseHeaderProtectedZoneSize = DatabaseHeaderCurrentIdBlockPosition;

        public static readonly int[] DatabaseHeaderPositions = new[]
            {
                DatabaseHeaderVersionPosition, DatabaseHeaderDatabaseIdPosition, DatabaseHeaderLastTransactionId,
                DatabaseHeaderNumberOfClassesPosition, DatabaseHeaderFirstClassOid,
                DatabaseHeaderLastCloseStatusPosition, DatabaseHeaderDatabaseCharacterEncodingPosition
            };

        public static readonly long ClassOffsetBlockType = ClassOffsetBlockSize + OdbType.Integer.GetSize();

        public static readonly long ClassOffsetCategory = ClassOffsetBlockType + OdbType.Byte.GetSize();

        public static readonly long ClassOffsetId = ClassOffsetCategory + OdbType.Byte.GetSize();

        public static readonly long ClassOffsetPreviousClassPosition = ClassOffsetId + OdbType.Long.GetSize();

        public static readonly long ClassOffsetNextClassPosition = ClassOffsetPreviousClassPosition +
                                                                   OdbType.Long.GetSize();

        public static readonly long ClassOffsetClassNbObjects = ClassOffsetNextClassPosition + OdbType.Long.GetSize();

        public static readonly long ClassOffsetClassFirstObjectPosition = ClassOffsetClassNbObjects +
                                                                          OdbType.Long.GetSize();

        public static readonly long ClassOffsetClassLastObjectPosition = ClassOffsetClassFirstObjectPosition +
                                                                         OdbType.Long.GetSize();

        public static readonly long ClassOffsetFullClassNameSize = ClassOffsetNextClassPosition + OdbType.Long.GetSize();

        public static readonly long ObjectOffsetBlockType = ObjectOffsetBlockSize + OdbType.Integer.GetSize();

        public static readonly long ObjectOffsetObjectId = ObjectOffsetBlockType + OdbType.Byte.GetSize();

        public static readonly long ObjectOffsetClassInfoId = ObjectOffsetObjectId + OdbType.Long.GetSize();

        public static readonly long ObjectOffsetPreviousObjectOid = ObjectOffsetClassInfoId + OdbType.Long.GetSize();

        public static readonly long ObjectOffsetNextObjectOid = ObjectOffsetPreviousObjectOid + OdbType.Long.GetSize();

        public static readonly long ObjectOffsetCreationDate = ObjectOffsetNextObjectOid + OdbType.Long.GetSize();

        public static readonly long ObjectOffsetUpdateDate = ObjectOffsetCreationDate + OdbType.Long.GetSize();

        public static readonly long ObjectOffsetVersion = ObjectOffsetUpdateDate + OdbType.Long.GetSize();

        public static readonly long ObjectOffsetReferencePointer = ObjectOffsetVersion + OdbType.Integer.GetSize();

        public static readonly long ObjectOffsetIsExternallySynchronized = ObjectOffsetReferencePointer +
                                                                           OdbType.Long.GetSize();

        public static readonly long ObjectOffsetNbAttributes = ObjectOffsetIsExternallySynchronized +
                                                               OdbType.Boolean.GetSize();

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
        public static readonly long BlockIdOffsetForBlockStatus = OdbType.Integer.GetSize() + OdbType.Byte.GetSize();

        public static readonly long BlockIdOffsetForPrevBlock = BlockIdOffsetForBlockStatus + OdbType.Byte.GetSize();

        public static readonly long BlockIdOffsetForNextBlock = BlockIdOffsetForPrevBlock + OdbType.Long.GetSize();

        public static readonly long BlockIdOffsetForBlockNumber = BlockIdOffsetForNextBlock + OdbType.Long.GetSize();

        public static readonly long BlockIdOffsetForMaxId = BlockIdOffsetForBlockNumber + OdbType.Integer.GetSize();

        public static readonly long BlockIdOffsetForStartOfRepetition = BlockIdOffsetForMaxId + OdbType.Long.GetSize();

        public static readonly long BlockIdRepetitionId = BlockIdRepetitionIdType + OdbType.Byte.GetSize();

        public static readonly long BlockIdRepetitionIdStatus = BlockIdRepetitionId + OdbType.Long.GetSize();

        public static readonly long BlockIdRepetitionObjectPosition = BlockIdRepetitionIdStatus + OdbType.Byte.GetSize();

        public static readonly long NativeObjectOffsetBlockType = NativeObjectOffsetBlockSize +
                                                                  OdbType.Integer.GetSize();

        public static readonly long NativeObjectOffsetOdbTypeId = NativeObjectOffsetBlockType + OdbType.Byte.GetSize();

        public static readonly long NativeObjectOffsetObjectIsNull = NativeObjectOffsetOdbTypeId +
                                                                     OdbType.Integer.GetSize();

        public static readonly long NativeObjectOffsetDataArea = NativeObjectOffsetObjectIsNull +
                                                                 OdbType.Boolean.GetSize();

        public static readonly string NoEncoding = "no-encoding";
        // TODO Something is wrong here : two constant with the same value!!*/
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
