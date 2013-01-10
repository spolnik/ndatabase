using NDatabase.Northwind.Domain;
using NDatabase.TypeResolution;
using NUnit.Framework;

namespace NDatabase.UnitTests.TypeResolution
{
    public class TypeResolution_UseCase
    {
        [Test]
        public void Test_simple_case_with_northwind_library()
        {
            var resolvedType = TypeResolutionUtils.ResolveType("NDatabase.Northwind.Domain.Category");

            Assert.That(resolvedType, Is.Not.Null);
            Assert.That(resolvedType.Name, Is.EqualTo(typeof(Category).Name));
        }
    }
}