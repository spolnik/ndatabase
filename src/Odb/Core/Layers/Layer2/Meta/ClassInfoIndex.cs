using System.Linq;
using NDatabase2.Btree;
using NDatabase2.Odb.Core.Query.Execution;
using NDatabase2.Tool.Wrappers;

namespace NDatabase2.Odb.Core.Layers.Layer2.Meta
{
    /// <summary>
    ///   An index of a class info
    /// </summary>
    internal sealed class ClassInfoIndex
    {
        public const byte Enabled = 1;
        public const byte Disabled = 2;

        public OID ClassInfoId { get; set; }

        public int[] AttributeIds { get; set; }

        public long CreationDate { get; set; }

        public bool IsUnique { get; set; }

        public long LastRebuild { get; set; }

        public string Name { get; set; }

        public byte Status { get; set; }

        public IBTree BTree { get; set; }

        public IOdbComparable ComputeKey(NonNativeObjectInfo nnoi)
        {
            return IndexTool.BuildIndexKey(Name, nnoi, AttributeIds);
        }

        /// <summary>
        ///   Check if a list of attribute can use the index
        /// </summary>
        /// <param name="attributeIdsToMatch"> </param>
        /// <returns> true if the list of attribute can use this index </returns>
        public bool MatchAttributeIds(int[] attributeIdsToMatch)
        {
            //TODO an index with lesser attribute than the one to match can be used
            if (AttributeIds.Length != attributeIdsToMatch.Length)
                return false;

            foreach (var attributeIdToMatch in attributeIdsToMatch)
            {
                var found = AttributeIds.Any(t => t == attributeIdToMatch);
                if (!found)
                    return false;
            }

            return true;
        }
    }
}
