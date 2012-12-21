using NDatabase2.Odb.Core.Layers.Layer2.Meta;
using NDatabase2.Tool.Wrappers.List;

namespace NDatabase2.Odb.Core.Query.Criteria
{
    /// <summary>
    ///   An interface for all criteria
    /// </summary>
    public interface IConstraint
    {
        IConstraint And(IConstraint with);
        IConstraint Or(IConstraint with);
        IConstraint Not();

        IConstraint Equals();
        IConstraint InvariantEquals();
        IConstraint Like();
        IConstraint InvariantLike();
        IConstraint Contains();
        IConstraint SmallerOrEqual();
        IConstraint GreaterOrEqual();
        IConstraint Greater();
        IConstraint Smaller();
    }

    internal interface IInternalConstraint : IConstraint
    {
        bool CanUseIndex();

        AttributeValuesMap GetValues();

        /// <summary>
        ///   to be able to optimize query execution.
        /// </summary>
        /// <remarks>
        ///   to be able to optimize query execution. 
        ///   Get only the field involved in the query instead of getting all the object
        /// </remarks>
        /// <returns> All involved fields in criteria, List of String </returns>
        IOdbList<string> GetAllInvolvedFields();

        /// <summary>
        ///   To check if an object matches this criterion
        /// </summary>
        /// <returns> true if object matches the criteria </returns>
        bool Match(object @object);
    }
}
