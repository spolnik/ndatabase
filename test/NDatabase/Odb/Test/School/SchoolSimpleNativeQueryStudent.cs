using NDatabase2.Odb.Core.Query.NQ;
using Test.NDatabase.Odb.Test.VO.School;

namespace Test.NDatabase.Odb.Test.School
{
    
    public class SchoolSimpleNativeQueryStudent : SimpleNativeQuery<Student>
    {
        private readonly string name;

        public SchoolSimpleNativeQueryStudent(string name)
        {
            this.name = name;
        }

        public override bool Match(Student @object)
        {
            return @object.GetName().Equals(name);
        }
    }
}
