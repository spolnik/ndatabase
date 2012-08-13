using NUnit.Framework;
namespace Cyclic
{
	/// <author>olivier</author>
	public class ClassA
	{
		internal string name;

		internal Cyclic.ClassB classb;

		public ClassA() : base()
		{
		}

		public ClassA(Cyclic.ClassB classb, string name) : base()
		{
			this.classb = classb;
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

		public virtual Cyclic.ClassB GetClassb()
		{
			return classb;
		}

		public virtual void SetClassb(Cyclic.ClassB classb)
		{
			this.classb = classb;
		}
	}
}
