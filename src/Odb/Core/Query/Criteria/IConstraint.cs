using NDatabase2.Odb.Core.Layers.Layer2.Meta;
using NDatabase2.Tool.Wrappers.List;

namespace NDatabase2.Odb.Core.Query.Criteria
{
    ///   An interface for all criteria
    public interface IConstraint
    {
        /// <summary>
        ///   To check if an object matches this criterion
        /// </summary>
        /// <returns> true if object matches the criteria </returns>
        bool Match(object @object);

        /// <summary>
        ///   to be able to optimize query execution.
        /// </summary>
        /// <remarks>
        ///   to be able to optimize query execution. 
        ///   Get only the field involved in the query instead of getting all the object
        /// </remarks>
        /// <returns> All involved fields in criteria, List of String </returns>
        IOdbList<string> GetAllInvolvedFields();

        AttributeValuesMap GetValues();

        /// <summary>
        ///   Gets thes whole query
        /// </summary>
        /// <returns> The owner query </returns>
        IQuery GetQuery();

        void SetQuery(IQuery query);

        bool CanUseIndex();

        /// <summary>
        ///   a method to explicitly indicate that the criteria is ready.
        /// </summary>
        void Ready();

        IExpression And(IConstraint criterion);
        IExpression Or(IConstraint criterion);
        IExpression Not();
    }
}
