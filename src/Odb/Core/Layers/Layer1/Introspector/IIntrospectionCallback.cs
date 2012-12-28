namespace NDatabase.Odb.Core.Layers.Layer1.Introspector
{
    /// <author>olivier
    ///   A simple callback used by the introspection API to inform when object are found</author>
    internal interface IIntrospectionCallback
    {
        /// <summary>
        ///   Called when the introspector find a non native object.
        /// </summary>
        /// <remarks>
        ///   Called when the introspector find a non native object.
        /// </remarks>
        /// <param name="object"> </param>
        void ObjectFound(object @object);
    }
}