using System.Collections.Generic;

namespace NDatabase2.Tool.Wrappers.List
{
    public interface IOdbList<TItem> : IList<TItem>
    {
        bool AddAll(ICollection<TItem> collection);
        bool RemoveAll(ICollection<TItem> collection);

        TItem Get(int index);
        
        bool IsEmpty();
    }
}