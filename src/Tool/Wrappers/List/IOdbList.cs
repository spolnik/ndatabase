using System.Collections.Generic;

namespace NDatabase2.Tool.Wrappers.List
{
    internal interface IOdbList<TItem> : IList<TItem>
    {
        void AddAll(IEnumerable<TItem> collection);
        void RemoveAll(IEnumerable<TItem> collection);

        bool IsEmpty();
    }
}