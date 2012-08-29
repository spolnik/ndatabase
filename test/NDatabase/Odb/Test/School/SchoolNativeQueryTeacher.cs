using System;
using NDatabase.Odb.Core.Query.NQ;
using Test.NDatabase.Odb.Test.VO.School;

namespace Test.NDatabase.Odb.Test.School
{
    [Serializable]
    public class SchoolNativeQueryTeacher : NativeQuery
    {
        private readonly string name;

        public SchoolNativeQueryTeacher(string name)
        {
            this.name = name;
        }

        public override bool Match(object @object)
        {
            var s = (History) @object;
            var dateTime = new DateTime(2005, 6, 1);

            return s.GetTeacher().GetName().Equals(name) && s.GetDate().Millisecond > (dateTime.Millisecond);
        }

        public override Type GetObjectType()
        {
            return typeof (History);
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
