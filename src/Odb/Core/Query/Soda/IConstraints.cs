// .net S.O.D.A - Simple Object Database Access

namespace NDatabase.Odb.Core.Query.Soda
{
    /// <summary>
    /// Set of Constraint objects.
    /// 
    /// This extension of the Constraint interface allows
    /// setting the evaluation mode of all contained Constraint
    /// objects with single calls.
    /// </summary>
    public interface IConstraints : IConstraint
    {
        /// <summary>
        /// returns an array of the contained Constraint objects.
        /// </summary>
        /// <returns>An array of the contained {@link Constraint} objects.</returns>
        IConstraint[] ToArray();
    }
}
