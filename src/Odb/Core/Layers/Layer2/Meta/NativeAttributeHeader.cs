namespace NDatabase.Odb.Core.Layers.Layer2.Meta
{
    /// <summary>
    ///   A class that contain basic information about a native object
    /// </summary>
    /// <author>osmadja</author>
    public class NativeAttributeHeader
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

        public virtual int GetBlockSize()
        {
            return _blockSize;
        }

        public virtual void SetBlockSize(int blockSize)
        {
            _blockSize = blockSize;
        }

        public virtual byte GetBlockType()
        {
            return _blockType;
        }

        public virtual void SetBlockType(byte blockType)
        {
            _blockType = blockType;
        }

        public virtual bool IsNative()
        {
            return _isNative;
        }

        public virtual void SetNative(bool isNative)
        {
            _isNative = isNative;
        }

        public virtual bool IsNull()
        {
            return _isNull;
        }

        public virtual void SetNull(bool isNull)
        {
            _isNull = isNull;
        }

        public virtual int GetOdbTypeId()
        {
            return _odbTypeId;
        }

        public virtual void SetOdbTypeId(int odbTypeId)
        {
            _odbTypeId = odbTypeId;
        }
    }
}
