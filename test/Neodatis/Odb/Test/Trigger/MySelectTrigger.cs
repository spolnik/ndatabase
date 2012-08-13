using NDatabase.Odb;
using NDatabase.Odb.Core.Trigger;

namespace Trigger
{
	public class MySelectTrigger : SelectTrigger
	{
		public int nbCalls;

		public override void AfterSelect(object @object, OID oid)
		{
			nbCalls++;
			System.Console.Out.WriteLine("Select on object with oid " + oid);
		}
	}
}
