using NUnit.Framework;
namespace Cyclic
{
	/// <author>olivier</author>
	public class ClassB
	{
		internal string name;

		internal Cyclic.ClassA classA;

		public ClassB() : base()
		{
		}

		public ClassB(Cyclic.ClassA classA, string name) : base()
		{
			this.classA = classA;
			this.name = name;
		}

		public virtual string GetName()
		{
			return name;
		}

		public virtual void SetName(string name)
		{
			this.name = name;
		}

		public virtual Cyclic.ClassA GetClassA()
		{
			return classA;
		}

		public virtual void SetClassA(Cyclic.ClassA classA)
		{
			this.classA = classA;
		}
	}
}
