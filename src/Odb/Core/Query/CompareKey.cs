using NDatabase2.Tool.Wrappers;

namespace NDatabase2.Odb.Core.Query
{
    public abstract class CompareKey : IOdbComparable
    {
        #region IOdbComparable Members

        public abstract int CompareTo(object o);

        #endregion
    }
}
