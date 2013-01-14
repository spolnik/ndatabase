using NDatabase.Odb;
using NUnit.Framework;

namespace NDatabase.Client.UnitTests.Misc
{
    public class TestCase_OdbConfiguration
    {
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