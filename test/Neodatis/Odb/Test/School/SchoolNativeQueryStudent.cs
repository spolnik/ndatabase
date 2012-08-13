using System;
using NDatabase.Odb.Core.Query.NQ;
using NUnit.Framework;
using Test.Odb.Test.VO.School;

namespace School
{
	[Serializable]
	public class SchoolNativeQueryStudent : NativeQuery
	{
		private string name;

		private int age;

		public SchoolNativeQueryStudent(string name, int age)
		{
			this.name = name;
			this.age = age;
		}

		public override bool Match(object @object)
		{
			Student s = (Student)@object;
			return s.GetName().Equals(name);
		}

		public override System.Type GetObjectType()
		{
			return typeof(Student);
		}

		public override System.Type[] GetObjectTypes()
		{
			return null;
		}

		public override string[] GetIndexFields()
		{
			return null;
		}
	}
}
