using NDatabase.Odb.Core.Query.Execution;

namespace NDatabase.Odb.Core.Query.Values
{
    /// <summary>
    ///   Used to implement custom query action.
    /// </summary>
    /// <remarks>
    ///   Used to implement custom query action.
    /// </remarks>
    /// <author>osmadja</author>
    public interface ICustomQueryFieldAction : IQueryFieldAction
    {
        void SetAttributeName(string attributeName);

        void SetAlias(string alias);
    }
}
