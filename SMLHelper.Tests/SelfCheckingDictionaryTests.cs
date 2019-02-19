namespace SMLHelper.Tests
{
    using NUnit.Framework;
    using SMLHelper.V2.Patchers;

    [TestFixture]
    internal class SelfCheckingDictionaryTests
    {
        [Test]
        public void Add_WhenUnique_AllAdded()
        {
            var testDictionary = new SelfCheckingDictionary<TechType, int>("Test")
            {
                { TechType.AcidMushroom, 1 },
                { TechType.AcidMushroomSpore, 2 },
                { TechType.WhiteMushroom, 3 },
                { TechType.WhiteMushroomSpore, 4 },
                { TechType.PinkMushroom, 5 },
                { TechType.PinkMushroomSpore, 6 },
            };

            Assert.AreEqual(6, testDictionary.Count);
            Assert.AreEqual(0, testDictionary.DuplicatesFound.Count);

            Assert.IsTrue(testDictionary.ContainsKey(TechType.AcidMushroom));
            Assert.IsTrue(testDictionary.ContainsKey(TechType.AcidMushroomSpore));
            Assert.IsTrue(testDictionary.ContainsKey(TechType.WhiteMushroom));
            Assert.IsTrue(testDictionary.ContainsKey(TechType.WhiteMushroomSpore));
            Assert.IsTrue(testDictionary.ContainsKey(TechType.PinkMushroom));
            Assert.IsTrue(testDictionary.ContainsKey(TechType.PinkMushroomSpore));
        }

        [Test]
        public void Add_WhenUnique_AllAdded_Alt()
        {
            var testDictionary = new SelfCheckingDictionary<TechType, int>("Test", TechTypeExtensions.sTechTypeComparer)
            {
                { TechType.AcidMushroom, 1 },
                { TechType.AcidMushroomSpore, 2 },
                { TechType.WhiteMushroom, 3 },
                { TechType.WhiteMushroomSpore, 4 },
                { TechType.PinkMushroom, 5 },
                { TechType.PinkMushroomSpore, 6 },
            };

            Assert.AreEqual(6, testDictionary.Count);
            Assert.AreEqual(0, testDictionary.DuplicatesFound.Count);

            Assert.IsTrue(testDictionary.ContainsKey(TechType.AcidMushroom));
            Assert.IsTrue(testDictionary.ContainsKey(TechType.AcidMushroomSpore));
            Assert.IsTrue(testDictionary.ContainsKey(TechType.WhiteMushroom));
            Assert.IsTrue(testDictionary.ContainsKey(TechType.WhiteMushroomSpore));
            Assert.IsTrue(testDictionary.ContainsKey(TechType.PinkMushroom));
            Assert.IsTrue(testDictionary.ContainsKey(TechType.PinkMushroomSpore));
        }

        [Test]
        public void Add_WhenDupEncountered_AutoRemoved_NotVisible()
        {
            var testDictionary = new SelfCheckingDictionary<TechType, int>("Test")
            {
                { TechType.AcidMushroom, 1 },
                { TechType.WhiteMushroom, 1 },
                { TechType.AcidMushroom, 2 }, // Dup
                { TechType.PinkMushroom, 2 },
                { TechType.AcidMushroom, 3 } // Dup
            };

            Assert.IsFalse(testDictionary.ContainsKey(TechType.AcidMushroom));
            Assert.IsTrue(testDictionary.DuplicatesFound.Contains(TechType.AcidMushroom));
            Assert.AreEqual(1, testDictionary.DuplicatesFound.Count);

            Assert.IsTrue(testDictionary.ContainsKey(TechType.WhiteMushroom));
            Assert.IsTrue(testDictionary.ContainsKey(TechType.PinkMushroom));

            Assert.AreEqual(2, testDictionary.Count);
        }

        [Test]
        public void Add_WhenDupEncountered_AutoRemoved_NotVisible_Alt()
        {
            var testDictionary = new SelfCheckingDictionary<TechType, int>("Test", TechTypeExtensions.sTechTypeComparer)
            {
                { TechType.AcidMushroom, 1 },
                { TechType.WhiteMushroom, 1 },
                { TechType.AcidMushroom, 2 }, // Dup
                { TechType.PinkMushroom, 2 },
                { TechType.AcidMushroom, 3 } // Dup
            };

            Assert.IsFalse(testDictionary.ContainsKey(TechType.AcidMushroom));
            Assert.IsTrue(testDictionary.DuplicatesFound.Contains(TechType.AcidMushroom));
            Assert.AreEqual(1, testDictionary.DuplicatesFound.Count);

            Assert.IsTrue(testDictionary.ContainsKey(TechType.WhiteMushroom));
            Assert.IsTrue(testDictionary.ContainsKey(TechType.PinkMushroom));

            Assert.AreEqual(2, testDictionary.Count);
        }

        [Test]
        public void Add_StepByStep_DupsRemoved()
        {
            var testDictionary = new SelfCheckingDictionary<TechType, int>("Test");

            Assert.AreEqual(0, testDictionary.Count);
            Assert.AreEqual(0, testDictionary.DuplicatesFound.Count);

            testDictionary.Add(TechType.Accumulator, 0);
            Assert.AreEqual(1, testDictionary.Count);
            Assert.AreEqual(0, testDictionary.DuplicatesFound.Count);

            testDictionary.Add(TechType.AdvancedWiringKit, 0);
            Assert.AreEqual(2, testDictionary.Count);
            Assert.AreEqual(0, testDictionary.DuplicatesFound.Count);

            testDictionary.Add(TechType.Accumulator, 0); // Dup
            Assert.AreEqual(1, testDictionary.Count); // Count goes down as dup is removed
            Assert.AreEqual(1, testDictionary.DuplicatesFound.Count);
            Assert.IsTrue(testDictionary.DuplicatesFound.Contains(TechType.Accumulator));
            Assert.IsFalse(testDictionary.ContainsKey(TechType.Accumulator));
        }
    }
}
