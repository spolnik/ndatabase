using NDatabase.Odb.Core.Layers.Layer3;
using Test.Odb.Test.VO.School;

namespace Test.Odb.Test.Other
{
	public class Student2 : Student
	{
		private IStorageEngine storageEngine;

		private bool isModified;

		public Student2() : base(0, null, new System.DateTime(), null, null)
		{
		}
	}
}
