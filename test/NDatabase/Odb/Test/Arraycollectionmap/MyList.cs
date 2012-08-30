using System;
using System.Collections;

namespace Test.NDatabase.Odb.Test.Arraycollectionmap
{
    
    public class MyList : ArrayList
    {
        public virtual object MyGet(int i)
        {
            return this[i];
        }
    }
}
