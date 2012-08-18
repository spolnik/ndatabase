namespace NSoda
{
    /// <summary>
    /// database engine interface.
    /// 
    /// The <code>ObjectContainer</code> interface provides all methods
    /// to store, retrieve and delete objects and to change object state.
    /// </summary>
    public interface IObjectContainer
    {
        /// <summary>
        /// factory method to create a new Query
        /// <code>Query</code> object to query this ObjectContainer.
        /// </summary>
        /// <returns>A new Query object</returns>
        IQuery Query();
    }
}