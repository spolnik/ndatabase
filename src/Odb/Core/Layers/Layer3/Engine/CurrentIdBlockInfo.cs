namespace NDatabase2.Odb.Core.Layers.Layer3.Engine
{
    public sealed class CurrentIdBlockInfo
    {
        /// <summary>
        ///   The max id already allocated in the current id block
        /// </summary>
        public OID CurrentIdBlockMaxOid { get; set; }

        /// <summary>
        ///   The current id block number
        /// </summary>
        public int CurrentIdBlockNumber { get; set; }

        /// <summary>
        ///   The position of the current block where IDs are stored
        /// </summary>
        public long CurrentIdBlockPosition { get; set; }
    }
}