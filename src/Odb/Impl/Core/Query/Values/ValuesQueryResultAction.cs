using System.Collections;
using NDatabase.Odb.Core.Layers.Layer2.Instance;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Query;
using NDatabase.Odb.Core.Query.Execution;
using NDatabase.Odb.Impl.Core.Oid;
using NDatabase.Odb.Impl.Core.Query.List.Values;
using NDatabase.Tool.Wrappers;

namespace NDatabase.Odb.Impl.Core.Query.Values
{
    public class ValuesQueryResultAction : IMatchingObjectAction
    {
        private readonly IStorageEngine _engine;
        private readonly IValuesQuery _query;

        /// <summary>
        ///   A copy of the query object actions
        /// </summary>
        private readonly IQueryFieldAction[] _queryFieldActions;

        private readonly bool _queryHasOrderBy;
        private readonly int _returnArraySize;
        private IValues _result;

        public ValuesQueryResultAction(IValuesQuery query, IStorageEngine storageEngine,
                                       IInstanceBuilder instanceBuilder)
        {
            _engine = storageEngine;
            _query = query;
            _queryHasOrderBy = query.HasOrderBy();
            _returnArraySize = query.GetObjectActions().Count;
            IEnumerator iterator = query.GetObjectActions().GetEnumerator();
            _queryFieldActions = new IQueryFieldAction[_returnArraySize];

            var i = 0;
            while (iterator.MoveNext())
            {
                var queryFieldAction = (IQueryFieldAction) iterator.Current;
                _queryFieldActions[i] = queryFieldAction.Copy();
                _queryFieldActions[i].SetReturnInstance(query.ReturnInstance());
                _queryFieldActions[i].SetInstanceBuilder(instanceBuilder);
                i++;
            }
        }

        #region IMatchingObjectAction Members

        public virtual void ObjectMatch(OID oid, IOdbComparable orderByKey)
        {
        }

        // This method os not used in Values Query API
        public virtual void ObjectMatch(OID oid, object @object, IOdbComparable orderByKey)
        {
            if (_query.IsMultiRow())
            {
                var values = ConvertObject((AttributeValuesMap) @object);
                if (_queryHasOrderBy)
                    _result.AddWithKey(orderByKey, values);
                else
                    _result.Add(values);
            }
            else
                Compute((AttributeValuesMap) @object);
        }

        public virtual void Start()
        {
            if (_query != null && _query.HasOrderBy())
                _result = new InMemoryBTreeCollectionForValues(_query.GetOrderByType());
            else
                _result = new SimpleListForValues(_returnArraySize);

            for (var i = 0; i < _returnArraySize; i++)
            {
                var queryFieldAction = _queryFieldActions[i];
                queryFieldAction.Start();
            }
        }

        public virtual void End()
        {
            DefaultObjectValues dov = null;
            if (!_query.IsMultiRow())
                dov = new DefaultObjectValues(_returnArraySize);

            for (var i = 0; i < _returnArraySize; i++)
            {
                var queryFieldAction = _queryFieldActions[i];
                queryFieldAction.End();
                
                if (!_query.IsMultiRow())
                    SetValue(i, dov, queryFieldAction);
            }
            if (!_query.IsMultiRow())
                _result.Add(dov);
        }

        public virtual IObjects<T> GetObjects<T>()
        {
            return (IObjects<T>) _result;
        }

        #endregion

        private void Compute(AttributeValuesMap values)
        {
            for (var i = 0; i < _returnArraySize; i++)
                _queryFieldActions[i].Execute(values.GetObjectInfoHeader().GetOid(), values);
        }

        private IObjectValues ConvertObject(AttributeValuesMap values)
        {
            var dov = new DefaultObjectValues(_returnArraySize);
            for (var i = 0; i < _returnArraySize; i++)
            {
                var queryFieldAction = _queryFieldActions[i];
                queryFieldAction.Execute(values.GetObjectInfoHeader().GetOid(), values);

                SetValue(i, dov, queryFieldAction);
            }
            return dov;
        }

        private void SetValue(int i, DefaultObjectValues dov, IQueryFieldAction queryFieldAction)
        {
            var value = queryFieldAction.GetValue();

            // When Values queries return objects, they actually return the oid of the object
            // So we must load it here
            if (value is OID)
            {
                var oid = (OdbObjectOID) value;
                value = _engine.GetObjectFromOid(oid);
            }

            dov.Set(i, queryFieldAction.GetAlias(), value);
        }

        public virtual IValues GetValues()
        {
            return _result;
        }
    }
}
