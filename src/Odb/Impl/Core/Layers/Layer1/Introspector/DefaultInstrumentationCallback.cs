using NDatabase.Odb.Core.Layers.Layer1.Introspector;

namespace NDatabase.Odb.Impl.Core.Layers.Layer1.Introspector
{
    /// <author>olivier</author>
    public class DefaultInstrumentationCallback : IIntrospectionCallback
    {
        #region IIntrospectionCallback Members

        public virtual bool ObjectFound(object @object)
        {
            return true;
        }

        #endregion
    }
}