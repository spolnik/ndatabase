using NDatabase2.Odb.Core.Query.List.Objects;
using NDatabase2.Tool.Wrappers;

namespace NDatabase2.Odb.Core.Query.List.Values
{
    /// <summary>
    ///   A simple list to hold query result for Object Values API.
    /// </summary>
    /// <remarks>
    ///   A simple list to hold query result for Object Values API. It is used when no index and no order by is used and inMemory = true
    /// </remarks>
    public sealed class SimpleListForValues : SimpleList<IObjectValues>, IInternalValues
    {
        public SimpleListForValues()
        {
        }

        public SimpleListForValues(int initialCapacity) : base(initialCapacity)
        {
        }

        #region IValues Members

        public IObjectValues NextValues()
        {
            return Next();
        }

        public override bool AddWithKey(IOdbComparable key, IObjectValues @object)
        {
            throw new OdbRuntimeException(NDatabaseError.OperationNotImplemented.AddParameter("addWithKey"));
        }

        public override bool AddWithKey(int key, IObjectValues @object)
        {
            throw new OdbRuntimeException(NDatabaseError.OperationNotImplemented.AddParameter("addWithKey"));
        }

        public new void AddOid(OID oid)
        {
            throw new OdbRuntimeException(NDatabaseError.InternalError.AddParameter("Add Oid not implemented "));
        }

        #endregion
    }
}
