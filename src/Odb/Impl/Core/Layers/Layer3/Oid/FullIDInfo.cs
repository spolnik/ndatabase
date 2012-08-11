using System;
using System.Text;

namespace NDatabase.Odb.Impl.Core.Layers.Layer3.Oid
{
    /// <summary>
    ///   Used to obtain internal infos about all database ids
    /// </summary>
    /// <author>osmadja</author>
    public class FullIDInfo
    {
        private readonly byte idStatus;
        private readonly OID nextOID;

        private readonly string objectToString;
        private readonly long position;

        private readonly OID prevOID;
        private long blockId;
        private long id;
        private string objectClassName;

        public FullIDInfo(long id, long position, byte idStatus, long blockId, string objectClassName
                          , string objectToString, OID prevOID, OID nextOID)
        {
            this.id = id;
            this.position = position;
            this.blockId = blockId;
            this.objectClassName = objectClassName;
            this.objectToString = objectToString;
            this.idStatus = idStatus;
            this.prevOID = prevOID;
            this.nextOID = nextOID;
        }

        public virtual long GetBlockId()
        {
            return blockId;
        }

        public virtual void SetBlockId(long blockId)
        {
            this.blockId = blockId;
        }

        public virtual long GetId()
        {
            return id;
        }

        public virtual void SetId(long id)
        {
            this.id = id;
        }

        public virtual string GetObjectClassName()
        {
            return objectClassName;
        }

        public virtual void SetObjectClassName(string objectClassName)
        {
            this.objectClassName = objectClassName;
        }

        public override string ToString()
        {
            var buffer = new StringBuilder();
            buffer.Append("Id=").Append(id).Append(" - Posi=").Append(position).Append(" - Status="
                ).Append(idStatus).Append(" - Block Id=").Append(blockId);
            buffer.Append(" - Type=").Append(objectClassName);
            buffer.Append(" - prev inst. pos=").Append(prevOID);
            buffer.Append(" - next inst. pos=").Append(nextOID);
            buffer.Append(" - Object=").Append(objectToString);
            return buffer.ToString();
        }

        public static void Main2(string[] args)
        {
            var ii = new FullIDInfo
                (1, 1, 1, 1, string.Empty, string.Empty, null, null);
            ii.SetObjectClassName("ola");
            Console.Out.WriteLine("ll=" + ii.GetObjectClassName());
        }
    }
}