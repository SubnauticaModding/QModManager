using System;
using NUnit.Framework;
using QModManager;

namespace QMMTests
{
    [TestFixture]
    public class SortModTests
    {
        [Test]
        public void Test()
        {
            QMod mod1 = new QMod();
            mod1.Id = "Mod1";
            mod1.LoadBefore = new string[2]
            {
                "Mod2",
                "Mod3"
            };
            mod1.LoadAfter = new string[1]
            {
                "Mod4"
            };

            QMod mod2 = new QMod();
            mod2.Id = "Mod2";
            mod2.LoadAfter = new string[2]
            {
                "Mod1",
                "Mod3"
            };

            QMod mod3 = new QMod();
            mod3.Id = "Mod3";

            QMod mod4 = new QMod();
            mod4.Id = "Mod4";

            QModPatcher.foundMods = new System.Collections.Generic.List<QMod>();

            QModPatcher.foundMods.Add(mod1);
            QModPatcher.foundMods.Add(mod2);
            QModPatcher.foundMods.Add(mod3);
            QModPatcher.foundMods.Add(mod4);

            QModPatcher.sortedMods = new System.Collections.Generic.List<QMod>(QModPatcher.foundMods);

            foreach(var mod in QModPatcher.foundMods)
            {
                Console.WriteLine("Now Loading: " + mod.Id);
                QModPatcher.modSortingChain.Clear();
                Assert.IsTrue(QModPatcher.SortMod(mod));
            }

            foreach(var mod in QModPatcher.sortedMods)
            {
                Console.WriteLine(mod.Id);
            }

            int indexOfMod1 = QModPatcher.sortedMods.IndexOf(mod1);
            int indexOfMod2 = QModPatcher.sortedMods.IndexOf(mod2);
            int indexOfMod3 = QModPatcher.sortedMods.IndexOf(mod3);
            int indexOfMod4 = QModPatcher.sortedMods.IndexOf(mod4);

            Assert.IsTrue((indexOfMod1 < indexOfMod2) && (indexOfMod1 < indexOfMod3));
            Assert.IsTrue((indexOfMod2 > indexOfMod1) && (indexOfMod2 > indexOfMod3));
        }
    }
}
