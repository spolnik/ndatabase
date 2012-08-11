using System.Collections;
using NDatabase.Odb.Core.Layers.Layer2.Meta;

namespace NDatabase.Odb.Core.Query.Criteria
{
    /// <author>olivier
    ///   An interface for all criteria</author>
    public interface ISingleCriterion : ICriterion
    {
        /// <summary>
        ///   Returns a list of attributes names that are involved in the query
        /// </summary>
        /// <returns> The attribute names </returns>
        IList GetAttributeNames();

        string GetAttributeName();

        bool Match(AbstractObjectInfo aoi);

        bool Match(IDictionary map);
    }
}
