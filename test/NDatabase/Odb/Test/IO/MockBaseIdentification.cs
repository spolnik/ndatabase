using System;
using NDatabase.Odb.Core.Layers.Layer3;

namespace Test.Odb.Test.IO
{
    public class MockBaseIdentification : IBaseIdentification
    {
        #region IBaseIdentification Members

        public bool CanWrite()
        {
            return false;
        }

        public String GetIdentification()
        {
            return "mock";
        }

        public bool IsNew()
        {
            return false;
        }

        public String GetDirectory()
        {
            return string.Empty;
        }

        #endregion

        public bool IsLocal()
        {
            return false;
        }
    }
}
