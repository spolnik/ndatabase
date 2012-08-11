using System;
using NDatabase.Btree.Exception;

namespace NDatabase.Btree.Tool
{
    public class BTreeValidator
    {
        private static bool _on;

        public static bool IsOn()
        {
            return _on;
        }

        public static void SetOn(bool on)
        {
            _on = on;
        }

        public static void CheckDuplicateChildren(IBTreeNode node1, IBTreeNode node2)
        {
            if (!_on)
                return;

            for (var i = 0; i < node1.GetNbChildren(); i++)
            {
                var child1 = node1.GetChildAt(i, true);

                for (var j = 0; j < node2.GetNbChildren(); j++)
                {
                    if (child1 == node2.GetChildAt(j, true))
                        throw new BTreeNodeValidationException("Duplicated node : " + child1);
                }
            }
        }

        public static void ValidateNode(IBTreeNode node, bool isRoot)
        {
            if (!_on)
                return;

            ValidateNode(node);

            if (isRoot && node.HasParent())
                throw new BTreeNodeValidationException("Root node with a parent: " + node);

            if (!isRoot && !node.HasParent())
                throw new BTreeNodeValidationException("Internal node without parent: " + node);
        }

        public static void ValidateNode(IBTreeNode node)
        {
            if (!_on)
                return;

            var nbKeys = node.GetNbKeys();
            if (node.HasParent() && nbKeys < node.GetDegree() - 1)
                throw new BTreeNodeValidationException("Node with less than " + (node.GetDegree() - 1) + " keys");

            var maxNbKeys = node.GetDegree() * 2 - 1;
            var nbChildren = node.GetNbChildren();
            var maxNbChildren = node.GetDegree() * 2;

            if (nbChildren != 0 && nbKeys == 0)
                throw new BTreeNodeValidationException("Node with no key but with children : " + node);

            for (var i = 0; i < nbKeys; i++)
            {
                if (node.GetKeyAndValueAt(i) == null)
                    throw new BTreeNodeValidationException("Null key at " + i + " on node " + node);

                CheckValuesOfChild(node.GetKeyAndValueAt(i), node.GetChildAt(i, false));
            }

            for (var i = nbKeys; i < maxNbKeys; i++)
            {
                if (node.GetKeyAndValueAt(i) != null)
                    throw new BTreeNodeValidationException("Not Null key at " + i + " on node " + node);
            }

            IBTreeNode previousNode = null;

            for (var i = 0; i < nbChildren; i++)
            {
                if (node.GetChildAt(i, false) == null)
                    throw new BTreeNodeValidationException("Null child at index " + i + " on node " + node);

                if (previousNode != null && previousNode == node.GetChildAt(i, false))
                    throw new BTreeNodeValidationException("Two equals children at index " + i + " : " + previousNode);

                previousNode = node.GetChildAt(i, false);
            }

            for (var i = nbChildren; i < maxNbChildren; i++)
            {
                if (node.GetChildAt(i, false) != null)
                    throw new BTreeNodeValidationException("Not Null child at " + i + " on node " + node);
            }
        }

        private static void CheckValuesOfChild(IKeyAndValue key, IBTreeNode node)
        {
            if (!_on)
                return;

            if (node == null)
                return;

            for (var i = 0; i < node.GetNbKeys(); i++)
            {
                if (node.GetKeyAndValueAt(i).GetKey().CompareTo(key.GetKey()) >= 0)
                    throw new BTreeNodeValidationException("Left child with values bigger than pivot " + key + " : " +
                                                           node);
            }
        }

        public static bool SearchKey(IComparable key, IBTreeNodeOneValuePerKey node)
        {
            if (!_on)
                return false;

            for (var i = 0; i < node.GetNbKeys(); i++)
            {
                if (node.GetKeyAndValueAt(i).GetKey().CompareTo(key) == 0)
                    return true;
            }

            return false;
        }
    }
}
