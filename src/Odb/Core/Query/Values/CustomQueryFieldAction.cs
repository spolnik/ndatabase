using System;
using NDatabase.Odb.Core.Query.Execution;

namespace NDatabase.Odb.Core.Query.Values
{
    
    public abstract class CustomQueryFieldAction : AbstractQueryFieldAction, ICustomQueryFieldAction
    {
        protected CustomQueryFieldAction() : base(null, null, true)
        {
        }

        #region ICustomQueryFieldAction Members

        public virtual void SetAlias(string alias)
        {
            Alias = alias;
        }

        public virtual void SetAttributeName(string attributeName)
        {
            AttributeName = attributeName;
        }

        public override IQueryFieldAction Copy()
        {
            try
            {
                var customQueryFieldAction = (ICustomQueryFieldAction) Activator.CreateInstance(GetType());
                customQueryFieldAction.SetAttributeName(AttributeName);
                customQueryFieldAction.SetAlias(Alias);
                return customQueryFieldAction;
            }
            catch (Exception)
            {
                throw new OdbRuntimeException(
                    NDatabaseError.ValuesQueryErrorWhileCloningCustumQfa.AddParameter(GetType().FullName));
            }
        }

        public abstract override void End();

        public abstract override object GetValue();

        public abstract override void Start();

        #endregion
    }
}
