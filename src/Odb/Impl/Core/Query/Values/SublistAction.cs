using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Query.Execution;
using NDatabase.Odb.Core.Query.Values;
using NDatabase.Odb.Impl.Core.Query.List.Objects;
using NDatabase.Tool.Wrappers.List;

namespace NDatabase.Odb.Impl.Core.Query.Values
{
    /// <summary>
    ///   An action to retrieve a sublist of list.
    /// </summary>
    /// <remarks>
    ///   An action to retrieve a sublist of list. It is used by the Object Values API. When calling odb.getValues(new ValuesCriteriaQuery(Handler.class, Where .equal("id", id)).sublist("parameters",fromIndex, size); The sublist action will return Returns a view of the portion of this list between the specified fromIndex, inclusive, and toIndex, exclusive. if parameters list contains [param1,param2,param3,param4], sublist("parameters",1,2) will return a sublist containing [param2,param3]
    /// </remarks>
    [Serializable]
    public sealed class SublistAction : AbstractQueryFieldAction
    {
        private readonly int _fromIndex;
        private readonly int _size;
        private readonly bool _throwExceptionIfOutOfBound;
        private IOdbList<object> _sublist;

        public SublistAction(string attributeName, string alias, int fromIndex, int size,
                             bool throwExceptionIfOutOfBound) : base(attributeName, alias, true)
        {
            _fromIndex = fromIndex;
            _size = size;
            _throwExceptionIfOutOfBound = throwExceptionIfOutOfBound;
        }

        public SublistAction(string attributeName, string alias, int fromIndex, int toIndex)
            : base(attributeName, alias, true)
        {
            _fromIndex = fromIndex;
            _size = toIndex - fromIndex;
            _throwExceptionIfOutOfBound = true;
        }

        public override void Execute(OID oid, AttributeValuesMap values)
        {
            var l = ((IEnumerable)values[AttributeName]).Cast<object>().ToList();
            var localFromIndex = _fromIndex;
            var localEndIndex = _fromIndex + _size;

            // If not throw exception, we must implement 
            // Index Out Of Bound protection
            if (!_throwExceptionIfOutOfBound)
            {
                // Check from index
                if (localFromIndex > l.Count - 1)
                    localFromIndex = 0;

                // Check end index
                if (localEndIndex > l.Count)
                    localEndIndex = l.Count;
            }

            _sublist = new LazySimpleListOfAoi<object>(GetInstanceBuilder(), ReturnInstance());
            _sublist.AddAll(OdbCollectionUtil.SublistGeneric(l, localFromIndex, localEndIndex));
        }

        public override object GetValue()
        {
            return _sublist;
        }

        public override void End()
        {
        }

        public override void Start()
        {
        }

        public IList<object> GetSubList()
        {
            return _sublist;
        }

        public override IQueryFieldAction Copy()
        {
            return new SublistAction(AttributeName, Alias, _fromIndex, _size, _throwExceptionIfOutOfBound);
        }
    }
}
