namespace NDatabase.Odb.Core.Query.Soda
{
    /// <summary>
    /// for implementation of callback evaluations.
    /// 
    /// To constrain a IQuery node with your own callback
    /// <code>Evaluation</code>, construct an object that implements the
    /// <code>Evaluation</code> interface and register it by passing it
    /// to Query#constrain(Object).
    /// 
    /// Evaluations are called as the last step during query execution,
    /// after all other constraints have been applied. Evaluations in higher
    /// level IQuery nodes in the query graph are called first.
    /// </summary>
    public interface IEvaluation
    {
        /// <summary>
        /// callback method during query execution.
        /// </summary>
        /// <param name="candidate">reference to the candidate persistent object.</param>
        void Evaluate(ICandidate candidate);
    }
}