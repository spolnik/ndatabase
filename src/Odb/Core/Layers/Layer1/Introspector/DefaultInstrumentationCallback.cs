namespace NDatabase2.Odb.Core.Layers.Layer1.Introspector
{
    public sealed class DefaultInstrumentationCallback : IIntrospectionCallback
    {
        #region IIntrospectionCallback Members

        public bool ObjectFound(object @object)
        {
            return true;
        }

        #endregion
    }
}