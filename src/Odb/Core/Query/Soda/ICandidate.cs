using System;

namespace NSoda
{
    /// <summary>
    /// candidate for Evaluation callbacks.
    /// 
    /// During query execution all registered
    /// Evaluation callback handlers are called with 
    /// Candidate proxies that represent the persistent objects
    /// that meet all other Query criteria.
    /// 
    /// A Candidate provides access to the persistent object it
    /// represents and allows to specify, whether it is to be included in the 
    /// ObjectSet resultset.
    /// </summary>
    public interface ICandidate
    {
        /// <summary>
        /// specify whether the Candidate is to be included in the 
        /// ObjectSet resultset.
        /// 
        /// This method may be called multiple times. The last call prevails.
        /// </summary>
        /// <param name="flag">inclusion</param>
        void Include(bool flag);

        /// <summary>
        /// returns the persistent object that is represented by this query 
        /// Candidate.
        /// @return Object the persistent object.
        /// </summary>
        /// <returns>The persistent object</returns>
        Object GetObject();
    }
}