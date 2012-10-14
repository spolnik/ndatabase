using System;
using NDatabase2.Odb.Core.Query.Criteria;
using NUnit.Framework;

namespace NDatabase.UnitTests.Queries
{
    public class When_we_use_criteria_query : InstanceSpecification<CriteriaQuery<When_we_use_criteria_query.Employee>>
    {
        #region Nested type: Employee

        public class Employee
        {
            public string Name { get; set; }
        }

        #endregion

        #region Overrides of InstanceSpecification<CriteriaQuery<Employee>>

        protected override CriteriaQuery<Employee> Create_subject_under_test()
        {
            return new CriteriaQuery<Employee>();
        }

        #endregion

        [Test]
        public void It_should_be_able_to_filter_employee_instances()
        {
            var criterion = SubjectUnderTest.GetCriteria();
            
        }
    }
}
