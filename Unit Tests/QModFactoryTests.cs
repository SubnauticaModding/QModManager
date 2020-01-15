namespace QMMTests
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using QModManager.API;
    using QModManager.API.ModLoading;
    using QModManager.Patching;

    [TestFixture]
    internal class QModFactoryTests
    {
        [Test]
        public void CreateModStatusList_EarlyErrorsCombineWithSuccessfullMods()
        {
            // Arange
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
            List<QMod> combinedList = QModFactory.CreateModStatusList(earlyErrors, modsToLoad);

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
            var earlyErrors = new List<QMod>
            {
                new QMod {Id = "5", Status = ModStatus.MissingPatchMethod },
            };

            var modToInspect = new QMod
            {
                Id = "8",
                Status = ModStatus.Success,
                RequiredMods = new List<RequiredQMod>
                {
                    new RequiredQMod(missingOrOutdatedMod, new Version(1, 0, 2))
                }
            };

            var modsToLoad = new List<QMod>
            {
                new QMod { Id = "6", Status = ModStatus.Success },
                new QMod
                {
                    Id = "7", Status = ModStatus.Success,
                    ParsedVersion = new Version(1, 0, 1)
                },
                modToInspect
            };

            // Act
            List<QMod> combinedList = QModFactory.CreateModStatusList(earlyErrors, modsToLoad);

            // Assert
            Assert.AreEqual(expectedStatus, modToInspect.Status);
        }
    }
}
