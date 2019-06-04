using System.Reflection;
using NUnit.Framework;
using QModManager.API;
using QModManager.API.ModLoading.Internal;

namespace QMMTests.QModManager
{
    [TestFixture]
    internal class QModTests
    {
        public static void TestMethodSuccessfull()
        {
        }

        public static void TestMethodFailure()
        {
            throw new System.Exception("Failed");
        }

        [TestCase(Game.Subnautica, Game.BelowZero)]
        [TestCase(Game.BelowZero, Game.Subnautica)]
        public void TryLoading_WhenModGameDoesNotMatchCurrentGame_GameNotSupported(Game currentGame, Game supportedByMod)
        {
            var qmod = new QMod
            {
                Game = $"{supportedByMod}"
            };
            qmod.TryCompletingJsonLoading("");
            qmod.PatchMethods.Add(PatchingOrder.NormalInitialize, new PatchMethod(null, qmod));

            ModLoadingResults result = qmod.TryLoading(PatchingOrder.NormalInitialize, currentGame);
            Assert.AreEqual(ModLoadingResults.CurrentGameNotSupported, result);
            Assert.AreEqual(0, qmod.PatchMethods.Count);
        }

        [Test]
        public void TryLoading_WhenNoMethodExistsForOrder_NoMethodToExecute()
        {
            var qmod = new QMod
            {
                Game = $"{Game.Both}"
            };
            qmod.TryCompletingJsonLoading("");

            qmod.PatchMethods.Add(PatchingOrder.NormalInitialize, new PatchMethod(null, qmod));

            ModLoadingResults result = qmod.TryLoading(PatchingOrder.PreInitialize, Game.Subnautica);
            Assert.AreEqual(ModLoadingResults.NoMethodToExecute, result);
        }

        [Test]
        public void TryLoading_WhenMethodPreviouslyInvoked_AlreadyLoaded()
        {
            var qmod = new QMod
            {
                Game = $"{Game.Both}",
                LoadedAssembly = typeof(QModTests).Assembly
            };
            qmod.TryCompletingJsonLoading("");

            MethodInfo testMethodInfo = typeof(QModTests).GetMethod(nameof(TestMethodSuccessfull));

            qmod.PatchMethods.Add(PatchingOrder.NormalInitialize, new PatchMethod(testMethodInfo, qmod));

            // First call
            ModLoadingResults result = qmod.TryLoading(PatchingOrder.NormalInitialize, Game.Subnautica);
            Assert.AreEqual(ModLoadingResults.Success, result);

            // Second call
            result = qmod.TryLoading(PatchingOrder.NormalInitialize, Game.Subnautica);
            Assert.AreEqual(ModLoadingResults.AlreadyLoaded, result);
        }

        [Test]
        public void TryLoading_WhenMethodFails_Failure()
        {
            var qmod = new QMod
            {
                Game = $"{Game.Both}",
                LoadedAssembly = typeof(QModTests).Assembly
            };
            qmod.TryCompletingJsonLoading("");

            MethodInfo testMethodInfo = typeof(QModTests).GetMethod(nameof(TestMethodFailure));

            qmod.PatchMethods.Add(PatchingOrder.NormalInitialize, new PatchMethod(testMethodInfo, qmod));

            // First call
            ModLoadingResults result = qmod.TryLoading(PatchingOrder.NormalInitialize, Game.Subnautica);
            Assert.AreEqual(ModLoadingResults.Failure, result);
            Assert.AreEqual(0, qmod.PatchMethods.Count);
        }
    }
}
