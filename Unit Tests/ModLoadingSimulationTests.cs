namespace QMMTests
{
    using System.Reflection;
    using NUnit.Framework;
    using QModManager.API;
    using QModManager.API.ModLoading;
    using QModManager.DataStructures;
    using QModManager.Patching;

    [TestFixture]
    public class ModLoadingSimulationTests
    {
        private readonly QModJson qmod = new QModJson();

        [OneTimeSetUp]
        public void SetUpTestMod()
        {
            qmod.LoadedAssembly = Assembly.GetExecutingAssembly();
        }

        [Test]
        public void SimulateModLoading()
        {
            PatchMethodFinder_FetchPatchMethods();
        }

        
        public void PatchMethodFinder_FetchPatchMethods()
        {
            var methodFinder = new PatchMethodFinder();

            methodFinder.LoadPatchMethods(qmod);

            Assert.AreEqual(3, qmod.PatchMethods.Count);
            Assert.AreEqual("QPrePatch", qmod.PatchMethods[PatchingOrder.PreInitialize].Method.Name);
            Assert.AreEqual("QPatch", qmod.PatchMethods[PatchingOrder.NormalInitialize].Method.Name);
            Assert.AreEqual("QPostPatch", qmod.PatchMethods[PatchingOrder.PostInitialize].Method.Name);
        }

        public void InitializeMods_MethodsInvoked()
        {
            var list = new PairedList<QMod, ModStatus>
            {
                { qmod, ModStatus.Success }
            };

            TestPatchClass.Reset();

            var initializer = new Initializer(QModGame.Subnautica);
            initializer.InitializeMods(list);

            Assert.AreEqual(ModStatus.Success, list[0].Value);

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
