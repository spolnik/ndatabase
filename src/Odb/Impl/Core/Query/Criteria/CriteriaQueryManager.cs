using System.Collections;
using NDatabase.Odb.Core.Layers.Layer2.Meta;

namespace NDatabase.Odb.Impl.Core.Query.Criteria
{
    public class CriteriaQueryManager
    {
        public static bool Match(CriteriaQuery query, IDictionary map)
        {
            return query.Match(map);
        }

        public static bool Match(CriteriaQuery query, object @object)
        {
            return query.Match((AbstractObjectInfo) @object);
        }

        public static string GetFullClassName(CriteriaQuery query)
        {
            return query.GetFullClassName();
        }
    }
}
