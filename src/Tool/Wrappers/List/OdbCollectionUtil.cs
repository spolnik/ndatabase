using System.Collections.Generic;

namespace NDatabase.Tool.Wrappers.List
{
    public static class OdbCollectionUtil
    {
        public static IList<TItem> SublistGeneric<TItem>(IList<TItem> l1, int from, int to)
        {
            var list = new List<TItem>();

            for (var i = from; i < to; i++)
                list.Add(l1[i]);

            return list;
        }
    }
}