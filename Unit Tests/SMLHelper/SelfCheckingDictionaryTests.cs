namespace QModManager.Tests.SMLHelper
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
            Assert.AreEqual(0, testDictionary.DuplicatesDiscarded.Count);

            Assert.IsTrue(testDictionary.ContainsKey(TechType.AcidMushroom));
            Assert.IsTrue(testDictionary.ContainsKey(TechType.AcidMushroomSpore));
            Assert.IsTrue(testDictionary.ContainsKey(TechType.WhiteMushroom));
            Assert.IsTrue(testDictionary.ContainsKey(TechType.WhiteMushroomSpore));
            Assert.IsTrue(testDictionary.ContainsKey(TechType.PinkMushroom));
            Assert.IsTrue(testDictionary.ContainsKey(TechType.PinkMushroomSpore));
        }

        [Test]
        public void Add_WhenUnique_AllAdded_AltComparer()
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
            Assert.AreEqual(0, testDictionary.DuplicatesDiscarded.Count);

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

            Assert.IsFalse(testDictionary.UniqueEntries.ContainsKey(TechType.AcidMushroom));
            Assert.IsTrue(testDictionary.DuplicatesDiscarded.ContainsKey(TechType.AcidMushroom));
            Assert.AreEqual(1, testDictionary.DuplicatesDiscarded.Count);
            Assert.AreEqual(3, testDictionary.DuplicatesDiscarded[TechType.AcidMushroom]); // Discarded all three

            Assert.IsTrue(testDictionary.ContainsKey(TechType.WhiteMushroom));
            Assert.IsTrue(testDictionary.ContainsKey(TechType.PinkMushroom));

            Assert.AreEqual(2, testDictionary.Count);
        }

        [Test]
        public void Add_WhenDupEncountered_AutoRemoved_NotVisible_AltComparer()
        {
            var testDictionary = new SelfCheckingDictionary<TechType, int>("Test", TechTypeExtensions.sTechTypeComparer)
            {
                { TechType.AcidMushroom, 1 },
                { TechType.WhiteMushroom, 1 },
                { TechType.AcidMushroom, 2 }, // Dup
                { TechType.PinkMushroom, 2 },
                { TechType.AcidMushroom, 3 } // Dup
            };

            Assert.IsFalse(testDictionary.UniqueEntries.ContainsKey(TechType.AcidMushroom));
            Assert.IsTrue(testDictionary.DuplicatesDiscarded.ContainsKey(TechType.AcidMushroom));            
            Assert.AreEqual(1, testDictionary.DuplicatesDiscarded.Count);
            Assert.AreEqual(3, testDictionary.DuplicatesDiscarded[TechType.AcidMushroom]); // Discarded all three

            Assert.IsTrue(testDictionary.ContainsKey(TechType.WhiteMushroom));
            Assert.IsTrue(testDictionary.ContainsKey(TechType.PinkMushroom));

            Assert.AreEqual(2, testDictionary.Count);
        }

        [Test]
        public void Add_StepByStep_DupsRemoved()
        {
            var testDictionary = new SelfCheckingDictionary<TechType, int>("Test");

            Assert.AreEqual(0, testDictionary.Count);
            Assert.AreEqual(0, testDictionary.DuplicatesDiscarded.Count);

            testDictionary.Add(TechType.Accumulator, 0);
            Assert.AreEqual(1, testDictionary.Count);
            Assert.AreEqual(0, testDictionary.DuplicatesDiscarded.Count);

            testDictionary.Add(TechType.AdvancedWiringKit, 0);
            Assert.AreEqual(2, testDictionary.Count);
            Assert.AreEqual(0, testDictionary.DuplicatesDiscarded.Count);

            testDictionary.Add(TechType.Accumulator, 0); // Dup
            Assert.AreEqual(1, testDictionary.Count); // Count goes down as dup is removed
            Assert.AreEqual(1, testDictionary.DuplicatesDiscarded.Count);
            Assert.IsTrue(testDictionary.DuplicatesDiscarded.ContainsKey(TechType.Accumulator));
            Assert.IsFalse(testDictionary.UniqueEntries.ContainsKey(TechType.Accumulator));
            Assert.AreEqual(2, testDictionary.DuplicatesDiscarded[TechType.Accumulator]); // Discarded twice
        }

        // ----

        [Test]
        public void SetByIndexer_WhenUnique_AllAdded()
        {
            var testDictionary = new SelfCheckingDictionary<TechType, int>("Test")
            {
                [TechType.AcidMushroom] = 1,
                [TechType.AcidMushroomSpore] = 2,
                [TechType.WhiteMushroom] = 3,
                [TechType.WhiteMushroomSpore] = 4,
                [TechType.PinkMushroom] = 5,
                [TechType.PinkMushroomSpore] = 6
            };

            Assert.AreEqual(6, testDictionary.Count);
            Assert.AreEqual(0, testDictionary.DuplicatesDiscarded.Count);

            Assert.IsTrue(testDictionary.ContainsKey(TechType.AcidMushroom));
            Assert.IsTrue(testDictionary.ContainsKey(TechType.AcidMushroomSpore));
            Assert.IsTrue(testDictionary.ContainsKey(TechType.WhiteMushroom));
            Assert.IsTrue(testDictionary.ContainsKey(TechType.WhiteMushroomSpore));
            Assert.IsTrue(testDictionary.ContainsKey(TechType.PinkMushroom));
            Assert.IsTrue(testDictionary.ContainsKey(TechType.PinkMushroomSpore));
        }

        [Test]
        public void SetByIndexer_WhenUnique_AllAdded_AltComparer()
        {
            var testDictionary = new SelfCheckingDictionary<TechType, int>("Test", TechTypeExtensions.sTechTypeComparer)
            {
                [TechType.AcidMushroom] = 1,
                [TechType.AcidMushroomSpore] = 2,
                [TechType.WhiteMushroom] = 3,
                [TechType.WhiteMushroomSpore] = 4,
                [TechType.PinkMushroom] = 5,
                [TechType.PinkMushroomSpore] = 6,
            };

            Assert.AreEqual(6, testDictionary.Count);
            Assert.AreEqual(0, testDictionary.DuplicatesDiscarded.Count);

            Assert.IsTrue(testDictionary.ContainsKey(TechType.AcidMushroom));
            Assert.IsTrue(testDictionary.ContainsKey(TechType.AcidMushroomSpore));
            Assert.IsTrue(testDictionary.ContainsKey(TechType.WhiteMushroom));
            Assert.IsTrue(testDictionary.ContainsKey(TechType.WhiteMushroomSpore));
            Assert.IsTrue(testDictionary.ContainsKey(TechType.PinkMushroom));
            Assert.IsTrue(testDictionary.ContainsKey(TechType.PinkMushroomSpore));
        }

        [Test]
        public void SetByIndexer_WhenDupEncountered_OriginalOverwritten_NotVisible()
        {
            const int firstValue = 1;
            const int finalValue = 3;
            var testDictionary = new SelfCheckingDictionary<TechType, int>("Test")
            {
                [TechType.AcidMushroom] = firstValue,
                [TechType.WhiteMushroom] = 1,
                [TechType.AcidMushroom] = 2, // Dup
                [TechType.PinkMushroom] = 2,
                [TechType.AcidMushroom] = finalValue  // Dup
            };

            Assert.AreEqual(1, testDictionary.DuplicatesDiscarded.Count);
            Assert.AreEqual(2, testDictionary.DuplicatesDiscarded[TechType.AcidMushroom]); // Discarded twice

            Assert.IsTrue(testDictionary.ContainsKey(TechType.AcidMushroom));
            Assert.IsTrue(testDictionary.ContainsKey(TechType.WhiteMushroom));
            Assert.IsTrue(testDictionary.ContainsKey(TechType.PinkMushroom));

            Assert.AreEqual(finalValue, testDictionary[TechType.AcidMushroom]);

            Assert.AreEqual(3, testDictionary.Count);
        }

        [Test]
        public void SetByIndexer_WhenDupEncountered_OriginalOverwritten_AltComparer()
        {
            const int firstValue = 1;
            const int finalValue = 3;
            var testDictionary = new SelfCheckingDictionary<TechType, int>("Test", TechTypeExtensions.sTechTypeComparer)
            {
                [TechType.AcidMushroom] = firstValue,
                [TechType.WhiteMushroom] = 1,
                [TechType.AcidMushroom] = 2, // Dup
                [TechType.PinkMushroom] = 2,
                [TechType.AcidMushroom] = finalValue // Dup
            };

            Assert.AreEqual(1, testDictionary.DuplicatesDiscarded.Count);
            Assert.AreEqual(2, testDictionary.DuplicatesDiscarded[TechType.AcidMushroom]); // Discarded twice

            Assert.IsTrue(testDictionary.ContainsKey(TechType.AcidMushroom));
            Assert.IsTrue(testDictionary.ContainsKey(TechType.WhiteMushroom));
            Assert.IsTrue(testDictionary.ContainsKey(TechType.PinkMushroom));

            Assert.AreEqual(finalValue, testDictionary[TechType.AcidMushroom]);

            Assert.AreEqual(3, testDictionary.Count);
        }
    }
}
