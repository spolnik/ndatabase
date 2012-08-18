using System;

namespace NSoda
{
    /// <summary>
    /// query resultset.
    /// 
    /// The <code>ObjectSet</code> interface serves as a cursor to
    /// iterate through a set of objects retrieved by a query.
    /// </summary>

    public interface IObjectSet
    {
        /// <summary>
        /// returns <code>true</code> if the <code>ObjectSet</code> has more elements.
        /// </summary>
        /// <returns><code>true</code> if the <code>ObjectSet</code> has more elements</returns>
        bool HasNext();


        /// <summary>
        /// returns the next object in the <code>ObjectSet</code>.
        /// </summary>
        /// <returns>the next object in the <code>ObjectSet</code>.</returns>
        Object Next();


        /// <summary>
        /// resets the <code>ObjectSet</code> cursor before the first element. 
        /// 
        /// A subsequent call to <code>next()</code> will return the first element.
        /// </summary>
        void Reset();


        /// <summary>
        /// returns the number of elements in the <code>ObjectSet</code>.
        /// </summary>
        /// <returns>the number of elements in the <code>ObjectSet</code>.</returns>
        int Size();
    }
}