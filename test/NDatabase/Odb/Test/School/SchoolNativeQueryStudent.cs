using NDatabase2.Odb.Core.Query.NQ;
using Test.NDatabase.Odb.Test.VO.School;

namespace Test.NDatabase.Odb.Test.School
{
    public class SchoolNativeQueryStudent : SimpleNativeQuery<Student>
    {
        private readonly string name;

        private int age;

        public SchoolNativeQueryStudent(string name, int age)
        {
            this.name = name;
            this.age = age;
        }

        public override bool Match(Student @object)
        {
            return @object.GetName().Equals(name);
        }
    }
}
