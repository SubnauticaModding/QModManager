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

            public ICollection<string> DependencyCollection => depends;

            public ICollection<string> LoadBeforeCollection => loadBefore;

            public ICollection<string> LoadAfterCollection => loadAfter;

            public PatchingOrder LoadPriority { get; } = PatchingOrder.NormalInitialize;
        }

        [Test]
        public void Test_NoPreferences_GetExpectedOrder()
        {
            var tree = new SortedTree<string, TestData>();

            tree.Add(new TestData(0));
            tree.Add(new TestData(1));
            tree.Add(new TestData(2));

            List<string> list = tree.CreateFlatIndexList(out PairedList<TestData, ErrorTypes> errors);
            Console.WriteLine(ListToString(list));

            Assert.AreEqual(3, list.Count);

            Assert.IsTrue(list.IndexOf("0") < list.IndexOf("1"));
            Assert.IsTrue(list.IndexOf("1") < list.IndexOf("2"));

            Assert.Pass(ListToString(list));
        }

        [Test]
        public void Test_DupId_A_GetError()
        {
            var tree = new SortedTree<string, TestData>();

            tree.Add(new TestData(0));
            tree.Add(new TestData(0));
            tree.Add(new TestData(1));

            List<string> list = tree.CreateFlatIndexList(out PairedList<TestData, ErrorTypes> errors);
            Console.WriteLine(ListToString(list));

            Assert.AreEqual(1, list.Count);

            Assert.AreEqual("1", list[0]);

            Assert.Pass(ListToString(list));
        }

        [Test]
        public void Test_DupId_B_GetError()
        {
            var tree = new SortedTree<string, TestData>();

            tree.Add(new TestData(0));
            tree.Add(new TestData(1));
            tree.Add(new TestData(0));

            List<string> list = tree.CreateFlatIndexList(out PairedList<TestData, ErrorTypes> errors);
            Console.WriteLine(ListToString(list));

            Assert.AreEqual(1, list.Count);

            Assert.AreEqual("1", list[0]);

            Assert.Pass(ListToString(list));
        }

        [Test]
        public void Test_DupId_C_GetError()
        {
            var tree = new SortedTree<string, TestData>();

            var i1 = new TestData(1);
            var i0 = new TestData(0);
            var i02 = new TestData(0);

            tree.Add(i1);
            tree.Add(i0);
            tree.Add(i02);

            List<string> list = tree.CreateFlatIndexList(out PairedList<TestData, ErrorTypes> errors);
            Console.WriteLine(ListToString(list));

            Assert.AreEqual(1, list.Count);

            Assert.AreEqual("1", list[0]);

            Assert.Pass(ListToString(list));
        }

        [Test]
        public void Test_MissingDependency_GetExpectedOrder()
        {
            var tree = new SortedTree<string, TestData>();

            var entity = new TestData(0);
            entity.DependencyCollection.Add("1");

            tree.Add(entity);

            List<string> list = tree.CreateFlatIndexList(out PairedList<TestData, ErrorTypes> errors);
            Console.WriteLine(ListToString(list));

            Assert.AreEqual(0, list.Count);

            Assert.Pass(ListToString(list));
        }

        //-------

        [Test]
        public void Test_MutualSortPrefrence_AB_GetExpectedOrder()
        {
            var tree = new SortedTree<string, TestData>();

            var iA = new TestData(0);
            iA.LoadBeforeCollection.Add("1");

            var iB = new TestData(1);
            iB.LoadAfterCollection.Add("0");

            tree.Add(iA);
            tree.Add(iB);
            tree.Add(new TestData(2));

            List<string> list = tree.CreateFlatIndexList(out PairedList<TestData, ErrorTypes> errors);
            Console.WriteLine(ListToString(list));

            Assert.AreEqual(3, list.Count);

            Assert.IsTrue(list.IndexOf("1") < list.IndexOf("0"));

            Assert.Pass(ListToString(list));
        }

        [Test]
        public void Test_MutualSortPrefrence_BA_GetExpectedOrder()
        {
            var tree = new SortedTree<string, TestData>();

            var iA = new TestData(0);
            iA.LoadBeforeCollection.Add("1");

            var iB = new TestData(1);
            iB.LoadAfterCollection.Add("0");

            tree.Add(iB);
            tree.Add(iA);
            tree.Add(new TestData(2));

            List<string> list = tree.CreateFlatIndexList(out PairedList<TestData, ErrorTypes> errors);
            Console.WriteLine(ListToString(list));

            Assert.AreEqual(3, list.Count);

            Assert.IsTrue(list.IndexOf("1") < list.IndexOf("0"));

            Assert.Pass(ListToString(list));
        }

        [Test]
        public void Test_MutualSortPrefrence_ACB_GetExpectedOrder()
        {
            var tree = new SortedTree<string, TestData>();

            var iA = new TestData(0);
            iA.LoadBeforeCollection.Add("1");

            var iB = new TestData(1);
            iB.LoadAfterCollection.Add("0");

            tree.Add(iA);
            tree.Add(new TestData(2));
            tree.Add(iB);

            List<string> list = tree.CreateFlatIndexList(out PairedList<TestData, ErrorTypes> errors);
            Console.WriteLine(ListToString(list));

            Assert.AreEqual(3, list.Count);

            Assert.IsTrue(list.IndexOf("1") < list.IndexOf("0"));

            Assert.Pass(ListToString(list));
        }

        [Test]
        public void Test_MutualSortPrefrence_BCA_GetExpectedOrder()
        {
            var tree = new SortedTree<string, TestData>();

            var iA = new TestData(0);
            iA.LoadBeforeCollection.Add("1");

            var iB = new TestData(1);
            iB.LoadAfterCollection.Add("0");

            tree.Add(iB);
            tree.Add(new TestData(2));
            tree.Add(iA);

            List<string> list = tree.CreateFlatIndexList(out PairedList<TestData, ErrorTypes> errors);
            Console.WriteLine(ListToString(list));

            Assert.AreEqual(3, list.Count);

            Assert.IsTrue(list.IndexOf("1") < list.IndexOf("0"));

            Assert.Pass(ListToString(list));
        }

        //-------

        [Test]
        public void Test_BeforeOnlySortPrefrence_AB_GetExpectedOrder()
        {
            var tree = new SortedTree<string, TestData>();

            var iA = new TestData(0);
            iA.LoadBeforeCollection.Add("1");

            var iB = new TestData(1);

            tree.Add(iA);
            tree.Add(iB);
            tree.Add(new TestData(2));

            List<string> list = tree.CreateFlatIndexList(out PairedList<TestData, ErrorTypes> errors);
            Console.WriteLine(ListToString(list));

            Assert.AreEqual(3, list.Count);

            Assert.IsTrue(list.IndexOf("1") < list.IndexOf("0"));

            Assert.Pass(ListToString(list));
        }

        [Test]
        public void Test_BeforeOnlySortPrefrence_BA_GetExpectedOrder()
        {
            var tree = new SortedTree<string, TestData>();

            var iA = new TestData(0);
            iA.LoadBeforeCollection.Add("1");

            var iB = new TestData(1);

            tree.Add(iB);
            tree.Add(iA);
            tree.Add(new TestData(2));

            List<string> list = tree.CreateFlatIndexList(out PairedList<TestData, ErrorTypes> errors);
            Console.WriteLine(ListToString(list));

            Assert.AreEqual(3, list.Count);

            Assert.IsTrue(list.IndexOf("1") < list.IndexOf("0"));

            Assert.Pass(ListToString(list));
        }

        [Test]
        public void Test_BeforeOnlySortPrefrence_ACB_GetExpectedOrder()
        {
            var tree = new SortedTree<string, TestData>();

            var iA = new TestData(0);
            iA.LoadBeforeCollection.Add("1");

            var iB = new TestData(1);

            tree.Add(iA);
            tree.Add(new TestData(2));
            tree.Add(iB);

            List<string> list = tree.CreateFlatIndexList(out PairedList<TestData, ErrorTypes> errors);
            Console.WriteLine(ListToString(list));

            Assert.AreEqual(3, list.Count);

            Assert.IsTrue(list.IndexOf("1") < list.IndexOf("0"));

            Assert.Pass(ListToString(list));
        }

        [Test]
        public void Test_BeforeOnlySortPrefrence_BCA_GetExpectedOrder()
        {
            var tree = new SortedTree<string, TestData>();

            var iA = new TestData(0);
            iA.LoadBeforeCollection.Add("1");

            var iB = new TestData(1);

            tree.Add(iB);
            tree.Add(new TestData(2));
            tree.Add(iA);

            List<string> list = tree.CreateFlatIndexList(out PairedList<TestData, ErrorTypes> errors);
            Console.WriteLine(ListToString(list));

            Assert.AreEqual(3, list.Count);

            Assert.IsTrue(list.IndexOf("1") < list.IndexOf("0"));

            Assert.Pass(ListToString(list));
        }

        //-------

        [Test]
        public void Test_AfterOnlySortPrefrence_AB_GetExpectedOrder()
        {
            var tree = new SortedTree<string, TestData>();

            var iA = new TestData(0);

            var iB = new TestData(1);
            iB.LoadAfterCollection.Add("0");

            tree.Add(iA);
            tree.Add(iB);
            tree.Add(new TestData(2));

            List<string> list = tree.CreateFlatIndexList(out PairedList<TestData, ErrorTypes> errors);
            Console.WriteLine(ListToString(list));

            Assert.AreEqual(3, list.Count);

            Assert.IsTrue(list.IndexOf("1") < list.IndexOf("0"));

            Assert.Pass(ListToString(list));
        }

        [Test]
        public void Test_AfterOnlySortPrefrence_BA_GetExpectedOrder()
        {
            var tree = new SortedTree<string, TestData>();

            var iA = new TestData(0);

            var iB = new TestData(1);
            iB.LoadAfterCollection.Add("0");

            tree.Add(iB);
            tree.Add(iA);
            tree.Add(new TestData(2));

            List<string> list = tree.CreateFlatIndexList(out PairedList<TestData, ErrorTypes> errors);
            Console.WriteLine(ListToString(list));

            Assert.AreEqual(3, list.Count);

            Assert.IsTrue(list.IndexOf("1") < list.IndexOf("0"));

            Assert.Pass(ListToString(list));
        }

        //-------

        [Test]
        public void Test_CircularLoadOrder_BothLoadBeforeRequirements_AB_GetError()
        {
            var tree = new SortedTree<string, TestData>();

            var iA = new TestData(0);
            iA.LoadBeforeCollection.Add("1");

            var iB = new TestData(1);
            iB.LoadBeforeCollection.Add("0");

            tree.Add(iA);
            SortResults result = tree.Add(iB);

            Assert.AreEqual(SortResults.CircularLoadOrder, result);

            List<string> list = tree.CreateFlatIndexList(out PairedList<TestData, ErrorTypes> errors);
            Console.WriteLine(ListToString(list));

            Assert.AreEqual(0, list.Count);

            Assert.Pass(ListToString(list));
        }

        [Test]
        public void Test_CircularLoadOrder_BothLoadBeforeRequirements_BA_GetError()
        {
            var tree = new SortedTree<string, TestData>();

            var iA = new TestData(0);
            iA.LoadBeforeCollection.Add("1");

            var iB = new TestData(1);
            iB.LoadBeforeCollection.Add("0");

            tree.Add(iB);
            SortResults result = tree.Add(iA);

            Assert.AreEqual(SortResults.CircularLoadOrder, result);

            List<string> list = tree.CreateFlatIndexList(out PairedList<TestData, ErrorTypes> errors);
            Console.WriteLine(ListToString(list));

            Assert.AreEqual(0, list.Count);

            Assert.Pass(ListToString(list));
        }

        [Test]
        public void Test_CircularLoadOrder_BothLoadBeforeRequirements_ACB_GetError()
        {
            var tree = new SortedTree<string, TestData>();

            var iA = new TestData(0);
            iA.LoadBeforeCollection.Add("1");

            var iB = new TestData(1);
            iB.LoadBeforeCollection.Add("0");

            tree.Add(iA);
            tree.Add(new TestData(2));
            SortResults result = tree.Add(iB);

            Assert.AreEqual(SortResults.CircularLoadOrder, result);

            List<string> list = tree.CreateFlatIndexList(out PairedList<TestData, ErrorTypes> errors);
            Console.WriteLine(ListToString(list));

            Assert.AreEqual(1, list.Count);

            Assert.Pass(ListToString(list));
        }

        [Test]
        public void Test_CircularLoadOrder_BothLoadBeforeRequirements_BCA_GetError()
        {
            var tree = new SortedTree<string, TestData>();

            var iA = new TestData(0);
            iA.LoadBeforeCollection.Add("1");

            var iB = new TestData(1);
            iB.LoadBeforeCollection.Add("0");

            tree.Add(iA);
            tree.Add(new TestData(2));
            SortResults result = tree.Add(iB);

            Assert.AreEqual(SortResults.CircularLoadOrder, result);

            List<string> list = tree.CreateFlatIndexList(out PairedList<TestData, ErrorTypes> errors);
            Console.WriteLine(ListToString(list));

            Assert.AreEqual(1, list.Count);

            Assert.Pass(ListToString(list));
        }

        //-------

        [Test]
        public void Test_CircularLoadOrder_BothLoadAfterRequirements_AB_GetError()
        {
            var tree = new SortedTree<string, TestData>();

            var iA = new TestData(0);
            iA.LoadAfterCollection.Add("1");

            var iB = new TestData(1);
            iB.LoadAfterCollection.Add("0");

            tree.Add(iA);
            SortResults result = tree.Add(iB);

            Assert.AreEqual(SortResults.CircularLoadOrder, result);

            List<string> list = tree.CreateFlatIndexList(out PairedList<TestData, ErrorTypes> errors);
            Console.WriteLine(ListToString(list));

            Assert.AreEqual(0, list.Count);

            Assert.Pass(ListToString(list));
        }

        [Test]
        public void Test_CircularLoadOrder_BothLoadAfterRequirements_BA_GetError()
        {
            var tree = new SortedTree<string, TestData>();

            var iA = new TestData(0);
            iA.LoadAfterCollection.Add("1");

            var iB = new TestData(1);
            iB.LoadAfterCollection.Add("0");

            tree.Add(iB);
            SortResults result = tree.Add(iA);

            Assert.AreEqual(SortResults.CircularLoadOrder, result);

            List<string> list = tree.CreateFlatIndexList(out PairedList<TestData, ErrorTypes> errors);
            Console.WriteLine(ListToString(list));

            Assert.AreEqual(0, list.Count);

            Assert.Pass(ListToString(list));
        }

        [Test]
        public void Test_CircularLoadOrder_BothLoadAfterRequirements_ACB_GetError()
        {
            var tree = new SortedTree<string, TestData>();

            var iA = new TestData(0);
            iA.LoadAfterCollection.Add("1");

            var iB = new TestData(1);
            iB.LoadAfterCollection.Add("0");

            tree.Add(iA);
            tree.Add(new TestData(2));
            SortResults result = tree.Add(iB);

            Assert.AreEqual(SortResults.CircularLoadOrder, result);

            List<string> list = tree.CreateFlatIndexList(out PairedList<TestData, ErrorTypes> errors);
            Console.WriteLine(ListToString(list));

            Assert.AreEqual(1, list.Count);

            Assert.Pass(ListToString(list));
        }

        [Test]
        public void Test_CircularLoadOrder_BothLoadAfterRequirements_BCA_GetError()
        {
            var tree = new SortedTree<string, TestData>();

            var iA = new TestData(0);
            iA.LoadAfterCollection.Add("1");

            var iB = new TestData(1);
            iB.LoadAfterCollection.Add("0");

            tree.Add(iA);
            tree.Add(new TestData(2));
            SortResults result = tree.Add(iB);

            Assert.AreEqual(SortResults.CircularLoadOrder, result);

            List<string> list = tree.CreateFlatIndexList(out PairedList<TestData, ErrorTypes> errors);
            Console.WriteLine(ListToString(list));

            Assert.AreEqual(1, list.Count);

            Assert.Pass(ListToString(list));
        }

        //-------

        [Test]
        public void Test_CircularLoadOrder_DependencyChain3inLoop_AllLoadAfterRequirements_GetError()
        {
            // A -> B ~ B -> C ~ C -> A
            var tree = new SortedTree<string, TestData>();

            var iA = new TestData(0);
            iA.LoadAfterCollection.Add("1");

            var iB = new TestData(1);
            iB.LoadAfterCollection.Add("2");

            var iC = new TestData(2);
            iC.LoadAfterCollection.Add("0");

            tree.Add(iA);
            tree.Add(iB);
            tree.Add(iC);

            List<string> list = tree.CreateFlatIndexList(out PairedList<TestData, ErrorTypes> errors);
            Console.WriteLine(ListToString(list));

            Assert.AreEqual(0, list.Count);

            Assert.Pass(ListToString(list));
        }

        [Test]
        public void Test_CircularLoadOrder_DependencyChain4inLoop_AllLoadAfterRequirements_GetError()
        {
            // A -> B ~ B -> C ~ C -> D ~ D -> A
            var tree = new SortedTree<string, TestData>();

            var iA = new TestData(0);
            iA.LoadAfterCollection.Add("1");

            var iB = new TestData(1);
            iB.LoadAfterCollection.Add("2");

            var iC = new TestData(2);
            iC.LoadAfterCollection.Add("3");

            var iD = new TestData(3);
            iD.LoadAfterCollection.Add("0");

            tree.Add(iA);
            tree.Add(iB);
            tree.Add(iC);
            tree.Add(iD);

            List<string> list = tree.CreateFlatIndexList(out PairedList<TestData, ErrorTypes> errors);
            Console.WriteLine(ListToString(list));

            Assert.AreEqual(0, list.Count);

            Assert.Pass(ListToString(list));
        }

        [Test]
        public void Test_CircularLoadOrder_DependencyChain3inLoop_AllLoadBeforeRequirements_GetError()
        {
            // A -> B ~ B -> C ~ C -> A
            var tree = new SortedTree<string, TestData>();

            var iA = new TestData(0);
            iA.LoadBeforeCollection.Add("1");

            var iB = new TestData(1);
            iB.LoadBeforeCollection.Add("2");

            var iC = new TestData(2);
            iC.LoadBeforeCollection.Add("0");

            tree.Add(iA);
            tree.Add(iB);
            tree.Add(iC);

            List<string> list = tree.CreateFlatIndexList(out PairedList<TestData, ErrorTypes> errors);
            Console.WriteLine(ListToString(list));

            Assert.AreEqual(0, list.Count);

            Assert.Pass(ListToString(list));
        }

        [Test]
        public void Test_CircularLoadOrder_DependencyChain4inLoop_AllLoadBeforeRequirements_GetError()
        {
            // A -> B ~ B -> C ~ C -> D ~ D -> A
            var tree = new SortedTree<string, TestData>();

            var iA = new TestData(0);
            iA.LoadBeforeCollection.Add("1");

            var iB = new TestData(1);
            iB.LoadBeforeCollection.Add("2");

            var iC = new TestData(2);
            iC.LoadBeforeCollection.Add("3");

            var iD = new TestData(3);
            iD.LoadBeforeCollection.Add("0");

            tree.Add(iA);
            tree.Add(iB);
            tree.Add(iC);
            tree.Add(iD);

            List<string> list = tree.CreateFlatIndexList(out PairedList<TestData, ErrorTypes> errors);
            Console.WriteLine(ListToString(list));

            Assert.AreEqual(0, list.Count);

            Assert.Pass(ListToString(list));
        }

        //-------

        [Test]
        public void Test_CircularLoadOrder_DependencyChain3inLoop_NonDependentEntitiesIncluded()
        {
            // A -> B ~ B -> C ~ C -> A
            var tree = new SortedTree<string, TestData>();

            var iA = new TestData(0);
            iA.LoadAfterCollection.Add("1");

            var iB = new TestData(1);
            iB.LoadAfterCollection.Add("2");

            var iC = new TestData(2);
            iC.LoadAfterCollection.Add("0");

            tree.Add(new TestData(3));
            tree.Add(iA);
            tree.Add(new TestData(4));
            tree.Add(iB);
            tree.Add(new TestData(5));
            tree.Add(iC);
            tree.Add(new TestData(6));

            List<string> list = tree.CreateFlatIndexList(out PairedList<TestData, ErrorTypes> errors);
            Console.WriteLine(ListToString(list));

            Assert.AreEqual(4, list.Count);
            Assert.IsTrue(list.Contains("3"));
            Assert.IsTrue(list.Contains("4"));
            Assert.IsTrue(list.Contains("5"));
            Assert.IsTrue(list.Contains("6"));

            Assert.IsFalse(list.Contains("0"));
            Assert.IsFalse(list.Contains("1"));
            Assert.IsFalse(list.Contains("2"));

            Assert.Pass(ListToString(list));
        }

        [Test]
        public void Test_CircularLoadOrder_DependencyChain3inLoop_NonLoopChainEntitiesIncluded()
        {
            // A -> B ~ B -> C ~ C -> A
            var tree = new SortedTree<string, TestData>();

            var iA = new TestData(0);
            iA.LoadAfterCollection.Add("1");

            var iB = new TestData(1);
            iB.LoadAfterCollection.Add("2");

            var iC = new TestData(2);
            iC.LoadAfterCollection.Add("0");

            // -- 

            var i3 = new TestData(3);
            i3.LoadAfterCollection.Add("4");

            var i4 = new TestData(4);
            i4.LoadAfterCollection.Add("5");

            var i5 = new TestData(5);
            i5.LoadAfterCollection.Add("6");

            var i6 = new TestData(6);

            tree.Add(i6);
            tree.Add(iA);
            tree.Add(i5);
            tree.Add(iB);
            tree.Add(i4);
            tree.Add(iC);
            tree.Add(i3);

            List<string> list = tree.CreateFlatIndexList(out PairedList<TestData, ErrorTypes> errors);
            Console.WriteLine(ListToString(list));

            Assert.AreEqual(4, list.Count);
            Assert.IsTrue(list.Contains("3"));
            Assert.IsTrue(list.Contains("4"));
            Assert.IsTrue(list.Contains("5"));
            Assert.IsTrue(list.Contains("6"));

            Assert.IsFalse(list.Contains("0"));
            Assert.IsFalse(list.Contains("1"));
            Assert.IsFalse(list.Contains("2"));

            Assert.IsTrue(list.IndexOf("3") < list.IndexOf("4"));
            Assert.IsTrue(list.IndexOf("4") < list.IndexOf("5"));
            Assert.IsTrue(list.IndexOf("5") < list.IndexOf("6"));

            Assert.Pass(ListToString(list));
        }

        // TODO - Meta priority tests

        [Test]
        public void Test_RealData()
        {
            var tree = new SortedTree<string, TestData>();
            tree.Add(new TestData("AcceleratedStart"));
            tree.Add(new TestData("AutosortLockers"));
            tree.Add(new TestData("BaseLightSwitch", "SMLHelper"));
            tree.Add(new TestData("BetterBioReactor"));
            tree.Add(new TestData("BiomeHUDIndicator", "SMLHelper"));
            tree.Add(new TestData("BuilderModule", "SMLHelper"));
            tree.Add(new TestData("BuilderModuleInputFix", "SMLHelper"));
            tree.Add(new TestData("CustomBatteries", "SMLHelper"));
            tree.Add(new TestData("CustomCraft2SML", "SMLHelper"));
            tree.Add(new TestData("CustomizedStorage"));
            tree.Add(new TestData("CyclopsAutoZapper", "SMLHelper", "MoreCyclopsUpgrades"));
            tree.Add(new TestData("CyclopsBioReactor", "SMLHelper", "MoreCyclopsUpgrades"));
            tree.Add(new TestData("CyclopsEngineUpgrades", "SMLHelper", "MoreCyclopsUpgrades"));
            tree.Add(new TestData("CyclopsLaserCannonModule", "SMLHelper", "MoreCyclopsUpgrades"));
            tree.Add(new TestData("CyclopsNuclearReactor", "SMLHelper", "MoreCyclopsUpgrades"));
            tree.Add(new TestData("CyclopsNuclearUpgrades", "SMLHelper", "MoreCyclopsUpgrades"));
            tree.Add(new TestData("CyclopsSolarUpgrades", "SMLHelper", "MoreCyclopsUpgrades"));
            tree.Add(new TestData("CyclopsSpeedUpgrades", "SMLHelper", "MoreCyclopsUpgrades"));
            tree.Add(new TestData("CyclopsThermalUpgrades", "SMLHelper", "MoreCyclopsUpgrades"));
            tree.Add(new TestData("DockedVehicleStorageAccess"));
            tree.Add(new TestData("EasyCraft"));
            tree.Add(new TestData("EnzymeChargedBattery", "SMLHelper"));
            tree.Add(new TestData("ExteriorPlantPots", "SMLHelper"));
            tree.Add(new TestData("FCSDeepDriller", "SMLHelper"));
            tree.Add(new TestData("AIMarineTurbine", "SMLHelper"));
            tree.Add(new TestData("MiniFountainFilter", "SMLHelper"));
            tree.Add(new TestData("FCSAIPowerCellSocket", "SMLHelper"));
            tree.Add(new TestData("FCSPowerStorage", "SMLHelper"));
            tree.Add(new TestData("FCSTechFabricator", "SMLHelper"));
            tree.Add(new TestData("snowrabbit007_subnautica_FishOverflowDistributor"));
            tree.Add(new TestData("FloatingCargoCrate", "SMLHelper"));
            tree.Add(new TestData("HabitatControlPanel"));
            tree.Add(new TestData("InstantBulkheadAnimations"));
            tree.Add(new TestData("IonCubeGenerator", "SMLHelper"));
            tree.Add(new TestData("LaserCannon", "SMLHelper"));
            tree.Add(new TestData("SMLHelper"));
            tree.Add(new TestData("MoonpoolVehicleRepair"));
            tree.Add(new TestData("MoreCyclopsUpgrades", "SMLHelper"));
            tree.Add(new TestData("MoreQuickSlots"));
            tree.Add(new TestData("MoreSeamothDepth", "SMLHelper"));
            tree.Add(new TestData("PrawnSuitSonarUpgrade", "SMLHelper"));
            tree.Add(new TestData("QPrawnUpgradeAccess"));
            tree.Add(new TestData("QuitToDesktop"));
            tree.Add(new TestData("AgonyRadialCraftingTabs"));
            tree.Add(new TestData("RepairModule", "SMLHelper"));
            tree.Add(new TestData("ResourceMonitor", "SMLHelper"));
            tree.Add(new TestData("RunningWithTools"));
            tree.Add(new TestData("ScannerModule", "SMLHelper"));
            tree.Add(new TestData("SeamothArms", "SMLHelper"));
            tree.Add(new TestData("SeamothEnergyShield", "SMLHelper"));
            tree.Add(new TestData("SeamothStorageAccess"));
            tree.Add(new TestData("SeamothThermal", "SMLHelper"));
            tree.Add(new TestData("SlotExtender"));
            tree.Add(new TestData("TimeCapsuleLogger"));
            tree.Add(new TestData("UniversalChargingModule", "SMLHelper"));
            tree.Add(new TestData("UpgradedVehicles", "SMLHelper"));
            tree.Add(new TestData("VehicleUpgradesInCyclops", "SMLHelper"));

            List<TestData> list = tree.CreateFlatList(out PairedList<TestData, ErrorTypes> errors);
            Assert.AreEqual(0, tree.NodesInError);
            Assert.AreNotEqual(0, list.Count);
            Assert.AreEqual(0, errors.Count);
        }

        private static string ListToString<T>(IList<T> list)
        {
            string s = "List: ";
            if (list.Count == 0)
            {
                s += "Empty";
                return s;
            }

            int lastIndex = list.Count - 1;

            for (int i = 0; i < lastIndex; i++)
            {
                T item = list[i];
                s += item;
                s += ", ";
            }

            s += list[lastIndex];
            return s;
        }
    }

}
