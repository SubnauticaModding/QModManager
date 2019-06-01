using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace QModManager.Tests.QModManager
{
    [TestFixture]
    public class SortingTests
    {
        [Test]
        public void FourModTest()
        {
            Patcher.loadedMods.Clear();
            Patcher.foundMods.Clear();
            Patcher.sortedMods.Clear();
            Patcher.erroredMods.Clear();

            QModFromJson mod1 = new QModFromJson
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

            QModFromJson mod2 = new QModFromJson
            {
                Id = "Mod2",
                LoadAfter = new string[2]
                {
                    "Mod1",
                    "Mod3"
                }
            };

            QModFromJson mod3 = new QModFromJson
            {
                Id = "Mod3"
            };

            QModFromJson mod4 = new QModFromJson
            {
                Id = "Mod4"
            };

            Patcher.foundMods = new List<QModFromJson>
            {
                mod1,
                mod2,
                mod3,
                mod4
            };

            Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

            foreach (QModFromJson mod in Patcher.foundMods)
            {
                Console.WriteLine("Now Loading: " + mod.Id);
                Patcher.modSortingChain.Clear();
                Assert.IsTrue(Patcher.SortMod(mod));
            }

            foreach (QModFromJson mod in Patcher.sortedMods)
            {
                Console.WriteLine(mod.Id);
            }

            int indexOfMod1 = Patcher.sortedMods.IndexOf(mod1);
            int indexOfMod2 = Patcher.sortedMods.IndexOf(mod2);
            int indexOfMod3 = Patcher.sortedMods.IndexOf(mod3);
            int indexOfMod4 = Patcher.sortedMods.IndexOf(mod4);

            Assert.IsTrue(indexOfMod1 < indexOfMod2);
            Assert.IsTrue(indexOfMod1 < indexOfMod3);
            Assert.IsTrue(indexOfMod2 > indexOfMod1);
            Assert.IsTrue(indexOfMod2 > indexOfMod3);
        }

        [Test]
        public void TwoModTest_BothOrdersDefined_GetExpectedOrder()
        {
            Patcher.loadedMods.Clear();
            Patcher.foundMods.Clear();
            Patcher.sortedMods.Clear();
            Patcher.erroredMods.Clear();

            QModFromJson mod1 = new QModFromJson
            {
                Id = "Mod1",
                LoadBefore = new string[1]
                {
                    "Mod2"
                }
            };

            QModFromJson mod2 = new QModFromJson
            {
                Id = "Mod2",
                LoadAfter = new string[1]
                {
                    "Mod1"
                }
            };

            Patcher.foundMods = new List<QModFromJson>
            {
                mod2, mod1 // Simulate being picked up in the wrong order
            };

            Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

            foreach (QModFromJson mod in Patcher.foundMods)
            {
                Console.WriteLine("Now Loading: " + mod.Id);
                Patcher.modSortingChain.Clear();
                Assert.IsTrue(Patcher.SortMod(mod));
            }

            foreach (QModFromJson mod in Patcher.sortedMods)
            {
                Console.WriteLine(mod.Id);
            }

            int indexOfMod1 = Patcher.sortedMods.IndexOf(mod1);
            int indexOfMod2 = Patcher.sortedMods.IndexOf(mod2);

            Assert.IsTrue(indexOfMod1 < indexOfMod2);
            Assert.IsTrue(indexOfMod2 > indexOfMod1);
        }

        [Test]
        public void TwoModTest_OrdersDefinedForOne_GetExpectedOrder()
        {
            Patcher.loadedMods.Clear();
            Patcher.foundMods.Clear();
            Patcher.sortedMods.Clear();
            Patcher.erroredMods.Clear();

            QModFromJson mod1 = new QModFromJson
            {
                Id = "Mod1"
            };

            QModFromJson mod2 = new QModFromJson
            {
                Id = "Mod2",
                LoadAfter = new string[1]
                {
                    "Mod1"
                }
            };

            Patcher.foundMods = new List<QModFromJson>
            {
                mod2, mod1 // Simulate being picked up in the wrong order
            };

            Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

            foreach (QModFromJson mod in Patcher.foundMods)
            {
                Console.WriteLine("Now Loading: " + mod.Id);
                Patcher.modSortingChain.Clear();
                Assert.IsTrue(Patcher.SortMod(mod));
            }

            foreach (QModFromJson mod in Patcher.sortedMods)
            {
                Console.WriteLine(mod.Id);
            }

            int indexOfMod1 = Patcher.sortedMods.IndexOf(mod1);
            int indexOfMod2 = Patcher.sortedMods.IndexOf(mod2);

            Assert.IsTrue(indexOfMod1 < indexOfMod2);
            Assert.IsTrue(indexOfMod2 > indexOfMod1);
        }

        [Test]
        public void TwoModTest_OrdersDefinedForOther_GetExpectedOrder()
        {
            Patcher.loadedMods.Clear();
            Patcher.foundMods.Clear();
            Patcher.sortedMods.Clear();
            Patcher.erroredMods.Clear();

            QModFromJson mod1 = new QModFromJson
            {
                Id = "Mod1",
                LoadBefore = new string[1]
                {
                    "Mod2"
                }
            };

            QModFromJson mod2 = new QModFromJson
            {
                Id = "Mod2",
            };

            Patcher.foundMods = new List<QModFromJson>
            {
                mod2, mod1 // Simulate being picked up in the wrong order
            };

            Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

            foreach (QModFromJson mod in Patcher.foundMods)
            {
                Console.WriteLine("Now Loading: " + mod.Id);
                Patcher.modSortingChain.Clear();
                Assert.IsTrue(Patcher.SortMod(mod));
            }

            foreach (QModFromJson mod in Patcher.sortedMods)
            {
                Console.WriteLine(mod.Id);
            }

            int indexOfMod1 = Patcher.sortedMods.IndexOf(mod1);
            int indexOfMod2 = Patcher.sortedMods.IndexOf(mod2);

            Assert.IsTrue(indexOfMod1 < indexOfMod2);
            Assert.IsTrue(indexOfMod2 > indexOfMod1);
        }

        [Test]
        public void TwoModTest_CircularDependency_SortModReturnsFalse()
        {
            Patcher.loadedMods.Clear();
            Patcher.foundMods.Clear();
            Patcher.sortedMods.Clear();
            Patcher.erroredMods.Clear();

            QModFromJson mod1 = new QModFromJson
            {
                Id = "Mod1",
                LoadBefore = new string[1]
                {
                    "Mod2"
                }
            };

            QModFromJson mod2 = new QModFromJson
            {
                Id = "Mod2",
                LoadBefore = new string[1]
                {
                    "Mod1"
                }
            };

            Patcher.foundMods = new List<QModFromJson>
            {
                mod2, mod1 // Simulate being picked up in the wrong order
            };

            Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

            // Mod 1
            Console.WriteLine("Now Loading: " + Patcher.foundMods[0].Id);
            Patcher.modSortingChain.Clear();
            Assert.IsTrue(Patcher.SortMod(Patcher.foundMods[0]));

            // Mod 2 - Circular dependency encountered
            Console.WriteLine("Now Loading: " + Patcher.foundMods[1].Id);
            Patcher.modSortingChain.Clear();
            Assert.IsFalse(Patcher.SortMod(Patcher.foundMods[1]));
        }
    }
}
