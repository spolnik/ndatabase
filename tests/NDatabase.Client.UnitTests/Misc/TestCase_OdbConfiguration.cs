using NDatabase.Odb;
using NDatabase.Odb.Core.Layers.Layer3.Engine;
using NUnit.Framework;

namespace NDatabase.Client.UnitTests.Misc
{
    public class TestCase_OdbConfiguration
    {
        [Test]
        public void Check_turning_on_the_logging()
        {
            Assert.That(OdbConfiguration.IsLoggingEnabled(), Is.False);

            OdbConfiguration.EnableLogging();

            Assert.That(OdbConfiguration.IsLoggingEnabled(), Is.True);

            OdbConfiguration.DisableLogging();

            Assert.That(OdbConfiguration.IsLoggingEnabled(), Is.False);
        }

        [Test]
        public void Check_changing_btree_degree()
        {
            Assert.That(OdbConfiguration.GetIndexBTreeDegree(), Is.EqualTo(OdbConfiguration.DefaultIndexBTreeDegree));
            
            const int newIndexBTreeSize = 30;
            OdbConfiguration.SetIndexBTreeDegree(newIndexBTreeSize);

            Assert.That(OdbConfiguration.GetIndexBTreeDegree(), Is.EqualTo(newIndexBTreeSize));
        }

        [Test]
        public void Check_turning_on_the_btree_validation()
        {
            Assert.That(OdbConfiguration.IsBTreeValidationEnabled(), Is.False);

            OdbConfiguration.EnableBTreeValidation();

            Assert.That(OdbConfiguration.IsBTreeValidationEnabled(), Is.True);

            OdbConfiguration.DisableBTreeValidation();

            Assert.That(OdbConfiguration.IsBTreeValidationEnabled(), Is.False);
        }

        [Test]
        public void Check_changing_types_resolution_mode()
        {
            Assert.That(OdbConfiguration.IsWorkingInNormalTypeResolutionMode(), Is.True);

            OdbConfiguration.EnableLessRestrictedTypeResolutionMode();

            Assert.That(OdbConfiguration.IsWorkingInNormalTypeResolutionMode(), Is.False);

            OdbConfiguration.EnableNormalTypeResolutionMode();

            Assert.That(OdbConfiguration.IsWorkingInNormalTypeResolutionMode(), Is.True);
        }
    }
}