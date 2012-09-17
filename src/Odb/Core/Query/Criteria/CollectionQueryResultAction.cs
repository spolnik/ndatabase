using NDatabase.Odb.Core.Layers.Layer2.Instance;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Query.Execution;
using NDatabase.Odb.Core.Query.List.Objects;
using NDatabase.Tool.Wrappers;

namespace NDatabase.Odb.Core.Query.Criteria
{
    /// <summary>
    ///   Class that manage normal query.
    /// </summary>
    /// <remarks>
    ///   Class that manage normal query. Query that return a list of objects. For each object That matches the query criteria, the objectMatch method is called and it keeps the objects in the 'objects' instance.
    /// </remarks>
    /// <author>olivier</author>
    public sealed class CollectionQueryResultAction<T> : IMatchingObjectAction
    {
        private readonly bool _inMemory;
        private readonly IQuery _query;

        private readonly bool _queryHasOrderBy;
        private readonly bool _returnObjects;
        private readonly IStorageEngine _storageEngine;

        /// <summary>
        ///   An object to build instances
        /// </summary>
        private readonly IInstanceBuilder _instanceBuilder;

        private IObjects<T> _result;

        public CollectionQueryResultAction(IQuery query, bool inMemory, IStorageEngine storageEngine, bool returnObjects,
                                           IInstanceBuilder instanceBuilder)
        {
            // TODO check if Object is ok here
            _query = query;
            _inMemory = inMemory;
            _storageEngine = storageEngine;
            _returnObjects = returnObjects;
            _queryHasOrderBy = query.HasOrderBy();
            _instanceBuilder = instanceBuilder;
        }

        #region IMatchingObjectAction Members

        public void ObjectMatch(OID oid, IOdbComparable orderByKey)
        {
            if (_queryHasOrderBy)
                _result.AddWithKey(orderByKey, (T) oid);
            else
                _result.Add((T) oid);
        }

        public void ObjectMatch(OID oid, object o, IOdbComparable orderByKey)
        {
            var nnoi = (NonNativeObjectInfo) o;
            if (_inMemory)
            {
                if (_returnObjects)
                {
                    if (_queryHasOrderBy)
                        _result.AddWithKey(orderByKey, (T) GetCurrentInstance(nnoi));
                    else
                        _result.Add((T) GetCurrentInstance(nnoi));
                }
            }
            else
            {
                if (_queryHasOrderBy)
                    _result.AddWithKey(orderByKey, (T) oid);
                else
                    _result.AddOid(oid);
            }
        }

        public void Start()
        {
            if (_inMemory)
            {
                if (_query != null && _query.HasOrderBy())
                    _result = new InMemoryBTreeCollection<T>(_query.GetOrderByType());
                else
                    _result = new SimpleList<T>();
            }
            else
            {
                // result = new InMemoryBTreeCollection((int) nbObjects);
                if (_query != null && _query.HasOrderBy())
                    _result = new LazyBTreeCollection<T>(_storageEngine, _returnObjects);
                else
                    _result = new LazySimpleListFromOid<T>(_storageEngine, _returnObjects);
            }
        }

        public void End()
        {
        }

        public IObjects<TItem> GetObjects<TItem>()
        {
            return (IObjects<TItem>) _result;
        }

        #endregion

        public object GetCurrentInstance(NonNativeObjectInfo nnoi)
        {
            //FIXME no need
            if (nnoi.GetObject() != null)
                return nnoi.GetObject();
            return _instanceBuilder.BuildOneInstance(nnoi);
        }
    }
}
