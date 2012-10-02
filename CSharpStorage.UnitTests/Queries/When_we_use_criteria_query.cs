using NDatabase.Odb.Core.Query;
using NDatabase.Odb.Core.Query.Criteria;

namespace NDatabase.UnitTests.Queries
{
    public class When_we_use_criteria_query : InstanceSpecification<IQuery>
    {
        protected override void Establish_context()
        {
            base.Establish_context();
        }

        protected override IQuery Create_subject_under_test()
        {
            return CriteriaQuery.New<Employee>();
        }

        protected override void Because()
        {
            
        }

        #region Nested type: Employee

        private class Employee
        {
            public string Name { get; set; }
        }

        #endregion
    }
}