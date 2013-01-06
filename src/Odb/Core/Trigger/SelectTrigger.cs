namespace NDatabase.Odb.Core.Trigger
{
    /// <summary>
    /// Abstract class - derive from it if you want to create select trigger
    /// </summary>
    public abstract class SelectTrigger : Trigger
    {
        /// <summary>
        /// Action which will happen after select
        /// </summary>
        /// <param name="object">Selected object</param>
        /// <param name="oid">OID of selected object</param>
        public abstract void AfterSelect(object @object, OID oid);
    }
}
