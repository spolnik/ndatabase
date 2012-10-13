using System.Collections;
using NDatabase2.Odb.Core.Layers.Layer2.Meta;
using NDatabase2.Odb.Core.Query.Execution;

namespace NDatabase2.Odb.Core.Query.Values
{
    /// <summary>
    ///   An action to retrieve a size of a list.
    /// </summary>
    /// <remarks>
    ///   An action to retrieve a size of a list. It is used by the Object Values API. When calling odb.getValues(new ValuesCriteriaQuery(Handler.class, Where .equal("id", id)).size("parameters"); The sublist action will return Returns a view of the portion of this list between the specified fromIndex, inclusive, and toIndex, exclusive. if parameters list contains [param1,param2,param3,param4], sublist("parameters",1,2) will return a sublist containing [param2,param3]
    /// </remarks>
    internal sealed class SizeAction : AbstractQueryFieldAction
    {
        private long _size;

        public SizeAction(string attributeName, string alias) : base(attributeName, alias, true)
        {
        }

        public override void Execute(OID oid, AttributeValuesMap values)
        {
            var list = (IList) values[AttributeName];
            _size = list.Count;
        }

        public override object GetValue()
        {
            return _size;
        }

        public override void End()
        {
        }

        public override void Start()
        {
        }

        public long GetSize()
        {
            return _size;
        }

        public override IQueryFieldAction Copy()
        {
            return new SizeAction(AttributeName, Alias);
        }
    }
}
