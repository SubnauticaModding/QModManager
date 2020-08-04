namespace QMMTests
{
    using System;
    using NUnit.Framework;
    using QModManager.API;

    [TestFixture]
    internal class RequiredQModTests
    {
        [TestCase("2.8")]
        [TestCase("2.8.0")]
        public void WhenVersionStringIsTrimmed_EqualsExplicitVersion(string trimmedVersionString)
        {
            var explicitVersion = new RequiredQMod("SMLHelper", "2.8.0.0");

            var trimmedVersion = new RequiredQMod("SMLHelper", trimmedVersionString);

            Assert.AreEqual(explicitVersion.MinimumVersion, trimmedVersion.MinimumVersion);
        }

        [Test]
        public void WhenVersionStringIsMissing_EqualsVersionZero()
        {
            var required = new RequiredQMod("SomeMod");

            var expectedVersion = new Version(0, 0, 0, 0);

            Assert.AreEqual(expectedVersion, required.MinimumVersion);
        }
    }
}
