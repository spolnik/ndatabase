namespace NDatabase.Odb.Core.Layers.Layer2.Meta
{
    /// <summary>
    ///   A class that contain basic information about a native object
    /// </summary>
    public sealed class NativeAttributeHeader
    {
        private int _blockSize;
        private byte _blockType;
        private bool _isNative;
        private bool _isNull;
        private int _odbTypeId;

        public NativeAttributeHeader()
        {
        }

        public NativeAttributeHeader(int blockSize, byte blockType, bool isNative, int odbTypeId, bool isNull)
        {
            _blockSize = blockSize;
            _blockType = blockType;
            _isNative = isNative;
            _odbTypeId = odbTypeId;
            _isNull = isNull;
        }

        public int GetBlockSize()
        {
            return _blockSize;
        }

        public void SetBlockSize(int blockSize)
        {
            _blockSize = blockSize;
        }

        public byte GetBlockType()
        {
            return _blockType;
        }

        public void SetBlockType(byte blockType)
        {
            _blockType = blockType;
        }

        public bool IsNative()
        {
            return _isNative;
        }

        public void SetNative(bool isNative)
        {
            _isNative = isNative;
        }

        public bool IsNull()
        {
            return _isNull;
        }

        public void SetNull(bool isNull)
        {
            _isNull = isNull;
        }

        public int GetOdbTypeId()
        {
            return _odbTypeId;
        }

        public void SetOdbTypeId(int odbTypeId)
        {
            _odbTypeId = odbTypeId;
        }
    }
}
