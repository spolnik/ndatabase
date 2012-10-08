using System;
using NDatabase.Odb.Core.Query.NQ;
using Test.NDatabase.Odb.Test.VO.School;

namespace Test.NDatabase.Odb.Test.School
{
    
    public class SchoolNativeQueryTeacher : NativeQuery<History>
    {
        private readonly string name;

        public SchoolNativeQueryTeacher(string name)
        {
            this.name = name;
        }

        public override bool Match(History @object)
        {
            var dateTime = new DateTime(2005, 6, 1);

            return @object.GetTeacher().GetName().Equals(name) && @object.GetDate().Millisecond > (dateTime.Millisecond);
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
