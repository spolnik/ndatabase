using System;
using NDatabase.Btree;
using NDatabase.Odb.Core.Query.Execution;
using NDatabase.Tool.Wrappers;

namespace NDatabase.Odb.Core.Layers.Layer2.Meta
{
    /// <summary>
    ///   An index of a class info
    /// </summary>
    /// <author>osmadja</author>
    [Serializable]
    public class ClassInfoIndex
    {
        public const byte Enabled = 1;

        public const byte Disabled = 2;
        private int[] _attributeIds;

        private IBTree _btree;

        private OID _classInfoId;

        private long _creationDate;
        private bool _isUnique;

        private long _lastRebuild;
        private string _name;

        private byte _status;

        public virtual OID GetClassInfoId()
        {
            return _classInfoId;
        }

        public virtual void SetClassInfoId(OID classInfoId)
        {
            _classInfoId = classInfoId;
        }

        public virtual int[] GetAttributeIds()
        {
            return _attributeIds;
        }

        public virtual void SetAttributeIds(int[] attributeIds)
        {
            _attributeIds = attributeIds;
        }

        public virtual long GetCreationDate()
        {
            return _creationDate;
        }

        public virtual void SetCreationDate(long creationDate)
        {
            _creationDate = creationDate;
        }

        public virtual bool IsUnique()
        {
            return _isUnique;
        }

        public virtual void SetUnique(bool isUnique)
        {
            _isUnique = isUnique;
        }

        public virtual long GetLastRebuild()
        {
            return _lastRebuild;
        }

        public virtual void SetLastRebuild(long lastRebuild)
        {
            _lastRebuild = lastRebuild;
        }

        public virtual string GetName()
        {
            return _name;
        }

        public virtual void SetName(string name)
        {
            _name = name;
        }

        public virtual byte GetStatus()
        {
            return _status;
        }

        public virtual void SetStatus(byte status)
        {
            _status = status;
        }

        public virtual int GetAttributeId(int index)
        {
            return _attributeIds[index];
        }

        public virtual void SetBTree(IBTree btree)
        {
            _btree = btree;
        }

        public virtual IBTree GetBTree()
        {
            return _btree;
        }

        public virtual IOdbComparable ComputeKey(NonNativeObjectInfo nnoi)
        {
            return IndexTool.BuildIndexKey(_name, nnoi, _attributeIds);
        }

        public virtual int GetNbAttributes()
        {
            return _attributeIds.Length;
        }

        /// <summary>
        ///   Check if a list of attribute can use the index
        /// </summary>
        /// <param name="attributeIdsToMatch"> </param>
        /// <returns> true if the list of attribute can use this index </returns>
        public virtual bool MatchAttributeIds(int[] attributeIdsToMatch)
        {
            //TODO an index with lesser attribute than the one to match can be used
            if (_attributeIds.Length != attributeIdsToMatch.Length)
                return false;

            foreach (var attributeIdToMatch in attributeIdsToMatch)
            {
                var found = false;
                for (var j = 0; j < _attributeIds.Length; j++)
                {
                    if (_attributeIds[j] == attributeIdToMatch)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                    return false;
            }
            return true;
        }
    }
}
