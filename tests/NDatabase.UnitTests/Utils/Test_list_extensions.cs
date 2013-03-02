using System.Collections.Generic;
using NUnit.Framework;
using NDatabase.Tool;

namespace NDatabase.UnitTests.Utils
{
    public class Test_list_extensions
    {
        [Test]
        public void It_should_throw_exception_when_list_is_null()
        {
            IList<string> nullList = null;
            Assert.That(() => nullList.IsEmpty(), Throws.Exception);
            Assert.That(() => nullList.IsNotEmpty(), Throws.Exception);
        }
    }
}