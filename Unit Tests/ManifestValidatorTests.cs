namespace QMMTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;
    using QModManager.API;
    using QModManager.Patching;

    [TestFixture]
    internal class ManifestValidatorTests
    {
        [Test]
        public void CheckRequiredMods_WhenRequiresNothing_GetEmptyRequiredMods()
        {
            // Mod that requires no other mods.
            var standaloneMod = new QMod();

            var validator = new ManifestValidator();
            validator.CheckRequiredMods(standaloneMod);

            Assert.AreEqual(0, standaloneMod.RequiredMods.Count());
        }

        [Test]
        public void CheckRequiredMods_WhenRequiresIdOnly_GetExpectedRequiredMod()
        {
            const string requiredMod = "SMLHelper";
            var simpleDependentMod = new QMod
            {
                // Mod that requires one other mod, only by ID.
                Dependencies = new[] { requiredMod }
            };

            var validator = new ManifestValidator();
            validator.CheckRequiredMods(simpleDependentMod);

            Assert.AreEqual(1, simpleDependentMod.RequiredMods.Count());

            RequiredQMod expected = new RequiredQMod(requiredMod);
            RequiredQMod actual = simpleDependentMod.RequiredMods.First();

            Assert.AreEqual(requiredMod, actual.Id);
            Assert.AreEqual(expected.RequiresMinimumVersion, actual.RequiresMinimumVersion);
            Assert.AreEqual(expected.MinimumVersion, actual.MinimumVersion);
        }

        [Test]
        public void CheckRequiredMods_WhenRequiresVersion_GetExpectedRequiredMod()
        {
            const string requiredMod = "SMLHelper";
            const string requiredVersionString = "8.6.7";

            var versionDependentMod = new QMod
            {
                // Mod that requires a minimum version of another mod.
                VersionDependencies = new Dictionary<string, string>
                {
                    { requiredMod, requiredVersionString }
                }
            };

            var validator = new ManifestValidator();
            validator.CheckRequiredMods(versionDependentMod);

            Assert.AreEqual(1, versionDependentMod.RequiredMods.Count());

            RequiredQMod expected = new RequiredQMod(requiredMod, requiredVersionString);
            RequiredQMod actual = versionDependentMod.RequiredMods.First();

            Assert.AreEqual(requiredMod, actual.Id);
            Assert.AreEqual(expected.RequiresMinimumVersion, actual.RequiresMinimumVersion);
            Assert.AreEqual(expected.MinimumVersion, actual.MinimumVersion);
        }

        [Test]
        public void CheckRequiredMods_WhenRequirementsRedundant_GetExpectedRequiredMod()
        {
            const string requiredMod = "SMLHelper";
            const string requiredVersionString = "8.6.7";

            var versionDependentMod = new QMod
            {
                // Mod that redundantly includes the same mod both ID and minimum version.
                // Unnecessary, but should not cause errors.
                Dependencies = new[] { requiredMod },
                VersionDependencies = new Dictionary<string, string>
                {
                    { requiredMod, requiredVersionString }
                }
            };

            var validator = new ManifestValidator();
            validator.CheckRequiredMods(versionDependentMod);

            // Only a single entry is added to RequiredMods.
            Assert.AreEqual(1, versionDependentMod.RequiredMods.Count());

            RequiredQMod expected = new RequiredQMod(requiredMod, requiredVersionString);
            RequiredQMod actual = versionDependentMod.RequiredMods.First();

            Assert.AreEqual(requiredMod, actual.Id);
            Assert.AreEqual(expected.RequiresMinimumVersion, actual.RequiresMinimumVersion);
            Assert.AreEqual(expected.MinimumVersion, actual.MinimumVersion);
        }

        [Test]
        public void CheckRequiredMods_WhenRequirementsMixed_GetExpectedRequiredMods()
        {
            const string requiredModA = "SMLHelper";
            const string requiredModB = "MoreCyclopsUpgrades";
            const string requiredVersionBString = "8.6.7";

            var versionDependentMod = new QMod
            {
                // Mod that requires one mod by id only and another at a minimum verison.
                Dependencies = new[] { requiredModA },
                VersionDependencies = new Dictionary<string, string>
                {
                    { requiredModB, requiredVersionBString }
                }
            };

            var validator = new ManifestValidator();
            validator.CheckRequiredMods(versionDependentMod);

            Assert.AreEqual(2, versionDependentMod.RequiredMods.Count());

            RequiredQMod expectedA = new RequiredQMod(requiredModA);
            RequiredQMod actualA = versionDependentMod.RequiredMods.ElementAt(0);

            Assert.AreEqual(requiredModA, actualA.Id);
            Assert.AreEqual(expectedA.RequiresMinimumVersion, actualA.RequiresMinimumVersion);
            Assert.AreEqual(expectedA.MinimumVersion, actualA.MinimumVersion);

            RequiredQMod expectedB = new RequiredQMod(requiredModB, requiredVersionBString);
            RequiredQMod actualB = versionDependentMod.RequiredMods.ElementAt(1);

            Assert.AreEqual(requiredModB, actualB.Id);
            Assert.AreEqual(expectedB.RequiresMinimumVersion, actualB.RequiresMinimumVersion);
            Assert.AreEqual(expectedB.MinimumVersion, actualB.MinimumVersion);
        }

        [Test]
        public void CheckRequiredMods_WhenRequiredVersionInvalid_GetExpectedRequiredMod()
        {
            const string requiredMod = "SMLHelper";
            const string invalidVersionString = "1.X.0.Y";

            var versionDependentMod = new QMod
            {
                // Mod with a bad version string in its version dependencies.
                VersionDependencies = new Dictionary<string, string>
                {
                    { requiredMod, invalidVersionString }
                }
            };

            var validator = new ManifestValidator();
            validator.CheckRequiredMods(versionDependentMod);

            // The entry is added to RequiredMods but without a minimum version.
            Assert.AreEqual(1, versionDependentMod.RequiredMods.Count());

            RequiredQMod expected = new RequiredQMod(requiredMod);
            RequiredQMod actual = versionDependentMod.RequiredMods.First();

            Assert.AreEqual(requiredMod, actual.Id);
            Assert.AreEqual(expected.RequiresMinimumVersion, actual.RequiresMinimumVersion);
            Assert.AreEqual(expected.MinimumVersion, actual.MinimumVersion);
        }
    }
}
