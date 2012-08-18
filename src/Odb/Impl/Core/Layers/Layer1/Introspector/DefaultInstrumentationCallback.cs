using NDatabase.Odb.Core.Layers.Layer1.Introspector;

namespace NDatabase.Odb.Impl.Core.Layers.Layer1.Introspector
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