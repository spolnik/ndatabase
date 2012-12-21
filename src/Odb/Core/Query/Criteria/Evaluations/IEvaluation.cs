namespace NDatabase2.Odb.Core.Query.Criteria.Evaluations
{
    public interface IEvaluation
    {
        bool Evaluate(object candidate);
    }
}