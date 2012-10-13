using NDatabase2.Odb.Core.Query.Execution;
using NDatabase2.Tool.Wrappers.List;

namespace NDatabase2.Odb.Core.Query
{
    public interface IValuesQuery : IQuery
    {
        IValuesQuery Count(string alias);

        IValuesQuery Sum(string fieldName);

        IValuesQuery Sum(string fieldName, string alias);

        IValuesQuery Avg(string fieldName, string alias);

        IValuesQuery Avg(string fieldName);

        IValuesQuery Max(string fieldName, string alias);

        IValuesQuery Max(string fieldName);

        IValuesQuery Field(string fieldName);

        IValuesQuery Field(string fieldName, string alias);

        IValuesQuery Sublist(string attributeName, string alias, int fromIndex, int size, bool throwException);

        IValuesQuery Sublist(string attributeName, int fromIndex, int size, bool throwException);

        IValuesQuery Sublist(string attributeName, string alias, int fromIndex, int toIndex);

        IValuesQuery Sublist(string attributeName, int fromIndex, int toIndex);

        IValuesQuery Size(string attributeName);

        IValuesQuery Size(string attributeName, string alias);

        IValuesQuery GroupBy(string fieldList);

        string[] GetGroupByFieldList();

        bool HasGroupBy();

        IOdbList<string> GetAllInvolvedFields();

        /// <summary>
        ///   To indicate if a query will return one row (for example, sum, average, max and min, or will return more than one row
        /// </summary>
        bool IsMultiRow();

        /// <returns> </returns>
        bool ReturnInstance();

        /// <summary>
        ///   To indicate if query execution must build instances or return object representation, Default value is true(return instance)
        /// </summary>
        void SetReturnInstance(bool returnInstance);

        int ObjectActionsCount { get; }
        IOdbList<IQueryFieldAction> GetObjectActions();
    }
}
