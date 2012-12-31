using System.Collections.Generic;

namespace NDatabase.Odb.Core.Query
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

        /// <summary>
        ///   To indicate if query execution must build instances or return object representation, Default value is true(return instance)
        /// </summary>
        void SetReturnInstance(bool returnInstance);

        IValuesQuery Min(string attributeName);
        IValuesQuery Min(string attributeName, string alias);

        /// <summary>
        ///   Returns the field names of the order by
        /// </summary>
        /// <returns> The array of fields of the order by </returns>
        IList<string> GetOrderByFieldNames();

        /// <returns> the type of the order by - NONE, DESC, ASC </returns>
        OrderByConstants GetOrderByType();
    }
}
