namespace QMMTests
{
    using System.Collections.Generic;
    using System.Reflection;
    using NUnit.Framework;
    using QModManager.API;
    using QModManager.API.ModLoading;
    using QModManager.Patching;

    [TestFixture]
    public class ModLoadingSimulationTests
    {
        private readonly QMod qmod = new QMod();

        [OneTimeSetUp]
        public void SetUpTestMod()
        {
            qmod.SupportedGame = QModGame.Subnautica;
            qmod.LoadedAssembly = Assembly.GetExecutingAssembly();
        }

        [Test]
        public void SimulateModLoading()
        {
            FindPatchMethods_FetchPatchMethods();
            InitializeMods_MethodsInvoked();
        }
        
        public void FindPatchMethods_FetchPatchMethods()
        {
            var methodFinder = new ManifestValidator();

            methodFinder.FindPatchMethods(qmod);

            Assert.AreEqual(3, qmod.PatchMethods.Count);
            Assert.AreEqual("QPrePatch", qmod.PatchMethods[PatchingOrder.PreInitialize].Method.Name);
            Assert.AreEqual("QPatch", qmod.PatchMethods[PatchingOrder.NormalInitialize].Method.Name);
            Assert.AreEqual("QPostPatch", qmod.PatchMethods[PatchingOrder.PostInitialize].Method.Name);
        }

        public void InitializeMods_MethodsInvoked()
        {
            qmod.Status = ModStatus.Success;
            var list = new List<QMod>
            {
                qmod
            };

            TestPatchClass.Reset();

            var initializer = new Initializer(QModGame.Subnautica);
            initializer.InitializeMods(list);

            Assert.AreEqual(ModStatus.Success, list[0].Status);

            Assert.IsTrue(TestPatchClass.PrePatchInvoked);
            Assert.IsTrue(TestPatchClass.PatchInvoked);
            Assert.IsTrue(TestPatchClass.PostPatchInvoked);

            Assert.AreEqual(nameof(TestPatchClass.QPrePatch), TestPatchClass.Invocations[0]);
            Assert.AreEqual(nameof(TestPatchClass.QPatch), TestPatchClass.Invocations[1]);
            Assert.AreEqual(nameof(TestPatchClass.QPostPatch), TestPatchClass.Invocations[2]);

            Assert.IsNull(qmod.instance);
        }
    }
}
