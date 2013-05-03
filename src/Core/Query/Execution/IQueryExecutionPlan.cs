using NDatabase.Core.Layers.Layer2.Meta;

namespace NDatabase.Core.Query.Execution
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
