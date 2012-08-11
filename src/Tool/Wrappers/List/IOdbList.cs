using System.Collections.Generic;

namespace NDatabase.Tool.Wrappers.List
{
    /// <author>olivier</author>
    public interface IOdbList<TItem> : IList<TItem>
    {
        bool AddAll(ICollection<TItem> collection);
        bool RemoveAll(ICollection<TItem> collection);

        TItem Get(int index);
        void Set(int index, TItem element);

        bool IsEmpty();
    }
}