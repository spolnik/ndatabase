using NDatabase.Compability;
using NDatabase.Container;
using NDatabase.Services;
using NUnit.Framework;

namespace NDatabase.UnitTests.Container
{
    public class When_we_use_internal_dependency_container
    {
        [Test]
        public void It_should_return_easily_registered_implementation_by_interface()
        {
            DependencyContainer.Register<IMetaModelCompabilityChecker>(() => new MetaModelCompabilityChecker());

            var checker = DependencyContainer.Resolve<IMetaModelCompabilityChecker>();
            Assert.That(checker, Is.Not.Null);
        }
    }
}