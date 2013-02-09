using System;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer3.IO;

namespace NDatabase.Odb.Core.Layers.Layer3
{
    internal interface IFileSystemInterface : IDisposable
    {
        void Flush();

        long GetPosition();

        long GetLength();

        /// <summary>
        ///   Does the same thing than setWritePosition, but do not control write position
        /// </summary>
        /// <param name="position"> </param>
        /// <param name="writeInTransacation"> </param>
        void SetWritePositionNoVerification(long position, bool writeInTransacation);

        void SetWritePosition(long position, bool writeInTransacation);

        void SetReadPosition(long position);

        long GetAvailablePosition();

        void EnsureSpaceFor(OdbType type);

        void WriteByte(byte i, bool writeInTransaction);

        void WriteByte(byte i, bool writeInTransaction, string label);

        byte ReadByte();

        byte ReadByte(string label);

        void WriteBytes(byte[] bytes, bool writeInTransaction, string label);

        byte[] ReadBytes(int length);

        void WriteChar(char c, bool writeInTransaction);

        char ReadChar();

        char ReadChar(string label);

        void WriteShort(short s, bool writeInTransaction);

        short ReadShort();

        short ReadShort(string label);

        void WriteInt(int i, bool writeInTransaction, string label);

        int ReadInt();

        int ReadInt(string label);

        void WriteLong(long i, bool writeInTransaction, string label);

        long ReadLong();

        long ReadLong(string label);

        void WriteFloat(float f, bool writeInTransaction);

        float ReadFloat();

        float ReadFloat(string label);

        void WriteDouble(double d, bool writeInTransaction);

        double ReadDouble(string label);

        void WriteBigDecimal(Decimal d, bool writeInTransaction);

        Decimal ReadBigDecimal();

        Decimal ReadBigDecimal(string label);

        void WriteDate(DateTime d, bool writeInTransaction);

        DateTime ReadDate(string label);

        void WriteString(string s, bool writeInTransaction);

        void WriteString(string s, bool writeInTransaction, int totalSpace);

        string ReadString();

        string ReadString(string label);

        void WriteBoolean(bool b, bool writeInTransaction);

        void WriteBoolean(bool b, bool writeInTransaction, string label);

        bool ReadBoolean();

        bool ReadBoolean(string label);

        void Close();

        /// <returns> Returns the parameters. </returns>
        IDbIdentification GetFileIdentification();

        IMultiBufferedFileIO GetIo();

        void WriteUShort(ushort s, bool writeInTransaction);
        
        ushort ReadUShort(string label);
        
        void WriteUInt(uint i, bool writeInTransaction, string label);
        
        uint ReadUInt(string label);
        
        void WriteULong(ulong i, bool writeInTransaction, string label);
        
        ulong ReadULong(string label);
        
        void WriteSByte(sbyte i, bool writeInTransaction);
        
        void WriteSByte(sbyte i, bool writeInTransaction, string label);
        
        sbyte ReadSByte(string label);
    }
}
