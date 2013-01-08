using System;
using NDatabase.Exceptions;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer3.IO;
using NDatabase.Odb.Core.Transaction;
using NDatabase.Tool;

namespace NDatabase.Odb.Core.Layers.Layer3.Engine
{
    /// <summary>
    ///   Class that knows how to read/write all language native types : byte, char, String, int, long,....
    /// </summary>
    internal sealed class FileSystemInterface : IFileSystemInterface
    {
        private const byte ReservedSpace = 128;

        private static readonly int IntSizeX2 = OdbType.Integer.Size * 2;

        private readonly IDbIdentification _fileIdentification;
        private readonly ISession _session;

        private IMultiBufferedFileIO _io;

        public FileSystemInterface(IDbIdentification fileIdentification, int bufferSize, ISession session)
        {
            fileIdentification.EnsureDirectories();
            
            _fileIdentification = fileIdentification;
            _io = fileIdentification.GetIO(bufferSize);

            _session = session;
        }

        #region IFileSystemInterface Members

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
            return _io.CurrentPosition;
        }

        public long GetLength()
        {
            return _io.Length;
        }

        public void SetWritePositionNoVerification(long position, bool writeInTransacation)
        {
            _io.SetCurrentWritePosition(position);

            if (writeInTransacation)
                _session.GetTransaction().SetWritePosition(position);
        }

        public void SetWritePosition(long position, bool writeInTransacation)
        {
            if (position < StorageEngineConstant.DatabaseHeaderProtectedZoneSize)
            {
                if (IsWritingInWrongPlace(position))
                {
                    throw new OdbRuntimeException(
                        NDatabaseError.InternalError.AddParameter(
                            string.Concat("Trying to write in Protected area at position ", position.ToString())));
                }
            }

            _io.SetCurrentWritePosition(position);
            if (writeInTransacation)
                _session.GetTransaction().SetWritePosition(position);
        }

        public void SetReadPosition(long position)
        {
            _io.SetCurrentReadPosition(position);
        }

        public long GetAvailablePosition()
        {
            return _io.Length;
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

            if (OdbConfiguration.IsLoggingEnabled())
            {
                var byteAsString = i.ToString();
                var position = GetPosition().ToString();
                var message = string.Format("writing byte {0} at {1}{2}", byteAsString, position, (label != null
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
                _session.GetTransaction().ManageWriteAction(_io.CurrentPosition, bytes);
                EnsureSpaceFor(OdbType.Byte);
            }
        }

        public byte ReadByte()
        {
            return ReadByte(null);
        }

        public byte ReadByte(string label)
        {
            var currentPosition = _io.CurrentPosition;
            var i = _io.ReadByte();

            if (OdbConfiguration.IsLoggingEnabled())
            {
                var byteAsString = i.ToString();
                var positionAsString = currentPosition.ToString();
                var message = string.Format("reading byte {0} at {1}{2}", byteAsString, positionAsString, (label != null
                                                                                                   ? string.Format(" : {0}", label)
                                                                                                   : string.Empty));
                DLogger.Debug(message);
            }

            return i;
        }

        public void WriteSByte(sbyte i, bool writeInTransaction)
        {
            WriteSByte(i, writeInTransaction, null);
        }

        public void WriteSByte(sbyte i, bool writeInTransaction, string label)
        {
            var asByte = unchecked ((byte) i);
            var bytes = new[] { asByte };

            if (OdbConfiguration.IsLoggingEnabled())
            {
                var byteAsString = i.ToString();
                var position = GetPosition().ToString();
                var message = string.Format("writing sbyte {0} at {1}{2}", byteAsString, position, (label != null
                                                                                                 ? string.Format(" : {0}", label)
                                                                                                 : string.Empty));
                DLogger.Debug(message);
            }

            if (!writeInTransaction)
            {
                _io.WriteByte(asByte);
            }
            else
            {
                _session.GetTransaction().ManageWriteAction(_io.CurrentPosition, bytes);
                EnsureSpaceFor(OdbType.SByte);
            }
        }

        public sbyte ReadSByte(string label)
        {
            var currentPosition = _io.CurrentPosition;
            var i = _io.ReadByte();

            var asSByte = unchecked((sbyte)i);

            if (OdbConfiguration.IsLoggingEnabled())
            {
                var byteAsString = asSByte.ToString();
                var positionAsString = currentPosition.ToString();
                var message = string.Format("reading sbyte {0} at {1}{2}", byteAsString, positionAsString, (label != null
                                                                                                   ? string.Format(" : {0}", label)
                                                                                                   : string.Empty));
                DLogger.Debug(message);
            }

            return asSByte;
        }

        public void WriteBytes(byte[] bytes, bool writeInTransaction, string label)
        {
            if (OdbConfiguration.IsLoggingEnabled())
            {
                var length = bytes.Length.ToString();
                var position = GetPosition().ToString();
                var message = string.Format("writing {0} bytes at {1}{2} = {3}", length, position,
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
                _session.GetTransaction().ManageWriteAction(_io.CurrentPosition, bytes);
                EnsureSpaceFor(bytes.Length, OdbType.Byte);
            }
        }

        public byte[] ReadBytes(int length)
        {
            var currentPosition = _io.CurrentPosition;
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
            WriteValue(c, writeInTransaction, ByteArrayConverter.CharToByteArray, OdbType.Character);
        }

        private byte[] ReadCharBytes()
        {
            return _io.ReadBytes(OdbType.Character.Size);
        }

        public char ReadChar()
        {
            return ReadChar(null);
        }

        public char ReadChar(string label)
        {
            var currentPosition = _io.CurrentPosition;
            var toChar = ByteArrayConverter.ByteArrayToChar(ReadCharBytes());

            if (OdbConfiguration.IsLoggingEnabled() && label != null)
            {
                var charAsString = toChar.ToString();
                var positionAsString = currentPosition.ToString();

                DLogger.Debug(string.Format("reading char {0} at {1} : {2}", charAsString, positionAsString, label));
            }

            return toChar;
        }

        public void WriteShort(short s, bool writeInTransaction)
        {
            if (OdbConfiguration.IsLoggingEnabled())
                DLogger.Debug(string.Concat("writing short ", s.ToString(), " at ", GetPosition().ToString()));

            WriteValue(s, writeInTransaction, ByteArrayConverter.ShortToByteArray, OdbType.Short);
        }

        private byte[] ReadShortBytes()
        {
            return _io.ReadBytes(OdbType.Short.Size);
        }

        public short ReadShort()
        {
            return ReadShort(null);
        }

        public short ReadShort(string label)
        {
            var position = _io.CurrentPosition;
            var toShort = ByteArrayConverter.ByteArrayToShort(ReadShortBytes());

            if (OdbConfiguration.IsLoggingEnabled() && label != null)
            {
                var shortAsString = toShort.ToString();
                var positionAsString = position.ToString();
                DLogger.Debug(string.Format("reading short {0} at {1} : {2}", shortAsString, positionAsString, label));
            }

            return toShort;
        }

        public void WriteUShort(ushort s, bool writeInTransaction)
        {
            if (OdbConfiguration.IsLoggingEnabled())
                DLogger.Debug(string.Concat("writing ushort ", s.ToString(), " at ", GetPosition().ToString()));

            WriteValue(s, writeInTransaction, ByteArrayConverter.UShortToByteArray, OdbType.UShort);
        }

        private byte[] ReadUShortBytes()
        {
            return _io.ReadBytes(OdbType.UShort.Size);
        }

        public ushort ReadUShort(string label)
        {
            var position = _io.CurrentPosition;
            var toShort = ByteArrayConverter.ByteArrayToUShort(ReadUShortBytes());

            if (OdbConfiguration.IsLoggingEnabled() && label != null)
            {
                var shortAsString = toShort.ToString();
                var positionAsString = position.ToString();
                DLogger.Debug(string.Format("reading ushort {0} at {1} : {2}", shortAsString, positionAsString, label));
            }

            return toShort;
        }

        public void WriteInt(int i, bool writeInTransaction, string label)
        {
            if (OdbConfiguration.IsLoggingEnabled())
            {
                var intAsString = i.ToString();
                var positionAsString = GetPosition().ToString();
                DLogger.Debug(string.Format("writing int {0} at {1} : {2}", intAsString, positionAsString, label));
            }

            WriteValue(i, writeInTransaction, ByteArrayConverter.IntToByteArray, OdbType.Integer);
        }

        private byte[] ReadIntBytes()
        {
            return _io.ReadBytes(OdbType.Integer.Size);
        }

        public int ReadInt()
        {
            return ReadInt(null);
        }

        public int ReadInt(string label)
        {
            var currentPosition = _io.CurrentPosition;
            var i = ByteArrayConverter.ByteArrayToInt(ReadIntBytes());

            if (OdbConfiguration.IsLoggingEnabled())
            {
                var intAsString = i.ToString();
                var positionAsString = currentPosition.ToString();

                var message = string.Format("reading int {0} at {1}{2}", intAsString, positionAsString, (label != null
                                                                                                  ? string.Format(" : {0}", label)
                                                                                                  : string.Empty));
                DLogger.Debug(message);
            }

            return i;
        }

        public void WriteUInt(uint i, bool writeInTransaction, string label)
        {
            if (OdbConfiguration.IsLoggingEnabled())
            {
                var intAsString = i.ToString();
                var positionAsString = GetPosition().ToString();
                DLogger.Debug(string.Format("writing int {0} at {1} : {2}", intAsString, positionAsString, label));
            }

            WriteValue(i, writeInTransaction, ByteArrayConverter.UIntToByteArray, OdbType.UInteger);
        }

        private byte[] ReadUIntBytes()
        {
            return _io.ReadBytes(OdbType.UInteger.Size);
        }

        public uint ReadUInt(string label)
        {
            var currentPosition = _io.CurrentPosition;
            var i = ByteArrayConverter.ByteArrayToUInt(ReadUIntBytes());

            if (OdbConfiguration.IsLoggingEnabled())
            {
                var intAsString = i.ToString();
                var positionAsString = currentPosition.ToString();

                var message = string.Format("reading uint {0} at {1}{2}", intAsString, positionAsString, (label != null
                                                                                                  ? string.Format(" : {0}", label)
                                                                                                  : string.Empty));
                DLogger.Debug(message);
            }

            return i;
        }

        public void WriteLong(long i, bool writeInTransaction, string label)
        {
            if (OdbConfiguration.IsLoggingEnabled() && label != null)
            {
                var longAsString = i.ToString();
                var positionAsString = GetPosition().ToString();
                DLogger.Debug(string.Format("writing long {0} at {1} : {2}", longAsString, positionAsString, label));
            }

            WriteValue(i, writeInTransaction, ByteArrayConverter.LongToByteArray, OdbType.Long);
        }

        private byte[] ReadLongBytes()
        {
            return _io.ReadBytes(OdbType.Long.Size);
        }

        public long ReadLong()
        {
            return ReadLong(null);
        }

        public long ReadLong(string label)
        {
            var position = _io.CurrentPosition;
            var toLong = ByteArrayConverter.ByteArrayToLong(ReadLongBytes());

            if (OdbConfiguration.IsLoggingEnabled())
            {
                var longAsString = toLong.ToString();
                var positionAsString = position.ToString();
                var message = string.Format("reading long {0} at {1}{2}", longAsString, positionAsString, (label != null
                                                                                                 ? string.Format(" : {0}", label)
                                                                                                 : string.Empty));
                DLogger.Debug(message);
            }

            return toLong;
        }

        public void WriteULong(ulong i, bool writeInTransaction, string label)
        {
            if (OdbConfiguration.IsLoggingEnabled() && label != null)
            {
                var longAsString = i.ToString();
                var positionAsString = GetPosition().ToString();
                DLogger.Debug(string.Format("writing ulong {0} at {1} : {2}", longAsString, positionAsString, label));
            }

            WriteValue(i, writeInTransaction, ByteArrayConverter.ULongToByteArray, OdbType.ULong);
        }

        private byte[] ReadULongBytes()
        {
            return _io.ReadBytes(OdbType.ULong.Size);
        }

        public ulong ReadULong(string label)
        {
            var position = _io.CurrentPosition;
            var toLong = ByteArrayConverter.ByteArrayToULong(ReadULongBytes());

            if (OdbConfiguration.IsLoggingEnabled())
            {
                var longAsString = toLong.ToString();
                var positionAsString = position.ToString();
                var message = string.Format("reading ulong {0} at {1}{2}", longAsString, positionAsString, (label != null
                                                                                                 ? string.Format(" : {0}", label)
                                                                                                 : string.Empty));
                DLogger.Debug(message);
            }

            return toLong;
        }

        public void WriteFloat(float f, bool writeInTransaction)
        {
            WriteValue(f, writeInTransaction, ByteArrayConverter.FloatToByteArray, OdbType.Float);
        }

        private byte[] ReadFloatBytes()
        {
            return _io.ReadBytes(OdbType.Float.Size);
        }

        public float ReadFloat()
        {
            return ReadFloat(null);
        }

        public float ReadFloat(string label)
        {
            var currentPosition = _io.CurrentPosition;
            var toFloat = ByteArrayConverter.ByteArrayToFloat(ReadFloatBytes());

            if (OdbConfiguration.IsLoggingEnabled())
            {
                var floatAsString = toFloat.ToString();
                var positionAsString = currentPosition.ToString();

                var message = string.Format("Reading float '{0}' at {1}{2}", floatAsString, positionAsString,
                                            (label != null
                                                 ? string.Format(" : {0}", label)
                                                 : string.Empty));
                DLogger.Debug(message);
            }

            return toFloat;
        }

        public void WriteDouble(double d, bool writeInTransaction)
        {
            WriteValue(d, writeInTransaction, ByteArrayConverter.DoubleToByteArray, OdbType.Double);
        }

        private byte[] ReadDoubleBytes()
        {
            return _io.ReadBytes(OdbType.Double.Size);
        }

        public double ReadDouble(string label)
        {
            var currentPosition = _io.CurrentPosition;
            var toDouble = ByteArrayConverter.ByteArrayToDouble(ReadDoubleBytes());

            if (OdbConfiguration.IsLoggingEnabled())
            {
                var doubleAsString = toDouble.ToString();
                var positionAsString = currentPosition.ToString();

                var message = string.Format("Reading double '{0}' at {1}{2}", doubleAsString, positionAsString,
                                            (label != null
                                                 ? string.Format(" : {0}", label)
                                                 : string.Empty));
                DLogger.Debug(message);
            }

            return toDouble;
        }

        public void WriteBigDecimal(Decimal d, bool writeInTransaction)
        {
            var bytes = ByteArrayConverter.DecimalToByteArray(d);

            if (OdbConfiguration.IsLoggingEnabled())
                DLogger.Debug(string.Concat("writing Decimal ", d.ToString(), " at ", GetPosition().ToString()));

            if (!writeInTransaction)
                _io.WriteBytes(bytes);
            else
            {
                _session.GetTransaction().ManageWriteAction(_io.CurrentPosition, bytes);
                EnsureSpaceFor(bytes.Length, OdbType.Decimal);
            }
        }

        private byte[] ReadBigDecimalBytes()
        {
            return _io.ReadBytes(OdbType.Decimal.Size);
        }

        public Decimal ReadBigDecimal()
        {
            return ReadBigDecimal(null);
        }

        public Decimal ReadBigDecimal(string label)
        {
            var currentPosition = _io.CurrentPosition;

            var toBigDecimal = ByteArrayConverter.ByteArrayToDecimal(ReadBigDecimalBytes());

            if (OdbConfiguration.IsLoggingEnabled())
            {
                var bigDecimalAsString = toBigDecimal.ToString();
                var positionAsString = currentPosition.ToString();

                var message = string.Format("Reading bigDecimal '{0}' at {1}{2}", bigDecimalAsString, positionAsString,
                                            (label != null
                                                 ? string.Format(" : {0}", label)
                                                 : string.Empty));
                DLogger.Debug(message);
            }

            return toBigDecimal;
        }

        public void WriteDate(DateTime d, bool writeInTransaction)
        {
            if (OdbConfiguration.IsLoggingEnabled())
                DLogger.Debug(string.Concat("writing Date ", d.Ticks.ToString(), " at ", GetPosition().ToString()));

            WriteValue(d, writeInTransaction, ByteArrayConverter.DateToByteArray, OdbType.Date);
        }

        private byte[] ReadDateBytes()
        {
            return _io.ReadBytes(OdbType.Date.Size);
        }

        public DateTime ReadDate(string label)
        {
            var currentPosition = _io.CurrentPosition;
            var date = ByteArrayConverter.ByteArrayToDate(ReadDateBytes());

            if (OdbConfiguration.IsLoggingEnabled())
            {
                var positionAsString = currentPosition.ToString();
                var dateAsString = date.ToString();
                var message = string.Format("Reading date '{0}' at {1}{2}", dateAsString, positionAsString,
                                            (label != null
                                                 ? string.Format(" : {0}", label)
                                                 : string.Empty));
                DLogger.Debug(message);
            }

            return date;
        }

        public void WriteString(string s, bool writeInTransaction)
        {
            WriteString(s, writeInTransaction, -1);
        }

        public void WriteString(string s, bool writeInTransaction, int totalSpace)
        {
            var bytes = ByteArrayConverter.StringToByteArray(s, totalSpace);
            
            if (OdbConfiguration.IsLoggingEnabled())
            {
                var position = GetPosition().ToString();
                var length = bytes.Length.ToString();
                DLogger.Debug(string.Format("Writing string '{0}' at {1} size={2} bytes", s, position, length));
            }

            if (!writeInTransaction)
            {
                _io.WriteBytes(bytes);
            }
            else
            {
                _session.GetTransaction().ManageWriteAction(_io.CurrentPosition, bytes);
                EnsureSpaceFor(bytes.Length, OdbType.String);
            }
        }

        private byte[] ReadStringBytes()
        {
            var sizeBytes = _io.ReadBytes(IntSizeX2);
            var totalSize = ByteArrayConverter.ByteArrayToInt(sizeBytes);

            // Use offset of int size to read real size
            var stringSize = ByteArrayConverter.ByteArrayToInt(sizeBytes, OdbType.Integer.Size);
            var bytes = ReadBytes(stringSize);

            // Reads extra bytes
            ReadBytes(totalSize - stringSize);

            var bytes2 = new byte[stringSize + IntSizeX2];

            for (var i = 0; i < IntSizeX2; i++)
                bytes2[i] = sizeBytes[i];

            for (var i = 0; i < bytes.Length; i++)
                bytes2[i + 8] = bytes[i];

            return bytes2;
        }

        public string ReadString()
        {
            return ReadString(null);
        }

        public string ReadString(string label)
        {
            var toString = ByteArrayConverter.ByteArrayToString(ReadStringBytes());

            if (OdbConfiguration.IsLoggingEnabled())
            {
                var startPosition = _io.CurrentPosition.ToString();
                var format = "Reading string '{0}' at " + startPosition + "{1}";
                var message = string.Format(format, toString, (label != null
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
            if (OdbConfiguration.IsLoggingEnabled() && label != null)
            {
                var booleanAsString = b.ToString();
                var positionAsString = GetPosition().ToString();
                DLogger.Debug("writing boolean " + booleanAsString + " at " + positionAsString + " : " + label);
            }

            WriteValue(b, writeInTransaction, ByteArrayConverter.BooleanToByteArray, OdbType.Boolean);
        }

        private byte[] ReadBooleanBytes()
        {
            return _io.ReadBytes(OdbType.Boolean.Size);
        }

        public bool ReadBoolean()
        {
            return ReadBoolean(null);
        }

        public bool ReadBoolean(string label)
        {
            var currentPosition = _io.CurrentPosition;
            var toBoolean = ByteArrayConverter.ByteArrayToBoolean(ReadBooleanBytes());

            if (OdbConfiguration.IsLoggingEnabled() && label != null)
            {
                var booleanAsString = toBoolean.ToString();
                var positionAsString = currentPosition.ToString();
                DLogger.Debug("reading boolean " + booleanAsString + " at " + positionAsString + " : " + label);
            }

            return toBoolean;
        }

        public void Close()
        {
            _io.Close();
            _io = null;
        }

        public IDbIdentification GetFileIdentification()
        {
            return _fileIdentification;
        }

        public IMultiBufferedFileIO GetIo()
        {
            return _io;
        }

        #endregion

        /// <summary>
        ///   Writing at position &lt; DATABASE_HEADER_PROTECTED_ZONE_SIZE is writing in ODB Header place.
        /// </summary>
        /// <remarks>
        ///   Writing at position &lt; DATABASE_HEADER_PROTECTED_ZONE_SIZE is writing in ODB Header place. 
        ///   Here we check the positions where the writing is done. 
        ///   Search for 'page format' in ODB wiki to understand the positions
        /// </remarks>
        /// <param name="position"> </param>
        /// <returns> </returns>
        private static bool IsWritingInWrongPlace(long position)
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
            return _io.CurrentPosition == _io.Length;
        }

        /// <summary>
        ///   Reserve space in the file when it is at the end of the file Used in transaction mode where real write will happen later
        /// </summary>
        /// <param name="quantity"> The number of object to reserve space for </param>
        /// <param name="type"> The type of the object to reserve space for </param>
        private void EnsureSpaceFor(long quantity, OdbType type)
        {
            var space = type.Size * quantity;

            // We are in transaction mode - do not write just reserve space if
            // necessary
            // ensure space will be available when applying transaction
            if (PointerAtTheEndOfTheFile())
            {
                if (space != 1)
                    _io.SetCurrentWritePosition(_io.CurrentPosition + space - 1);

                _io.WriteByte(ReservedSpace);
            }
            else
            {
                // DLogger.debug("Reserving " + space + " bytes (" + quantity +
                // " " + type.getName() + ")");
                // We must simulate the move
                _io.SetCurrentWritePosition(_io.CurrentPosition + space);
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
                _session.GetTransaction().ManageWriteAction(_io.CurrentPosition, bytes);
                EnsureSpaceFor(odbType);
            }
        }

        public void Dispose()
        {
            Close();
        }
    }
}
