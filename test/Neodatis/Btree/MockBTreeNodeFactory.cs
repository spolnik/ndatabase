using NDatabase.Btree;
using NDatabase.Odb.Impl.Core.Btree;

namespace Test.Btree
{
	
	public class MockBTreeNodeFactory
	{
		public static IBTreeNode getBTreeNode(IBTree btree)
		{
            return new OdbBtreeNodeSingle(btree);
		}
	}
}