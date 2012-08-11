using System;
using NDatabase.Btree.Exception;

namespace NDatabase.Btree.Impl.Multiplevalue
{
    [Serializable]
    public class InMemoryBTreeNodeMultipleValuesPerKey : BTreeNodeMultipleValuesPerKey
    {
        protected static int NextId = 1;

        protected IBTreeNode[] Children;
        protected int Id;

        protected IBTreeNode Parent;

        public InMemoryBTreeNodeMultipleValuesPerKey(IBTree btree) : base(btree)
        {
            Id = NextId++;
        }

        public override IBTreeNode GetChildAt(int index, bool throwExceptionIfNotExist)
        {
            if (Children[index] == null && throwExceptionIfNotExist)
            {
                throw new BTreeException("Trying to load null child node at index "
                                         + index);
            }
            return Children[index];
        }

        public override IBTreeNode GetParent()
        {
            return Parent;
        }

        public override void SetChildAt(IBTreeNode child, int index)
        {
            Children[index] = child;
            if (child != null)
            {
                child.SetParent(this);
            }
        }

        public override void SetChildAt(IBTreeNode node, int childIndex, int
                                                                             index, bool throwExceptionIfDoesNotExist)
        {
            var childTreeNode = node.GetChildAt(childIndex, throwExceptionIfDoesNotExist);

            Children[index] = childTreeNode;

            if (childTreeNode != null)
                childTreeNode.SetParent(this);
        }

        public override void SetParent(IBTreeNode node)
        {
            Parent = node;
        }

        public override bool HasParent()
        {
            return Parent != null;
        }

        protected override void Init()
        {
            Children = new IBTreeNode[MaxNbChildren];
        }

        public override object GetId()
        {
            return Id;
        }

        public override void SetId(object id)
        {
            Id = (int) id;
        }

        public override void DeleteChildAt(int index)
        {
            Children[index] = null;
            NbChildren--;
        }

        public override void MoveChildFromTo(int sourceIndex, int destinationIndex, bool
                                                                                        throwExceptionIfDoesNotExist)
        {
            if (Children[sourceIndex] == null && throwExceptionIfDoesNotExist)
            {
                var errorMessage = string.Format("Trying to move null child node at index {0}", sourceIndex);
                throw new BTreeException(errorMessage);
            }
            Children[destinationIndex] = Children[sourceIndex];
        }

        public override void SetNullChildAt(int childIndex)
        {
            Children[childIndex] = null;
        }

        public override object GetChildIdAt(int childIndex, bool throwExceptionIfDoesNotExist)
        {
            if (Children[childIndex] == null && throwExceptionIfDoesNotExist)
            {
                throw new BTreeException("Trying to move null child node at index "
                                         + childIndex);
            }
            return Children[childIndex].GetId();
        }

        public override object GetParentId()
        {
            return Id;
        }

        public override object GetValueAsObjectAt(int index)
        {
            return GetValueAt(index);
        }
    }
}
