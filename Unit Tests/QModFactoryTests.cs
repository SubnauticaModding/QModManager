namespace QMMTests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using BepInEx;
    using NUnit.Framework;
    using QModManager.API;
    using QModManager.Patching;

    [TestFixture]
    internal class QModFactoryTests
    {
        private class DummyValidator : IManifestValidator
        {
            public void CheckRequiredMods(QMod mod)
            {
            }

            public void FindPatchMethods(QMod qMod)
            {
            }

            public void LoadAssembly(QMod mod)
            {
            }

            public void ValidateBasicManifest(QMod mod)
            {
            }
        }

        private class DummyPluginCollection : IPluginCollection
        {
            public IEnumerable<PluginInfo> AllPlugins { get; }

            public bool IsKnownPlugin(string id)
            {
                return false;
            }

            public void MarkAsRequired(string id)
            {
            }
        }

        [Test]
        public void CreateModStatusList_EarlyErrorsCombineWithSuccessfullMods()
        {
            // Arange
            var factory = new QModFactory();
            var earlyErrors = new List<QMod>
            {
                new QMod {Id = "1", Status = ModStatus.CanceledByUser },
                new QMod {Id = "2", Status = ModStatus.InvalidCoreInfo },
                new QMod {Id = "3", Status = ModStatus.MissingAssemblyFile },
                new QMod {Id = "4", Status = ModStatus.MissingDependency },
                new QMod {Id = "5", Status = ModStatus.MissingPatchMethod },
            };

            var modsToLoad = new List<QMod>
            {
                new QMod { Id = "6", Status = ModStatus.Success },
                new QMod { Id = "7", Status = ModStatus.Success },
                new QMod { Id = "8", Status = ModStatus.Success },
            };

            // Act
            List<QMod> combinedList = factory.CreateModStatusList(earlyErrors, modsToLoad);

            Assert.AreEqual(earlyErrors.Count + modsToLoad.Count, combinedList.Count);

            foreach (QMod erroredMod in earlyErrors)
                Assert.IsTrue(combinedList.Contains(erroredMod));

            foreach (QMod readyMod in modsToLoad)
                Assert.IsTrue(combinedList.Contains(readyMod));
        }

        [TestCase("0", ModStatus.MissingDependency)]
        [TestCase("5", ModStatus.MissingDependency)]
        [TestCase("7", ModStatus.OutOfDateDependency)]
        public void CreateModStatusList_WhenMissingVersionDependencies_StatusUpdates(string missingOrOutdatedMod, ModStatus expectedStatus)
        {
            // Arange
            var factory = new QModFactory(new DummyPluginCollection(), new DummyValidator())
            {
            };

            var noModsRequired = new RequiredQMod[0];

            var earlyErrors = new List<QMod>
            {
                new QMod {Id = "5", Status = ModStatus.MissingPatchMethod, RequiredMods = noModsRequired },
            };

            var modToInspect = new QMod
            {
                Id = "8",
                Status = ModStatus.Success,
                RequiredMods = new List<RequiredQMod>
                {
                    new RequiredQMod(missingOrOutdatedMod, "1.0.2")
                }
            };

            var modsToLoad = new List<QMod>
            {
                new QMod { Id = "6", Status = ModStatus.Success, RequiredMods = noModsRequired },
                new QMod
                {
                    Id = "7", Status = ModStatus.Success,
                    ParsedVersion = new Version(1, 0, 1),
                    LoadedAssembly = Assembly.GetExecutingAssembly(),
                    RequiredMods = noModsRequired
                },
                modToInspect
            };

            // Act
            List<QMod> combinedList = factory.CreateModStatusList(earlyErrors, modsToLoad);

            // Assert
            Assert.AreEqual(expectedStatus, modToInspect.Status);
        }
    }
}
