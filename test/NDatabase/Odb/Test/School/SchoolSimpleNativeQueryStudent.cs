using System;
using NDatabase.Odb.Core.Query.NQ;
using Test.Odb.Test.VO.School;

namespace Test.Odb.Test.School
{
    [Serializable]
    public class SchoolSimpleNativeQueryStudent : SimpleNativeQuery
    {
        private readonly string name;

        public SchoolSimpleNativeQueryStudent(string name)
        {
            this.name = name;
        }

        public virtual bool Match(Student @object)
        {
            return @object.GetName().Equals(name);
        }
    }
}
