namespace NDatabase.Odb.Core.Layers.Layer3.Oid
{
    /// <summary>
    ///   Status of ID.
    /// </summary>
    /// <author>osmadja</author>
    public static class IDStatus
    {
        public const byte Unknown = 0;

        public const byte Active = 1;

        public const byte Deleted = 2;

        public static bool IsActive(byte status)
        {
            return status == Active;
        }
    }
}