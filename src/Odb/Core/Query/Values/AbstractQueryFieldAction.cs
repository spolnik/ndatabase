using NDatabase.Odb.Core.Layers.Layer2.Instance;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Query.Execution;
using NDatabase2.Odb;

namespace NDatabase.Odb.Core.Query.Values
{
    public abstract class AbstractQueryFieldAction : IQueryFieldAction
    {
        protected string Alias;
        protected string AttributeName;

        private IInstanceBuilder _instanceBuilder;
        private bool _isMultiRow;
        private bool _returnInstance;

        protected AbstractQueryFieldAction(string attributeName, string alias, bool isMultiRow)
        {
            AttributeName = attributeName;
            Alias = alias;
            _isMultiRow = isMultiRow;
        }

        #region IQueryFieldAction Members

        public virtual string GetAttributeName()
        {
            return AttributeName;
        }

        public virtual string GetAlias()
        {
            return Alias;
        }

        public abstract void Execute(OID oid, AttributeValuesMap values);

        public virtual bool IsMultiRow()
        {
            return _isMultiRow;
        }

        public virtual void SetMultiRow(bool isMultiRow)
        {
            _isMultiRow = isMultiRow;
        }

        internal IInstanceBuilder GetInstanceBuilder()
        {
            return _instanceBuilder;
        }

        internal void SetInstanceBuilder(IInstanceBuilder instanceBuilder)
        {
            _instanceBuilder = instanceBuilder;
        }

        public virtual bool ReturnInstance()
        {
            return _returnInstance;
        }

        public virtual void SetReturnInstance(bool returnInstance)
        {
            _returnInstance = returnInstance;
        }

        public abstract IQueryFieldAction Copy();

        public abstract void End();

        public abstract object GetValue();

        public abstract void Start();

        #endregion
    }
}
