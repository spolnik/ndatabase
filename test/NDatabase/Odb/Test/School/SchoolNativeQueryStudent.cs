using System;
using NDatabase2.Odb.Core.Query.NQ;
using Test.NDatabase.Odb.Test.VO.School;

namespace Test.NDatabase.Odb.Test.School
{
    public class SchoolNativeQueryStudent : NativeQuery<Student>
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

        public override Type[] GetObjectTypes()
        {
            return null;
        }

        public override string[] GetIndexFields()
        {
            return null;
        }
    }
}
