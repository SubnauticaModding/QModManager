namespace QModManager.DataStructures
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using QModManager.Patching;

    [TestFixture]
    internal class SortedTreeTests
    {
        private class TestData : ISortable<string>
        {
            private readonly List<string> depends = new List<string>();
            private readonly List<string> loadBefore = new List<string>();
            private readonly List<string> loadAfter = new List<string>();

            public TestData(string id, params string[] dependencies)
            {
                this.Id = id;

                if (dependencies != null)
                {
                    foreach (string item in dependencies)
                    {
                        depends.Add(item);
                    }
                }
            }

            public TestData(int id)
            {
                this.Id = id.ToString();
            }

            public string Id { get; }

            public IList<string> RequiredDependencies => depends;

            public IList<string> LoadBeforePreferences => loadBefore;

            public IList<string> LoadAfterPreferences => loadAfter;

            public PatchingOrder LoadPriority { get; } = PatchingOrder.NormalInitialize;

            public override string ToString()
            {
                return this.Id.ToString();
            }
        }

        [Test]
        public void Test_NoPreferences_GetExpectedOrder()
        {
            var tree = new SortedCollection<string, TestData>();

            tree.AddSorted(new TestData(0));
            tree.AddSorted(new TestData(1));
            tree.AddSorted(new TestData(2));

            List<string> list = tree.GetSortedIndexList();
            Console.WriteLine(ListToString(list));

            Assert.AreEqual(3, list.Count);

            Assert.AreEqual("1", list[0]);
            Assert.AreEqual("2", list[1]);
            Assert.AreEqual("0", list[2]);

            Assert.Pass(ListToString(list));
        }

        [Test]
        public void Test_DupId_A_GetError()
        {
            var tree = new SortedCollection<string, TestData>();

            tree.AddSorted(new TestData(0));
            tree.AddSorted(new TestData(0));
            tree.AddSorted(new TestData(1));

            List<string> list = tree.GetSortedIndexList();
            Console.WriteLine(ListToString(list));

            Assert.AreEqual(1, list.Count);

            Assert.AreEqual("1", list[0]);

            Assert.Pass(ListToString(list));
        }

        [Test]
        public void Test_DupId_B_GetError()
        {
            var tree = new SortedCollection<string, TestData>();

            tree.AddSorted(new TestData(0));
            tree.AddSorted(new TestData(1));
            tree.AddSorted(new TestData(0));

            List<string> list = tree.GetSortedIndexList();
            Console.WriteLine(ListToString(list));

            Assert.AreEqual(1, list.Count);

            Assert.AreEqual("1", list[0]);

            Assert.Pass(ListToString(list));
        }

        [Test]
        public void Test_DupId_C_GetError()
        {
            var tree = new SortedCollection<string, TestData>();

            var i1 = new TestData(1);
            var i0 = new TestData(0);
            var i02 = new TestData(0);

            tree.AddSorted(i1);
            tree.AddSorted(i0);
            tree.AddSorted(i02);

            List<string> list = tree.GetSortedIndexList();
            Console.WriteLine(ListToString(list));

            Assert.AreEqual(1, list.Count);

            Assert.AreEqual("1", list[0]);

            Assert.Pass(ListToString(list));
        }

        [Test]
        public void Test_MissingDependency_GetExpectedOrder()
        {
            var tree = new SortedCollection<string, TestData>();

            var entity = new TestData(0);
            entity.RequiredDependencies.Add("1");

            tree.AddSorted(entity);

            List<string> list = tree.GetSortedIndexList();
            Console.WriteLine(ListToString(list));

            Assert.AreEqual(0, list.Count);

            Assert.Pass(ListToString(list));
        }

        //-------

        [Test]
        public void Test_MutualSortPrefrence_AB_GetExpectedOrder()
        {
            var tree = new SortedCollection<string, TestData>();

            var iA = new TestData(0);
            iA.LoadBeforePreferences.Add("1");

            var iB = new TestData(1);
            iB.LoadAfterPreferences.Add("0");

            tree.AddSorted(iA);
            tree.AddSorted(iB);
            tree.AddSorted(new TestData(2));

            List<string> list = tree.GetSortedIndexList();
            Console.WriteLine(ListToString(list));

            Assert.AreEqual(3, list.Count);

            Assert.IsTrue(list.IndexOf("1") < list.IndexOf("0"));

            Assert.Pass(ListToString(list));
        }

        [Test]
        public void Test_MutualSortPrefrence_BA_GetExpectedOrder()
        {
            var tree = new SortedCollection<string, TestData>();

            var iA = new TestData(0);
            iA.LoadBeforePreferences.Add("1");

            var iB = new TestData(1);
            iB.LoadAfterPreferences.Add("0");

            tree.AddSorted(iB);
            tree.AddSorted(iA);
            tree.AddSorted(new TestData(2));

            List<string> list = tree.GetSortedIndexList();
            Console.WriteLine(ListToString(list));

            Assert.AreEqual(3, list.Count);

            Assert.IsTrue(list.IndexOf("1") < list.IndexOf("0"));

            Assert.Pass(ListToString(list));
        }

        [Test]
        public void Test_MutualSortPrefrence_ACB_GetExpectedOrder()
        {
            var tree = new SortedCollection<string, TestData>();

            var iA = new TestData(0);
            iA.LoadBeforePreferences.Add("1");

            var iB = new TestData(1);
            iB.LoadAfterPreferences.Add("0");

            tree.AddSorted(iA);
            tree.AddSorted(new TestData(2));
            tree.AddSorted(iB);

            List<string> list = tree.GetSortedIndexList();
            Console.WriteLine(ListToString(list));

            Assert.AreEqual(3, list.Count);

            Assert.IsTrue(list.IndexOf("1") < list.IndexOf("0"));

            Assert.Pass(ListToString(list));
        }

        [Test]
        public void Test_MutualSortPrefrence_BCA_GetExpectedOrder()
        {
            var tree = new SortedCollection<string, TestData>();

            var iA = new TestData(0);
            iA.LoadBeforePreferences.Add("1");

            var iB = new TestData(1);
            iB.LoadAfterPreferences.Add("0");

            tree.AddSorted(iB);
            tree.AddSorted(new TestData(2));
            tree.AddSorted(iA);

            List<string> list = tree.GetSortedIndexList();
            Console.WriteLine(ListToString(list));

            Assert.AreEqual(3, list.Count);

            Assert.IsTrue(list.IndexOf("1") < list.IndexOf("0"));

            Assert.Pass(ListToString(list));
        }

        //-------

        [Test]
        public void Test_BeforeOnlySortPrefrence_AB_GetExpectedOrder()
        {
            var tree = new SortedCollection<string, TestData>();

            var iA = new TestData(0);
            iA.LoadBeforePreferences.Add("1");

            var iB = new TestData(1);

            tree.AddSorted(iA);
            tree.AddSorted(iB);
            tree.AddSorted(new TestData(2));

            List<string> list = tree.GetSortedIndexList();
            Console.WriteLine(ListToString(list));

            Assert.AreEqual(3, list.Count);

            Assert.IsTrue(list.IndexOf("1") < list.IndexOf("0"));

            Assert.Pass(ListToString(list));
        }

        [Test]
        public void Test_BeforeOnlySortPrefrence_BA_GetExpectedOrder()
        {
            var tree = new SortedCollection<string, TestData>();

            var iA = new TestData(0);
            iA.LoadBeforePreferences.Add("1");

            var iB = new TestData(1);

            tree.AddSorted(iB);
            tree.AddSorted(iA);
            tree.AddSorted(new TestData(2));

            List<string> list = tree.GetSortedIndexList();
            Console.WriteLine(ListToString(list));

            Assert.AreEqual(3, list.Count);

            Assert.IsTrue(list.IndexOf("1") < list.IndexOf("0"));

            Assert.Pass(ListToString(list));
        }

        [Test]
        public void Test_BeforeOnlySortPrefrence_ACB_GetExpectedOrder()
        {
            var tree = new SortedCollection<string, TestData>();

            var iA = new TestData(0);
            iA.LoadBeforePreferences.Add("1");

            var iB = new TestData(1);

            tree.AddSorted(iA);
            tree.AddSorted(new TestData(2));
            tree.AddSorted(iB);

            List<string> list = tree.GetSortedIndexList();
            Console.WriteLine(ListToString(list));

            Assert.AreEqual(3, list.Count);

            Assert.IsTrue(list.IndexOf("1") < list.IndexOf("0"));

            Assert.Pass(ListToString(list));
        }

        [Test]
        public void Test_BeforeOnlySortPrefrence_BCA_GetExpectedOrder()
        {
            var tree = new SortedCollection<string, TestData>();

            var iA = new TestData(0);
            iA.LoadBeforePreferences.Add("1");

            var iB = new TestData(1);

            tree.AddSorted(iB);
            tree.AddSorted(new TestData(2));
            tree.AddSorted(iA);

            List<string> list = tree.GetSortedIndexList();
            Console.WriteLine(ListToString(list));

            Assert.AreEqual(3, list.Count);

            Assert.IsTrue(list.IndexOf("1") < list.IndexOf("0"));

            Assert.Pass(ListToString(list));
        }

        //-------

        [Test]
        public void Test_AfterOnlySortPrefrence_AB_GetExpectedOrder()
        {
            var tree = new SortedCollection<string, TestData>();

            var iA = new TestData(0);

            var iB = new TestData(1);
            iB.LoadAfterPreferences.Add("0");

            tree.AddSorted(iA);
            tree.AddSorted(iB);
            tree.AddSorted(new TestData(2));

            List<string> list = tree.GetSortedIndexList();
            Console.WriteLine(ListToString(list));

            Assert.AreEqual(3, list.Count);

            Assert.IsTrue(list.IndexOf("1") < list.IndexOf("0"));

            Assert.Pass(ListToString(list));
        }

        [Test]
        public void Test_AfterOnlySortPrefrence_BA_GetExpectedOrder()
        {
            var tree = new SortedCollection<string, TestData>();

            var iA = new TestData(0);

            var iB = new TestData(1);
            iB.LoadAfterPreferences.Add("0");

            tree.AddSorted(iB);
            tree.AddSorted(iA);
            tree.AddSorted(new TestData(2));

            List<string> list = tree.GetSortedIndexList();
            Console.WriteLine(ListToString(list));

            Assert.AreEqual(3, list.Count);

            Assert.IsTrue(list.IndexOf("1") < list.IndexOf("0"));

            Assert.Pass(ListToString(list));
        }

        // TODO - Detecting circular load order and preferences before sorting

        [Test]
        public void TestDependencies_UsingRealData_ConfirmAllEntriesIncluded_ConfirmCorrectOrder()
        {
            var tree = new SortedCollection<string, TestData>();
            /* 01 */
            tree.AddSorted(new TestData("AcceleratedStart"));
            /* 02 */
            tree.AddSorted(new TestData("AutosortLockers"));
            /* 03 */
            tree.AddSorted(new TestData("BaseLightSwitch", "SMLHelper"));
            /* 04 */
            tree.AddSorted(new TestData("BetterBioReactor"));
            /* 05 */
            tree.AddSorted(new TestData("BiomeHUDIndicator", "SMLHelper"));
            /* 06 */
            tree.AddSorted(new TestData("BuilderModule", "SMLHelper"));
            /* 07 */
            tree.AddSorted(new TestData("BuilderModuleInputFix", "SMLHelper"));
            /* 08 */
            tree.AddSorted(new TestData("CustomBatteries", "SMLHelper"));
            /* 09 */
            tree.AddSorted(new TestData("CustomCraft2SML", "SMLHelper"));
            /* 10 */
            tree.AddSorted(new TestData("CustomizedStorage"));
            /* 11 */
            tree.AddSorted(new TestData("CyclopsAutoZapper", "SMLHelper", "MoreCyclopsUpgrades"));
            /* 12 */
            tree.AddSorted(new TestData("CyclopsBioReactor", "SMLHelper", "MoreCyclopsUpgrades"));
            /* 13 */
            tree.AddSorted(new TestData("CyclopsEngineUpgrades", "SMLHelper", "MoreCyclopsUpgrades"));
            /* 14 */
            tree.AddSorted(new TestData("CyclopsLaserCannonModule", "SMLHelper", "MoreCyclopsUpgrades"));
            /* 15 */
            tree.AddSorted(new TestData("CyclopsNuclearReactor", "SMLHelper", "MoreCyclopsUpgrades"));
            /* 16 */
            tree.AddSorted(new TestData("CyclopsNuclearUpgrades", "SMLHelper", "MoreCyclopsUpgrades"));
            /* 17 */
            tree.AddSorted(new TestData("CyclopsSolarUpgrades", "SMLHelper", "MoreCyclopsUpgrades"));
            /* 18 */
            tree.AddSorted(new TestData("CyclopsSpeedUpgrades", "SMLHelper", "MoreCyclopsUpgrades"));
            /* 19 */
            tree.AddSorted(new TestData("CyclopsThermalUpgrades", "SMLHelper", "MoreCyclopsUpgrades"));
            /* 20 */
            tree.AddSorted(new TestData("DockedVehicleStorageAccess"));
            /* 21 */
            tree.AddSorted(new TestData("EasyCraft"));
            /* 22 */
            tree.AddSorted(new TestData("EnzymeChargedBattery", "SMLHelper"));
            /* 23 */
            tree.AddSorted(new TestData("ExteriorPlantPots", "SMLHelper"));
            /* 24 */
            tree.AddSorted(new TestData("FCSDeepDriller", "SMLHelper"));
            /* 25 */
            tree.AddSorted(new TestData("AIMarineTurbine", "SMLHelper"));
            /* 26 */
            tree.AddSorted(new TestData("MiniFountainFilter", "SMLHelper"));
            /* 27 */
            tree.AddSorted(new TestData("FCSAIPowerCellSocket", "SMLHelper"));
            /* 28 */
            tree.AddSorted(new TestData("FCSPowerStorage", "SMLHelper"));
            /* 29 */
            tree.AddSorted(new TestData("FCSTechFabricator", "SMLHelper"));
            /* 30 */
            tree.AddSorted(new TestData("snowrabbit007_subnautica_FishOverflowDistributor"));
            /* 31 */
            tree.AddSorted(new TestData("FloatingCargoCrate", "SMLHelper"));
            /* 32 */
            tree.AddSorted(new TestData("HabitatControlPanel"));
            /* 33 */
            tree.AddSorted(new TestData("InstantBulkheadAnimations"));
            /* 34 */
            tree.AddSorted(new TestData("IonCubeGenerator", "SMLHelper"));
            /* 35 */
            tree.AddSorted(new TestData("LaserCannon", "SMLHelper"));
            /* 36 */
            tree.AddSorted(new TestData("SMLHelper"));
            /* 37 */
            tree.AddSorted(new TestData("MoonpoolVehicleRepair"));
            /* 38 */
            tree.AddSorted(new TestData("MoreCyclopsUpgrades", "SMLHelper"));
            /* 39 */
            tree.AddSorted(new TestData("MoreQuickSlots"));
            /* 40 */
            tree.AddSorted(new TestData("MoreSeamothDepth", "SMLHelper"));
            /* 41 */
            tree.AddSorted(new TestData("PrawnSuitSonarUpgrade", "SMLHelper"));
            /* 42 */
            tree.AddSorted(new TestData("QPrawnUpgradeAccess"));
            /* 43 */
            tree.AddSorted(new TestData("QuitToDesktop"));
            /* 44 */
            tree.AddSorted(new TestData("AgonyRadialCraftingTabs"));
            /* 45 */
            tree.AddSorted(new TestData("RepairModule", "SMLHelper"));
            /* 46 */
            tree.AddSorted(new TestData("ResourceMonitor", "SMLHelper"));
            /* 47 */
            tree.AddSorted(new TestData("RunningWithTools"));
            /* 48 */
            tree.AddSorted(new TestData("ScannerModule", "SMLHelper"));
            /* 49 */
            tree.AddSorted(new TestData("SeamothArms", "SMLHelper"));
            /* 50 */
            tree.AddSorted(new TestData("SeamothEnergyShield", "SMLHelper"));
            /* 51 */
            tree.AddSorted(new TestData("SeamothStorageAccess"));
            /* 52 */
            tree.AddSorted(new TestData("SeamothThermal", "SMLHelper"));
            /* 53 */
            tree.AddSorted(new TestData("SlotExtender"));
            /* 54 */
            tree.AddSorted(new TestData("TimeCapsuleLogger"));
            /* 55 */
            tree.AddSorted(new TestData("UniversalChargingModule", "SMLHelper"));
            /* 56 */
            tree.AddSorted(new TestData("UpgradedVehicles", "SMLHelper"));
            /* 57 */
            tree.AddSorted(new TestData("VehicleUpgradesInCyclops", "SMLHelper"));

            List<TestData> list = tree.GetSortedList();

            Console.WriteLine(ListToString(list));

            foreach (string item in tree.KnownNodes)
            {
                Assert.IsNotNull(list.Find(n => n.Id == item), item + " was missing");
            }

            Assert.AreEqual(57, list.Count);

            foreach (SortedTreeNode<string, TestData> node in tree.NodesToSort.Values)
            {
                if (node.Dependencies.Count == 0)
                    continue;

                int indexOfNode = list.IndexOf(node.Data);
                foreach (string dependency in node.Dependencies)
                {
                    int indexOfDependency = list.FindIndex(d => d.Id == dependency);

                    Assert.IsTrue(indexOfNode < indexOfDependency);
                }
            }
        }

        private class TestDependencies : ISortable<string>
        {
            public TestDependencies(string id)
            {
                this.Id = id;
            }

            public TestDependencies(string id, params string[] dependencies)
               : this(id)
            {
                foreach (string dependency in dependencies)
                    this.RequiredDependencies.Add(dependency);
            }

            public string Id { get; }

            public IList<string> RequiredDependencies { get; } = new List<string>();
            public IList<string> LoadBeforePreferences { get; } = new List<string>();
            public IList<string> LoadAfterPreferences { get; } = new List<string>();
        }

        private class TestLoadBefore : ISortable<string>
        {
            public TestLoadBefore(string id)
            {
                this.Id = id;
            }

            public TestLoadBefore(string id, params string[] loadBefore)
               : this(id)
            {
                foreach (string before in loadBefore)
                    this.LoadBeforePreferences.Add(before);
            }

            public string Id { get; }

            public IList<string> RequiredDependencies { get; } = new List<string>();
            public IList<string> LoadBeforePreferences { get; } = new List<string>();
            public IList<string> LoadAfterPreferences { get; } = new List<string>();
        }

        [Test]
        public void CleanRedundantDependencies_SingleRedundancy_RedundanciesCleared()
        {
            var rng = new Random();
            const string innerDependency = "mostUsedDependency";
            const string outterDependency = "redundant";
            var entries = new List<TestDependencies>
            {
                new TestDependencies(innerDependency),
                new TestDependencies(outterDependency, innerDependency),
                new TestDependencies("commonEntry1", innerDependency),
                new TestDependencies("commonEntry2", innerDependency, outterDependency),
                new TestDependencies("commonEntry3", innerDependency, outterDependency),
                new TestDependencies("commonEntry4"),
                new TestDependencies("commonEntry5", innerDependency, outterDependency),
                new TestDependencies("commonEntry6", innerDependency, outterDependency),
                new TestDependencies("commonEntry7", innerDependency),
            };
            int originalCount = entries.Count;
            var tree = new SortedCollection<string, TestDependencies>();

            // Add entries in random order
            while (entries.Count > 0)
            {
                int rngSelected = rng.Next(0, entries.Count);
                Assert.IsTrue(tree.AddSorted(entries[rngSelected]));
                entries.RemoveAt(rngSelected);
            }

            Assert.AreEqual(originalCount, tree.Count);

            tree.CleanRedundantDependencies();

            Assert.AreEqual(originalCount, tree.Count);
            Assert.AreEqual(4, tree.DependencyUsedBy(outterDependency));
            Assert.AreEqual(3, tree.DependencyUsedBy(innerDependency));

            Assert.IsTrue(tree[outterDependency].RequiredDependencies.Contains(innerDependency));

            Assert.IsTrue(tree["commonEntry1"].RequiredDependencies.Contains(innerDependency));
            Assert.IsTrue(tree["commonEntry2"].RequiredDependencies.Contains(outterDependency));
            Assert.IsTrue(tree["commonEntry3"].RequiredDependencies.Contains(outterDependency));

            Assert.IsTrue(tree["commonEntry5"].RequiredDependencies.Contains(outterDependency));
            Assert.IsTrue(tree["commonEntry6"].RequiredDependencies.Contains(outterDependency));
            Assert.IsTrue(tree["commonEntry7"].RequiredDependencies.Contains(innerDependency));

            Assert.IsFalse(tree["commonEntry2"].RequiredDependencies.Contains(innerDependency));
            Assert.IsFalse(tree["commonEntry3"].RequiredDependencies.Contains(innerDependency));

            Assert.IsFalse(tree["commonEntry5"].RequiredDependencies.Contains(innerDependency));
            Assert.IsFalse(tree["commonEntry6"].RequiredDependencies.Contains(innerDependency));
        }

        [Test]
        public void CleanRedundantDependencies_MultipleRedundancy_RedundanciesCleared()
        {
            var rng = new Random();
            const string innerDependency = "mostUsedDependency";
            const string outterDep1 = "redundant1";
            const string outterDep2 = "redundant2";
            const string outterDep3 = "redundant3";
            var entries = new List<TestDependencies>
            {
                new TestDependencies(innerDependency),
                new TestDependencies(outterDep1, innerDependency),
                new TestDependencies(outterDep2, innerDependency, outterDep1),
                new TestDependencies(outterDep3, outterDep2, innerDependency),
                new TestDependencies("commonEntry1", innerDependency),
                new TestDependencies("commonEntry1a", outterDep2, innerDependency),
                new TestDependencies("commonEntry2", innerDependency, outterDep1),
                new TestDependencies("commonEntry2a", innerDependency, outterDep1, outterDep2),
                new TestDependencies("commonEntry3", innerDependency, outterDep2, outterDep3, outterDep1),
                new TestDependencies("commonEntry3a", innerDependency, outterDep1),
                new TestDependencies("commonEntry4"),
                new TestDependencies("commonEntry5", innerDependency, outterDep2),
                new TestDependencies("commonEntry5a", innerDependency, outterDep1, outterDep2, outterDep3),
                new TestDependencies("commonEntry6", innerDependency, outterDep1),
                new TestDependencies("commonEntry6a", innerDependency, outterDep2, outterDep1),
                new TestDependencies("commonEntry7", innerDependency),
            };
            int originalCount = entries.Count;
            var tree = new SortedCollection<string, TestDependencies>();

            // Add entries in random order
            while (entries.Count > 0)
            {
                int rngSelected = rng.Next(0, entries.Count);
                Assert.IsTrue(tree.AddSorted(entries[rngSelected]));
                entries.RemoveAt(rngSelected);
            }

            Assert.AreEqual(originalCount, tree.Count);

            tree.CleanRedundantDependencies();

            Assert.AreEqual(originalCount, tree.Count);

            Assert.IsTrue(tree[outterDep1].RequiredDependencies.Contains(innerDependency));
            Assert.IsTrue(tree[outterDep2].RequiredDependencies.Contains(outterDep1));
            Assert.IsTrue(tree[outterDep3].RequiredDependencies.Contains(outterDep2));

            Assert.IsFalse(tree["commonEntry1a"].RequiredDependencies.Contains(innerDependency));

            Assert.IsFalse(tree["commonEntry2"].RequiredDependencies.Contains(innerDependency));

            Assert.IsFalse(tree["commonEntry2a"].RequiredDependencies.Contains(innerDependency));
            Assert.IsFalse(tree["commonEntry2a"].RequiredDependencies.Contains(outterDep1));

            Assert.IsFalse(tree["commonEntry3"].RequiredDependencies.Contains(innerDependency));
            Assert.IsFalse(tree["commonEntry3"].RequiredDependencies.Contains(outterDep1));
            Assert.IsFalse(tree["commonEntry3"].RequiredDependencies.Contains(outterDep2));

            Assert.IsFalse(tree["commonEntry3a"].RequiredDependencies.Contains(innerDependency));

            Assert.IsFalse(tree["commonEntry5"].RequiredDependencies.Contains(outterDep1));
            Assert.IsFalse(tree["commonEntry5"].RequiredDependencies.Contains(innerDependency));

            Assert.IsFalse(tree["commonEntry5a"].RequiredDependencies.Contains(innerDependency));
            Assert.IsFalse(tree["commonEntry5a"].RequiredDependencies.Contains(outterDep1));
            Assert.IsFalse(tree["commonEntry5a"].RequiredDependencies.Contains(outterDep2));

            Assert.IsFalse(tree["commonEntry6a"].RequiredDependencies.Contains(innerDependency));
            Assert.IsFalse(tree["commonEntry6a"].RequiredDependencies.Contains(outterDep1));

            Assert.AreEqual(4, tree.DependencyUsedBy(outterDep1));
            Assert.AreEqual(5, tree.DependencyUsedBy(outterDep2));
            Assert.AreEqual(2, tree.DependencyUsedBy(outterDep3));
            Assert.AreEqual(3, tree.DependencyUsedBy(innerDependency));
        }

        [Test]
        public void CleanRedundantLoadBefore_SingleRedundancy_RedundanciesCleared()
        {
            var rng = new Random();
            const string innerDependency = "mostUsedDependency";
            const string outterDependency = "redundant";
            var entries = new List<TestLoadBefore>
            {
                new TestLoadBefore(innerDependency),
                new TestLoadBefore(outterDependency, innerDependency),
                new TestLoadBefore("commonEntry1", innerDependency),
                new TestLoadBefore("commonEntry2", innerDependency, outterDependency),
                new TestLoadBefore("commonEntry3", innerDependency, outterDependency),
                new TestLoadBefore("commonEntry4"),
                new TestLoadBefore("commonEntry5", innerDependency, outterDependency),
                new TestLoadBefore("commonEntry6", innerDependency, outterDependency),
                new TestLoadBefore("commonEntry7", innerDependency),
            };
            int originalCount = entries.Count;
            var tree = new SortedCollection<string, TestLoadBefore>();

            // Add entries in random order
            while (entries.Count > 0)
            {
                int rngSelected = rng.Next(0, entries.Count);
                Assert.IsTrue(tree.AddSorted(entries[rngSelected]));
                entries.RemoveAt(rngSelected);
            }

            Assert.AreEqual(originalCount, tree.Count);

            tree.CleanRedundantLoadBefore();

            Assert.AreEqual(originalCount, tree.Count);

            Assert.IsTrue(tree[outterDependency].LoadBeforePreferences.Contains(innerDependency));

            Assert.IsTrue(tree["commonEntry1"].LoadBeforePreferences.Contains(innerDependency));
            Assert.IsTrue(tree["commonEntry2"].LoadBeforePreferences.Contains(outterDependency));
            Assert.IsTrue(tree["commonEntry3"].LoadBeforePreferences.Contains(outterDependency));

            Assert.IsTrue(tree["commonEntry5"].LoadBeforePreferences.Contains(outterDependency));
            Assert.IsTrue(tree["commonEntry6"].LoadBeforePreferences.Contains(outterDependency));
            Assert.IsTrue(tree["commonEntry7"].LoadBeforePreferences.Contains(innerDependency));

            Assert.IsFalse(tree["commonEntry2"].LoadBeforePreferences.Contains(innerDependency));
            Assert.IsFalse(tree["commonEntry3"].LoadBeforePreferences.Contains(innerDependency));

            Assert.IsFalse(tree["commonEntry5"].LoadBeforePreferences.Contains(innerDependency));
            Assert.IsFalse(tree["commonEntry6"].LoadBeforePreferences.Contains(innerDependency));
        }

        [Test]
        public void CleanRedundantLoadBefore_MultipleRedundancy_RedundanciesCleared()
        {
            var rng = new Random();
            const string innerDependency = "mostUsedDependency";
            const string outterDep1 = "redundant1";
            const string outterDep2 = "redundant2";
            const string outterDep3 = "redundant3";
            var entries = new List<TestLoadBefore>
            {
                new TestLoadBefore(innerDependency),
                new TestLoadBefore(outterDep1, innerDependency),
                new TestLoadBefore(outterDep2, innerDependency, outterDep1),
                new TestLoadBefore(outterDep3, outterDep2, innerDependency),
                new TestLoadBefore("commonEntry1", innerDependency),
                new TestLoadBefore("commonEntry1a", outterDep2, innerDependency),
                new TestLoadBefore("commonEntry2", innerDependency, outterDep1),
                new TestLoadBefore("commonEntry2a", innerDependency, outterDep1, outterDep2),
                new TestLoadBefore("commonEntry3", innerDependency, outterDep2, outterDep3, outterDep1),
                new TestLoadBefore("commonEntry3a", innerDependency, outterDep1),
                new TestLoadBefore("commonEntry4"),
                new TestLoadBefore("commonEntry5", innerDependency, outterDep2),
                new TestLoadBefore("commonEntry5a", innerDependency, outterDep1, outterDep2, outterDep3),
                new TestLoadBefore("commonEntry6", innerDependency, outterDep1),
                new TestLoadBefore("commonEntry6a", innerDependency, outterDep2, outterDep1),
                new TestLoadBefore("commonEntry7", innerDependency),
            };
            int originalCount = entries.Count;
            var tree = new SortedCollection<string, TestLoadBefore>();

            // Add entries in random order
            while (entries.Count > 0)
            {
                int rngSelected = rng.Next(0, entries.Count);
                Assert.IsTrue(tree.AddSorted(entries[rngSelected]));
                entries.RemoveAt(rngSelected);
            }

            Assert.AreEqual(originalCount, tree.Count);

            tree.CleanRedundantLoadBefore();

            Assert.AreEqual(originalCount, tree.Count);

            Assert.IsTrue(tree[outterDep1].LoadBeforePreferences.Contains(innerDependency));
            Assert.IsTrue(tree[outterDep2].LoadBeforePreferences.Contains(outterDep1));
            Assert.IsTrue(tree[outterDep3].LoadBeforePreferences.Contains(outterDep2));

            Assert.IsFalse(tree["commonEntry1a"].LoadBeforePreferences.Contains(innerDependency));

            Assert.IsFalse(tree["commonEntry2"].LoadBeforePreferences.Contains(innerDependency));

            Assert.IsFalse(tree["commonEntry2a"].LoadBeforePreferences.Contains(innerDependency));
            Assert.IsFalse(tree["commonEntry2a"].LoadBeforePreferences.Contains(outterDep1));

            Assert.IsFalse(tree["commonEntry3"].LoadBeforePreferences.Contains(innerDependency));
            Assert.IsFalse(tree["commonEntry3"].LoadBeforePreferences.Contains(outterDep1));
            Assert.IsFalse(tree["commonEntry3"].LoadBeforePreferences.Contains(outterDep2));

            Assert.IsFalse(tree["commonEntry3a"].LoadBeforePreferences.Contains(innerDependency));

            Assert.IsFalse(tree["commonEntry5"].LoadBeforePreferences.Contains(outterDep1));
            Assert.IsFalse(tree["commonEntry5"].LoadBeforePreferences.Contains(innerDependency));

            Assert.IsFalse(tree["commonEntry5a"].LoadBeforePreferences.Contains(innerDependency));
            Assert.IsFalse(tree["commonEntry5a"].LoadBeforePreferences.Contains(outterDep1));
            Assert.IsFalse(tree["commonEntry5a"].LoadBeforePreferences.Contains(outterDep2));

            Assert.IsFalse(tree["commonEntry6a"].LoadBeforePreferences.Contains(innerDependency));
            Assert.IsFalse(tree["commonEntry6a"].LoadBeforePreferences.Contains(outterDep1));

        }

        private static string ListToString<T>(IList<T> list)
        {
            string s = "List: ";
            if (list.Count == 0)
            {
                s += "Empty";
                return s;
            }

            for (int i = 0; i < list.Count; i++)
            {
                T item = list[i];
                s += Environment.NewLine;
                s += item;
            }

            return s;
        }
    }

}
