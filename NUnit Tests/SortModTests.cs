namespace QMMTests
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using QModManager;

    [TestFixture]
    public class SortModTests
    {
        [Test]
        public void FourModTest()
        {
            QModPatcher.ClearModLists();

            var mod1 = new QMod
            {
                Id = "Mod1",
                LoadBefore = new string[2]
                {
                    "Mod2",
                    "Mod3"
                },
                LoadAfter = new string[1]
                {
                    "Mod4"
                }
            };

            var mod2 = new QMod
            {
                Id = "Mod2",
                LoadAfter = new string[2]
                {
                    "Mod1",
                    "Mod3"
                }
            };

            var mod3 = new QMod
            {
                Id = "Mod3"
            };

            var mod4 = new QMod
            {
                Id = "Mod4"
            };

            QModPatcher.foundMods = new List<QMod>
            {
                mod1,
                mod2,
                mod3,
                mod4
            };

            QModPatcher.sortedMods = new List<QMod>(QModPatcher.foundMods);

            foreach (QMod mod in QModPatcher.foundMods)
            {
                Console.WriteLine("Now Loading: " + mod.Id);
                QModPatcher.modSortingChain.Clear();
                Assert.IsTrue(QModPatcher.SortMod(mod));
            }

            foreach (QMod mod in QModPatcher.sortedMods)
            {
                Console.WriteLine(mod.Id);
            }

            int indexOfMod1 = QModPatcher.sortedMods.IndexOf(mod1);
            int indexOfMod2 = QModPatcher.sortedMods.IndexOf(mod2);
            int indexOfMod3 = QModPatcher.sortedMods.IndexOf(mod3);
            int indexOfMod4 = QModPatcher.sortedMods.IndexOf(mod4);

            Assert.IsTrue(indexOfMod1 < indexOfMod2);
            Assert.IsTrue(indexOfMod1 < indexOfMod3);
            Assert.IsTrue(indexOfMod2 > indexOfMod1);
            Assert.IsTrue(indexOfMod2 > indexOfMod3);
        }

        [Test]
        public void TwoModTest_BothOrdersDefined_GetExpectedOrder()
        {
            QModPatcher.ClearModLists();

            var mod1 = new QMod
            {
                Id = "Mod1",
                LoadBefore = new string[1]
                {
                    "Mod2"
                }
            };

            var mod2 = new QMod
            {
                Id = "Mod2",
                LoadAfter = new string[1]
                {
                    "Mod1"
                }
            };

            QModPatcher.foundMods = new List<QMod>
            {
                mod2, mod1 // Simulate being picked up in the wrong order
            };

            QModPatcher.sortedMods = new List<QMod>(QModPatcher.foundMods);

            foreach (QMod mod in QModPatcher.foundMods)
            {
                Console.WriteLine("Now Loading: " + mod.Id);
                QModPatcher.modSortingChain.Clear();
                Assert.IsTrue(QModPatcher.SortMod(mod));
            }

            foreach (QMod mod in QModPatcher.sortedMods)
            {
                Console.WriteLine(mod.Id);
            }

            int indexOfMod1 = QModPatcher.sortedMods.IndexOf(mod1);
            int indexOfMod2 = QModPatcher.sortedMods.IndexOf(mod2);

            Assert.IsTrue(indexOfMod1 < indexOfMod2);
            Assert.IsTrue(indexOfMod2 > indexOfMod1);
        }

        [Test]
        public void TwoModTest_OrdersDefinedForOne_GetExpectedOrder()
        {
            QModPatcher.ClearModLists();

            var mod1 = new QMod
            {
                Id = "Mod1"
            };

            var mod2 = new QMod
            {
                Id = "Mod2",
                LoadAfter = new string[1]
                {
                    "Mod1"
                }
            };

            QModPatcher.foundMods = new List<QMod>
            {
                mod2, mod1 // Simulate being picked up in the wrong order
            };

            QModPatcher.sortedMods = new List<QMod>(QModPatcher.foundMods);

            foreach (QMod mod in QModPatcher.foundMods)
            {
                Console.WriteLine("Now Loading: " + mod.Id);
                QModPatcher.modSortingChain.Clear();
                Assert.IsTrue(QModPatcher.SortMod(mod));
            }

            foreach (QMod mod in QModPatcher.sortedMods)
            {
                Console.WriteLine(mod.Id);
            }

            int indexOfMod1 = QModPatcher.sortedMods.IndexOf(mod1);
            int indexOfMod2 = QModPatcher.sortedMods.IndexOf(mod2);

            Assert.IsTrue(indexOfMod1 < indexOfMod2);
            Assert.IsTrue(indexOfMod2 > indexOfMod1);
        }

        [Test]
        public void TwoModTest_OrdersDefinedForOther_GetExpectedOrder()
        {
            QModPatcher.ClearModLists();

            var mod1 = new QMod
            {
                Id = "Mod1",
                LoadBefore = new string[1]
                {
                    "Mod2"
                }
            };

            var mod2 = new QMod
            {
                Id = "Mod2",
            };

            QModPatcher.foundMods = new List<QMod>
            {
                mod2, mod1 // Simulate being picked up in the wrong order
            };

            QModPatcher.sortedMods = new List<QMod>(QModPatcher.foundMods);

            foreach (QMod mod in QModPatcher.foundMods)
            {
                Console.WriteLine("Now Loading: " + mod.Id);
                QModPatcher.modSortingChain.Clear();
                Assert.IsTrue(QModPatcher.SortMod(mod));
            }

            foreach (QMod mod in QModPatcher.sortedMods)
            {
                Console.WriteLine(mod.Id);
            }

            int indexOfMod1 = QModPatcher.sortedMods.IndexOf(mod1);
            int indexOfMod2 = QModPatcher.sortedMods.IndexOf(mod2);

            Assert.IsTrue(indexOfMod1 < indexOfMod2);
            Assert.IsTrue(indexOfMod2 > indexOfMod1);
        }

        [Test]
        public void TwoModTest_CircularDependency_SortModReturnsFalse()
        {
            QModPatcher.ClearModLists();

            var mod1 = new QMod
            {
                Id = "Mod1",
                LoadBefore = new string[1]
                {
                    "Mod2"
                }
            };

            var mod2 = new QMod
            {
                Id = "Mod2",
                LoadBefore = new string[1]
                {
                    "Mod1"
                }
            };

            QModPatcher.foundMods = new List<QMod>
            {
                mod2, mod1 // Simulate being picked up in the wrong order
            };

            QModPatcher.sortedMods = new List<QMod>(QModPatcher.foundMods);

            // Mod 1
            Console.WriteLine("Now Loading: " + QModPatcher.foundMods[0].Id);
            QModPatcher.modSortingChain.Clear();
            Assert.IsTrue(QModPatcher.SortMod(QModPatcher.foundMods[0]));

            // Mod 2 - Circular dependency encountered
            Console.WriteLine("Now Loading: " + QModPatcher.foundMods[1].Id);
            QModPatcher.modSortingChain.Clear();
            Assert.IsFalse(QModPatcher.SortMod(QModPatcher.foundMods[1]));
        }
    }
}
