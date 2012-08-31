using System;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer3.IO;
using NDatabase.Odb.Core.Transaction;
using NDatabase.Odb.Impl.Core.Layers.Layer3.Buffer;
using NDatabase.Odb.Impl.Core.Layers.Layer3.Engine;
using NDatabase.Tool;

namespace NDatabase.Odb.Core.Layers.Layer3.Engine
{
    /// <summary>
    ///   Class that knows how to read/write all language native types : byte, char, String, int, long,....
    /// </summary>
    public sealed class FileSystemInterface : IFileSystemInterface
    {
        //TODO: consider usage of BinaryReader and BinaryWriter

        private const byte ReservedSpace = 128;

        private static readonly int IntSize = OdbType.Integer.GetSize();

        private static readonly int IntSizeX2 = IntSize * 2;

        public static readonly string LogId = "FileSystemInterface";

        private IByteArrayConverter _byteArrayConverter;
        
        private readonly IFileIdentification _fileIdentification;
        private readonly ISession _session;

        private IBufferedIO _io;

        public FileSystemInterface(string name, IFileIdentification fileIdentification, int bufferSize, ISession session)
        {
            _fileIdentification = fileIdentification;

            OdbDirectory.Mkdirs(fileIdentification.FileName);
            _io = new MultiBufferedFileIO(name, fileIdentification.FileName, bufferSize);

            _byteArrayConverter = OdbConfiguration.GetCoreProvider().GetByteArrayConverter();
            _session = session;
        }

        public static int NbCall1 { get; private set; }
        public static int NbCall2 { get; private set; }

        #region IFileSystemInterface Members

        public void SetDatabaseCharacterEncoding(string databaseCharacterEncoding)
        {
            _byteArrayConverter.SetDatabaseCharacterEncoding(databaseCharacterEncoding);
        }

        public void SetIo(IBufferedIO io)
        {
            _io = io;
        }

        public void SetByteArrayConverter(IByteArrayConverter byteArrayConverter)
        {
            _byteArrayConverter = byteArrayConverter;
        }

        public void UseBuffer(bool useBuffer)
        {
            _io.SetUseBuffer(useBuffer);
        }

        public void Flush()
        {
            _io.FlushAll();
        }

        public long GetPosition()
        {
            return _io.GetCurrentPosition();
        }

        public long GetLength()
        {
            return _io.GetLength();
        }

        public void SetWritePositionNoVerification(long position, bool writeInTransacation)
        {
            _io.SetCurrentWritePosition(position);

            if (writeInTransacation)
                GetSession().GetTransaction().SetWritePosition(position);
        }

        public void SetWritePosition(long position, bool writeInTransacation)
        {
            if (position < StorageEngineConstant.DatabaseHeaderProtectedZoneSize)
            {
                if (IsWritingInWrongPlace(position))
                {
                    throw new OdbRuntimeException(
                        NDatabaseError.InternalError.AddParameter(
                            string.Format("Trying to write in Protected area at position {0}", position)));
                }
            }

            _io.SetCurrentWritePosition(position);
            if (writeInTransacation)
                GetSession().GetTransaction().SetWritePosition(position);
        }

        public void SetReadPosition(long position)
        {
            _io.SetCurrentReadPosition(position);
        }

        public long GetAvailablePosition()
        {
            return _io.GetLength();
        }

        public void EnsureSpaceFor(OdbType type)
        {
            EnsureSpaceFor(1, type);
        }

        public void WriteByte(byte i, bool writeInTransaction)
        {
            WriteByte(i, writeInTransaction, null);
        }

        public void WriteByte(byte i, bool writeInTransaction, string label)
        {
            var bytes = new[] {i};

            if (OdbConfiguration.IsDebugEnabled(LogId))
            {
                var message = string.Format("writing byte {0} at {1}{2}", i, GetPosition(), (label != null
                                                                                                 ? string.Format(" : {0}", label)
                                                                                                 : string.Empty));
                DLogger.Debug(message);
            }

            if (!writeInTransaction)
            {
                _io.WriteByte(i);
            }
            else
            {
                GetSession().GetTransaction().ManageWriteAction(_io.GetCurrentPosition(), bytes);
                EnsureSpaceFor(OdbType.Byte);
            }
        }

        public byte ReadByte()
        {
            return ReadByte(null);
        }

        public byte ReadByte(string label)
        {
            var currentPosition = _io.GetCurrentPosition();
            var i = _io.ReadByte();

            if (OdbConfiguration.IsDebugEnabled(LogId))
            {
                var message = string.Format("reading byte {0} at {1}{2}", i, currentPosition, (label != null
                                                                                                   ? string.Format(" : {0}", label)
                                                                                                   : string.Empty));
                DLogger.Debug(message);
            }

            return i;
        }

        public void WriteBytes(byte[] bytes, bool writeInTransaction, string label)
        {
            if (OdbConfiguration.IsDebugEnabled(LogId))
            {
                var message = string.Format("writing {0} bytes at {1}{2} = {3}", bytes.Length, GetPosition(),
                                            (label != null
                                                 ? string.Format(" : {0}", label)
                                                 : string.Empty), DisplayUtility.ByteArrayToString(bytes));
                DLogger.Debug(message);
            }

            if (!writeInTransaction)
            {
                _io.WriteBytes(bytes);
            }
            else
            {
                GetSession().GetTransaction().ManageWriteAction(_io.GetCurrentPosition(), bytes);
                EnsureSpaceFor(bytes.Length, OdbType.Byte);
            }
        }

        public byte[] ReadBytes(int length)
        {
            var currentPosition = _io.GetCurrentPosition();
            var bytes = _io.ReadBytes(length);
            var byteCount = bytes.Length;

            if (byteCount != length)
            {
                throw new OdbRuntimeException(
                    NDatabaseError.FileInterfaceReadError.AddParameter(length).AddParameter(currentPosition).AddParameter(
                        byteCount));
            }

            return bytes;
        }

        public void WriteChar(char c, bool writeInTransaction)
        {
            WriteValue(c, writeInTransaction, _byteArrayConverter.CharToByteArray, OdbType.Character);
        }

        public byte[] ReadCharBytes()
        {
            return _io.ReadBytes(OdbType.Character.GetSize());
        }

        public char ReadChar()
        {
            return ReadChar(null);
        }

        public char ReadChar(string label)
        {
            var currentPosition = _io.GetCurrentPosition();
            var toChar = _byteArrayConverter.ByteArrayToChar(ReadCharBytes());

            if (OdbConfiguration.IsDebugEnabled(LogId) && label != null)
                DLogger.Debug(string.Format("reading char {0} at {1} : {2}", toChar, currentPosition, label));

            return toChar;
        }

        public void WriteShort(short s, bool writeInTransaction)
        {
            if (OdbConfiguration.IsDebugEnabled(LogId))
                DLogger.Debug(string.Format("writing short {0} at {1}", s, GetPosition()));

            WriteValue(s, writeInTransaction, _byteArrayConverter.ShortToByteArray, OdbType.Short);
        }

        public byte[] ReadShortBytes()
        {
            return _io.ReadBytes(OdbType.Short.GetSize());
        }

        public short ReadShort()
        {
            return ReadShort(null);
        }

        public short ReadShort(string label)
        {
            var position = _io.GetCurrentPosition();
            var toShort = _byteArrayConverter.ByteArrayToShort(ReadShortBytes());

            if (OdbConfiguration.IsDebugEnabled(LogId) && label != null)
                DLogger.Debug(string.Format("reading short {0} at {1} : {2}", toShort, position, label));

            return toShort;
        }

        public void WriteInt(int i, bool writeInTransaction, string label)
        {
            if (OdbConfiguration.IsDebugEnabled(LogId))
                DLogger.Debug(string.Format("writing int {0} at {1} : {2}", i, GetPosition(), label));

            WriteValue(i, writeInTransaction, _byteArrayConverter.IntToByteArray, OdbType.Integer);
        }

        public byte[] ReadIntBytes()
        {
            return _io.ReadBytes(OdbType.Integer.GetSize());
        }

        public int ReadInt()
        {
            return ReadInt(null);
        }

        public int ReadInt(string label)
        {
            var currentPosition = _io.GetCurrentPosition();
            var i = _byteArrayConverter.ByteArrayToInt(ReadIntBytes(), 0);

            if (OdbConfiguration.IsDebugEnabled(LogId))
            {
                var message = string.Format("reading int {0} at {1}{2}", i, currentPosition, (label != null
                                                                                                  ? string.Format(" : {0}", label)
                                                                                                  : string.Empty));
                DLogger.Debug(message);
            }

            return i;
        }

        public void WriteLong(long i, bool writeInTransaction, string label, int writeActionType)
        {
            if (OdbConfiguration.IsDebugEnabled(LogId) && label != null)
                DLogger.Debug(string.Format("writing long {0} at {1} : {2}", i, GetPosition(), label));

            WriteValue(i, writeInTransaction, _byteArrayConverter.LongToByteArray, OdbType.Long);
        }

        public byte[] ReadLongBytes()
        {
            return _io.ReadBytes(OdbType.Long.GetSize());
        }

        public long ReadLong()
        {
            return ReadLong(null);
        }

        public long ReadLong(string label)
        {
            var position = _io.GetCurrentPosition();
            var toLong = _byteArrayConverter.ByteArrayToLong(ReadLongBytes(), 0);

            if (OdbConfiguration.IsDebugEnabled(LogId))
            {
                var message = string.Format("reading long {0} at {1}{2}", toLong, position, (label != null
                                                                                                 ? string.Format(" : {0}", label)
                                                                                                 : string.Empty));
                DLogger.Debug(message);
            }

            return toLong;
        }

        public void WriteFloat(float f, bool writeInTransaction)
        {
            WriteValue(f, writeInTransaction, _byteArrayConverter.FloatToByteArray, OdbType.Float);
        }

        public byte[] ReadFloatBytes()
        {
            return _io.ReadBytes(OdbType.Float.GetSize());
        }

        public float ReadFloat()
        {
            return ReadFloat(null);
        }

        public float ReadFloat(string label)
        {
            var currentPosition = _io.GetCurrentPosition();
            var toFloat = _byteArrayConverter.ByteArrayToFloat(ReadFloatBytes());

            if (OdbConfiguration.IsDebugEnabled(LogId))
            {
                var message = string.Format("Reading float '{0}' at {1}{2}", toFloat, currentPosition, (label != null
                                                                                                            ? string.Format(" : {0}", label)
                                                                                                            : string.Empty));
                DLogger.Debug(message);
            }

            return toFloat;
        }

        public void WriteDouble(double d, bool writeInTransaction)
        {
            WriteValue(d, writeInTransaction, _byteArrayConverter.DoubleToByteArray, OdbType.Double);
        }

        public byte[] ReadDoubleBytes()
        {
            return _io.ReadBytes(OdbType.Double.GetSize());
        }

        public double ReadDouble()
        {
            return ReadDouble(null);
        }

        public double ReadDouble(string label)
        {
            var currentPosition = _io.GetCurrentPosition();
            var toDouble = _byteArrayConverter.ByteArrayToDouble(ReadDoubleBytes());

            if (OdbConfiguration.IsDebugEnabled(LogId))
            {
                var message = string.Format("Reading double '{0}' at {1}{2}", toDouble, currentPosition, (label != null
                                                                                                             ? string.Format(" : {0}", label)
                                                                                                             : string.Empty));
                DLogger.Debug(message);
            }

            return toDouble;
        }

        public void WriteBigDecimal(Decimal d, bool writeInTransaction)
        {
            var bytes = _byteArrayConverter.BigDecimalToByteArray(d, true);

            if (OdbConfiguration.IsDebugEnabled(LogId))
                DLogger.Debug(string.Format("writing BigDecimal {0} at {1}", d, GetPosition()));

            if (!writeInTransaction)
                _io.WriteBytes(bytes);
            else
            {
                GetSession().GetTransaction().ManageWriteAction(_io.GetCurrentPosition(), bytes);
                EnsureSpaceFor(bytes.Length, OdbType.BigDecimal);
            }
        }

        public byte[] ReadBigDecimalBytes()
        {
            return ReadStringBytes(false);
        }

        public Decimal ReadBigDecimal()
        {
            return ReadBigDecimal(null);
        }

        public Decimal ReadBigDecimal(string label)
        {
            var currentPosition = _io.GetCurrentPosition();

            var toBigDecimal = _byteArrayConverter.ByteArrayToBigDecimal(ReadBigDecimalBytes(), false);

            if (OdbConfiguration.IsDebugEnabled(LogId))
            {
                var message = string.Format("Reading bigDecimal '{0}' at {1}{2}", toBigDecimal, currentPosition,
                                            (label != null
                                                 ? string.Format(" : {0}", label)
                                                 : string.Empty));
                DLogger.Debug(message);
            }

            return toBigDecimal;
        }

        public byte[] ReadBigIntegerBytes(bool hasSize)
        {
            return ReadStringBytes(hasSize);
        }

        public void WriteDate(DateTime d, bool writeInTransaction)
        {
            if (OdbConfiguration.IsDebugEnabled(LogId))
                DLogger.Debug(string.Format("writing Date {0} at {1}", d.Ticks, GetPosition()));

            WriteValue(d, writeInTransaction, _byteArrayConverter.DateToByteArray, OdbType.Date);
        }

        public byte[] ReadDateBytes()
        {
            return _io.ReadBytes(OdbType.Date.GetSize());
        }

        public DateTime ReadDate()
        {
            return ReadDate(null);
        }

        public DateTime ReadDate(string label)
        {
            var currentPosition = _io.GetCurrentPosition();
            var date = _byteArrayConverter.ByteArrayToDate(ReadDateBytes());

            if (OdbConfiguration.IsDebugEnabled(LogId))
            {
                var message = string.Format("Reading date '{0}' at {1}{2}", date, currentPosition, (label != null
                                                                                                        ? string.Format(" : {0}", label)
                                                                                                        : string.Empty));
                DLogger.Debug(message);
            }

            return date;
        }

        public void WriteString(string s, bool writeInTransaction, bool useEncoding)
        {
            WriteString(s, writeInTransaction, useEncoding, -1);
        }

        public void WriteString(string s, bool writeInTransaction, bool useEncoding, int totalSpace)
        {
            var bytes = _byteArrayConverter.StringToByteArray(s, true, totalSpace, useEncoding);
            
            if (OdbConfiguration.IsDebugEnabled(LogId))
            {
                var position = GetPosition();
                DLogger.Debug(string.Format("Writing string '{0}' at {1} size={2} bytes", s, position, bytes.Length));
            }

            if (!writeInTransaction)
            {
                var startPosition = _io.GetCurrentPosition();
                _io.WriteBytes(bytes);
//                var endPosition = _io.GetCurrentPosition();

                if (OdbConfiguration.IsEnableAfterWriteChecking())
                {
                    // To check the write
                    _io.SetCurrentWritePosition(startPosition);
                    var asString = ReadString(useEncoding);
                    // DLogger.debug("s1 : " + s.length() + " = " + s + "\ts2 : " +
                    // s2.length() + " = " + s2);
                    // FIXME replace RuntimeException by a ODBRuntimeException with
                    // an Error constant
                    throw new Exception(
                        string.Format("error while writing string at {0} :  {1} / check after writing ={2}",
                                      startPosition, s, asString));
                }
            }
            else
            {
                GetSession().GetTransaction().ManageWriteAction(_io.GetCurrentPosition(), bytes);
                EnsureSpaceFor(bytes.Length, OdbType.String);
            }
        }

        public byte[] ReadStringBytes(bool withSize)
        {
            if (withSize)
            {
                var sizeBytes = _io.ReadBytes(IntSizeX2);
                var totalSize = _byteArrayConverter.ByteArrayToInt(sizeBytes, 0);

                // Use offset of int size to read real size
                var stringSize = _byteArrayConverter.ByteArrayToInt(sizeBytes, IntSize);
                var bytes = ReadBytes(stringSize);

                NbCall2++;
                // Reads extra bytes
                ReadBytes(totalSize - stringSize);

                var bytes2 = new byte[stringSize + IntSizeX2];

                for (var i = 0; i < IntSizeX2; i++)
                    bytes2[i] = sizeBytes[i];

                for (var i = 0; i < bytes.Length; i++)
                    bytes2[i + 8] = bytes[i];

                return bytes2;
            }

            var sizeBytesNoSize = _io.ReadBytes(IntSizeX2);
            var stringSizeNoSize = _byteArrayConverter.ByteArrayToInt(sizeBytesNoSize, IntSize);

            var bytesNoSize = ReadBytes(stringSizeNoSize);
            NbCall1++;

            return bytesNoSize;
        }

        public string ReadString(bool useEncoding)
        {
            return ReadString(useEncoding, OdbConfiguration.GetDatabaseCharacterEncoding());
        }

        public string ReadString(bool useEncoding, string label)
        {
            var toString = _byteArrayConverter.ByteArrayToString(ReadStringBytes(true), true, useEncoding);

            if (OdbConfiguration.IsDebugEnabled(LogId))
            {
                var startPosition = _io.GetCurrentPosition();
                var message = string.Format("Reading string '{0}' at {1}{2}", toString, startPosition, (label != null
                                                                                                            ? string.Format(" : {0}", label)
                                                                                                            : string.Empty));
                DLogger.Debug(message);
            }

            return toString;
        }

        public void WriteBoolean(bool b, bool writeInTransaction)
        {
            WriteBoolean(b, writeInTransaction, null);
        }

        public void WriteBoolean(bool b, bool writeInTransaction, string label)
        {
            if (OdbConfiguration.IsDebugEnabled(LogId) && label != null)
                DLogger.Debug(string.Format("writing boolean {0} at {1} : {2}", b, GetPosition(), label));

            WriteValue(b, writeInTransaction, _byteArrayConverter.BooleanToByteArray, OdbType.Boolean);
        }

        public byte[] ReadBooleanBytes()
        {
            return _io.ReadBytes(OdbType.Boolean.GetSize());
        }

        public bool ReadBoolean()
        {
            return ReadBoolean(null);
        }

        public bool ReadBoolean(string label)
        {
            var currentPosition = _io.GetCurrentPosition();
            var toBoolean = _byteArrayConverter.ByteArrayToBoolean(ReadBooleanBytes(), 0);

            if (OdbConfiguration.IsDebugEnabled(LogId) && label != null)
                DLogger.Debug(string.Format("reading boolean {0} at {1} : {2}", toBoolean, currentPosition, label));

            return toBoolean;
        }

        public byte[] ReadNativeAttributeBytes(int attributeType)
        {
            switch (attributeType)
            {
                case OdbType.ByteId:
                {
                    var bytes = new byte[1];
                    bytes[0] = ReadByte();
                    return bytes;
                }

                case OdbType.LongId:
                {
                    return ReadLongBytes();
                }

                case OdbType.ShortId:
                {
                    return ReadShortBytes();
                }

                case OdbType.BigDecimalId:
                {
                    return ReadBigDecimalBytes();
                }

                case OdbType.BooleanId:
                {
                    return ReadBooleanBytes();
                }

                case OdbType.CharacterId:
                {
                    return ReadCharBytes();
                }

                case OdbType.DateId:
                case OdbType.DateSqlId:
                case OdbType.DateTimestampId:
                {
                    return ReadDateBytes();
                }

                case OdbType.FloatId:
                {
                    return ReadFloatBytes();
                }

                case OdbType.DoubleId:
                {
                    return ReadDoubleBytes();
                }

                case OdbType.IntegerId:
                {
                    return ReadIntBytes();
                }

                case OdbType.StringId:
                {
                    return ReadStringBytes(true);
                }

                default:
                {
                    throw new OdbRuntimeException(
                        NDatabaseError.NativeTypeNotSupported.AddParameter(attributeType).AddParameter(string.Empty));
                }
            }
        }

        public void Close()
        {
            Clear();
            _io.Close();
            _io = null;
        }

        public void Clear()
        {
        }

        // Nothing to do

        public IFileIdentification GetFileIdentification()
        {
            return _fileIdentification;
        }

        public IBufferedIO GetIo()
        {
            return _io;
        }

        #endregion

        public ISession GetSession()
        {
            return _session;
        }

        /// <summary>
        ///   Writing at position &lt; DATABASE_HEADER_PROTECTED_ZONE_SIZE is writing in ODB Header place.
        /// </summary>
        /// <remarks>
        ///   Writing at position &lt; DATABASE_HEADER_PROTECTED_ZONE_SIZE is writing in ODB Header place. Here we check the positions where the writing is done. Search for 'page format' in ODB wiki to understand the positions
        /// </remarks>
        /// <param name="position"> </param>
        /// <returns> </returns>
        internal bool IsWritingInWrongPlace(long position)
        {
            if (position < StorageEngineConstant.DatabaseHeaderProtectedZoneSize)
            {
                var size = StorageEngineConstant.DatabaseHeaderPositions.Length;
                for (var i = 0; i < size; i++)
                {
                    if (position == StorageEngineConstant.DatabaseHeaderPositions[i])
                        return false;
                }
                return true;
            }
            return false;
        }

        private bool PointerAtTheEndOfTheFile()
        {
            return _io.GetCurrentPosition() == _io.GetLength();
        }

        /// <summary>
        ///   Reserve space in the file when it is at the end of the file Used in transaction mode where real write will happen later
        /// </summary>
        /// <param name="quantity"> The number of object to reserve space for </param>
        /// <param name="type"> The type of the object to reserve space for </param>
        private void EnsureSpaceFor(long quantity, OdbType type)
        {
            var space = type.GetSize() * quantity;

            // We are in transaction mode - do not write just reserve space if
            // necessary
            // ensure space will be available when applying transaction
            if (PointerAtTheEndOfTheFile())
            {
                if (space != 1)
                    _io.SetCurrentWritePosition(_io.GetCurrentPosition() + space - 1);

                _io.WriteByte(ReservedSpace);
            }
            else
            {
                // DLogger.debug("Reserving " + space + " bytes (" + quantity +
                // " " + type.getName() + ")");
                // We must simulate the move
                _io.SetCurrentWritePosition(_io.GetCurrentPosition() + space);
            }
        }

        private void WriteValue<TValue>(TValue value, bool writeInTransaction, Func<TValue, Byte[]> convert, OdbType odbType)
        {
            var bytes = convert(value);

            if (!writeInTransaction)
            {
                _io.WriteBytes(bytes);
            }
            else
            {
                GetSession().GetTransaction().ManageWriteAction(_io.GetCurrentPosition(), bytes);
                EnsureSpaceFor(odbType);
            }
        }
    }
}
