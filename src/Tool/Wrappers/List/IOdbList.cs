using System.Collections.Generic;

namespace NDatabase2.Tool.Wrappers.List
{
    public interface IOdbList<TItem> : IList<TItem>
    {
        bool AddAll(IEnumerable<TItem> collection);
        bool RemoveAll(IEnumerable<TItem> collection);

        TItem Get(int index);
        
        bool IsEmpty();
    }
}