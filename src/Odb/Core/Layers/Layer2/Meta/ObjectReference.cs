using System;
using System.Collections.Generic;

namespace NDatabase.Odb.Core.Layers.Layer2.Meta
{
    /// <summary>
    ///   Meta representation of an object reference.
    /// </summary>
    /// <remarks>
    ///   Meta representation of an object reference.
    /// </remarks>
    /// <author>osmadja</author>
    [Serializable]
    public class ObjectReference : AbstractObjectInfo
    {
        private readonly OID _id;

        private readonly NonNativeObjectInfo _nnoi;

        public ObjectReference(OID id) : base(Meta.OdbType.NonNativeId)
        {
            _id = id;
        }

        public ObjectReference(NonNativeObjectInfo nnoi) : base(Meta.OdbType.NonNativeId)
        {
            _id = null;
            _nnoi = nnoi;
        }

        /// <returns> Returns the id. </returns>
        public virtual OID GetOid()
        {
            if (_nnoi != null)
                return _nnoi.GetOid();
            return _id;
        }

        public override bool IsObjectReference()
        {
            return true;
        }

        public override string ToString()
        {
            return string.Format("ObjectReference to oid {0}", GetOid());
        }

        public override bool IsNull()
        {
            return false;
        }

        public override object GetObject()
        {
            throw new OdbRuntimeException(
                NDatabaseError.MethodShouldNotBeCalled.AddParameter("getObject").AddParameter(GetType().FullName));
        }

        public override void SetObject(object @object)
        {
            throw new OdbRuntimeException(
                NDatabaseError.MethodShouldNotBeCalled.AddParameter("setObject").AddParameter(GetType().FullName));
        }

        public virtual NonNativeObjectInfo GetNnoi()
        {
            return _nnoi;
        }

        public override AbstractObjectInfo CreateCopy(IDictionary<OID, AbstractObjectInfo> cache, bool onlyData)
        {
            return new ObjectReference((NonNativeObjectInfo) _nnoi.CreateCopy(cache, onlyData));
        }
    }
}
