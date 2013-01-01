using NDatabase.Odb.Core.Layers.Layer2.Meta;

namespace NDatabase.Odb.Core.Query.Execution
{
    internal interface IQueryExecutionPlan
    {
        bool UseIndex();

        ClassInfoIndex GetIndex();

        string GetDetails();

        void Start();

        void End();
    }
}
