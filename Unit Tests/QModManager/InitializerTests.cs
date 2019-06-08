using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using QModManager.API;
using QModManager.API.ModLoading;
using QModManager.API.ModLoading.Internal;

namespace QMMTests.QModManager
{
    [TestFixture]
    internal class InitializerTests
    {
        private class TestMod : IQModLoadable
        {
            internal ModLoadingResults ResultOnLoad { get; set; } = ModLoadingResults.Success;

            public string Id { get; set; }
            public string ModDirectory { get; set; }
            public bool IsLoaded { get; set; }
            public bool IsValid { get; set; }
            public Assembly LoadedAssembly { get; set; }
            public string AssemblyName { get; set; }
            public Version ParsedVersion { get; set; }
            public Dictionary<PatchingOrder, QPatchMethod> PatchMethods { get; set; }

            public ModLoadingResults TryLoading(PatchingOrder order, QModGame currentGame)
            {
                if (this.PatchMethods.ContainsKey(order))
                    return this.ResultOnLoad;

                return ModLoadingResults.NoMethodToExecute;
            }
        }

        [Test]
        public void InitializeMods_GivenModLoadResults_GetExpectedState()
        {
            var list = new List<TestMod>
            {
                new TestMod
                {
                    Id = "TestMod1",
                    ResultOnLoad = ModLoadingResults.Success,
                    PatchMethods = new Dictionary<PatchingOrder, QPatchMethod>(1){ {PatchingOrder.NormalInitialize, null} },
                    IsLoaded = true,
                },
                new TestMod
                {
                    Id = "TestMod2",
                    ResultOnLoad = ModLoadingResults.Failure,
                    PatchMethods = new Dictionary<PatchingOrder, QPatchMethod>(1){ {PatchingOrder.NormalInitialize, null} },
                    IsLoaded = false,
                },
                new TestMod
                {
                    Id = "TestMod3",
                    ResultOnLoad = ModLoadingResults.AlreadyLoaded,
                    PatchMethods = new Dictionary<PatchingOrder, QPatchMethod>(1){ {PatchingOrder.NormalInitialize, null} },
                    IsLoaded = false,
                },
                new TestMod
                {
                    Id = "TestMod4",
                    ResultOnLoad = ModLoadingResults.CurrentGameNotSupported,
                    PatchMethods = new Dictionary<PatchingOrder, QPatchMethod>(1){ {PatchingOrder.NormalInitialize, null} },
                    IsLoaded = false,
                }
            };

            var initializer = new Initializer(QModGame.None);

            initializer.InitializeMods(list);

            Assert.AreEqual(1, initializer.ErrorTotals[ModLoadingResults.Failure]);
            Assert.AreEqual(1, initializer.ErrorTotals[ModLoadingResults.AlreadyLoaded]);
            Assert.AreEqual(1, initializer.ErrorTotals[ModLoadingResults.CurrentGameNotSupported]);
            Assert.AreEqual(3, initializer.FailedToLoad);

            Assert.IsFalse(initializer.NonLoadedMods.ContainsKey("TestMod1"));
            Assert.AreEqual(ModLoadingResults.Failure, initializer.NonLoadedMods["TestMod2"]);
            Assert.AreEqual(ModLoadingResults.AlreadyLoaded, initializer.NonLoadedMods["TestMod3"]);
            Assert.AreEqual(ModLoadingResults.CurrentGameNotSupported, initializer.NonLoadedMods["TestMod4"]);
        }

        private class MultiPatchingTestMod : IQModLoadable
        {
            internal ModLoadingResults[] ResultOnLoad { get; set; }
            internal int TryLoadingCalls = 0;

            public string Id { get; set; }
            public string ModDirectory { get; set; }
            public bool IsLoaded { get; set; }
            public bool IsValid { get; set; }
            public Assembly LoadedAssembly { get; set; }
            public string AssemblyName { get; set; }
            public Version ParsedVersion { get; set; }
            public Dictionary<PatchingOrder, QPatchMethod> PatchMethods { get; } = new Dictionary<PatchingOrder, QPatchMethod>();

            public ModLoadingResults TryLoading(PatchingOrder order, QModGame currentGame)
            {
                if (this.PatchMethods.ContainsKey(order))
                {
                    return this.ResultOnLoad[TryLoadingCalls++];
                }

                return ModLoadingResults.NoMethodToExecute;
            }
        }

        [Test]
        public void InitializeMods_GivenMultiStagePatchingMods_GetExpectedState()
        {
            var testMod1 = new MultiPatchingTestMod
            {
                Id = "TestMod1",
                ResultOnLoad = new[] { ModLoadingResults.Success, ModLoadingResults.Success },
                IsLoaded = true,
            };
            testMod1.PatchMethods.Add(PatchingOrder.PreInitialize, null);
            testMod1.PatchMethods.Add(PatchingOrder.NormalInitialize, null);

            var testMod2 = new MultiPatchingTestMod
            {
                Id = "TestMod2",
                ResultOnLoad = new[] { ModLoadingResults.Failure, ModLoadingResults.NoMethodToExecute },
                IsLoaded = false, // Simulated failure
            };
            testMod2.PatchMethods.Add(PatchingOrder.NormalInitialize, null);
            testMod2.PatchMethods.Add(PatchingOrder.PostInitialize, null);

            var testMod3 = new MultiPatchingTestMod
            {
                Id = "TestMod3",
                ResultOnLoad = new[] { ModLoadingResults.Success, ModLoadingResults.Success, ModLoadingResults.Success },
                IsLoaded = true,
            };
            testMod3.PatchMethods.Add(PatchingOrder.PreInitialize, null);
            testMod3.PatchMethods.Add(PatchingOrder.NormalInitialize, null);
            testMod3.PatchMethods.Add(PatchingOrder.PostInitialize, null);

            var list = new List<MultiPatchingTestMod>
            {
                testMod1, testMod2, testMod3
            };

            var initializer = new Initializer(QModGame.None);

            initializer.InitializeMods(list);
            Assert.AreEqual(2, testMod1.TryLoadingCalls);
            Assert.AreEqual(2, testMod2.TryLoadingCalls);
            Assert.AreEqual(3, testMod3.TryLoadingCalls);

            Assert.AreEqual(1, initializer.FailedToLoad);
        }
    }
}
