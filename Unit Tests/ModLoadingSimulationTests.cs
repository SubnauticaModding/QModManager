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
        private QMod qmod;

        [OneTimeSetUp]
        public void SetUpTestMod()
        {
            qmod = new QMod
            {
                Id = nameof(QMMTests),
                SupportedGame = QModGame.Subnautica,
                LoadedAssembly = Assembly.GetExecutingAssembly()
            };
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

            Assert.AreEqual(5, qmod.PatchMethods.Count);
            Assert.AreEqual(nameof(TestPatchClass.QPrePatch), qmod.PatchMethods[PatchingOrder.MetaPreInitialize].Method.Name);
            Assert.AreEqual(nameof(TestPatchClass.StandardPrePatch), qmod.PatchMethods[PatchingOrder.PreInitialize].Method.Name);
            Assert.AreEqual(nameof(TestPatchClass.QPatch), qmod.PatchMethods[PatchingOrder.NormalInitialize].Method.Name);
            Assert.AreEqual(nameof(TestPatchClass.StandardPostPatch), qmod.PatchMethods[PatchingOrder.PostInitialize].Method.Name);
            Assert.AreEqual(nameof(TestPatchClass.QPostPatch), qmod.PatchMethods[PatchingOrder.MetaPostInitialize].Method.Name);
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

            Assert.IsTrue(TestPatchClass.MetaPrePatchInvoked);
            Assert.IsTrue(TestPatchClass.PrePatchInvoked);
            Assert.IsTrue(TestPatchClass.PatchInvoked);
            Assert.IsTrue(TestPatchClass.PostPatchInvoked);
            Assert.IsTrue(TestPatchClass.MetaPostPatchInvoked);

            Assert.AreEqual(nameof(TestPatchClass.QPrePatch), TestPatchClass.Invocations[0]);
            Assert.AreEqual(nameof(TestPatchClass.StandardPrePatch), TestPatchClass.Invocations[1]);
            Assert.AreEqual(nameof(TestPatchClass.QPatch), TestPatchClass.Invocations[2]);
            Assert.AreEqual(nameof(TestPatchClass.StandardPostPatch), TestPatchClass.Invocations[3]);
            Assert.AreEqual(nameof(TestPatchClass.QPostPatch), TestPatchClass.Invocations[4]);

            Assert.IsNull(qmod.instance);
        }
    }
}
