using System;
using System.Collections.Generic;

namespace NDatabase.Odb.Core.Layers.Layer2.Meta
{
    /// <summary>
    ///   Meta representation of a null native object
    /// </summary>
    /// <author>osmadja</author>
    
    public sealed class NullNativeObjectInfo : NativeObjectInfo
    {
        private static readonly NullNativeObjectInfo Instance = new NullNativeObjectInfo();

        private NullNativeObjectInfo() : base(null, OdbType.Null)
        {
        }

        public NullNativeObjectInfo(int odbTypeId) : base(null, odbTypeId)
        {
        }

        public NullNativeObjectInfo(OdbType type) : base(null, type)
        {
        }

        public override string ToString()
        {
            return "null";
        }

        public override bool IsNull()
        {
            return true;
        }

        public override bool IsNative()
        {
            return true;
        }

        public override AbstractObjectInfo CreateCopy(IDictionary<OID, AbstractObjectInfo> cache, bool onlyData)
        {
            return GetInstance();
        }

        /// <returns> </returns>
        public static NullNativeObjectInfo GetInstance()
        {
            return Instance;
        }
    }
}
