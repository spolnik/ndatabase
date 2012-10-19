using System.Collections.Generic;

namespace NDatabase2.Odb
{
    /// <summary>
    ///   The main interface of all query results of NDatabase ODB.
    /// </summary>
    /// <remarks>
    ///   The main interface of all query results of NDatabase ODB. 
    ///   Objects interface extends the Collection interface so it provides a standard collection behavior.
    /// </remarks>
    public interface IObjectSet<TItem> : ICollection<TItem>
    {
        /// <summary>
        ///   Inform if the internal Iterator has more objects
        /// </summary>
        /// <returns> </returns>
        bool HasNext();

        /// <summary>
        ///   Returns the next object of the internal iterator of the collection
        /// </summary>
        /// <returns> </returns>
        TItem Next();

        /// <summary>
        ///   Return the first object of the collection, if exist
        /// </summary>
        /// <returns> </returns>
        TItem GetFirst();

        /// <summary>
        ///   Reset the internal iterator of the collection
        /// </summary>
        void Reset();
    }
}
