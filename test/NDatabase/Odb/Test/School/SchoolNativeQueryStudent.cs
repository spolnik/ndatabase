using System;
using NDatabase.Odb.Core.Query.NQ;
using Test.NDatabase.Odb.Test.VO.School;

namespace Test.NDatabase.Odb.Test.School
{
    [Serializable]
    public class SchoolNativeQueryStudent : NativeQuery
    {
        private readonly string name;

        private int age;

        public SchoolNativeQueryStudent(string name, int age)
        {
            this.name = name;
            this.age = age;
        }

        public override bool Match(object @object)
        {
            var s = (Student) @object;
            return s.GetName().Equals(name);
        }

        public override Type GetObjectType()
        {
            return typeof (Student);
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
