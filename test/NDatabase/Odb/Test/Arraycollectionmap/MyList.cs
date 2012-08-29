using System;
using System.Collections;

namespace Test.NDatabase.Odb.Test.Arraycollectionmap
{
    [Serializable]
    public class MyList : ArrayList
    {
        public virtual object MyGet(int i)
        {
            return this[i];
        }
    }
}
